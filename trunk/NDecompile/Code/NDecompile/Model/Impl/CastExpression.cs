
namespace LittleNet.NDecompile.Model.Impl
{
	internal class CastExpression : Expression, ICastExpression
	{
		private readonly IExpression _expression;
		private readonly ITypeReference _targetType;

		public CastExpression(IExpression expression, ITypeReference targetType)
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
