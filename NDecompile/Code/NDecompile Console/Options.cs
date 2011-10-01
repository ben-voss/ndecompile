using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Console
{
	/// <summary>
	/// 
	/// </summary>
	internal class Options
	{
		#region Fields

		private Language _language;

		private readonly List<String> _assemblies = new List<String>();

		#endregion

		public Language Language
		{
			get
			{
				return _language;
			}
			set
			{
				_language = value;
			}
		}

		public List<String> Assemblies
		{
			get
			{
				return _assemblies;
			}
		}
	}
}