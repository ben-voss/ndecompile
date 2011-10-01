
namespace LittleNet.NDecompile.Model
{
	public interface ICaseStatement : IStatement
	{
		IExpression Label
		{
			get;
		}

		IBlockStatement Statement
		{
			get;
		}
	}
}
