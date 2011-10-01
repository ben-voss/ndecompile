using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class ArgumentSaveTest
	{
		[Test]
		public void TestStarg_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Starg_S, 0, (byte)42));

			new ArgumentSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Starg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}

	}
}
