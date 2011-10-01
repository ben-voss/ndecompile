
namespace LittleNet.NDecompile.Model
{
	public interface IThrowExceptionStatement : IStatement
	{

		IExpression Expression
		{
			get;
		}
	}
}
