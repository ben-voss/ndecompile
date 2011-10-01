
namespace LittleNet.NDecompile.Model
{
	public interface IExpressionStatement : IStatement
	{
		IExpression Expression
		{
			get;
		}
	}
}
