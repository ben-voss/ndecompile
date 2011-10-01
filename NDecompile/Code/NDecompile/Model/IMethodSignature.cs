using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IMethodSignature
	{
		ITypeReference ReturnType
		{
			get;
		}

		List<IParameterDeclaration> Parameters
		{
			get;
		}

	}
}