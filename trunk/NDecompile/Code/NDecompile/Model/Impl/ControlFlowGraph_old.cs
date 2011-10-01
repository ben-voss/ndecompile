using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ControlFlowGraph : IControlFlowGraph
	{
		#region Fields

		private readonly CallGraphNode _rootNode;

		private readonly CallGraphNode[] _depthFirstSearchFirst;
		private readonly CallGraphNode[] _depthFirstSearchLast;
		private readonly SortedList<ushort, CallGraphNode> _nodes;

		private readonly List<List<IGraphNode>> _derivedSequence = new List<List<IGraphNode>>();

		#endregion


		#region Graph Building

		/// <summary>
		/// Construct a control flow graph out of the method body
		/// </summary>
		/// <param name="instructions"></param>
		public ControlFlowGraph(SortedList<ushort, IInstruction> instructions)
		{
			// Iterate the list of instructions building a list of call graph nodes
			SortedList<ushort, CallGraphNode> nodes = new SortedList<ushort, CallGraphNode>();
			List<IInstruction> nodeInstructions = new List<IInstruction>();
			foreach (IInstruction instruction in instructions.Values)
			{
				if ((instruction.IsTarget) && (nodeInstructions.Count > 0))
				{
					// This is the start of a block.  Create the previous block
					CallGraphNode callGraphNode = new CallGraphNode(nodeInstructions);
					nodes.Add(callGraphNode.StartIP, callGraphNode);
					nodeInstructions = new List<IInstruction>();
				}

				nodeInstructions.Add(instruction);
			}

			// Add the last node to the list
			if (nodeInstructions.Count > 0)
			{
				CallGraphNode callGraphNode = new CallGraphNode(nodeInstructions);
				nodes.Add(callGraphNode.StartIP, callGraphNode);
			}

			// Link the list of nodes
			for (int i = 0; i < nodes.Count; i++)
			{
				CallGraphNode node = nodes.Values[i];

				// Examine the last instruction to determine how to link this node to others
				IInstruction instruction = node.Instructions[node.Instructions.Count - 1];

				switch (instruction.OpCode.FlowControl)
				{
					case FlowControl.Branch:
						// Direct branch to a new block
						node.NodeType = NodeType.OneBranch;
						LinkNode(node, nodes, (ushort)instruction.Argument);
						break;

					case FlowControl.Cond_Branch:
						if (instruction.OpCode.Value == OpCodes.Switch.Value)
						{
							// Conditional branch to n-blocks
							node.NodeType = NodeType.MultiBranch;
							foreach (ushort switchTargetIp in (ushort[])instruction.Argument)
								LinkNode(node, nodes, switchTargetIp);
						}
						else
						{
							// Conditional branch to two blocks
							node.NodeType = NodeType.TwoBranch;
							LinkNode(node, nodes, (ushort)instruction.Argument);
						}

						// Add the continuation link
						LinkContinuationNode(node, nodes);
						break;

					case FlowControl.Return:
						node.NodeType = NodeType.Exit;
						break;

					case FlowControl.Throw:
						node.NodeType = NodeType.Throw;
						// End of graph
						break;

					case FlowControl.Break:
					case FlowControl.Call:
					case FlowControl.Meta:
					case FlowControl.Next:
#pragma warning disable 612,618
					case FlowControl.Phi:
#pragma warning restore 612,618
						// Add the continuation link
						node.NodeType = LinkContinuationNode(node, nodes) ? NodeType.FallThrough : NodeType.ExitBlock;
						break;

					default:
						throw new ApplicationException("Unexpected flow control type in instruction " + instruction.OpCode.Name);
				}
			}

			_rootNode = nodes.Values[0];
			_nodes = nodes;

			CheckGraph();

			// Remove redundancies and add in-edge information
			Optimize(nodes);

			IList<IGraphNode> ln = new List<IGraphNode>();
			foreach (CallGraphNode n in _nodes.Values)
				ln.Add(n);
			PrintGraph(ln);

			CheckGraph();

			// Visit the graph in depth first order and label the nodes
			_depthFirstSearchLast = new CallGraphNode[nodes.Count];
			_depthFirstSearchFirst = new CallGraphNode[nodes.Count];
			int first = 0;
			int last = nodes.Count - 1;
			DepthFirstTraverse(_rootNode, ref first, ref last);

			// Find the immediate dominators of each node
			FindImmediateDominators();

			// Check the graph for reducibility
			FindDerivedSequence();

			// Work out the graphs back edges
			DetermineBackEdges();

			ResetTraversal();
			StructureCases();
			StructureLoops();
			StructureIfs();
		}

		private void CheckGraph()
		{
			foreach (CallGraphNode callGraphNode in _nodes.Values)
				switch (callGraphNode.NodeType)
				{
					case NodeType.FallThrough:
					case NodeType.OneBranch:
						{
							if (callGraphNode.OutEdges.Count != 1)
								throw new ApplicationException("Expected one follow node for a " + callGraphNode.NodeType + " node");

							break;
						}

					case NodeType.TwoBranch:
						{
							if (callGraphNode.OutEdges.Count != 2)
								throw new ApplicationException("Expected two follow nodes for a TwoBranch node");
							break;
						}

					case NodeType.ExitBlock:
					case NodeType.Exit:
					case NodeType.Throw:
						{
							if (callGraphNode.OutEdges.Count != 0)
								throw new ApplicationException("Expected zero follow nodes for a " + callGraphNode.NodeType + " node");

							break;
						}
				}
		}

		private static void LinkNode(IGraphNode node, IDictionary<ushort, CallGraphNode> nodes, ushort targetIP)
		{
			CallGraphNode targetNode;

			if (nodes.TryGetValue(targetIP, out targetNode))
			{
				node.OutEdges.Add(targetNode);
				targetNode.InEdges.Add(node);
			}
			else
			{
				// Create a new empty node at the IP
				IList<IInstruction> instructions = new List<IInstruction>();
				instructions.Add(OpCodeTable.GetInstruction(OpCodes.Nop, targetIP, null));
				CallGraphNode emptyNode = new CallGraphNode(instructions);
				nodes.Add(targetIP, emptyNode);
				node.OutEdges.Add(emptyNode);
				emptyNode.InEdges.Add(node);
			}
		}

		private static bool LinkContinuationNode(CallGraphNode node, SortedList<ushort, CallGraphNode> nodes)
		{
			int index = nodes.IndexOfValue(node) + 1;
			if (index == nodes.Count)
				return false;

			IGraphNode targetNode = nodes.Values[index];

			node.OutEdges.Add(targetNode);
			targetNode.InEdges.Add(node);

			return true;
		}

		/// <summary>
		/// Performs a depth first traversal of the call graph and labels each node with its
		/// postion in the traversal
		/// </summary>
		/// <param name="node"></param>
		/// <param name="firstNumber"></param>
		/// <param name="lastNumber"></param>
		private void DepthFirstTraverse(CallGraphNode node, ref int firstNumber, ref int lastNumber)
		{
			node.Traversed = true;
			node.DepthFirstSearchFirstNumber = firstNumber;
			_depthFirstSearchFirst[firstNumber++] = node;

			foreach (CallGraphNode childNode in node.OutEdges)
			{
				// Traverse the child node if it has not already been traversed
				if (!childNode.Traversed)
					DepthFirstTraverse(childNode, ref firstNumber, ref lastNumber);
			}

			node.DepthFirstSearchLastNumber = lastNumber;
			_depthFirstSearchLast[lastNumber--] = node;
		}

		private void ResetTraversal()
		{
			foreach (CallGraphNode node in _depthFirstSearchLast)
				node.Traversed = false;
		}

		
		private static void PrintGraph(IEnumerable<IGraphNode> nodes)
		{
			Console.WriteLine("Graph");

			foreach (IGraphNode node in nodes)
			{
				Console.Write("{0:x2} ", node.Number);
				foreach (IGraphNode childNode in node.OutEdges)
					Console.Write("{0:x2} ", childNode.Number);

				Console.WriteLine();
			}
		}
		#endregion

		#region Optimization

		/// <summary>
		/// Remove redundancies and add in-edge information
		/// </summary>
		private void Optimize(SortedList<ushort, CallGraphNode> nodes)
		{
			// Remove redundant jumps directly to the next block that is not rechable from any other
			for (int i = 0; i < nodes.Count; i++)
			{
				CallGraphNode node = nodes.Values[i];
				if ((node.NodeType == NodeType.OneBranch) && (node.OutEdges.Count == 1) && (node.OutEdges[0].InEdges.Count == 1))
				{
					ICallGraphNode targetNode = (ICallGraphNode)node.OutEdges[0];

					// Remove the jump instruction at the end of this node
					node.Instructions.RemoveAt(node.Instructions.Count - 1);

					// Add all the instructions in the next node
					node.EndIP = targetNode.EndIP;
					foreach (IInstruction instruction in targetNode.Instructions)
						node.Instructions.Add(instruction);

					// Remove the target node
					nodes.Remove(targetNode.StartIP);
					node.OutEdges.Clear();

					((List<IGraphNode>)node.OutEdges).AddRange(targetNode.OutEdges);

					foreach (CallGraphNode n in targetNode.OutEdges)
					{
						n.InEdges.Remove(targetNode);
						n.InEdges.Add(node);
					}

					// This node now becomes the type of the target node
					node.NodeType = targetNode.NodeType;
				}
			}


			// First pass over block list removes redundant jumps of the form
			// (Un)Conditional-> Unconditional jump
			foreach (CallGraphNode node in nodes.Values)
				if ((node.OutEdges.Count == 0) &&
					((node.NodeType == NodeType.OneBranch) || (node.NodeType == NodeType.TwoBranch)))
					for (int i = 0; i < node.OutEdges.Count; i++)
					{
						CallGraphNode newTargetNode = RemoveJump((CallGraphNode)node.OutEdges[i]);
						node.OutEdges[i] = newTargetNode;
						newTargetNode.InEdges.Add(node);
					}

			// Next is a depth-first traversal merging any FallThrough node or
			// OneBranch node that fall through to a node with that as their only in-edge.
			MergeFallThrough();

			// Remove redundant nodes created by the optimizations
		}

		/// <summary>
		/// If the node is just a jump then replace the node with its target
		/// </summary>
		/// <param name="node">The node that is just a jump instruction.</param>
		/// <returns>The node that the jump instruction jumped to.</returns>
		private static CallGraphNode RemoveJump(CallGraphNode node)
		{
			while ((node.NodeType == NodeType.OneBranch) && (node.EndIP - node.StartIP == 1))
			{
				if (!node.Traversed)
				{
					node.Traversed = true;
					if (node.InEdges.Count == 1)
					{
						// Remove this node from the parents nodes out edges
						node.InEdges[0].OutEdges.Remove(node);

						// Remove this node from the child nodes in edges
						node.OutEdges[0].InEdges.Remove(node);
					}
				}

				node = (CallGraphNode)node.OutEdges[0];
			}

			return node;
		}

		private void MergeFallThrough()
		{

		}

		#endregion

		#region Dominators

		/// <summary>
		/// Finds the immediate dominator of each node in the graph
		/// This is based on the findImmedDom function of the Control.C file
		/// in the dcc source code which is in turn an adapted version
		/// of the dominators algorithm by Hecht and Ullman; finds
		/// immediate dominators only.
		/// Note graph should be reducible
		/// </summary>
		private void FindImmediateDominators()
		{
			for (int currentIndex = 0; currentIndex < _depthFirstSearchLast.Length; currentIndex++)
			{
				CallGraphNode currentNode = _depthFirstSearchLast[currentIndex];

				for (int j = 0; j < currentNode.InEdges.Count; j++)
				{
					int predecessorIndex = currentNode.InEdges[j].DepthFirstSearchLastNumber;
					if (predecessorIndex < currentIndex)
						currentNode.ImmediateDominatorNumber = CommonDominator(currentNode.ImmediateDominatorNumber, predecessorIndex);
				}
			}
		}

		/// <summary>
		/// Finds the common dominator of the current immediate dominator
		/// current immediate domintor and its predecessor's immediate dominator preceeding
		/// immediate dominator
		/// </summary>
		/// <param name="currentImmediateDominator"></param>
		/// <param name="preceedingImmediateDominator"></param>
		/// <returns></returns>
		private int CommonDominator(int currentImmediateDominator, int preceedingImmediateDominator)
		{
			if (currentImmediateDominator == Int32.MaxValue)
				return preceedingImmediateDominator;

			if (preceedingImmediateDominator == Int32.MaxValue)  // predeccessor is the root
				return currentImmediateDominator;

			while ((currentImmediateDominator != Int32.MaxValue) && (preceedingImmediateDominator != Int32.MaxValue) &&
				   (currentImmediateDominator != preceedingImmediateDominator))
			{
				if (currentImmediateDominator < preceedingImmediateDominator)
					preceedingImmediateDominator = _depthFirstSearchLast[preceedingImmediateDominator].ImmediateDominatorNumber;
				else
					currentImmediateDominator = _depthFirstSearchLast[currentImmediateDominator].ImmediateDominatorNumber;
			}

			return currentImmediateDominator;
		}

		#endregion

		#region Intervals

		private class IntervalNode : IGraphNode
		{
			private readonly IList<IGraphNode> _nodes = new List<IGraphNode>();
			private readonly IList<IGraphNode> _inEdges = new List<IGraphNode>();
			private readonly IList<IGraphNode> _outEdges = new List<IGraphNode>();

			private IGraphNode _interval;

			public IntervalNode(IGraphNode headerNode)
			{
				Add(headerNode);
			}

			public int Number
			{
				get
				{
					return _nodes[0].Number;
				}
			}

			public bool Contains(IGraphNode node)
			{
				return _nodes.Contains(node);
			}

			public void Add(IGraphNode node)
			{
				node.Interval = this;
				_nodes.Add(node);
			}

			public IGraphNode Interval
			{
				get
				{
					return _interval;
				}
				set
				{
					_interval = value;
				}
			}

			public IList<IGraphNode> Nodes
			{
				get
				{
					return _nodes;
				}
			}

			public int ImmediateDominatorNumber
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public int DepthFirstSearchLastNumber
			{
				get
				{
					return _nodes[0].DepthFirstSearchLastNumber;
				}
			}

			public IList<IGraphNode> OutEdges
			{
				get
				{
					return _outEdges;
				}
			}

			public IList<IGraphNode> InEdges
			{
				get
				{
					return _inEdges;
				}
			}
		}

		/// <summary>
		/// Interval finding algorithm:
		/// 
		/// 1. Establish a list H for header nodes and initialize it to e 0.
		/// 2. For h E H find l(h) as follows.
		/// 2.1 Put h in l(h) as the first element of l(h)
		/// 2.2 For any b ~ G for which r~l(b) rd l(h) add b to l(h).
		///     Thus a node is added to an interval if and only if all of its
		///     immediate predecessors are already in the interval.
		/// 2.3 Repeat 2.2 until no more nodes can be added to l(h).
		/// 3.1 Add to H all nodes in G which are not already in H and which
		///     not in l(h) but which have immediate predecessors in l(h).
		///     Therefore a node is added to H the first time any (but not all) of its
		///     immediate predecessors are members of an interval.
		/// 3.2 Add l(h) to the set of intervals being developed.
		/// 4. Select the next unprocessed node in H and repeat steps 2,3,4.
		/// </summary>
		private static List<IGraphNode> FindIntervals(IList<IGraphNode> nodeList)
		{
			List<IGraphNode> intervals = new List<IGraphNode>();

			// 1. Establish a list of header nodes and initialize it to contain the root node.
			List<IGraphNode> headerNodes = new List<IGraphNode>();
			headerNodes.Add(nodeList[0]);

			// 2. For all call graph nodes in headerNodes find the interval as follows.
			for (int i = 0; i < headerNodes.Count; i++)
			{
				IGraphNode headerNode = headerNodes[i];

				// 2.1 Add the header node to the interval
				IntervalNode interval = new IntervalNode(headerNode);

				// 2.2 For any node b in the graph G for which ImmediatePredecessors(b) < I(h) add b to I(h).
				//     Thus a node is added to an interval if and only if all of its
				//     immediate predecessors are already in the interval.
				// 2.3 Repeat 2.2 until no more nodes can be added to I(h).
				for (int j = headerNode.DepthFirstSearchLastNumber + 1; j < nodeList.Count; j++)
				{
					IGraphNode b = nodeList[j];
					if (headerNodes.Contains(b))
						continue;

					if (interval.Contains(b))
						continue;

					bool add = true;
					foreach (IGraphNode p in b.InEdges)
						if (!interval.Contains(p))
							add = false;

					if (add)
						interval.Add(b);
				}

				// 3.1 Add to H all nodes in G which are not already in H and which
				//     are not in I(h) but which have immediate predecessors in I(h).
				//     Therefore a node is added to H the first time any (but not all) of its
				//     immediate predecessors are members of an interval.
				for (int j = headerNode.DepthFirstSearchLastNumber + 1; j < nodeList.Count; j++)
				{
					IGraphNode g = nodeList[j];
					if (headerNodes.Contains(g))
						continue;

					if (interval.Contains(g))
						continue;

					foreach (IGraphNode ipg in g.InEdges)
						if (interval.Contains(ipg))
						{
							headerNodes.Add(g);
							break;
						}
				}

				// 3.2 Add l(h) to the set of intervals being developed.
				intervals.Add(interval);

				// 4 Select the next unprocessed node in H and repeat steps 2,3,4.
			}

			//PrintIntervals(intervals);

			return intervals;
		}

		/*private static void PrintIntervals(IEnumerable<IGraphNode> intervals)
		{
			Console.WriteLine("Intervals");
			foreach (IntervalNode interval in intervals)
			{
				foreach (IGraphNode node in interval.Nodes)
				{
					Console.Write(node.Number);
					Console.Write(" ");
				}
				Console.WriteLine();
			}
		}*/

		#endregion

		#region Derived Sequence Graph

		private void FindDerivedSequence()
		{
			List<IGraphNode> intervals = FindIntervals(_depthFirstSearchLast);

			while (intervals.Count > 1)
			{
				_derivedSequence.Add(intervals);

				// Work out the intervals in edges
				foreach (IntervalNode intervalNode in intervals)
				{
					foreach (IGraphNode node in intervalNode.Nodes)
					{
						foreach (IGraphNode edge in node.InEdges)
						{
							if ((edge.Interval != intervalNode) && (!intervalNode.InEdges.Contains(edge.Interval)))
								intervalNode.InEdges.Add(edge.Interval);
						}

						foreach (IGraphNode edge in node.OutEdges)
						{
							if ((edge.Interval != intervalNode) && (!intervalNode.OutEdges.Contains(edge.Interval)))
								intervalNode.OutEdges.Add(edge.Interval);
						}
					}
				}

				//PrintGraph(intervals);
				intervals = FindIntervals(intervals);
			}
		}


		#endregion

		#region Back Edges

		private void DetermineBackEdges()
		{
			foreach (ICallGraphNode node in _depthFirstSearchLast)
				foreach (ICallGraphNode edge in node.OutEdges)
					if (edge.DepthFirstSearchLastNumber < node.DepthFirstSearchLastNumber)
						node.BackEdges.Add(edge);
		}

		#endregion

		#region Structure Loops

		/// <summary>
		/// Recursive procedure to find nodes that belong to the interval
		/// </summary>
		/// <param name="level"></param>
		/// <param name="interval"></param>
		/// <returns></returns>
		private static List<IGraphNode> FindNodesInInterval(int level, IntervalNode interval)
		{
			List<IGraphNode> nodes = new List<IGraphNode>();
			if (level == 1)
				nodes.AddRange(interval.Nodes);
			else
				foreach (IntervalNode intervalNode in interval.Nodes)
					nodes.AddRange(FindNodesInInterval(level - 1, intervalNode));

			return nodes;
		}

		/// <summary>
		/// Flag nodes that belong to the loop determined by (latchNode, head) and determines the type of loop
		/// </summary>
		/// <param name="latchNode"></param>
		/// <param name="intervalHead"></param>
		/// <param name="intervalNodes"></param>
		private void FindNodesInLoop(ICallGraphNode latchNode, ICallGraphNode intervalHead, ICollection<IGraphNode> intervalNodes)
		{
			const int Then = 0;
			const int Else = 1;

			List<int> loopNodes = new List<int>();
			int headDfsNumber = intervalHead.DepthFirstSearchLastNumber;
			intervalHead.LoopHead = intervalHead;
			loopNodes.Add(headDfsNumber);

			for (int i = headDfsNumber + 1; i < latchNode.DepthFirstSearchLastNumber; i++)
			{
				int immediateDominator = _depthFirstSearchLast[i].ImmediateDominatorNumber;
				if (loopNodes.Contains(immediateDominator) && intervalNodes.Contains(_depthFirstSearchLast[i]))
				{
					loopNodes.Add(i);
					if (_depthFirstSearchLast[i].LoopHead == null)
						_depthFirstSearchLast[i].LoopHead = _depthFirstSearchLast[headDfsNumber];
				}
			}

			latchNode.LoopHead = _depthFirstSearchLast[headDfsNumber];
			if (latchNode != intervalHead)
				loopNodes.Add(latchNode.DepthFirstSearchLastNumber);

			// Determine type of loop and follow node
			NodeType intNodeType = intervalHead.NodeType;
			if (latchNode.NodeType == NodeType.TwoBranch)
				if ((intNodeType == NodeType.TwoBranch) || (latchNode == intervalHead))
					if ((latchNode == intervalHead) ||
						loopNodes.Contains(intervalHead.OutEdges[Then].DepthFirstSearchLastNumber) &&
						loopNodes.Contains(intervalHead.OutEdges[Else].DepthFirstSearchLastNumber))
					{
						intervalHead.LoopType = LoopType.Do;
						if (latchNode.OutEdges[0] == intervalHead)
							intervalHead.LoopFollow = (ICallGraphNode)latchNode.OutEdges[Else];
						else
							intervalHead.LoopFollow = (ICallGraphNode)latchNode.OutEdges[Then];
					}
					else
					{
						intervalHead.LoopType = LoopType.While;
						if (loopNodes.Contains(intervalHead.OutEdges[Then].DepthFirstSearchLastNumber))
							intervalHead.LoopFollow = (ICallGraphNode)intervalHead.OutEdges[Else];
						else
							intervalHead.LoopFollow = (ICallGraphNode)intervalHead.OutEdges[Then];
					}
				else
				{
					intervalHead.LoopType = LoopType.Do;
					if (loopNodes.Contains(intervalHead.OutEdges[Then].DepthFirstSearchLastNumber))
						intervalHead.LoopFollow = (ICallGraphNode)intervalHead.OutEdges[Else];
					else
						intervalHead.LoopFollow = (ICallGraphNode)intervalHead.OutEdges[Then];
				}
			else
				if (latchNode.NodeType == NodeType.OneBranch)
				{
					intervalHead.LoopType = LoopType.Do;
					intervalHead.LoopFollow = (ICallGraphNode)latchNode.OutEdges[0];
				}
				else if (intNodeType == NodeType.TwoBranch)
				{
					intervalHead.LoopType = LoopType.While;
					ICallGraphNode pbb = latchNode;
					int thenDfs = intervalHead.OutEdges[Then].DepthFirstSearchLastNumber;
					int elseDfs = intervalHead.OutEdges[Else].DepthFirstSearchLastNumber;
					while (true)
					{
						if (pbb.DepthFirstSearchLastNumber == thenDfs)
						{
							intervalHead.LoopFollow = _depthFirstSearchLast[elseDfs];
							break;
						}

						if (pbb.DepthFirstSearchLastNumber == elseDfs)
						{
							intervalHead.LoopFollow = _depthFirstSearchLast[thenDfs];
							break;
						}

						// Check if couldn't find it, then its a stragenly formed
						// loop, so it is safer to consider it an endless loop
						if (pbb.DepthFirstSearchLastNumber <= intervalHead.DepthFirstSearchLastNumber)
						{
							intervalHead.LoopType = LoopType.Endless;
							break;
						}
						pbb = _depthFirstSearchLast[pbb.ImmediateDominatorNumber];
					}

					if (pbb.DepthFirstSearchLastNumber > intervalHead.DepthFirstSearchLastNumber)
						intervalHead.LoopFollow.LoopHead = null;

				}
				else
				{
					intervalHead.LoopType = LoopType.Endless;
				}
		}

		/// <summary>
		/// Loop structuring
		/// </summary>
		private void StructureLoops()
		{
			int level = 0;   // Derived Sequence Level

			// For all derived sequences of Gi
			foreach (List<IGraphNode> derivedSequence in _derivedSequence)
			{
				level++;

				// For all intervals Ii of Gi
				foreach (IntervalNode interval in derivedSequence)
				{
					ICallGraphNode latchNode = null;

					// Find the interval head (original block node in G1) and create
					// list of nodes of interval Ii.
					IntervalNode initInt = interval;
					for (int i = 1; i < level; i++)
						initInt = (IntervalNode)initInt.Nodes[0];

					ICallGraphNode intervalHead = (ICallGraphNode)initInt.Nodes[0];

					// Find nodes that belong to the interval (nodes from G1)
					List<IGraphNode> intervalNodes = FindNodesInInterval(level, interval);

					// Find the greatest enclosing back edge (if any)
					for (int i = 0; i < intervalHead.InEdges.Count; i++)
					{
						ICallGraphNode pred = (ICallGraphNode)intervalHead.InEdges[i];
						if (intervalNodes.Contains(pred) && (pred.DepthFirstSearchLastNumber >= intervalHead.DepthFirstSearchLastNumber))
							if (latchNode == null)
								latchNode = pred;
							else
							{
								if (pred.DepthFirstSearchLastNumber > latchNode.DepthFirstSearchLastNumber)
									latchNode = pred;
							}
					}

					// Find nodes in the loop and the type of loop
					if (latchNode != null)
					{
						// Check latching node is at the same nesting level of case
						// statements (if any) and that the node doesnt belong to
						// another loop.
						if ((latchNode.CaseHead == intervalHead.CaseHead) &&
							(latchNode.LoopHead == null))
						{
							intervalHead.LatchNode = latchNode;
							FindNodesInLoop(latchNode, intervalHead, intervalNodes);
						}
					}
				}
			}
		}

		#endregion

		#region Structure Cases

		/// <summary>
		/// Recursive procedure to tag nodes that belong to the case described by
		/// the list l, head and tail (dfsLast index to first and exit node of the 
		/// case).  
		/// </summary>
		/// <param name="node"></param>
		/// <param name="l"></param>
		/// <param name="head"></param>
		/// <param name="tail"></param>
		private static void TagNodesInCase(CallGraphNode node, ICollection<IGraphNode> l, ICallGraphNode head, IGraphNode tail)
		{
			node.Traversed = true;
			if ((node != tail) && (node.Instructions[node.Instructions.Count - 1].OpCode != OpCodeTable.Switch) && l.Contains(node))
			{
				l.Add(node);
				node.CaseHead = head;
				for (int i = 0; i < node.OutEdges.Count; i++)
					if (!((CallGraphNode)node.OutEdges[i]).Traversed)
						TagNodesInCase((CallGraphNode)node.OutEdges[i], l, head, tail);
			}
		}

		/// <summary>
		/// Structures case statements
		/// </summary>
		private void StructureCases()
		{

			// Linear scan of nodes in reverse depth first search last order, searching
			// for case nodes
			for (int i = _depthFirstSearchLast.Length - 1; i >= 0; i--)
			{
				// A case node is a node where the last instruction is a Switch opcode
				ICallGraphNode caseHeader = _depthFirstSearchLast[i];
				if (caseHeader.Instructions[caseHeader.Instructions.Count - 1].OpCode != OpCodeTable.Switch)
					continue;

				// Find descendant node which has as immediate preseccessor
				// the current header node
				ICallGraphNode exitNode = null;
				for (int j = i + 2; j < _depthFirstSearchLast.Length; j++)
					if (_depthFirstSearchLast[j].ImmediateDominatorNumber == i)
						if ((exitNode == null) || (exitNode.InEdges.Count < _depthFirstSearchLast[j].InEdges.Count))
							exitNode = _depthFirstSearchLast[j];

				caseHeader.CaseTail = exitNode;

				// Tag nodes that belong to the case by recording the header field
				// with caseHeader
				caseHeader.CaseHead = caseHeader;

				List<IGraphNode> caseNodes = new List<IGraphNode>();
				caseNodes.Add(caseHeader);

				foreach (CallGraphNode childNode in caseHeader.OutEdges)
					TagNodesInCase(childNode, caseNodes, caseHeader, exitNode);

				if (exitNode != null)
					exitNode.CaseHead = caseHeader;
			}
		}

		#endregion

		#region Structure Ifs

		/// <summary>
		/// Structures if statements
		/// </summary>
		private void StructureIfs()
		{
			List<ICallGraphNode> unresolved = new List<ICallGraphNode>();	// List of unresolved if nodes

			// Linear scan of nodes in reverse depth first search order
			for (int curr = _depthFirstSearchLast.Length - 1; curr >= 0; curr--)
			{
				List<ICallGraphNode> domDesc = new List<ICallGraphNode>(); // List of nodes dominated by curr
				ICallGraphNode currNode = _depthFirstSearchLast[curr];

				if (currNode.NodeType == NodeType.TwoBranch)
				{
					int followInEdges = 0;
					int follow = 0;

					// Find all nodes that have this node as immediate dominator
					for (int desc = curr + 1; desc < _nodes.Count; desc++)
					{
						if (_depthFirstSearchLast[desc].ImmediateDominatorNumber == curr)
						{
							domDesc.Add(_depthFirstSearchLast[desc]);
							ICallGraphNode pbb = _depthFirstSearchLast[desc];
							if ((pbb.InEdges.Count - pbb.BackEdges.Count) > followInEdges)
							{
								follow = desc;
								followInEdges = pbb.InEdges.Count - pbb.BackEdges.Count;
							}
						}
					}

					// Determine follow according to number of descendants
					// immediately dominated by this node 
					if ((follow != 0) && (followInEdges > 1))
					{
						currNode.IfFollow = _depthFirstSearchLast[follow];

						// Flags all nodes in the list l as having follow node f, and deletes all
						// nodes from the list.
						foreach (ICallGraphNode node in unresolved)
							node.IfFollow = _depthFirstSearchLast[follow];

						unresolved.Clear();
					}
					else
						unresolved.Add(currNode);
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the graphs root node
		/// </summary>
		public ICallGraphNode RootNode
		{
			get
			{
				return _rootNode;
			}
		}

		public SortedList<ushort, CallGraphNode> Nodes
		{
			get
			{
				return _nodes;
			}
		}
	}
}
