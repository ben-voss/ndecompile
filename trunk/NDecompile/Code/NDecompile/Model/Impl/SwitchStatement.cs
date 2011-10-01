using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class SwitchStatement : Statement, ISwitchStatement
	{
		private readonly List<ICaseStatement> _cases;
		private readonly IExpression _condition;

		public SwitchStatement(IExpression condition, List<ICaseStatement> cases)
		{
			_condition = condition;
			_cases = cases;
		}

		public IExpression Condition
		{
			get
			{
				return _condition;
			}
		}

		public List<ICaseStatement> Cases
		{
			get
			{
				return _cases;
			}
		}
	}
}
