
namespace LittleNet.NDecompile.Model
{
	public interface IParameterDeclaration : IParameterReference, IAttributeProvider
	{
		ITypeReference ParameterType
		{
			get;
		}

        bool IsOut
        {
            get;
        }

        bool IsByRef
        {
            get;
        }
	}
}