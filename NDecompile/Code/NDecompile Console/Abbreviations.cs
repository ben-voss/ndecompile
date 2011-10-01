using System;

namespace LittleNet.NDecompile.Console
{
	/// <summary>
	/// Summary description for Abbreviations.
	/// </summary>
	public class Abbreviations
	{
		private readonly string[] _options;
		private readonly bool[] _canHaveValue;
		private readonly bool[] _requiresValue;

		/// <summary>
		/// Initialises a new instance of the <see cref="Abbreviations"/> class with the specified options
		/// </summary>
		/// <param name="options"></param>
		public Abbreviations(String[] options) 
		{
			_options = new String[options.Length];
			_canHaveValue = new bool[options.Length];
			_requiresValue = new bool[options.Length];

			for (int i = 0; i < options.Length; i++)
			{
				String option = options[i].ToLowerInvariant();
				if (option.StartsWith("*"))
				{
					// This option requires a mandatory value
					_requiresValue[i] = true;
					_canHaveValue[i] = true;
					option = option.Substring(1);
				}
				else if (option.StartsWith("+"))
				{
					// A + option does not require a value but can have one
					_requiresValue[i] = false;
					_canHaveValue[i] = true;
					option = option.Substring(1);
				}
				_options[i] = option;
			}
		}

		/// <summary>
		/// Lookups a option and returns its requirements
		/// </summary>
		/// <param name="optionName"></param>
		/// <param name="requiresValue"></param>
		/// <param name="canHaveValue"></param>
		/// <returns></returns>
		public String Lookup(String optionName, out bool requiresValue, out bool canHaveValue) 
		{
			optionName = optionName.ToLowerInvariant();

			int foundIndex = -1;
			for (int i = 0; i < _options.Length; i++) 
			{
				if (_options[i] == optionName) 
				{
					requiresValue = _requiresValue[i];
					canHaveValue = _canHaveValue[i];
					return _options[i];
				} 
				
				if (_options[i].StartsWith(optionName)) 
				{
					if (foundIndex != -1)
						throw new ApplicationException("Ambigious option");

					foundIndex = i;
				}
			}

			if (foundIndex != -1) 
			{
				requiresValue = _requiresValue[foundIndex];
				canHaveValue = _canHaveValue[foundIndex];
				return _options[foundIndex];
			}

			throw new ApplicationException("Unknown option");
		}

	}
}
