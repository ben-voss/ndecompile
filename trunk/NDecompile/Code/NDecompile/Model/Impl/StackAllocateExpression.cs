
namespace LittleNet.NDecompile.Model.Impl
{
	internal class StackAllocateExpression : Expression, IStackAllocateExpression
	{
		private readonly IExpression _expression;

		public StackAllocateExpression(IExpression expression)
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
