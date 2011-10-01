using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class BlockStatement : Statement, IBlockStatement
	{
		private readonly List<IStatement> _statements = new List<IStatement>();

		public BlockStatement(List<IStatement> statements)
		{
			_statements = statements;
		}

		public List<IStatement> Statements
		{
			get
			{
				return _statements;
			}
		}
	}
}
