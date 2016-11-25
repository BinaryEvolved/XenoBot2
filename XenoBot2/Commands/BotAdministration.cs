using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Humanizer;
using XenoBot2.Shared;

namespace XenoBot2.Commands
{
    /// <summary>
    ///		Bot administration commands. Commands in this group can only be executed by the bot administrator.
    /// </summary>
    internal static class BotAdministration
    {
        /// <summary>
        ///		Terminates the bot.
        /// </summary>
        internal static async Task HaltBot(CommandInfo info, User author, Channel channel)
        {
            Utilities.WriteLog(author, "is shutting down the bot.");
            await channel.SendMessage("Bot shutting down.");
            Thread.Sleep(1.Seconds());
            await Program.BotInstance.Exit();
        }

        /// <summary>
        ///		Toggles the global ignore status for a user.
        /// </summary>
        internal static async Task GlobalIgnoreUser(CommandInfo info, User member, Channel channel)
        {
            if (!info.HasArguments)
            {
                Utilities.WriteLog(member, "tried to ignore, but forgot to say who to ignore.");
                await channel.SendMessage("You must specify who to ignore.");
                return;
            }

            var user = info.Arguments.First().GetMemberFromMention(channel);

            if (Utilities.ToggleIgnore(member.Id))
            {
                Utilities.WriteLog(member, $"ignored {user.GetFullUsername()} globally.");
                await channel.SendMessage($"Now ignoring {user.NicknameMention} everywhere.");
            }
            else
            {
                Utilities.WriteLog(member, $"unignored {user.GetFullUsername()} globally.");
                await
                    channel.SendMessage(
                        $"No longer ignoring {user.NicknameMention} everywhere (unless specifically ignored elsewhere).");
            }
        }
    }
}
