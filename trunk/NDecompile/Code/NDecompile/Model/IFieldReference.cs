
namespace LittleNet.NDecompile.Model
{
	public interface IFieldReference : IMemberReference
	{
		IFieldDeclaration Resolve ();

		ITypeReference FieldType
		{
			get;
		}

		bool IsStatic
		{
			get;
		}

		bool IsConst
		{
			get;
		}
	}
}
