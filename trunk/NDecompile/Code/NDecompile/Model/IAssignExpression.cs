
namespace LittleNet.NDecompile.Model
{
	public interface IAssignExpression : IExpression
	{
		IExpression Expression
		{
			get;
		}

		IExpression Target
		{
			get;
		}
	}
}
