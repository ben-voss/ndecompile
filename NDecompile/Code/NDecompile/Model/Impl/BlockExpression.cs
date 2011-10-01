using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class BlockExpression : Expression, IBlockExpression
	{
		private readonly List<IExpression> _expressions = new List<IExpression>();

		public BlockExpression(List<IExpression> expressions)
		{
			_expressions = expressions;
		}

		public List<IExpression> Expressions
		{
			get
			{
				return _expressions;
			}
		}
	}
}
