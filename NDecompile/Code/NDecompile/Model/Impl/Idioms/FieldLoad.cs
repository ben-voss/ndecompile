using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class FieldLoad : Idiom
	{
		public override void Process(IList<IInstruction> instructions, int index)
		{
			IInstruction instruction = instructions[index];
			switch (instruction.OpCode.Value)
			{
				case 0x06: // Ldloc_0
					{
						instructions[index] = new Instruction(OpCodes.Ldloc, instruction.IP, (ushort)0);
						break;
					}

				case 0x07: // Ldloc_1
					{
						instructions[index] = new Instruction(OpCodes.Ldloc, instruction.IP, (ushort)1);
						break;
					}

				case 0x08: // Ldloc_2
					{
						instructions[index] = new Instruction(OpCodes.Ldloc, instruction.IP, (ushort)2);
						break;
					}

				case 0x09: // Ldloc_3
					{
						instructions[index] = new Instruction(OpCodes.Ldloc, instruction.IP, (ushort)3);
						break;
					}

				case 0x11: // Ldloc_S
					{
						instructions[index] = new Instruction(OpCodes.Ldloc, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}

				case 0x12: // Ldloca_s
					{
						instructions[index] = new Instruction(OpCodes.Ldloca, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}
			}
		}
	}
}
