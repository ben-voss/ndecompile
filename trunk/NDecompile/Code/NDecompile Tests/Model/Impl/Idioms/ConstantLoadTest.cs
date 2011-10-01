using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class ConstantLoadTest
	{
		[Test]
		public void TestLdc_I4_0()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_0, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(0, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_1, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(1, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_2()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_2, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(2, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_3()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_3, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(3, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_4()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_4, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(4, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_5()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_5, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(5, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_6()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_6, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(6, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_7()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_7, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(7, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_8()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_8, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(8, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_M1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_M1, 0, null));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(-1, instructions[0].Argument);
		}

		[Test]
		public void TestLdc_I4_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldc_I4_S, 0, (byte)42));

			new ConstantLoad().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Ldc_I4.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}
	}
}
