
namespace LittleNet.NDecompile.Model
{
	public interface IBinaryExpression : IExpression
	{

		IExpression Left
		{
			get;
		}

		BinaryOperator Operator
		{
			get;
		}

		IExpression Right
		{
			get;
		}
	}
}
