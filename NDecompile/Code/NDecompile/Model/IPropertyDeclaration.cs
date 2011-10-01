
namespace LittleNet.NDecompile.Model
{
	public interface IPropertyDeclaration : IPropertyReference, IMemberDeclaration
	{
		ITypeReference DeclaringType
		{
			get;
		}

		IMethodReference GetMethod
		{
			get;
		}

		IMethodReference SetMethod
		{
			get;
		}
	}
}
