
namespace LittleNet.NDecompile.Model.Impl
{
	internal class AddressDereferenceExpression : Expression, IAddressDereferenceExpression
	{
		private readonly IExpression _expression;
		
		public AddressDereferenceExpression(IExpression expression)
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
