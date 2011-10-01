
namespace LittleNet.NDecompile.Model.Impl
{
	internal class UnaryExpression : Expression, IUnaryExpression
	{
		private readonly IExpression _expression;
		private readonly UnaryOperator _operator;

		public UnaryExpression(IExpression expression, UnaryOperator op)
		{
			_expression = expression;
			_operator = op;
		}

		public IExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		public UnaryOperator Operator
		{
			get
			{
				return _operator;
			}
		}
	}
}
