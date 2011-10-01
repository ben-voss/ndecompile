
namespace LittleNet.NDecompile.Model
{
	public interface IMethodReturnStatement : IStatement
	{

		IExpression Expression
		{
			get;
		}

	}
}
