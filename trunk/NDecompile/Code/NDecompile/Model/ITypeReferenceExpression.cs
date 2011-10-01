
namespace LittleNet.NDecompile.Model
{
	public interface ITypeReferenceExpression : IExpression
	{
		ITypeReference TypeReference
		{
			get;
		}
	}
}