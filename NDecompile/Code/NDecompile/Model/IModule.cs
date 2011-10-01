using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IModule : IModuleReference
	{
		IAssemblyReference Assembly
		{
			get;
		}

		List<ITypeDeclaration> Types
		{
			get;
		}

	}
}