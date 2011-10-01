
namespace LittleNet.NDecompile.Model.Impl
{
	internal class VariableDeclarationExpression : Expression, IVariableDeclarationExpression
	{
		private readonly IVariableDeclaration _variable;

		public VariableDeclarationExpression(IVariableDeclaration variable)
		{
			_variable = variable;
		}

		public IVariableDeclaration Variable
		{
			get
			{
				return _variable;
			}
		}
	}
}
