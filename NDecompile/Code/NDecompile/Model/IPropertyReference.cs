using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IPropertyReference : IMemberReference
	{
		IPropertyDeclaration Resolve ();

		List<IParameterDeclaration> Parameters
		{
			get;
		}

		ITypeReference PropertyType
		{
			get;
		}
	}
}
