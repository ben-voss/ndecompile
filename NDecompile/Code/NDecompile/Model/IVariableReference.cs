using System;

namespace LittleNet.NDecompile.Model
{
	public interface IVariableReference
	{
		IVariableDeclaration Resolve ();

		String Name
		{
			get;
		}

		ITypeReference TypeReference
		{
			get;
		}
	}
}
