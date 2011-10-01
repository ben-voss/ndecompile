
namespace LittleNet.NDecompile.Model
{
	public interface IConditionStatement : IStatement
	{

		IExpression Expression
		{
			get;
		}

		IBlockStatement Then
		{
			get;
		}

		IBlockStatement Else
		{
			get;
		}
	}
}
