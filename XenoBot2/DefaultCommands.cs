using System.Collections.Generic;
using XenoBot2.Shared;

namespace XenoBot2
{
    internal static class DefaultCommands
    {
        public static readonly IDictionary<string, Command> Content = new Dictionary<string, Command>
        {
            {
                "wtc", new Command
                {
                    AliasFor = "commit"
                }
            },
            {
                "date", new Command
                {
                    HelpText = "Shows the current date in UTC.",
                    HelpCategory = "Utility",
                    Definition = Commands.Utility.Date
                }
            },
            {
                "time", new Command
                {
                    HelpText = "Shows the current time in UTC",
                    HelpCategory = "Utility",
                    Definition = Commands.Utility.Time
                }
            },
            {
                "enable", new Command
                {
                    HelpText = "Enables a disabled command on the current channel.",
                    Permission = UserFlag.Administrator,
                    HelpCategory = "Administration",
                    Flags = CommandFlag.NoPrivateChannel | CommandFlag.NonDisableable,
                    Definition = Commands.ChannelAdministration.Enable,
                    Arguments = "command"
                }
            },
            {
                "disable", new Command
                {
                    HelpText = "Disables an enabled command on the current channel.",
                    Permission = UserFlag.Administrator,
                    HelpCategory = "Administration",
                    Flags = CommandFlag.NoPrivateChannel | CommandFlag.NonDisableable,
                    Definition = Commands.ChannelAdministration.Disable,
                    Arguments = "command"
                }
            },
            {
                "test", new Command
                {
                    AliasFor = "echo"
                }
            },
            {
                "catfact", new Command
                {
                    HelpText = "I would like to unsubscribe from cat facts.",
                    HelpCategory = "Fun",
                    Definition = Commands.Fun.CatFact
                }
            },
            {
                "commit", new Command
                {
                    HelpText = "Shows a humorous commit message from WhatTheCommit.",
                    HelpCategory = "Fun",
                    Definition = Commands.Fun.WhatTheCommit
                }
            },
            {
                "echo", new Command
                {
                    HelpText = "Echos the command arguments.",
                    HelpCategory = "Utility",
                    Definition = Commands.Utility.Echo,
                    Arguments = "text"
                }
            },
            {
                "me", new Command
                {
                    HelpText = "Prints some basic profile info about you.",
                    HelpCategory = "Utility",
                    Definition = Commands.Utility.Me,
                    Flags = CommandFlag.UsableWhileIgnored
                }
            },
            {
                "numeral", new Command
                {
                    HelpText = "Converts an integer into another format.",
                    Arguments = "{roman|words|wordord|metric} number",
                    LongHelpText = "Converts an integer into another format.\n" +
                                   "Available formats:\n" +
                                   "* roman   - Roman Numerals, i.e. \"XIV\"\n" +
                                   "* words   - Words, i.e. \"twenty seven\"\n" +
                                   "* wordord - Words (ordinal), i.e. \"twenty seventh\"\n" +
                                   "* metric  - Metric prefixed, i.e. \"200K\"",
                    HelpCategory = "Utility",
                    Definition = Commands.Utility.ConvertNumber
                }
            },
            {
                "num", new Command
                {
                    AliasFor = "numeral"
                }
            },
            {
                "!halt", new Command
                {
                    HelpText = "Shuts down the bot.",
                    Permission = UserFlag.BotAdministrator,
                    HelpCategory = "Administration",
                    Definition = Commands.BotAdministration.HaltBot,
                    Flags = CommandFlag.NonDisableable | CommandFlag.UsableWhileIgnored,
                    LongHelpText =
                        "Terminates the bot's process. The bot cannot respond to commands after this command is run!"
                }
            },
            {
                "help", new Command
                {
                    HelpText =
                        "Displays this highly informative help text. Use \"$help command\" to get help for command.",
                    HelpCategory = "Core",
                    Definition = Commands.Base.Help,
                    Flags = CommandFlag.UsableWhileIgnored | CommandFlag.NonDisableable,
                    Arguments = "[topic]"
                }
            },
            {
                "version", new Command
                {
                    HelpText = "Prints the bot's current version number.",
                    HelpCategory = "Core",
                    Definition = Commands.Base.Version
                }
            },
            {
                "user", new Command
                {
                    HelpCategory = "Utility",
                    HelpText = "Gets some information about a user.",
                    Definition = Commands.Utility.UserInfo,
                    Arguments = "mention"
                }
            },
            {
                "8", new Command
                {
                    HelpCategory = "Fun",
                    Definition = Commands.Fun.EightBall,
                    HelpText = "Consult the 8 Ball.",
                    Arguments = "[question]"
                }
            },
            {
                "ping", new Command
                {
                    Definition = Commands.Utility.Ping,
                    HelpCategory = "Utility",
                    Flags = CommandFlag.Hidden
                }
            },
            {
                "avatar", new Command
                {
                    HelpCategory = "Utility",
                    HelpText = "Gets the avatar for the given user",
                    Definition = Commands.Utility.Avatar,
                    Arguments = "[mention]"
                }
            },
            {
                "ignore", new Command
                {
                    HelpCategory = "Core",
                    Permission = UserFlag.Moderator,
                    Flags = CommandFlag.UsableWhileIgnored | CommandFlag.NoPrivateChannel,
                    Definition = Commands.ChannelAdministration.IgnoreUser,
                    HelpText = "Toggles command ignore for a user.",
                    Arguments = "mention"
                }
            },
            {
                "globalignore", new Command
                {
                    HelpCategory = "Core",
                    Permission = UserFlag.BotAdministrator,
                    Flags = CommandFlag.NonDisableable | CommandFlag.UsableWhileIgnored,
                    Definition = Commands.BotAdministration.GlobalIgnoreUser,
                    HelpText = "Toggles command ignore for a user on all channels.",
                    Arguments = "mention"
                }
            },
            {
                "!cmdinfo", new Command
                {
                    HelpCategory = "Debug",
                    Permission = UserFlag.BotDebug,
                    Flags = CommandFlag.NonDisableable,
                    Definition = Commands.Debug.Cmdinfo,
                    Arguments = "command"
                }
            },
            {
                "!chinfo", new Command
                {
                    HelpCategory = "Debug",
                    Permission = UserFlag.BotDebug,
                    Flags = CommandFlag.NonDisableable,
                    Definition = Commands.Debug.GetChannelInfo
                }
            },
            {
                "cat", new Command
                {
                    HelpCategory = "Fun",
                    Definition = Commands.Fun.RandomCat,
                    HelpText = "Meow."
                }
            }
        };
    }
}