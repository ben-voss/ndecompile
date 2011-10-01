
namespace LittleNet.NDecompile.Model
{
	public interface ICastExpression : IExpression
	{
		IExpression Expression
		{
			get;
		}

		ITypeReference TargetType
		{
			get;
		}
	}
}
