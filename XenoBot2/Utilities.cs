using System;
using System.Reflection;
using Discord;
using XenoBot2.Shared;

namespace XenoBot2
{
    /// <summary>
    ///     Miscellaneous utility functions.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Checks if a <see cref="Command" /> has an Alias, and resolves it to a <see cref="Command" />.
        /// </summary>
        /// <param name="command">The command to resolve the alias for.</param>
        /// <returns>A resolved alias or the passed <see cref="Command" /> if there was no defined alias.</returns>
        internal static Command ResolveCommand(this Command command)
            => command.AliasFor == null ? command : Program.BotInstance.Commands[command.AliasFor];

        internal static bool Permitted(UserFlag flag, User member, Channel channel = null)
            => Permitted(flag, member.Id, channel?.Id);

        internal static bool Permitted(UserFlag flag, ulong member, ulong? channel = null)
        {
            var channelData = channel != null ? Program.BotInstance.UserFlags[member, channel.Value] : UserFlag.None;
            var globalData = Program.BotInstance.UserFlags[member];

            return (channelData | globalData).HasFlag(flag);
        }

        internal static bool HasState(CommandState flag, string command, ulong? channel = null)
        {
            var channelData = channel != null
                ? Program.BotInstance.CommandStateData[command, channel.Value]
                : CommandState.None;
            var globalData = Program.BotInstance.CommandStateData[command];

            return (channelData | globalData).HasFlag(flag);
        }

        /// <summary>
        ///     Writes an entry to the log with a date stamp.
        /// </summary>
        /// <param name="data">The message to write to the log.</param>
        internal static void WriteLog(string data)
        {
            var message = $"[{DateTime.Now}] {data}";
            NonBlockingConsole.WriteLine(message);
        }

        /// <summary>
        ///     Writes an entry to the log with a date stamp & event origin user.
        /// </summary>
        /// <param name="originClient">The <see cref="User" /> that triggered this event.</param>
        /// <param name="data">The message to write to the log.</param>
        internal static void WriteLog(User originClient, string data)
            => WriteLog($"Client {originClient.GetFullUsername()} {data}");

        public static Version GetVersion() => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        ///		Toggles the ignore state for a user, globally or per-channel.
        /// </summary>
        /// <param name="uid">The user ID to toggle ignore for.</param>
        /// <param name="chid">The channel ID to ignore on. Omit for global.</param>
        /// <returns>True if user is now ignored, false if not.</returns>
        public static bool ToggleIgnore(ulong uid, ulong chid = 0)
        {
            var ignored = Permitted(UserFlag.Ignored, uid);
            if (ignored)
                Program.BotInstance.UserFlags[uid, chid] ^= UserFlag.Ignored;
            else
                Program.BotInstance.UserFlags[uid, chid] |= UserFlag.Ignored;

            return !ignored;
        }
    }
}
