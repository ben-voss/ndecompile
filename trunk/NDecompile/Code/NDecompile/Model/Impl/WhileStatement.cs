﻿
namespace LittleNet.NDecompile.Model.Impl
{
	internal class WhileStatement : IWhileStatement
	{
		private readonly IBlockStatement _body;
		private readonly IExpression _condition;

		public WhileStatement(IExpression condition, IBlockStatement body)
		{
			_condition = condition;
			_body = body;
		}

		public IExpression Condition
		{
			get
			{
				return _condition;
			}
		}

		public IBlockStatement Body
		{
			get
			{
				return _body;
			}
		}
	}
}
