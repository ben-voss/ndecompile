
namespace LittleNet.NDecompile.Model
{
	public interface IEventReference : IMemberReference
	{
		ITypeReference DeclaringType
		{
			get;
		}

		IEventDeclaration Resolve ();
	}
}
