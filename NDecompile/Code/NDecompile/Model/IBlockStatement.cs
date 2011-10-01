using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IBlockStatement : IStatement
	{

		List<IStatement> Statements
		{
			get;
		}

	}
}
