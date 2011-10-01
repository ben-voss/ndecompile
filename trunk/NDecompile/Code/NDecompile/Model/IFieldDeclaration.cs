
namespace LittleNet.NDecompile.Model
{
	public interface IFieldDeclaration : IFieldReference, IMemberDeclaration
	{

		ITypeReference DeclaringType
		{
			get;
		}

		ILiteralExpression Initialiser
		{
			get;
		}

		bool IsReadOnly
		{
			get;
		}

		FieldVisibility Visibility
		{
			get;
		}
	}
}
