
namespace LittleNet.NDecompile.Model.Impl
{
	internal class CaseStatement : Statement, ICaseStatement
	{
		private readonly IExpression _label;
		private readonly IBlockStatement _statement;

		public CaseStatement(IExpression label, IBlockStatement statement)
		{
			_label = label;
			_statement = statement;
		}

		public IExpression Label
		{
			get
			{
				return _label;
			}
		}

		public IBlockStatement Statement
		{
			get
			{
				return _statement;
			}
		}
	}
}