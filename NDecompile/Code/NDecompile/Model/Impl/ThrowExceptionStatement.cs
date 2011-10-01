
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ThrowExceptionStatement : Statement, IThrowExceptionStatement
	{
		private readonly IExpression _expression;

		public ThrowExceptionStatement(IExpression expression)
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
