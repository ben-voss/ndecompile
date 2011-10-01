
namespace LittleNet.NDecompile.Model.Impl
{
	internal class MethodReferenceExpression : Expression, IMethodReferenceExpression
	{
		private readonly IMethodReference _method;
		private readonly IExpression _target;

		public MethodReferenceExpression(IMethodReference method, IExpression target)
		{
			_method = method;
			_target = target;
		}

		public IMethodReference Method
		{
			get
			{
				return _method;
			}
		}

		public IExpression Target
		{
			get
			{
				return _target;
			}
		}
	}
}
