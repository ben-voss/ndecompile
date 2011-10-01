using System;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class LiteralExpression : Expression, ILiteralExpression
	{
		private readonly object _value;

		public LiteralExpression(Object value)
		{
			_value = value;
		}

		public object Value
		{
			get
			{
				return _value;
			}
		}
	}
}