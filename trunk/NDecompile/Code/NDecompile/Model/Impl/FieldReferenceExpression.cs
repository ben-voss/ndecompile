
namespace LittleNet.NDecompile.Model.Impl
{
	internal class FieldReferenceExpression : Expression, IFieldReferenceExpression
	{
		private readonly IFieldReference _field;
		private readonly IExpression _target;

		public FieldReferenceExpression(IFieldReference field)
		{
			_field = field;
		}

		public FieldReferenceExpression(IFieldReference field, IExpression target)
		{
			_field = field;
			_target = target;
		}

		public IFieldReference Field
		{
			get
			{
				return _field;
			}
		}

		public IExpression Target
		{
			get
			{
				return _target;
			}
		}
	}
}
