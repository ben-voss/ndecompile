using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class ConstantLoad : Idiom
	{
		public override void Process(IList<IInstruction> instructions, int index)
		{
			IInstruction instruction = instructions[index];
			switch (instruction.OpCode.Value)
			{
				case 0x16: // Ldc_I4_0
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 0);
						break;
					}
				case 0x17: // Ldc_I4_1
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 1);
						break;
					}
				case 0x18: // Ldc_I4_2
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 2);
						break;
					}
				case 0x19: // Ldc_I4_3
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 3);
						break;
					}
				case 0x1a: // Ldc_I4_4
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 4);
						break;
					}
				case 0x1b: // Ldc_I4_5
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 5);
						break;
					}
				case 0x1c: // Ldc_I4_6
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 6);
						break;
					}
				case 0x1d: // Ldc_I4_7
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 7);
						break;
					}
				case 0x1e: // Ldc_I4_8
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, 8);
						break;
					}
				case 0x15: // Ldc_I4_M1
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, -1);
						break;
					}
				case 0x1f: // Ldc_I4_S
					{
						instructions[index] = new Instruction(OpCodes.Ldc_I4, instruction.IP, (int)((byte)instruction.Argument));
						break;
					}
			}
		}

	}
}
