using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Humanizer;
using XenoBot2.Data;
using XenoBot2.Shared;

namespace XenoBot2.Commands
{
	internal static class Utility
	{
		internal static async Task Echo(CommandInfo info, User author, Channel channel)
		{
			Utilities.WriteLog(author, "requested an echo.");
			await channel.SendMessage($"{author.Name} said: {string.Join(" ", info.Arguments)}");
		}

		internal static async Task Time(CommandInfo info, User author, Channel channel)
		{
			Utilities.WriteLog(author, "requested the time.");
			await channel.SendMessage($"The time is {DateTime.UtcNow.ToLongTimeString()} (UTC).");
		}

		internal static async Task Date(CommandInfo info, User author, Channel channel)
		{
			Utilities.WriteLog(author, "requested the date.");
			await channel.SendMessage($"It is {DateTime.UtcNow.ToLongDateString()} (UTC).");
		}

		internal static async Task Me(CommandInfo info, User origin, Channel channel)
		{
			Utilities.WriteLog(origin, "requested info about themselves.");
			await channel.SendMessage(GetInfoString(origin, channel));
		}

		private static string GetInfoString(User author, Channel channel)
			=> $"Information about: **{author.GetFullUsername()}**\n" +
			   "```\n" +
			   $"ID:                   {author.Id}\n" +
			   $"Bot:                  {author.IsBot}\n" +
			   $"Avatar:               {author.AvatarUrl}\n" +
			   $"User Flags (Channel): {Program.BotInstance.UserFlags[author.Id, channel.Id]}\n" +
			   $"User Flags (Global):  {Program.BotInstance.UserFlags[author.Id]}" +
			   "```";

		internal static async Task ConvertNumber(CommandInfo info, User author, Channel channel)
		{
			if (info.Arguments.LessThan(2))
			{
				await channel.SendMessage(Messages.CommandNotEnoughArguments);
				return;
			}
			int number;
			if (!int.TryParse(info.Arguments[1], out number))
			{
				await channel.SendMessage($"{info.Arguments[1]} is not a valid integer.");
				return;
			}
			try
			{
				switch (info.Arguments[0])
				{
					case "roman":
						await channel.SendMessage($"{number} in roman numerals is {number.ToRoman()}.");
						break;

					case "words":
						await channel.SendMessage($"{number} in words is {number.ToWords()}.");
						break;

					case "metric":
						await channel.SendMessage($"{number} in metric is {number.ToMetric()}.");
						break;

					case "wordord":
						await channel.SendMessage($"{number} is {number.ToOrdinalWords()}.");
						break;

					default:
						await channel.SendMessage($"Unknown conversion type *{info.Arguments[0]}*.");
						break;
				}
			}
			catch (ArgumentOutOfRangeException)
			{
				await channel.SendMessage("Error: number out of range.");
			}
		}

		internal static async Task UserInfo(CommandInfo info, User member, Channel channel)
		{
			var author = !info.HasArguments 
				? member 
				: info.Arguments.First().GetMemberFromMention(channel);
			if (author == null)
			{
				await channel.SendMessage("User does not exist.");
				return;
			}
			await channel.SendMessage(GetInfoString(author, channel));
		}

		internal static async Task Avatar(CommandInfo info, User member, Channel channel)
		{
			var author = !info.HasArguments
				? member
				: info.Arguments[0].GetMemberFromMention(channel);
			if (author == null)
			{
				await channel.SendMessage("User does not exist.");
				return;
			}

			await channel.SendMessage(
				string.IsNullOrWhiteSpace(author.AvatarUrl)
					? "https://www.lohikar.io/i/xb/avatar_missing.jpg"
					: author.AvatarUrl);
		}

		internal static async Task Ping(CommandInfo info, User member, Channel channel)
		{
			Utilities.WriteLog(member, "requested a ping.");
			await channel.SendMessage(info.CommandText == "ping" ? "pong" : $"{info.CommandText} response");
		}
	}
}
