
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ValueOfTypedReferenceExpression : Expression, IValueOfTypedReferenceExpression
	{
		private readonly IExpression _expression;
		private readonly ITypeReference _targetType;

		public ValueOfTypedReferenceExpression(IExpression expression, ITypeReference targetType)
		{
			_expression = expression;
			_targetType = targetType;
		}

		public IExpression Expression
		{
			get
			{
				return _expression;
			}
		}

		public ITypeReference TargetType
		{
			get
			{
				return _targetType;
			}
		}
	}
}
