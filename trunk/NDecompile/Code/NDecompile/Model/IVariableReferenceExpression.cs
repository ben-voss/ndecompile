
namespace LittleNet.NDecompile.Model
{
	public interface IVariableReferenceExpression : IExpression
	{
		IVariableReference VariableReference
		{
			get;
		}
	}
}
