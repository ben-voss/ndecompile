using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IAssembly : IAssemblyReference, IAttributeProvider
	{

		IList<IModule> Modules
		{
			get;
		}

		IList<IResource> Resources
		{
			get;
		}

		IList<IAssemblyReference> ReferencedAssemblies
		{
			get;
		}

        String FileName
        {
            get;
        }
	}
}