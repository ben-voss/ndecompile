
namespace LittleNet.NDecompile.Model
{
	public interface IValueOfTypedReferenceExpression : IExpression
	{
		IExpression Expression
		{
			get;
		}

		ITypeReference TargetType
		{
			get;
		}
	}
}
