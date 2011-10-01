
namespace LittleNet.NDecompile.Model
{
	public interface IAddressOfExpression : IExpression
	{
		IExpression Expression
		{
			get;
		}
	}
}
