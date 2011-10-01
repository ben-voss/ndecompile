using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
	public interface IMethodBody
	{

		int MaxStack
		{
			get;
		}

		IList<IInstruction> Instructions
		{
			get;
		}

		IControlFlowGraph ControlFlowGraph
		{
			get;
		}

		IList<IStatement> Statements
		{
			get;
		}

		bool InitVariables
		{
			get;
		}

		IList<IVariableDeclaration> Variables
		{
			get;
		}

		IMethodInvokeExpression Initialiser
		{
			get;
		}
	}
}