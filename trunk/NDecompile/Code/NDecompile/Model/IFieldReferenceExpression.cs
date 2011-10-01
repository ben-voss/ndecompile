
namespace LittleNet.NDecompile.Model
{
	public interface IFieldReferenceExpression : IExpression
	{

		IFieldReference Field
		{
			get;
		}

		IExpression Target
		{
			get;
		}

	}
}
