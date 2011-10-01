
namespace LittleNet.NDecompile.Model
{
	public interface IDoStatement : IStatement
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
