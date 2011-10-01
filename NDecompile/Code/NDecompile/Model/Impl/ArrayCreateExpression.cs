using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ArrayCreateExpression : Expression, IArrayCreateExpression
	{
		private readonly List<IExpression> _dimensions;
		private readonly ITypeReference _type;
		private readonly IBlockExpression _initializer;

		public ArrayCreateExpression(ITypeReference type, List<IExpression> dimensions, IBlockExpression initializer)
		{
			_type = type;
			_dimensions = dimensions;
			_initializer = initializer;
		}

		public ITypeReference TypeReference
		{
			get
			{
				return _type;
			}
		}

		public IList<IExpression> Dimensions
		{
			get
			{
				return _dimensions;
			}
		}

		public IBlockExpression Initializer
		{
			get
			{
				return _initializer;
			}
		}
	}
}
