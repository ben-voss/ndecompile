
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IArrayIndexerExpression : IExpression
	{
		IExpression Array
		{
			get;
		}

		IList<IExpression> Indexers
		{
			get;
		}
	}
}
