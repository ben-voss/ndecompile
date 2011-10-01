using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class FieldLoadTest
	{
		[Test]
		public void TestLdloc_0()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldloc_0, 0, null));

			new FieldLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(0, instructions[0].Argument);
		}
		[Test]
		public void TestLdloc_1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldloc_1, 0, null));

			new FieldLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(1, instructions[0].Argument);
		}
		[Test]
		public void TestLdloc_2()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldloc_2, 0, null));

			new FieldLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(2, instructions[0].Argument);
		}

		[Test]
		public void TestLdloc_3()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldloc_3, 0, null));

			new FieldLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(3, instructions[0].Argument);
		}

		[Test]
		public void TestLdloca_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldloca_S, 0, (byte)42));

			new FieldLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldloca.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}
	}
}
