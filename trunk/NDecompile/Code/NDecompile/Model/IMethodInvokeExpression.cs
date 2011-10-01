using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IMethodInvokeExpression : IExpression
	{

		IMethodReferenceExpression Method
		{
			get;
		}

		List<IExpression> Arguments
		{
			get;
		}
	}
}
