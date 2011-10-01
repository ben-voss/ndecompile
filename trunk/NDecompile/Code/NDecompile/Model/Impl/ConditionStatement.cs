
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ConditionStatement : Statement, IConditionStatement
	{
		private readonly IExpression _expression;
		private readonly IBlockStatement _then;
		private readonly IBlockStatement _else;

		public ConditionStatement(IExpression expression, IBlockStatement thenStatement, IBlockStatement elseStatement)
		{
			_expression = expression;
			_then = thenStatement;
			_else = elseStatement;
		}

		public IExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		public IBlockStatement Then
		{
			get
			{
				return _then;
			}
		}

		public IBlockStatement Else
		{
			get
			{
				return _else;
			}
		}
	}
}
