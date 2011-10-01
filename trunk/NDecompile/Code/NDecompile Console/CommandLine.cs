using System;
using System.Collections;
using System.Globalization;

namespace LittleNet.NDecompile.Console
{
	/// <summary>
	/// Parses a command line string into a list of argument flags and option key, value pairs
	/// </summary>
	public class CommandLine
	{
		#region Fields

		private readonly string[] _argumentList;
		private readonly CommandLineOption[] _optionList;

		private int _argumentCursor;
		private int _optionCursor;
		private readonly Abbreviations _validOptions; 

		#endregion

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the <see cref="CommandLine"/> class with the specified arguments and valid option specifiers
		/// </summary>
		/// <param name="args"></param>
		/// <param name="validOptions"></param>
		public CommandLine(String[] args, String[] validOptions) 
		{
			_validOptions = new Abbreviations(validOptions);
			ArrayList arguments = new ArrayList(args.Length);
			ArrayList options = new ArrayList(args.Length);

			for (int argumentIndex = 0; argumentIndex < args.Length; argumentIndex++)
			{
				if (args[argumentIndex].StartsWith("/") || args[argumentIndex].StartsWith("-"))
				{
					String optionName;
					int seperatorIndex = args[argumentIndex].IndexOfAny(new char[] { ':', '=' });
					if (seperatorIndex == -1)
						optionName = args[argumentIndex].Substring(1);
					else
						optionName = args[argumentIndex].Substring(1, seperatorIndex - 1);

					bool requiresValue;
					bool canHaveValue;
					optionName = _validOptions.Lookup(optionName, out requiresValue, out canHaveValue);
					
					if (!canHaveValue && (seperatorIndex != -1))
						throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, "The /{0} option does not require a value", optionName));

					if (requiresValue && (seperatorIndex == -1))
						throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, "The /{0} option requires a value", optionName));

					String optionValue = null;
					if (canHaveValue && (seperatorIndex != -1))
					{
						if (seperatorIndex == (args[argumentIndex].Length - 1))
						{
							if ((argumentIndex + 1) == args.Length)
							{
								throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, "The /{0} option requires a value", optionName));
							}
							if (args[argumentIndex + 1].StartsWith("/") || args[argumentIndex + 1].StartsWith("-"))
							{
								throw new ApplicationException(String.Format(CultureInfo.InvariantCulture, "The /{0} option requires a value", optionName));
							}
							optionValue = args[argumentIndex + 1];
							argumentIndex++;
						}
						else
						{
							optionValue = args[argumentIndex].Substring(seperatorIndex + 1);
						}
					}
					options.Add(new CommandLineOption(optionName, optionValue));
				}
				else
				{
					arguments.Add(args[argumentIndex]);
				}
			}
			
			_argumentList = (String[])arguments.ToArray(typeof(String));
			_optionList = (CommandLineOption[])options.ToArray(typeof(CommandLineOption));
		}

		#endregion

		/// <summary>
		/// Gets the next argument
		/// </summary>
		/// <returns></returns>
		public string GetNextArgument() 
		{
			if (_argumentCursor >= _argumentList.Length)
				return null;

			return _argumentList[_argumentCursor++];
		}

		/// <summary>
		/// Gets the next option
		/// </summary>
		/// <returns></returns>
		public CommandLineOption GetNextOption() 
		{
			if (_optionCursor >= _optionList.Length)
				return null;

			return _optionList[_optionCursor++];
		}

		/// <summary>
		/// Gets the number of arguments in the command line
		/// </summary>
		public int NumArguments
		{
			get 
			{
				return _argumentList.Length;
			}
		}

		/// <summary>
		/// Gets the number of options in the command line
		/// </summary>
		public int NumOptions 
		{
			get 
			{
				return _optionList.Length;
			}
		}

	}
}