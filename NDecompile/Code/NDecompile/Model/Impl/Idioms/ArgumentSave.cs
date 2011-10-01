using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class ArgumentSave : Idiom
	{
		public override void Process(IList<IInstruction> instructions, int index)
		{
			IInstruction instruction = instructions[index];
			switch (instruction.OpCode.Value)
			{
				case 0x10: // Starg_S:
					{
						instructions[index] = new Instruction(OpCodes.Starg, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}
			}
		}
	}
}
