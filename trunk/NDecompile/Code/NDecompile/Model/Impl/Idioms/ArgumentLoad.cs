using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	/// <summary>
	/// Subsitutes the variants of the ldarg instructions for the general case instruction
	/// this simplifies the test neccessary in other idoms
	/// </summary>
	internal class ArgumentLoad : Idiom
	{
		public override void Process(IList<IInstruction> instructions, int index)
		{
			IInstruction instruction = instructions[index];
			switch (instruction.OpCode.Value)
			{
				case 0x02: // Ldarg_0
					{
						instructions[index] = new Instruction(OpCodes.Ldarg, instruction.IP, (ushort)0);
						break;
					}

				case 0x03: //Ldarg_1
					{
						instructions[index] = new Instruction(OpCodes.Ldarg, instruction.IP, (ushort)1);
						break;
					}

				case 0x04: // Ldarg_2
					{
						instructions[index] = new Instruction(OpCodes.Ldarg, instruction.IP, (ushort)2);
						break;
					}

				case 0x05: //Ldarg_3
					{
						instructions[index] = new Instruction(OpCodes.Ldarg, instruction.IP, (ushort)3);
						break;
					}

				case 0x0e: // Ldarg_S
					{
						instructions[index] = new Instruction(OpCodes.Ldarg, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}

				case 0x0f: // Ldarga_S
					{
						instructions[index] = new Instruction(OpCodes.Ldarga, instruction.IP, (ushort)((byte)instruction.Argument));
						break;
					}
			}
		}
	}
}
