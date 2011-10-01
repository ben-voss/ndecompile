
namespace LittleNet.NDecompile.Model
{
	public interface IArgumentReferenceExpression : IExpression
	{
		IParameterReference Parameter
		{
			get;
		}

	}
}
