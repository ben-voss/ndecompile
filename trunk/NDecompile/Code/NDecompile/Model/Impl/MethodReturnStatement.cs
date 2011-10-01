
namespace LittleNet.NDecompile.Model.Impl
{
	internal class MethodReturnStatement : Statement, IMethodReturnStatement
	{
		private readonly IExpression _expression;

		public MethodReturnStatement()
		{
		}

		public MethodReturnStatement(IExpression expression)
		{
			_expression = expression;
		}

		public IExpression Expression
		{
			get
			{
				return _expression;
			}
		}
	}
}
