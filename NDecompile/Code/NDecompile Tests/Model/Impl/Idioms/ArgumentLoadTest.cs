using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class ArgumentLoadTest
	{
		[Test]
		public void TestLdarg_0()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg_0, 0, null));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(0, instructions[0].Argument);
		}

		[Test]
		public void TestLdarg_1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg_1, 0, null));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(1, instructions[0].Argument);
		}

		[Test]
		public void TestLdarg_2()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg_2, 0, null));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(2, instructions[0].Argument);
		}

		[Test]
		public void TestLdarg_3()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg_3, 0, null));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(3, instructions[0].Argument);
		}

		[Test]
		public void TestLdarg_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg_S, 0, (byte)42));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}

		[Test]
		public void TestLdarga_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarga_S, 0, (byte)42));

			new ArgumentLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarga.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}
	}
}
