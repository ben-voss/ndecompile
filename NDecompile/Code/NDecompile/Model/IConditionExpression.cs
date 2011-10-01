
namespace LittleNet.NDecompile.Model
{
	public interface IConditionExpression : IExpression
	{
		IExpression Condition
		{
			get;
		}

		IExpression Then
		{
			get;
		}

		IExpression Else
		{
			get;
		}
	}
}
