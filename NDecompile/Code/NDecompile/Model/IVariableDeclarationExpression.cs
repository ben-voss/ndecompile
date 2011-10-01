
namespace LittleNet.NDecompile.Model
{
	public interface IVariableDeclarationExpression : IExpression
	{
		IVariableDeclaration Variable
		{
			get;
		}
	}
}
