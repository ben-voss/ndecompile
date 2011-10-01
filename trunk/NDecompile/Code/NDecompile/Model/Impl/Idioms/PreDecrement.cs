using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class PreDerement : Idiom
	{
		// ldarg
		// ldc.i4
		// sub
		// dup
		// starg

		public override void Process(IList<IInstruction> instructions, int index)
		{
			if (index + 5 > instructions.Count)
				return;

			IInstruction instruction1 = instructions[index];
			IInstruction instruction2 = instructions[index + 1];
			IInstruction instruction3 = instructions[index + 2];
			IInstruction instruction4 = instructions[index + 3];
			IInstruction instruction5 = instructions[index + 4];

			if ((instruction1.OpCode == OpCodes.Ldarg) &&
				(instruction2.OpCode == OpCodes.Ldc_I4) &&
				((int)instruction2.Argument == 1) &&
				(instruction3.OpCode == OpCodes.Sub) &&
				(instruction4.OpCode == OpCodes.Dup) &&
				(instruction5.OpCode == OpCodes.Starg) &&
				((ushort)instruction5.Argument == (ushort)instruction1.Argument))
			{
				instructions[index + 1] = new Instruction(OpCodeTable.PreDecrementOpCode, instruction1.IP, 1);
				instructions.Remove(instruction3);
				instructions.Remove(instruction4);
				instructions.Remove(instruction5);
			}
		}
	}
}
