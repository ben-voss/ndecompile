
namespace LittleNet.NDecompile.Model.Impl
{
	internal class BinaryExpression : Expression, IBinaryExpression
	{
		private readonly IExpression _left;
		private readonly BinaryOperator _operator;
		private readonly IExpression _right;

		public BinaryExpression(IExpression left, BinaryOperator op, IExpression right)
		{
			_left = left;
			_operator = op;
			_right = right;
		}

		public IExpression Left
		{
			get
			{
				return _left;
			}
		}

		public BinaryOperator Operator
		{
			get
			{
				return _operator;
			}
		}

		public IExpression Right
		{
			get
			{
				return _right;
			}
		}
	}
}
