﻿using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using XenoBot2.Data;
using XenoBot2.Shared;

namespace XenoBot2.Commands
{
	/// <summary>
	///		Core bot commands. This group cannot be disabled.
	/// </summary>
	internal static class Base
	{
		internal static async Task Help(CommandInfo info, User author, Channel channel)
		{
			if (!info.HasArguments)
			{
				Utilities.WriteLog(author, "requested help index.");

				Action<string> send = s =>
				{
					channel.SendMessage(s);
					Thread.Sleep(100);
				};

				send(Messages.HelpTextHeader);
				// show list of commands + shorthelp

				var isAdmin = Utilities.Permitted(UserFlag.Administrator, author, channel);

				var giter = 0;

				var helpLines = from item in Program.BotInstance.Commands
								where isAdmin || !item.Value.Flags.HasFlag(CommandFlag.Hidden)
								let num = giter++
								group GenerateHelpEntry(item.Value, item.Key) by num / 10 into items
								select items;

				var builder = new StringBuilder();

				foreach (var i in helpLines)
				{
					builder.Clear();
					i.Aggregate(builder, (b, s) => b.AppendLine(s));
					send(builder.ToString());
				}

			}
			else
			{
				if (!Program.BotInstance.Commands.Contains(info.Arguments[0])) return;
				var cmdmeta = Program.BotInstance.Commands[info.Arguments[0]].ResolveCommand();
				if (string.IsNullOrEmpty(cmdmeta.LongHelpText))
				{
					Utilities.WriteLog(author, $"requested non-existent help page '{info.Arguments[0]}'");
					await channel.SendMessage("That page does not exist.");
				}
				else
				{
					Utilities.WriteLog(author, $"requested help page '{info.Arguments[0]}'");
					var builder = new StringBuilder();
					builder.AppendLine($"Help - {info.Arguments[0]}\n```");
					builder.AppendLine(cmdmeta.LongHelpText);
					builder.AppendLine("```");
					await channel.SendMessage(builder.ToString());
				}
			}
		}

		private static string GenerateHelpEntry(Command cmd, string cmdname)
		{
			var builder = new StringBuilder();

			var helptext = cmd.HelpText;
			if (cmd.AliasFor != null)
				helptext = cmd.ResolveCommand().HelpText;
			
			builder.Append($"{cmdname} - {cmd.HelpCategory}");

			if (cmd.AliasFor != null)
				builder.AppendLine($"(Alias For {cmd.AliasFor})");
			else
				builder.AppendLine();

			builder.AppendLine($"Required Permissions: {cmd.Permission}");

			if (!string.IsNullOrWhiteSpace(helptext))
			{
				builder.Append(" -> ");
				builder.AppendLine(helptext);	
			}
			
			return builder.ToString();
		}

		internal static async Task Version(CommandInfo info, User author, Channel channel)
		{
			Utilities.WriteLog(author, "requested bot version.");
			await channel.SendMessage($"XenoBot2 v{Utilities.GetVersion()}");
		}
	}
}
