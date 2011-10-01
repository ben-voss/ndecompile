using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ArrayIndexerExpression : Expression, IArrayIndexerExpression
	{
		private readonly IExpression _array;
		private readonly IList<IExpression> _indexers = new List<IExpression>();

		public ArrayIndexerExpression(IExpression array, IExpression indexer)
		{
			_array = array;
			_indexers.Add(indexer);
		}

		public IExpression Array
		{
			get
			{
				return _array;
			}
		}

		public IList<IExpression> Indexers
		{
			get
			{
				return _indexers;
			}
		}
	}
}
