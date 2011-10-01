
namespace LittleNet.NDecompile.Model.Impl
{
	internal class AddressOfExpression : Expression, IAddressOfExpression
	{
		private readonly IExpression _expression;

		public AddressOfExpression(IExpression expression)
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
