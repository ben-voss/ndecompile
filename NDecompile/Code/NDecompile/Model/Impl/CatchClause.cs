using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class CatchClause : ICatchClause
	{
		private readonly IBlockStatement _body = new BlockStatement(new List<IStatement>());

		private readonly IExpression _condition;

		private readonly IVariableReference _variable;

		public CatchClause(IVariableReference variable, IExpression condition)
		{
			_variable = variable;
			_condition = condition;
		}

		public IBlockStatement Body
		{
			get
			{
				return _body;
			}
		}

		public IExpression Condition
		{
			get
			{
				return _condition;
			}
		}

		public IVariableReference Variable
		{
			get
			{
				return _variable;
			}
		}
	}
}
