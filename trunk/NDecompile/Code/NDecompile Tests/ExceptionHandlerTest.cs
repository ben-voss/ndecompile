using System;
using System.Collections.Generic;
using System.Reflection;
using LittleNet.NDecompile.Model;
using NUnit.Framework;

namespace LittleNet.NDecompile.Tests
{
	[TestFixture]
	public class ExceptionHandlerTest
	{
		private class Node
		{
			public String Type;
			public List<Node> OutEdges = new List<Node>();
			public List<Node> InEdges = new List<Node>();
			public ushort StartIP;
			public ushort EndIP;
			public List<IInstruction> Instructions = new List<IInstruction>();
			
			public Node FollowNode;
			public Node HandlerNode;
		}
		
		[Test]
		public void TestExceptionTryCatch() {
			Test("ExceptionTryCatchMethod");
		}
		
		[Test]
		public void TestTryCatch() {
			Test("TryCatchMethod");
		}
		
		[Test]
		public void TestTryCatchFinally() {
			Test("TryCatchFinallyMethod");
		}

		private void Test(String methodName)
		{
			MethodInfo method = typeof (ExceptionHandlerTest).GetMethod(methodName);
			MethodBody body = method.GetMethodBody();
			IMethodReference methodReference = AssemblyManager.FindMethod(method);

			ushort ip = 0;
			byte[] il = body.GetILAsByteArray();
			SortedList<ushort, IInstruction> instructions = new SortedList<ushort, IInstruction>();
			while (ip < il.Length)
				instructions.Add(ip, OpCodeTable.GetInstruction(il, ref ip, (NDecompile.Model.Impl.Module)methodReference.Resolve().DeclaringType.Resolve().Module, new Type[0], new Type[0]));


			foreach (ExceptionHandlingClause clause in body.ExceptionHandlingClauses)
			{
				Console.WriteLine(clause.Flags);
				Console.WriteLine(clause.TryOffset + " " + (clause.TryOffset + clause.TryLength));
				Console.WriteLine(clause.HandlerOffset + " " + (clause.HandlerOffset + clause.HandlerLength));
				Console.WriteLine();
			}

			Node rootNode = new Node();
			rootNode.StartIP = 0;
			rootNode.EndIP = (ushort)body.GetILAsByteArray().Length;
			rootNode.Type = "Return";
			rootNode.Instructions.AddRange(instructions.Values);
			
			List<Node> nodes = new List<Node>();
			nodes.Add(rootNode);

			// Add the exception information - build a sorted tree of clauses
			SortedList<int, TryHandler> clauses = new SortedList<int, TryHandler>();
			foreach (ExceptionHandlingClause clause in body.ExceptionHandlingClauses)
				Add(new TryHandler(clause), clauses);
			
			Print(clauses, 0);
			Console.WriteLine();
			
			foreach (TryHandler tryHandler in clauses.Values)
				TraverseExceptionTree (tryHandler, nodes);
			
			foreach(Node node in nodes) {
				Console.Write(node.StartIP + " " + node.EndIP + " " + node.Type);
				
				if (node.Type == "Try") {
					//Console.Write(" - Handler Node " + node.OutEdges[1].StartIP + " - Follow Node " + node.OutEdges[2].StartIP);
					Console.Write(" - Handler Node " + node.HandlerNode.StartIP + " - Follow Node " + node.FollowNode.StartIP);
				};
				
				Console.WriteLine();
				
				foreach (IInstruction instruction in node.Instructions)
					Console.WriteLine("    " + instruction.IP + " " + instruction.OpCode.Name);
			}
		}
		
		private static void TraverseExceptionTree(TryHandler tryHandler, List<Node> nodes) {
			// Find the right node to add the try node before
			Node tryNode = SplitNode(nodes, (ushort)tryHandler.TryStartIp, "Try");
			Node handlerNode = SplitNode(nodes, (ushort)tryHandler.HandlerStartIp, "Handler");
			Node followNode = SplitNode(nodes, (ushort)tryHandler.HandlerEndIp, "Follow");
			
			tryNode.HandlerNode = handlerNode.OutEdges[0];
			tryNode.FollowNode = followNode.OutEdges[0];

			// Split the children
			foreach (TryHandler childTryHandler in tryHandler.NestedTry.Values)
				TraverseExceptionTree(childTryHandler, nodes);
			
			foreach (TryHandler childTryHandler in tryHandler.NestedHandler.Values)
				TraverseExceptionTree(childTryHandler, nodes);
		}

		private static Node SplitNode(List<Node> nodes, ushort ip, String type) {
			foreach (Node node in nodes) {
				if ((node.StartIP <= ip) && (node.EndIP >= ip)) {
					// Found the node
					Node newNode = new Node();
					newNode.StartIP = ip;
					newNode.EndIP = node.EndIP;
					newNode.Type = node.Type;
					node.Type = type;
					
					// Move the instructions
					int i = 0;
					while ((node.Instructions.Count > i) && (node.Instructions[i].IP < ip))					
						i++;

					while (node.Instructions.Count > i)
					{
						newNode.Instructions.Add(node.Instructions[i]);
						node.Instructions.RemoveAt(i);	
					}
					
					if (node.Instructions.Count == 0)
						node.EndIP = node.StartIP;
					else
						node.EndIP = node.Instructions[node.Instructions.Count - 1].IP;
					
					// Move the out edges from the existing node to the new node
					newNode.OutEdges.AddRange(node.OutEdges);
					foreach (Node outNode in node.OutEdges) {
						outNode.InEdges.Remove(node);
						outNode.InEdges.Add(newNode);
					}	
					node.OutEdges.Clear();
					
					// Link the two nodes
					node.OutEdges.Add(newNode);
					newNode.InEdges.Add(node);
					
					// Add the new node after the existing node into the node list
					nodes.Insert(nodes.IndexOf(node) + 1, newNode);	
					
					return node;
				}
			}
			
			Console.WriteLine("Returning null");
			return null;	
		}
		
		/*
		private void Test(String methodName)
		{
			MethodInfo method = typeof (ExceptionHandlerTest).GetMethod(methodName);
			MethodBody body = method.GetMethodBody();
			IMethodReference methodReference = AssemblyManager.FindMethod(method);

			foreach (ExceptionHandlingClause clause in body.ExceptionHandlingClauses)
			{
				Console.WriteLine(clause.Flags);
				Console.WriteLine("Try Offset " + clause.TryOffset + " Try End " + (clause.TryOffset + clause.TryLength));
				Console.WriteLine("Handler Offset " + clause.HandlerOffset + " Handler End " + (clause.HandlerOffset + clause.HandlerLength));
				Console.WriteLine();
			}

			ushort ip = 0;
			byte[] il = body.GetILAsByteArray();
			SortedList<ushort, IInstruction> instructions = new SortedList<ushort,IInstruction>();
			while (ip < il.Length)
				instructions.Add(ip, OpCodeTable.GetInstruction(il, ref ip, (NDecompile.Model.Impl.Module)methodReference.Resolve().DeclaringType.Resolve().Module, new Type[0], new Type[0]));

			SortedList<int, TryHandler> clauses = new SortedList<int, TryHandler>();
			foreach (ExceptionHandlingClause clause in body.ExceptionHandlingClauses)
				Add(new TryHandler(clause), clauses);

			Print(clauses, 0);

			TryHandler root = new TryHandler(il.Length, clauses);


			foreach (IInstruction instruction in instructions.Values)
			{
				TryHandler handler = GetHandlerForIp(root, instruction.IP);

				if (handler != null)
					Console.WriteLine(handler.TryStartIp + " " + instruction.IP + " " + instruction.OpCode.Name);
				else
					Console.WriteLine("    " + instruction.IP + " " + instruction.OpCode.Name);
			}

		}


		private TryHandler GetHandlerForIp(TryHandler handler, ushort ip)
		{
			foreach (TryHandler clause in handler.NestedTry.Values)
				if ((ip >= clause.TryStartIp) && (ip <= clause.HandlerEndIp))
					return GetHandlerForIp(clause, ip);

			foreach (TryHandler clause in handler.NestedHandler.Values)
				if ((ip >= clause.TryStartIp) && (ip <= clause.HandlerEndIp))
					return GetHandlerForIp(clause, ip);

			return handler;
		}
*/
		private void Print(SortedList<int, TryHandler> clauses, int indent)
		{
			foreach (TryHandler tryCatchFinally in clauses.Values)
			{
				Console.WriteLine(new string(' ', indent) + "try {" + tryCatchFinally.TryStartIp);
				Print(tryCatchFinally.NestedTry, indent + 2);
				Console.WriteLine(new string(' ', indent) + "} " + tryCatchFinally.TryEndIp);
				Console.WriteLine(new string(' ', indent) + "handle {" + tryCatchFinally.HandlerStartIp);
				Print(tryCatchFinally.NestedHandler, indent + 2);
				Console.WriteLine(new string(' ', indent) + "} " + tryCatchFinally.HandlerEndIp);
			}
		}

		private static bool Add(TryHandler tryHandler, SortedList<int, TryHandler> clauses)
		{
			int i = 0;
			while (i < clauses.Values.Count)
			{
				TryHandler nestedHandler = clauses.Values[i];

				// Find any handlers in the list that need to become children of the new handler
				if ((tryHandler.TryStartIp <= nestedHandler.TryStartIp) &&
					(tryHandler.TryEndIp >= nestedHandler.HandlerEndIp))
				{
					tryHandler.NestedTry.Add(nestedHandler.TryStartIp, nestedHandler);
					clauses.Remove(nestedHandler.TryStartIp);
					continue;
				}

				if ((tryHandler.HandlerStartIp <= nestedHandler.TryStartIp) &&
					(tryHandler.HandlerEndIp >= nestedHandler.HandlerEndIp))
				{
					tryHandler.NestedHandler.Add(nestedHandler.TryStartIp, nestedHandler);
					clauses.Remove(nestedHandler.TryStartIp);
					continue;
				}

				i++;
			}

			for (i = 0; i < clauses.Values.Count;i++)
			{
				TryHandler nestedHandler = clauses.Values[i];
				
				// Find the parent of this handler
				if ((nestedHandler.TryStartIp >= tryHandler.TryStartIp) &&
				    (nestedHandler.TryEndIp <= tryHandler.HandlerEndIp))
					return Add(tryHandler, nestedHandler.NestedTry);

				if ((nestedHandler.HandlerStartIp >= tryHandler.TryStartIp) &&
				    (nestedHandler.HandlerEndIp <= tryHandler.HandlerEndIp))
					return Add(tryHandler, nestedHandler.NestedHandler);
			}

			clauses.Add(tryHandler.TryStartIp, tryHandler);
			return true;
		}


		private class TryHandler
		{
			public int TryStartIp;
			public int TryEndIp;

			public int HandlerStartIp;
			public int HandlerEndIp;

			public SortedList<int, TryHandler> NestedTry = new SortedList<int, TryHandler>();
			public SortedList<int, TryHandler> NestedHandler = new SortedList<int, TryHandler>();

			public TryHandler(ExceptionHandlingClause clause)
			{
				TryStartIp = clause.TryOffset;
				TryEndIp = clause.TryOffset + clause.TryLength;
				HandlerStartIp = clause.HandlerOffset;
				HandlerEndIp = clause.HandlerOffset + clause.HandlerLength;

				if (HandlerStartIp == 0)
					HandlerStartIp = -1;

				if (HandlerEndIp == 0)
					HandlerEndIp = -1;
			}

			public TryHandler(int endIp, SortedList<int, TryHandler> nestedTry)
			{
				TryStartIp = 0;
				TryEndIp = endIp;
				NestedTry = nestedTry;
				HandlerStartIp = -1;
				HandlerEndIp = -1;
			}
		}
		
		public void TryCatchMethod() {
			Console.WriteLine(1);
			try {
				Console.WriteLine(2);
			} catch {
				Console.WriteLine(3);
			}
			Console.WriteLine(4);
		}
		
		public void TryCatchFinallyMethod() {
			Console.WriteLine(1);
			try {
				Console.WriteLine(2);
			} catch {
				Console.WriteLine(3);
			} finally {
				Console.WriteLine(4);			
			}
			Console.WriteLine(5);
		}

		public void ExceptionTryCatchMethod() {
			Console.WriteLine(1);
			try {
				Console.WriteLine(2);
			} catch (Exception e) {
				Console.WriteLine(3);
			}
			Console.WriteLine(4);
		}
		
		public void ExceptionMethod()
		{
			Console.WriteLine("1");

			try
			{
				Console.WriteLine("2");

				try
				{
					Console.WriteLine("3");
				}
				catch (Exception e)
				{
					Console.WriteLine("4");
				}
				finally
				{
					Console.WriteLine("5");
				}

				Console.WriteLine("6");
			}
			catch (Exception e1)
			{
				Console.WriteLine("7");
				try
				{
					Console.WriteLine("8");
				}
				catch (Exception e2)
				{
					Console.WriteLine("9");
				}
				finally
				{
					Console.WriteLine("10");
				}

				Console.WriteLine("11");
			}
			finally
			{
				Console.WriteLine("12");
				try
				{
					Console.WriteLine("13");
				}
				catch (Exception e3)
				{
					Console.WriteLine("14");
				}
				finally
				{
					Console.WriteLine("15");
				}

				Console.WriteLine("16");
			}
			Console.WriteLine("17");
		}
	}
}
