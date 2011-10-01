
namespace LittleNet.NDecompile.Model
{
	public interface IMethodReferenceExpression : IExpression
	{
		IMethodReference Method
		{
			get;
		}

		IExpression Target
		{
			get;
		}
	}
}
