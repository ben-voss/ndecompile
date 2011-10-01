using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model.Impl.Idioms;
using NUnit.Framework;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	[TestFixture]
	public class PostIncrementTest
	{
		[Test]
		public void TestPostIncrementStatement()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg, 0, (ushort)0));
			instructions.Add(new Instruction(OpCodes.Ldc_I4, 1, 42));
			instructions.Add(new Instruction(OpCodes.Add, 2, 1));
			instructions.Add(new Instruction(OpCodes.Starg, 3, (ushort)0));

			PostIncrement idiom = new PostIncrement();
			idiom.Process(instructions, 0);

			Assert.AreEqual(2, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(OpCodeTable.PostIncrementStatementOpCode.Value, instructions[1].OpCode.Value);
			Assert.AreEqual(42, instructions[1].Argument);
		}

		[Test]
		public void TestPostIncrementOpCode1()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg, 0, (ushort)0));
			instructions.Add(new Instruction(OpCodes.Dup, 1, 1));
			instructions.Add(new Instruction(OpCodes.Ldc_I4, 2, 42));
			instructions.Add(new Instruction(OpCodes.Add, 3, 1));
			instructions.Add(new Instruction(OpCodes.Starg, 4, (ushort)0));

			PostIncrement idiom = new PostIncrement();
			idiom.Process(instructions, 0);

			Assert.AreEqual(2, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(OpCodeTable.PostIncrementOpCode.Value, instructions[1].OpCode.Value);
			Assert.AreEqual(42, instructions[1].Argument);
		}

		[Test]
		public void TestPostIncrementOpCode2()
		{
			List<IInstruction> instructions = new List<IInstruction>();
			instructions.Add(new Instruction(OpCodes.Ldarg, 0, (ushort)0));
			instructions.Add(new Instruction(OpCodes.Ldc_I4, 2, 42));
			instructions.Add(new Instruction(OpCodes.Add, 3, 1));
			instructions.Add(new Instruction(OpCodes.Dup, 1, 1));
			instructions.Add(new Instruction(OpCodes.Starg, 4, (ushort)0));

			PostIncrement idiom = new PostIncrement();
			idiom.Process(instructions, 0);

			Assert.AreEqual(2, instructions.Count);
			Assert.AreEqual(OpCodes.Ldarg.Value, instructions[0].OpCode.Value);
			Assert.AreEqual(OpCodeTable.PostIncrementOpCode.Value, instructions[1].OpCode.Value);
			Assert.AreEqual(42, instructions[1].Argument);
		}

		[Test]
		public void TestDecompilePostIncrement1()
		{
			if (TestUtils.IsDebug)
				Assert.AreEqual("Int32 int;" + Environment.NewLine + "int = a++;" + Environment.NewLine + "return int;" + Environment.NewLine, Decompile(("PostIncrement1")));
			else
				Assert.AreEqual("return a++;" + Environment.NewLine, Decompile(("PostIncrement1")));
		}

		[Test]
		public void TestDecompilePostIncrement2()
		{
			if (TestUtils.IsDebug)
				Assert.AreEqual("Int32 int;" + Environment.NewLine + "int = a++;" + Environment.NewLine + "return int;" + Environment.NewLine, Decompile(("PostIncrement1")));
			else
				Assert.AreEqual("return a += 2;" + Environment.NewLine, Decompile(("PostIncrement2")));
		}

		[Test]
		public void TestDecompilePostIncrement3()
		{
			Assert.AreEqual("a++;" + Environment.NewLine + "return;" + Environment.NewLine, Decompile(("PostIncrement3")));
		}

		[Test]
		public void TestDecompilePostIncrement4()
		{
			Assert.AreEqual("a += 2;" + Environment.NewLine + "return;" + Environment.NewLine, Decompile(("PostIncrement4")));
		}

		private static String Decompile(String method)
		{
			IMethodReference methodReference = AssemblyManager.FindMethod(typeof(PostIncrementTest).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static));
			Assert.IsNotNull(methodReference);

			StringBuilder result = new StringBuilder();
			foreach (IStatement statement in methodReference.Resolve().Body.Statements)
				result.Append(TestUtils.WriteStatement(statement));

			return result.ToString();
		}

		internal static int PostIncrement1(int a)
		{
			return a++;
		}

		internal static int PostIncrement2(int a)
		{
			return a += 2;
		}

		internal static void PostIncrement3(int a)
		{
			a++;
		}

		internal static void PostIncrement4(int a)
		{
			a += 2;
		}
	}
}