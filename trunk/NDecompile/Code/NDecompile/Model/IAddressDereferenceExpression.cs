
namespace LittleNet.NDecompile.Model
{
	public interface IAddressDereferenceExpression : IExpression
	{

		IExpression Expression
		{
			get;
		}
	}
}
