﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XenoBot2.Shared;

namespace XenoBot2
{
	internal class CommandStore : IEnumerable<KeyValuePair<string,Command>>
	{
		public static CommandStore Commands;

		private readonly IDictionary<string, Command> _commands;

		public Command this[string id] => _commands[id.ToLower()];

		public void AddMany(IDictionary<string, Command> source) =>
			source.ToList().ForEach(x => _commands.Add(x.Key, x.Value));
		
		public void Add(string id, Command cmd) => _commands.Add(id, cmd);

		public bool Contains(string id) => _commands.ContainsKey(id.ToLower());

		public CommandStore()
		{
			_commands = new Dictionary<string, Command>();
		}

		public IEnumerator<KeyValuePair<string, Command>> GetEnumerator()
		{
			return _commands.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}