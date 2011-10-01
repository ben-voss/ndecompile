
namespace LittleNet.NDecompile.Model
{
	public interface IUnaryExpression : IExpression
	{

		IExpression Expression
		{
			get;
		}

		UnaryOperator Operator
		{
			get;
		}
	}
}
