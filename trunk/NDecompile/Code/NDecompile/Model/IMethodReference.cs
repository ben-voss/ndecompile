
namespace LittleNet.NDecompile.Model
{
	public interface IMethodReference : IMemberReference, IMethodSignature
	{
		IMethodDeclaration Resolve();

		bool IsStatic
		{
			get;
		}
	}
}