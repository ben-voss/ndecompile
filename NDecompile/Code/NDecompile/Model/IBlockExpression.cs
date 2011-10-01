using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IBlockExpression : IExpression
	{
		List<IExpression> Expressions
		{
			get;
		}
	}
}
