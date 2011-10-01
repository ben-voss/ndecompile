
using System.Collections.Generic;
using System.Reflection.Emit;
using LittleNet.NDecompile.Model;
using NUnit.Framework;
using LittleNet.NDecompile.Model.Impl;

namespace LittleNet.NDecompile.Tests
{
	[TestFixture]
	public class ControlFlowGraphTest
	{
		private static ControlFlowGraph MakeTestGraph1()
		{
			SortedList<ushort, IInstruction> instructions = new SortedList<ushort, IInstruction>();

			instructions.Add(0, new Instruction(OpCodes.Nop, 0, null));
			instructions.Add(1, new Instruction(OpCodes.Br, 1, (ushort)2));
			instructions.Add(2, new Instruction(OpCodes.Brtrue_S, 2, (ushort)7));
			instructions.Add(3, new Instruction(OpCodes.Brtrue_S, 3, (ushort)5));
			instructions.Add(4, new Instruction(OpCodes.Br, 4, (ushort)6));
			instructions.Add(5, new Instruction(OpCodes.Brtrue_S, 5, (ushort)3));
			instructions.Add(6, new Instruction(OpCodes.Br, 6, (ushort)7));
			instructions.Add(7, new Instruction(OpCodes.Brtrue_S, 7, (ushort)2));
			instructions.Add(8, new Instruction(OpCodes.Ret, 8, null));

			//for (int i = 2; i < instructions.Count; i++)
			//	instructions.Values[i].IsTarget = true;

			//return new ControlFlowGraph(instructions);
			return null;

		}

		public void Test()
		{
			// I(1) = 1
			// I(2) = 2
			// I(3) = 3, 4, 5, 6
			// I(7) = 7, 8

			ControlFlowGraph g = MakeTestGraph1();

			Assert.AreEqual(0, g.Nodes[0].InEdges.Count);
			Assert.AreEqual(1, g.Nodes[0].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[0].BackEdges.Count);

			Assert.AreEqual(2, g.Nodes[1].InEdges.Count);
			Assert.AreEqual(2, g.Nodes[1].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[1].BackEdges.Count);

			Assert.AreEqual(2, g.Nodes[2].InEdges.Count);
			Assert.AreEqual(2, g.Nodes[2].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[2].BackEdges.Count);

			Assert.AreEqual(1, g.Nodes[3].InEdges.Count);
			Assert.AreEqual(1, g.Nodes[3].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[3].BackEdges.Count);

			Assert.AreEqual(1, g.Nodes[4].InEdges.Count);
			Assert.AreEqual(2, g.Nodes[4].OutEdges.Count);
			Assert.AreEqual(1, g.Nodes[4].BackEdges.Count);
			Assert.AreSame(g.Nodes[2], g.Nodes[4].BackEdges[0]);

			Assert.AreEqual(2, g.Nodes[5].InEdges.Count);
			Assert.AreEqual(1, g.Nodes[5].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[5].BackEdges.Count);

			Assert.AreEqual(2, g.Nodes[6].InEdges.Count);
			Assert.AreEqual(2, g.Nodes[6].OutEdges.Count);
			Assert.AreEqual(1, g.Nodes[6].BackEdges.Count);
			Assert.AreSame(g.Nodes[1], g.Nodes[6].BackEdges[0]);

			Assert.AreEqual(1, g.Nodes[7].InEdges.Count);
			Assert.AreEqual(0, g.Nodes[7].OutEdges.Count);
			Assert.AreEqual(0, g.Nodes[7].BackEdges.Count);

		}
	}
}
