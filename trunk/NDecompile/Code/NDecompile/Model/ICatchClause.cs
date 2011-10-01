
namespace LittleNet.NDecompile.Model
{
	public interface ICatchClause
	{
		IBlockStatement Body
		{
			get;
		}

		IExpression Condition
		{
			get;
		}

		IVariableReference Variable
		{
			get;
		}
	}
}
