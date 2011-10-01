
namespace LittleNet.NDecompile.Model.Impl
{
	internal class ArgumentReferenceExpression : Expression, IArgumentReferenceExpression
	{
		private readonly IParameterReference _parameter;

		public ArgumentReferenceExpression(IParameterReference parameter)
		{
			_parameter = parameter;
		}

		public IParameterReference Parameter
		{
			get
			{
				return _parameter;
			}
		}
	}
}
