
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IObjectCreateExpression : IExpression
	{
		List<IExpression> Arguments
		{
			get;
		}

		IMethodReference Constructor
		{
			get;
		}

		ITypeReference TypeReference
		{
			get;
		}
	}
}
