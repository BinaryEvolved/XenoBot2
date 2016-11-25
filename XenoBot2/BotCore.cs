using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Discord;
using XenoBot2.Data;
using XenoBot2.Shared;

namespace XenoBot2
{
    internal class BotCore
    {
        private DiscordClient _client = null;

        private char _prefix;
        private string _key;
        private readonly string _configPath = Path.Combine("Data", "config.xml");

        public TwoKeyDictionary<ulong, ulong, UserFlag> UserFlags;
        public TwoKeyDictionary<string, ulong, CommandState> CommandStateData;
        public CommandStore Commands;

        public async Task Exit() => await _client.Disconnect();

        public void SetGame(string game) => _client.SetGame(game);

        public async Task Initialize()
        {
            Utilities.WriteLog($"XenoBot2 v{Utilities.GetVersion()} starting initialization...");

            // Initialize command storage, needs to be done before config load
            InitializeStorage();

            var timer = new Stopwatch();
            var serializer = new XmlSerializer(typeof(Config));

            Utilities.WriteLog("Looking for config...");
            timer.Start();

            if (!File.Exists(_configPath))
            {
                Utilities.WriteLog("Config not found, creating.");
                using (var fs = new FileStream(_configPath, FileMode.CreateNew))
                {
                    serializer.Serialize(fs, new Config());
                }

                Utilities.WriteLog(
                    $"Please fill in the config file at \"{Path.Combine(Directory.GetCurrentDirectory(), _configPath)}\" and restart the bot.");
                throw new FileNotFoundException();
            }

            Utilities.WriteLog("Found config, loading.");
            using (var fs = File.OpenRead(_configPath))
            {
                var conf = serializer.Deserialize(fs) as Config;
                if (conf?.ConfigVersion != 1)
                {
                    Utilities.WriteLog("WARNING: Unknown config version!");
                }
                _prefix = conf?.CommandPrefix ?? '$';
                Utilities.WriteLog($"Command prefix set to '{_prefix}'.");
                _key = conf?.ApiToken;
                if (string.IsNullOrWhiteSpace(_key))
                {
                    Utilities.WriteLog("WARNING: No API Key in config! Attempting legacy load.");
                    var path = Path.Combine("Data", "apikey.txt");
                    await LoadApiKey(path);
                }

                foreach (var user in conf?.BotAdmins ?? new ulong[0])
                {
                    UserFlags[user, UserFlags.GlobalValue] |= UserFlag.BotAdministrator;
                }

                foreach (var user in conf?.BotDebuggers ?? new ulong[0])
                {
                    UserFlags[user, UserFlags.GlobalValue] |= UserFlag.BotDebug;
                }
            }
            timer.Stop();

            Utilities.WriteLog($"Config load took {timer.ElapsedMilliseconds} ms.");

            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");

            Utilities.WriteLog("Setting up command data...");
            timer.Restart();
            Strings.WhatTheCommit =
                (await GetAsset("https://www.lohikar.io/assets/xenobot/commits.txt", "wtc.txt")).Split('\n');
            Strings.Names =
                (await GetAsset("https://www.lohikar.io/assets/xenobot/names.txt", "wtc_names.txt")).Split('\n');
            timer.Stop();
            Utilities.WriteLog($"Done setting up command data; took {timer.ElapsedMilliseconds} ms.");

            // initialize client
            _client = new DiscordClient();
            _client.MessageReceived += MessageReceived;
            _client.Ready += (s, e) =>
            {
                _client.SetGame($"v{Utilities.GetVersion()}");
                Utilities.WriteLog($"Logged into Discord as {_client.CurrentUser.GetFullUsername()}.");
                Console.Title = $"{_client.CurrentUser.GetFullUsername()} - XenoBot2 v{Utilities.GetVersion()}";
            };

            Utilities.WriteLog("Done initialization.");
        }

        private async void MessageReceived(object sender, MessageEventArgs e)
        {
            if (!e.Message.IsAuthor)
                await ParseMessage(e.User, e.Channel, e.Message.RawText, e.Channel.IsPrivate);
        }

        private void InitializeStorage()
        {
            Commands = new CommandStore();
            // load default commands
            Commands.AddMany(DefaultCommands.Content);
            CommandStateData = new TwoKeyDictionary<string, ulong, CommandState>(CommandState.None, 0);
            UserFlags = new TwoKeyDictionary<ulong, ulong, UserFlag>(UserFlag.User, 0);
        }

        private async Task LoadApiKey(string path)
        {
            if (!File.Exists(path))
            {
                Utilities.WriteLog("Error: API key not found.\n" +
                                   "Please create a file named 'apikey.txt' in the \"Data\" directory, " +
                                   "and put your Discord Bot API key into the file.\n" +
                                   "Press Enter to exit.");
                Console.ReadLine();
                throw new FileNotFoundException();
            }
            using (var keyfile = File.OpenText(path))
            {
                Utilities.WriteLog("Found API key.");
                _key = await keyfile.ReadToEndAsync();
            }
        }

        /// <summary>
        ///		Connects to discord. This call blocks until the client terminates.
        /// </summary>
        public void Connect()
        {
            if (_client == null) throw new InvalidOperationException("Object not initialized!");
            _client.ExecuteAndWait(async () =>
            {
                Utilities.WriteLog("Connecting to discord...");
                await _client.Connect(_key, TokenType.Bot);
            });
        }

        private async Task ParseMessage(User author, Channel channel, string messageText, bool skipPrefix = false)
        {
            // silently ignore our own messages & messages from other bots
            if (string.IsNullOrWhiteSpace(messageText) || (messageText[0] != _prefix && !skipPrefix)) return;

            var text = skipPrefix ? messageText : messageText.Substring(1);

            // Process command & execute
            var cmd = CommandParser.ParseCommand(text, channel);
            if (CommandInvalid(cmd, author)) return;
            if ((cmd.BoundCommand.Flags.HasFlag(CommandFlag.NoPrivateChannel) && channel.IsPrivate) ||
                cmd.BoundCommand.Flags.HasFlag(CommandFlag.NoPublicChannel) && !channel.IsPrivate)
            {
                await channel.SendMessage("That command cannot be used here.");
                return;
            }
            try
            {
                // execute command
                await cmd.BoundCommand.Definition(cmd, author, channel);
            }
            catch (Exception ex)
            {
                Utilities.WriteLog($"An exception occurred during execution of command '{cmd.CommandText}'," +
                                   $" args '{string.Join(", ", cmd.Arguments)}'.\n" + ex.Message);
                await channel.SendMessage($"An internal error occurred running command '{cmd.CommandText}'.");
            }
        }

        private static bool CommandInvalid(CommandInfo cmd, User author) =>
            cmd.State.HasFlag(CommandState.DoesNotExist) || cmd.BoundCommand == null ||
            !cmd.BoundCommand.Flags.HasFlag(CommandFlag.UsableWhileIgnored) &&
            Utilities.Permitted(UserFlag.Ignored, author);

        private static async Task<string> GetAsset(string url, string cacheFileName)
        {
            var path = Path.Combine("Data", cacheFileName);

            if (File.Exists(path))
                using (var reader = File.OpenText(path))
                    return await reader.ReadToEndAsync();

            var asset = await Shared.Utilities.GetStringAsync(url);
            using (var writer = new FileStream(path, FileMode.CreateNew))
            using (var text = new StreamWriter(writer))
                await text.WriteAsync(asset);

            return asset;
        }
    }
}