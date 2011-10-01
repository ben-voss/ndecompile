using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class TryCatchFinallyStatement : Statement, ITryCatchFinallyStatement
	{
		private readonly IList<ICatchClause> _catchClauses = new List<ICatchClause>();

		private readonly IBlockStatement _fault = new BlockStatement(new List<IStatement>());

		private readonly IBlockStatement _finally = new BlockStatement(new List<IStatement>());

		private readonly IBlockStatement _try = new BlockStatement(new List<IStatement>());

		public IList<ICatchClause> CatchClauses
		{
			get
			{
				return _catchClauses;
			}
		}

		public IBlockStatement Fault
		{
			get
			{
				return _fault;
			}
		}

		public IBlockStatement Finally
		{
			get
			{
				return _finally;
			}
		}

		public IBlockStatement Try
		{
			get
			{
				return _try;
			}
		}
	}
}
