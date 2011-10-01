
namespace LittleNet.NDecompile.Model
{
	public interface ITypeOfExpression : IExpression
	{
		ITypeReference TypeReference
		{
			get;
		}
	}
}
