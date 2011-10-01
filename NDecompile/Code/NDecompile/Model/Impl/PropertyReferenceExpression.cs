
namespace LittleNet.NDecompile.Model.Impl
{
	internal class PropertyReferenceExpression : Expression, IPropertyReferenceExpression
	{
		private readonly IPropertyReference _property;
		private readonly IExpression _target;

		public PropertyReferenceExpression(IPropertyReference property, IExpression target)
		{
			_property = property;
			_target = target;
		}

		public IPropertyReference Property
		{
			get
			{
				return _property;
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
