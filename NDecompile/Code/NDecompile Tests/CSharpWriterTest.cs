using System;
using System.Reflection;
using System.Text;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using NUnit.Framework;

namespace LittleNet.NDecompile.Tests
{
	[TestFixture]
	public class CSharpWriterTest
	{

		[Test]
		public void TestLiteralStringExpression()
		{
			String result = TestUtils.WriteExpression(new LiteralExpression("Test"));
			Assert.AreEqual("\"Test\"", result);
		}

		[Test]
		public void TestLiteralBooleanExpression()
		{
            String result = TestUtils.WriteExpression(new LiteralExpression(true));
			Assert.AreEqual("true", result);
		}

		[Test]
		public void TestAssignExpression()
		{
            String result = TestUtils.WriteExpression(new AssignExpression(new LiteralExpression("Test"), new VariableReferenceExpression(new VariableDeclaration("Test", AssemblyManager.FindType(typeof(String), new Type[0])))));
			Assert.AreEqual("Test = \"Test\"", result);
		}

		[Test]
		public void TestBinaryExpression()
		{
            String result = TestUtils.WriteExpression(new BinaryExpression(new LiteralExpression(true), BinaryOperator.BitwiseAnd, new LiteralExpression(false)));
			Assert.AreEqual("true & false", result);
		}
	}
}