using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class PostDecrement : Idiom
	{
		// ldarg
		// dup
		// ldc.i1
		// sub
		// starg

		public override void Process(IList<IInstruction> instructions, int index)
		{
			if (index + 4 > instructions.Count)
				return;

			IInstruction instruction1 = instructions[index];
			IInstruction instruction2 = instructions[index + 1];
			IInstruction instruction3 = instructions[index + 2];
			IInstruction instruction4 = instructions[index + 3];

			if ((instruction1.OpCode == OpCodes.Ldarg) &&
				(instruction2.OpCode == OpCodes.Ldc_I4) &&
				(instruction3.OpCode == OpCodes.Sub) &&
				(instruction4.OpCode == OpCodes.Starg) &&
				((ushort)instruction4.Argument == (ushort)instruction1.Argument))
			{
				instructions[index + 1] = new Instruction(OpCodeTable.PostDecrementStatementOpCode, instruction1.IP, (int)instruction2.Argument);
				instructions.Remove(instruction3);
				instructions.Remove(instruction4);
			}

			if (index + 5 > instructions.Count)
				return;

			instruction1 = instructions[index];
			instruction2 = instructions[index + 1];
			instruction3 = instructions[index + 2];
			instruction4 = instructions[index + 3];
			IInstruction instruction5 = instructions[index + 4];

			if ((instruction1.OpCode == OpCodes.Ldarg) &&
				(instruction2.OpCode == OpCodes.Dup) &&
				(instruction3.OpCode == OpCodes.Ldc_I4) &&
				(instruction4.OpCode == OpCodes.Sub) &&
				(instruction5.OpCode == OpCodes.Starg) &&
				((ushort)instruction5.Argument == (ushort)instruction1.Argument))
			{
				instructions[index + 1] = new Instruction(OpCodeTable.PostDecrementOpCode, instruction1.IP, (int)instruction3.Argument);
				instructions.Remove(instruction3);
				instructions.Remove(instruction4);
				instructions.Remove(instruction5);
			}

			if ((instruction1.OpCode == OpCodes.Ldarg) &&
				(instruction2.OpCode == OpCodes.Ldc_I4) &&
				(instruction3.OpCode == OpCodes.Sub) &&
				(instruction4.OpCode == OpCodes.Dup) &&
				(instruction5.OpCode == OpCodes.Starg) &&
				((ushort)instruction5.Argument == (ushort)instruction1.Argument))
			{
				instructions[index + 1] = new Instruction(OpCodeTable.PostDecrementOpCode, instruction1.IP, (int)instruction2.Argument);
				instructions.Remove(instruction3);
				instructions.Remove(instruction4);
				instructions.Remove(instruction5);
			}
		}
	}
}
