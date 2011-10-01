
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ExpressionStatement : Statement, IExpressionStatement
	{
		private readonly IExpression _expression;

		public ExpressionStatement(IExpression expression)
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
