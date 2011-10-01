
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ConditionExpression : Expression, IConditionExpression
	{
		private readonly IExpression _condition;
		private readonly IExpression _then;
		private readonly IExpression _else;

		public ConditionExpression(IExpression condition, IExpression thenExp, IExpression elseExp)
		{
			_condition = condition;
			_then = thenExp;
			_else = elseExp;
		}

		public IExpression Condition
		{
			get
			{
				return _condition;
			}
		}

		public IExpression Then
		{
			get
			{
				return _then;
			}
		}

		public IExpression Else
		{
			get
			{
				return _else;
			}
		}
	}
}
