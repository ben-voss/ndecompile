using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ObjectCreateExpression : Expression, IObjectCreateExpression
	{
		private readonly List<IExpression> _arguments;
		private readonly ITypeReference _type;
		private readonly IMethodReference _constructor;

		public ObjectCreateExpression(IMethodReference constructor, List<IExpression> argments, ITypeReference type)
		{
			_constructor = constructor;
			_arguments = argments;
			_type = type;
		}

		public List<IExpression> Arguments
		{
			get
			{
				return _arguments;
			}
		}

		public IMethodReference Constructor
		{
			get
			{
				return _constructor;
			}
		}

		public ITypeReference TypeReference
		{
			get
			{
				return _type;
			}
		}
	}
}
