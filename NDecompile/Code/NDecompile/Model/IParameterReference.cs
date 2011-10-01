using System;

namespace LittleNet.NDecompile.Model
{
	public interface IParameterReference
	{
		String Name
		{
			get;
		}

		IParameterDeclaration Resolve ();
	}
}
