﻿using System;
using System.Collections.Generic;
using System.Linq;
using DiscordSharp.Objects;
using XenoBot2.Shared;

namespace XenoBot2
{
	/// <summary>
	///     Tracks command usage, permissions & rate limiting.
	/// </summary>
	internal class CombinedChannelCommandManager
	{
		public delegate void ChannelRegisteredEventDelegate(DiscordChannelBase channel);

		public delegate void CommandRegisteredEventDelegate(string registeredCommand, DiscordChannelBase targetChannel);

		private readonly Dictionary<string, Dictionary<string, CommandMetadata>> _commands =
			new Dictionary<string, Dictionary<string, CommandMetadata>>();

		public event CommandRegisteredEventDelegate CommandRegistered;

		public event ChannelRegisteredEventDelegate ChannelRegistered;

		public bool EnableCommand(string command, DiscordChannelBase targetChannel) =>
			SetCommandEnableState(command, targetChannel, true);
		

		public bool DisableCommand(string command, DiscordChannelBase targetChannel) =>
			SetCommandEnableState(command, targetChannel, false);

		private bool SetCommandEnableState(string command, DiscordChannelBase targetChannel, bool state)
		{
			if (!CommandStore.Commands.Contains(command))
			{
				// command does not exist
				throw new InvalidCommandException(command);
			}
			if (_commands.ContainsKey(targetChannel.ID) && _commands[targetChannel.ID].ContainsKey(command))
			{
				var data = _commands[targetChannel.ID][command];
				data.IsEnabled = state;
				_commands[targetChannel.ID][command] = data;
			}
			else
			{
				if (!RegisterCommand(command, targetChannel)) return false;
				var data = _commands[targetChannel.ID][command];
				data.IsEnabled = state;
				_commands[targetChannel.ID][command] = data;
			}
			return true;
		}

		/// <summary>
		/// Registers a command in the per-channel command list.
		/// </summary>
		/// <param name="command">The command keyword to register.</param>
		/// <param name="channel">The channel to associate the command to.</param>
		/// <returns>True if the command is registered, false otherwise.</returns>
		private bool RegisterCommand(string command, DiscordChannelBase channel)
		{
			if (!CommandStore.Commands.Contains(command))
			{
				// attempted to register an undefined command
				Utilities.WriteLog($"WARN: Attempted to register invalid command '{command}' on channel '{channel.GetName()}'!");
				return false;
			}
			if (!_commands.ContainsKey(channel.ID))
			{
				_commands.Add(channel.ID, new Dictionary<string, CommandMetadata>());
				ChannelRegistered?.Invoke(channel);
			}

			if (_commands[channel.ID].ContainsKey(command))
			{
				// command already defined
				//throw new ArgumentException($"Command '{command}' was already registered.");
				return true;
			}

			_commands[channel.ID].Add(command, new CommandMetadata(command));
			CommandRegistered?.Invoke(command, channel);
			return true;
		}

		public CommandInfo ParseCommand(string commandline, DiscordChannelBase channelContext)
		{
			var lineparts = commandline.Split(' ');

			// Register command
			if (CommandStore.Commands.Contains(lineparts[0]))
				RegisterCommand(lineparts[0], channelContext);

			if (!_commands.ContainsKey(channelContext.ID) || !_commands[channelContext.ID].ContainsKey(lineparts[0]))
			{
				// TODO: Intelligent response to lack of command
				return new CommandInfo
				{
					CommandText = null
				};
			}
			var commandData = new CommandInfo
			{
				Meta = _commands[channelContext.ID][lineparts[0]],
				CommandText = lineparts[0],
				Arguments = lineparts.Skip(1).ToList()
			};
			commandData.Status = commandData.Meta.GetStatus();

			//var cmdinfo = Command.CommandList[commandData.CommandText];

			if (CommandStore.Commands[commandData.CommandText].Disabled)
			{
				commandData.Status = CommandStatus.AdminDisabled;
			}

			if (!_commands[channelContext.ID][commandData.CommandText].IsEnabled)
			{
				commandData.Status = CommandStatus.Disabled;
			}

			if (commandData.Status != CommandStatus.Allowed) return commandData;
			var x = _commands[channelContext.ID][commandData.CommandText];
			x.LastUseTime = DateTime.Now;
			_commands[channelContext.ID][commandData.CommandText] = x;

			return commandData;
		}
	}
}