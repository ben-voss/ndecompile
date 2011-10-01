using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal class FieldSave : Idiom
	{
		public override void Process(IList<IInstruction> instructions, int index)
		{
			IInstruction instruction = instructions[index];
			switch (instruction.OpCode.Value)
			{

				case 0x0a: // Stloc_0
				{
					instructions[index] = new Instruction(OpCodes.Stloc, instruction.IP, (ushort)0);
					break;
				}

				case 0x0b: // Stloc_1
				{
					instructions[index] = new Instruction(OpCodes.Stloc, instruction.IP, (ushort)1);
					break;
				}

				case 0x0c: // Stloc_2
				{
					instructions[index] = new Instruction(OpCodes.Stloc, instruction.IP, (ushort)2);
					break;
				}

				case 0x0d: // Stloc_3
				{
					instructions[index] = new Instruction(OpCodes.Stloc, instruction.IP, (ushort)3);
					break;
				}

				case 0x13: // Stloc_S
					{
						instructions[index] = new Instruction(OpCodes.Stloc, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}
			}
		}
	}
}
