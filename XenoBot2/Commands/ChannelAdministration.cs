using System.Linq;
using System.Threading.Tasks;
using Discord;
using XenoBot2.Shared;

namespace XenoBot2.Commands
{
    internal static class ChannelAdministration
    {
        /// <summary>
        ///		Enables a disabled command for this channel.
        /// </summary>
        internal static async Task Enable(CommandInfo info, User member, Channel channel)
        {
            if (!info.HasArguments)
            {
                await channel.SendMessage("You must specify the command to enable.");
                return;
            }
            if (
                Program.BotInstance.Commands[info.Arguments.First()].ResolveCommand()
                    .Flags.HasFlag(CommandFlag.NonDisableable))
            {
                await channel.SendMessage("That command cannot be disabled.");
                return;
            }
            if (!Utilities.HasState(CommandState.Disabled, info.Arguments.First()))
            {
                await channel.SendMessage("That command is already enabled.");
                return;
            }
            await channel.SendMessage($"Enabled command '{info.Arguments.First()}' on this channel.");
            Program.BotInstance.CommandStateData[info.Arguments.First(), channel.Id] ^= CommandState.Disabled;
        }

        /// <summary>
        ///		Disables an enabled command for this channel.
        /// </summary>
        internal static async Task Disable(CommandInfo info, User member, Channel channel)
        {
            if (!info.HasArguments)
            {
                await channel.SendMessage("You must specify the command to disable.");
                return;
            }
            if (
                Program.BotInstance.Commands[info.Arguments.First()].ResolveCommand()
                    .Flags.HasFlag(CommandFlag.NonDisableable))
            {
                await channel.SendMessage("That command cannot be disabled.");
                return;
            }
            if (Utilities.HasState(CommandState.Disabled, info.Arguments.First()))
            {
                await channel.SendMessage("That command is already disabled.");
                return;
            }
            await channel.SendMessage($"Disabled command '{info.Arguments.First()}' on this channel.");
            Program.BotInstance.CommandStateData[info.Arguments.First(), channel.Id] |= CommandState.Disabled;
        }

        /// <summary>
        ///		Toggles the ignore state for a user for this channel.
        /// </summary>
        internal static async Task IgnoreUser(CommandInfo info, User member, Channel channel)
        {
            if (!info.HasArguments)
            {
                Utilities.WriteLog(member, "tried to ignore, but forgot to say who to ignore.");
                await channel.SendMessage("You must specify who to ignore.");
                return;
            }

            var user = info.Arguments.First().GetMemberFromMention(channel);

            if (Utilities.ToggleIgnore(member.Id, channel.Id))
            {
                Utilities.WriteLog(member, $"ignored {user.GetFullUsername()} on channel {channel.Name}");
                await channel.SendMessage($"Now ignoring {user.NicknameMention} on this channel.");
            }
            else
            {
                Utilities.WriteLog(member, $"unignored {user.GetFullUsername()} on channel {channel.Name}");
                await channel.SendMessage($"No longer ignoring {user.NicknameMention} on this channel.");
            }
        }
    }
}
