
namespace LittleNet.NDecompile.Model.Impl
{
	internal class AssignExpression : Expression, IAssignExpression
	{
		private readonly IExpression _expression;
		private readonly IExpression _target;

		public AssignExpression(IExpression expression, IExpression target)
		{
			_expression = expression;
			_target = target;
		}

		public IExpression Expression
		{
			get
			{
				return _expression;
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
