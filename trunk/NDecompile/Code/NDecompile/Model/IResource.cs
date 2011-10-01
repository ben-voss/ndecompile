using System;

namespace LittleNet.NDecompile.Model
{
	public interface IResource : IResourceReference
	{
		Object Data
		{
			get;
		}

		IAssemblyReference Assembly
		{
			get;
		}
	}
}
