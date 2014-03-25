using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RyuaNerin
{
	public class CommandLineParser
	{
		private Dictionary<string, int>		dicOptionKey;
		private List<string>				dicOptionDefaultValue;

		private Dictionary<int, string>		dicParsedOptionValue;
		private List<int>					dicParsedOption;
		private List<string>				dicParsedCommand;

		public CommandLineParser()
		{
			this.dicOptionKey			= new Dictionary<string, int>();
			this.dicOptionDefaultValue	= new List<string>();

			this.dicParsedOptionValue	= new Dictionary<int, string>();
			this.dicParsedOption		= new List<int>();
			this.dicParsedCommand		= new List<string>();

			this.isParsed = false;
		}

		public void AddOptions(params string[] key)
		{
			this.dicOptionDefaultValue.Add(null);

			int index = this.dicOptionDefaultValue.Count - 1;

			for (int i = 0; i < key.Length; i++)
				this.dicOptionKey.Add(key[i], index);
		}
		public void AddOptionsDefault(string defaultValue, params string[] key)
		{
			this.dicOptionDefaultValue.Add(defaultValue);

			int index = this.dicOptionDefaultValue.Count - 1;

			for (int i = 0; i < key.Length; i++)
				this.dicOptionKey.Add(key[i], index);
		}

		public bool GetOption(string key)
		{
			if (!this.dicOptionKey.ContainsKey(key))
				return false;

			return this.dicParsedOption.Contains(this.dicOptionKey[key]);
		}

		public string GetOptionValue(string key)
		{
			if (this.dicOptionKey.ContainsKey(key))
			{
				int index = this.dicOptionKey[key];

				if (this.dicParsedOptionValue.ContainsKey(index))
					return this.dicParsedOptionValue[index];
				else
					return this.dicOptionDefaultValue[index];
			}
			else
				return null;
		}
		public string GetOptionDefaultValue(string key)
		{
			if (this.dicOptionKey.ContainsKey(key))
			{
				int index = this.dicOptionKey[key];
				
				return this.dicOptionDefaultValue[index];
			}
			else
				return null;
		}

		public string[] GetCommand()
		{
			return this.dicParsedCommand.ToArray();
		}
		public string GetCommand(int index)
		{
			if ((index < 0) || (index > this.dicParsedCommand.Count))
				return null;

			return this.dicParsedCommand[index];
		}
		public int CommandLength
		{
			get { return this.dicParsedCommand.Count; }
		}

		public bool isParsed { get; internal set; }

		public bool Parse(string[] args)
		{
			try
			{
				int i = 0;

				while (i < args.Length)
				{
					if (this.dicOptionKey.ContainsKey(args[i]))
					{
						int index = this.dicOptionKey[args[i]];

						if (this.dicOptionDefaultValue[index] == null)
							this.dicParsedOption.Add(index);
						else
							this.dicParsedOptionValue.Add(index, args[i + 1]);

						i += 2;
					}
					else
					{
						this.dicParsedCommand.Add(args[i]);
						i++;
					}
				}

				this.isParsed = true;
				return true;	
			}
			catch
			{
				this.isParsed = false;
				return false;
			}
		}
	}
}
