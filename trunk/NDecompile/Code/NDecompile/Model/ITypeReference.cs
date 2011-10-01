using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface ITypeReference : IMemberReference
	{
		ITypeDeclaration Resolve ();

		String Namespace
		{
			get;
		}

		IAssembly Assembly
		{
			get;
		}

        bool IsGenericParameter
        {
            get;
        }

		bool IsValueType
		{
			get;
		}

		bool IsArray
		{
			get;
		}

		bool IsGeneric
		{
			get;
		}

		IList<ITypeReference> GenericArguments
		{
			get;
		}
	}
}
