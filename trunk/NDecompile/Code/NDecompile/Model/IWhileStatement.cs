
namespace LittleNet.NDecompile.Model
{
	public interface IWhileStatement : IStatement
	{

		IExpression Condition
		{
			get;
		}

		IBlockStatement Body
		{
			get;
		}
	}
}
