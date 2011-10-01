namespace LittleNet.NDecompile.Model
{
	public interface IStackAllocateExpression : IExpression
	{
		IExpression Expression
		{
			get;
		}
	}
}
