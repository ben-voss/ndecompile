using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class MethodInvokeExpression : Expression, IMethodInvokeExpression
	{
		private readonly List<IExpression> _arguments;
		private readonly IMethodReferenceExpression _method;

		public MethodInvokeExpression(IMethodReferenceExpression method, List<IExpression> arguments)
		{
			_method = method;
			_arguments = arguments;
		}

		public IMethodReferenceExpression Method
		{
			get
			{
				return _method;
			}
		}

		public List<IExpression> Arguments
		{
			get
			{
				return _arguments;
			}
		}
	}
}
