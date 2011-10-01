using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface ISwitchStatement : IStatement
	{

		IExpression Condition
		{
			get;
		}

		List<ICaseStatement> Cases
		{
			get;
		}
	}
}
