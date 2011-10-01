using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IArrayCreateExpression : IExpression
	{

		ITypeReference TypeReference
		{
			get;
		}

		IList<IExpression> Dimensions
		{
			get;
		}

		IBlockExpression Initializer
		{
			get;
		}
	}
}
