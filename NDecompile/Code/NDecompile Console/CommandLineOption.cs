using System;

namespace LittleNet.NDecompile.Console
{
	/// <summary>
	/// A name value pair representing the value of a command line option
	/// </summary>
	public class CommandLineOption
	{
		#region Fields

		private readonly string _name;
		private readonly string _value;

		#endregion

		#region Constructors

		/// <summary>
		/// Initialises a new instance of the <see cref="CommandLineOption"/> class with the specified name and value
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public CommandLineOption(String name, String value) 
		{
			_name = name;
			_value = value;
		}

		#endregion

		/// <summary>
		/// Gets the name of the option
		/// </summary>
		public string Name 
		{ 
			get 
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the value of the option
		/// </summary>
		public string Value 
		{
			get 
			{
				return _value;
			}
		}
	}
}
