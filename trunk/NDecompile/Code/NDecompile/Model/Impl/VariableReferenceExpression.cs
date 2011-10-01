
namespace LittleNet.NDecompile.Model.Impl
{
	internal class VariableReferenceExpression : Expression, IVariableReferenceExpression
	{
		private readonly IVariableReference _variableReference;

		public VariableReferenceExpression(IVariableReference variableReference)
		{
			_variableReference = variableReference;
		}

		public IVariableReference VariableReference
		{
			get
			{
				return _variableReference;
			}
		}
	}
}
