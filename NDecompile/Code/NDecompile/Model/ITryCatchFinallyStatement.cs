using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface ITryCatchFinallyStatement : IStatement
	{
		IList<ICatchClause> CatchClauses
		{
			get;
		}

		IBlockStatement Fault
		{
			get;
		}

		IBlockStatement Finally
		{
			get;
		}

		IBlockStatement Try
		{
			get;
		}
	}
}
