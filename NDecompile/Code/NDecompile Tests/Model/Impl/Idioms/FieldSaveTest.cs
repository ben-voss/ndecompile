using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class FieldSaveTest
	{
		[Test]
		public void TestStloc_0()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Stloc_0, 0, null));

			new FieldSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Stloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(0, instructions[0].Argument);
		}

		[Test]
		public void TestStloc_1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Stloc_1, 0, null));

			new FieldSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Stloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(1, instructions[0].Argument);
		}

		[Test]
		public void TestStloc_2()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Stloc_2, 0, null));

			new FieldSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Stloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(2, instructions[0].Argument);
		}

		[Test]
		public void TestStloc_3()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Stloc_3, 0, null));

			new FieldSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Stloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(3, instructions[0].Argument);
		}

		[Test]
		public void TestStloc_S()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Stloc_S, 0, (byte)42));

			new FieldSave().Process(instructions, 0);

			Assert.AreEqual(1, instructions.Count);
			Assert.AreEqual(OpCodes.Stloc.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(42, instructions[0].Argument);
		}

	}
}
