using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NetMethod = System.Reflection.MethodBase;
using NetMethodBody = System.Reflection.MethodBody;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class MethodBody : IMethodBody
	{
		#region Fields

		private static readonly ITypeReference VoidTypeReference = AssemblyManager.FindType(typeof(void), new Type[0]);
		private static readonly ITypeReference TypeOfTypeReference = AssemblyManager.FindType(typeof(Type), new Type[0]);
		private static readonly IMethodReference TypeOfMethodReference = AssemblyManager.FindMethod(typeof(Type).GetMethod("GetTypeFromHandle"));
		private static readonly Dictionary<Type, String> VariableNamePrefixTable;

		private readonly Module _module;
		private readonly NetMethod _netMethod;
		private readonly NetMethodBody _body;

		private readonly ParameterInfo[] _parameters;
		private readonly Type[] _genericMethodArgs;

		private IList<IInstruction> _instructions;
		private IControlFlowGraph _controlFlowGraph;
		private List<IStatement> _statements;
		private List<IVariableDeclaration> _variables;
		private List<String> _variableNames;

		#endregion

		#region Constructors

		static MethodBody()
		{
			VariableNamePrefixTable = new Dictionary<Type, string>();

			VariableNamePrefixTable.Add(typeof(bool), "bool");
			VariableNamePrefixTable.Add(typeof(char), "char");
			VariableNamePrefixTable.Add(typeof(byte), "byte");
			VariableNamePrefixTable.Add(typeof(sbyte), "sbyte");
			VariableNamePrefixTable.Add(typeof(uint), "uint");
			VariableNamePrefixTable.Add(typeof(int), "int");
			VariableNamePrefixTable.Add(typeof(ushort), "ushort");
			VariableNamePrefixTable.Add(typeof(short), "short");
			VariableNamePrefixTable.Add(typeof(long), "long");
			VariableNamePrefixTable.Add(typeof(ulong), "ulong");
			VariableNamePrefixTable.Add(typeof(float), "float");
			VariableNamePrefixTable.Add(typeof(double), "double");
			VariableNamePrefixTable.Add(typeof(decimal), "decimal");

		}

		/// <summary>
		/// Initialises a new instance of the <see cref="MethodBody"/> class with the specified
		/// .net method and module
		/// </summary>
		/// <param name="method"></param>
		/// <param name="module"></param>
		public MethodBody(MethodBase method, Module module)
		{
			_netMethod = method;
			_module = module;
			_body = method.GetMethodBody();

			// Deal with the arguments
			_parameters = method.GetParameters();
			if (method.IsGenericMethod)
				_genericMethodArgs = method.GetGenericArguments();
			else
				_genericMethodArgs = new Type[0];
		}

		#endregion

		#region IMethodBody Members

		public int MaxStack
		{
			get
			{
				if (_body == null)
					return -1;

				return _body.MaxStackSize;
			}
		}

		public IList<IInstruction> Instructions
		{
			get
			{
				if (_body == null)
					return null;

				if (_instructions == null)
					DissasembleInstructions();

				return _instructions;
			}
		}

		public IControlFlowGraph ControlFlowGraph
		{
			get
			{
				if (_body == null)
					return null;

				if (_body.ExceptionHandlingClauses.Count != 0)
					return null;

				if (_statements == null)
					Decompile();

				return _controlFlowGraph;
			}
		}

		public List<IStatement> Statements
		{
			get
			{
				if (_body == null)
					return null;

				if (_statements == null)
					Decompile();

				return _statements;
			}
		}

		public IList<IVariableDeclaration> Variables
		{
			get
			{
				if (_body == null)
					return null;

				if (_variables == null)
					_variables = GenerateLocalVariables();

				return _variables;
			}
		}

		public bool InitVariables
		{
			get
			{
				return _body.InitLocals;
			}
		}

		#endregion

		#region Dissasembly

		private void DissasembleInstructions()
		{
			byte[] il = _body.GetILAsByteArray();

			SortedList<ushort, IInstruction> indexedInstructions = DissasembleInstructionRange(0, (ushort)il.Length, il);

			// Copy the instructions into the result list
			_instructions = new List<IInstruction>();
			foreach (IInstruction instruction in indexedInstructions.Values)
				_instructions.Add(instruction);
		}

		private static void SetTarget(IDictionary<ushort, IInstruction> indexedInstructions, ushort targetIp)
		{
			IInstruction instruction;
			if (indexedInstructions.TryGetValue(targetIp, out instruction))
				instruction.IsTarget = true;
		}

		private SortedList<ushort, IInstruction> DissasembleInstructionRange(ushort startIp, ushort endIp, byte[] il)
		{
			SortedList<ushort, IInstruction> indexedInstructions = new SortedList<ushort, IInstruction>();

			// Convert the byte code into instruction objects
			while (startIp < endIp)
				indexedInstructions.Add(startIp, OpCodeTable.GetInstruction(il, ref startIp));

			// Mark instructions that are targets of other instructions
			for (int i = 0; i < indexedInstructions.Count; i++)
			{
				IInstruction instruction = indexedInstructions.Values[i];

				// Resolve the type reference instruction arguments
				switch (instruction.OpCode.Value)
				{
					case 0x28: // Call:
					case 0x6F: // Callvirt 
					case 0x73: // Newobj
					case 0x74: // Castclass
					case 0x75: // Isinst
					case 0x8d: // Newarr
					case 0x7b: // Ldfld:
					case 0x7c: // Ldflda:
					case 0x7d: // Stfld
					case 0x7e: // Ldsfld
					case 0x80: // Stsfld
					case 0xd0: // Ldtoken
					case 0xa3: // Ldelem.any
						{
							instruction.Argument = _module.FindToken((uint)instruction.Argument,
																	 _netMethod.DeclaringType.GetGenericArguments(), _genericMethodArgs);
							break;
						}

					case 0x72: // Ldstr
						{
							instruction.Argument = _module.FindString((uint)instruction.Argument);
							break;
						}
				}

				// Resolve the flow control arguments
				switch (instruction.OpCode.FlowControl)
				{
					case FlowControl.Branch:
						// Direct branch to a new block - that might not exist if its a leave instruction
						SetTarget(indexedInstructions, (ushort)instruction.Argument);
						break;

					case FlowControl.Cond_Branch:
						if (instruction.OpCode.Value == OpCodes.Switch.Value)
						{
							// Conditional branch to n-blocks
							foreach (ushort switchTargetIp in (ushort[])instruction.Argument)
								SetTarget(indexedInstructions, switchTargetIp);
						}
						else
						{
							// Conditional branch to two blocks
							SetTarget(indexedInstructions, (ushort)instruction.Argument);
						}

						// Set the next instruction of a branch to also be a target
						int index = indexedInstructions.IndexOfValue(instruction) + 1;
						if (indexedInstructions.Values.Count > index)
							indexedInstructions.Values[index].IsTarget = true;
						else
						{
							// The next instruction is the start of another block.  This happens with the start
							// of a try block.  Fake a NOP instruction
							Instruction nopInstruction = new Instruction(OpCodes.Nop, (ushort)(instruction.IP + 1), null);
							indexedInstructions.Add(nopInstruction.IP, nopInstruction);
						}

						break;

					case FlowControl.Return:
					case FlowControl.Throw:
					case FlowControl.Break:
					case FlowControl.Call:
					case FlowControl.Meta:
					case FlowControl.Next:
#pragma warning disable 612,618
					case FlowControl.Phi:
#pragma warning restore 612,618
						break;

					default:
						throw new ApplicationException("Unexpected flow control type in instruction " + instruction.OpCode.Name);
				}
			}

			return indexedInstructions;
		}

		#endregion

		#region Decompile

		private class ExceptionClauseCodeBlock
		{
			private readonly ushort _tryStartIp;

			private readonly SortedList<ushort, ExceptionClauseCodeBlock> _clauseBlocks = new SortedList<ushort, ExceptionClauseCodeBlock>();

			private readonly ushort _tryEndIp;

			private readonly ushort _handlerStartIp;

			private readonly SortedList<ushort, ExceptionClauseCodeBlock> _handlerBlocks = new SortedList<ushort, ExceptionClauseCodeBlock>();

			private readonly ushort _handlerEndIp;

			private readonly IVariableDeclaration _exceptionVariable;

			public ExceptionClauseCodeBlock(ushort length)
			{
				_tryStartIp = 0;
				_tryEndIp = length;
				_handlerStartIp = ushort.MaxValue;
				_handlerEndIp = ushort.MaxValue;
			}

			public ExceptionClauseCodeBlock(ExceptionHandlingClause clause)
			{
				_tryStartIp = (ushort)clause.TryOffset;
				_tryEndIp = (ushort)(clause.TryOffset + clause.TryLength);
				_handlerStartIp = (ushort)clause.HandlerOffset;
				_handlerEndIp = (ushort)(clause.HandlerOffset + clause.HandlerLength);

				if (clause.Flags == ExceptionHandlingClauseOptions.Clause)
				{
					// This is a catch block.  Generate the expression that will hold the caught exception
					_exceptionVariable = new VariableDeclaration("e", AssemblyManager.FindType(clause.CatchType, clause.CatchType.GetGenericArguments()));
				}
			}

			public IVariableDeclaration ExceptionVariable
			{
				get
				{
					return _exceptionVariable;
				}
			}

			public SortedList<ushort, ExceptionClauseCodeBlock> ClauseBlocks
			{
				get
				{
					return _clauseBlocks;
				}
			}

			public SortedList<ushort, ExceptionClauseCodeBlock> HandlerBlocks
			{
				get
				{
					return _handlerBlocks;
				}
			}

			public ushort TryStartIp
			{
				get
				{
					return _tryStartIp;
				}
			}

			public ushort TryEndIp
			{
				get
				{
					return _tryEndIp;
				}
			}

			public ushort HandlerStartIp
			{
				get
				{
					return _handlerStartIp;
				}
			}

			public ushort HandlerEndIp
			{
				get
				{
					return _handlerEndIp;
				}
			}
		}

		public void Decompile()
		{
			// Build the list of variables
			if (_variables == null)
				_variables = GenerateLocalVariables();

			// Write the variable declaraions
			_statements = new List<IStatement>();
			foreach (IVariableDeclaration variableDeclaration in _variables)
				_statements.Add(new ExpressionStatement(new VariableDeclarationExpression(variableDeclaration)));

			byte[] il = _body.GetILAsByteArray();

			// Create an initial block that represents the entire method
			ExceptionClauseCodeBlock rootBlock = new ExceptionClauseCodeBlock((ushort)il.Length);

			// Reverse sort the exception clauses by the total number of instructions they cover.
			// Larger clauses must be higher up in the nesting tree than smaller ones.
			SortedList<int, ExceptionHandlingClause> sortedClauses = new SortedList<int, ExceptionHandlingClause>();
			foreach (ExceptionHandlingClause clause in _body.ExceptionHandlingClauses)
				sortedClauses.Add(int.MaxValue - clause.HandlerOffset + clause.HandlerLength - clause.TryOffset, clause);

			// Build a tree of clause blocks based on the sort order
			foreach (ExceptionHandlingClause clause in sortedClauses.Values)
				InsertBlock(rootBlock, new ExceptionClauseCodeBlock(clause));

			// Traverse the blocks in code generation order
			TraverseBlocks(rootBlock, il, _statements);
		}

		/// <summary>
		/// Generates variable declarations for all the local variables of a method
		/// </summary>
		private List<IVariableDeclaration> GenerateLocalVariables()
		{
			// Build the variable declaration table and invent some variable names based on type
			_variableNames = new List<string>();
			List<IVariableDeclaration> variables = new List<IVariableDeclaration>();
			for (int i = 0; i < _body.LocalVariables.Count; i++)
				variables.Add(GenerateVariableDeclaration(_body.LocalVariables[i].LocalType));

			return variables;
		}

		private IVariableDeclaration GenerateVariableDeclaration(Type variableType)
		{
			Type type = variableType;
			if (type.IsArray)
				type = variableType.GetElementType();

			String candicateName;
			if (!VariableNamePrefixTable.TryGetValue(type, out candicateName))
				if ((type.IsGenericType) && (type.Name.LastIndexOf('`') > 0))
					candicateName = type.Name.Substring(0, type.Name.LastIndexOf('`'));
				else
					candicateName = type.Name;

			int charPos = candicateName.Length - 1;
			while ((charPos > 0) && (Char.IsLower(candicateName[charPos])))
				charPos--;

			candicateName = candicateName.Substring(charPos).ToLowerInvariant();

			if (_variableNames.Contains(candicateName))
			{
				int postfix = 1;
				while (_variableNames.Contains(candicateName + postfix))
					postfix++;

				candicateName = candicateName + postfix;
			}

			_variableNames.Add(candicateName);

			return new VariableDeclaration(candicateName, AssemblyManager.FindType(variableType, variableType.GetGenericArguments()));
		}

		private static bool InsertBlock(ExceptionClauseCodeBlock parentBlock, ExceptionClauseCodeBlock block)
		{
			if ((block.TryStartIp >= parentBlock.TryStartIp) && (block.HandlerEndIp <= parentBlock.TryEndIp))
			{
				// Block goes inside the parents try block
				foreach (ExceptionClauseCodeBlock childBlock in parentBlock.ClauseBlocks.Values)
					if (InsertBlock(childBlock, block))
						return true;

				parentBlock.ClauseBlocks.Add(block.TryStartIp, block);
				return true;
			}

			if ((block.TryStartIp >= parentBlock.HandlerStartIp) && (block.HandlerEndIp <= parentBlock.HandlerEndIp))
			{
				// Block goes inside the parents handler block
				foreach (ExceptionClauseCodeBlock childBlock in parentBlock.HandlerBlocks.Values)
					if (InsertBlock(childBlock, block))
						return true;

				parentBlock.HandlerBlocks.Add(block.TryStartIp, block);
				return true;
			}

			return false;
		}

		/// <summary>
		/// Traverses a tree of exception blocks and generates code for each range of instructions
		/// </summary>
		/// <param name="block"></param>
		/// <param name="il"></param>
		/// <param name="statements"></param>
		private void TraverseBlocks(ExceptionClauseCodeBlock block, byte[] il, IList<IStatement> statements)
		{
			ITryCatchFinallyStatement tryCatchFinallyStatement = new TryCatchFinallyStatement();
			IList<IStatement> tryStatements;

			if (block.HandlerStartIp == ushort.MaxValue)
				tryStatements = statements;
			else
			{
				statements.Add(tryCatchFinallyStatement);
				tryStatements = tryCatchFinallyStatement.Try.Statements;
			}

			// Write the IL between the try IP and the first child blocks IP
			if (block.ClauseBlocks.Count == 0)
			{
				DecompileRange(block.TryStartIp, block.TryEndIp, il, tryStatements);
			}
			else
			{
				DecompileRange(block.TryStartIp, block.ClauseBlocks.Values[0].TryStartIp, il, tryStatements);

				if (block.ClauseBlocks.Count == 1)
				{
					// Write the child block
					TraverseBlocks(block.ClauseBlocks.Values[0], il, tryStatements);
				}
				else
				{
					for (int i = 0; i < block.ClauseBlocks.Count - 1; i++)
					{
						ExceptionClauseCodeBlock childBlock = block.ClauseBlocks.Values[i];
						TraverseBlocks(childBlock, il, statements);

						// Write any code between each child
						DecompileRange(childBlock.HandlerEndIp, block.ClauseBlocks.Values[i + 1].TryStartIp, il, tryStatements);
					}
				}

				// Write any code between the last child block and the end of the try block
				DecompileRange(block.ClauseBlocks.Values[block.ClauseBlocks.Count - 1].HandlerEndIp, block.TryEndIp, il, tryStatements);
			}

			if (block.HandlerBlocks.Count == 0)
			{
				// No children - write the code for the entire handler
				if (block.ExceptionVariable == null)
					DecompileRange(block.HandlerStartIp, block.HandlerEndIp, il, tryCatchFinallyStatement.Finally.Statements);
				else
				{
					tryCatchFinallyStatement.CatchClauses.Add(DecompileCatchClause(block.HandlerStartIp, block.HandlerEndIp, il, block.ExceptionVariable));
				}
			}
			else
			{
				// Write the code between the start of the handler and the first child block
				if (block.ExceptionVariable == null)
					DecompileRange(block.HandlerStartIp, block.HandlerBlocks.Values[0].HandlerStartIp, il, tryCatchFinallyStatement.Finally.Statements);
				else
				{
					tryCatchFinallyStatement.CatchClauses.Add(DecompileCatchClause(block.HandlerStartIp, block.HandlerBlocks.Values[0].HandlerStartIp, il, block.ExceptionVariable));
				}

				if (block.HandlerBlocks.Count == 1)
				{
					// Write the child block
					TraverseBlocks(block.HandlerBlocks.Values[0], il, statements);
				}
				else
				{
					for (int i = 0; i < block.HandlerBlocks.Count - 1; i++)
					{
						ExceptionClauseCodeBlock childBlock = block.HandlerBlocks.Values[i];
						TraverseBlocks(childBlock, il, statements);

						// Write any code between each child
						if (block.ExceptionVariable == null)
							DecompileRange(childBlock.HandlerEndIp, block.HandlerBlocks.Values[i + 1].HandlerStartIp, il, tryCatchFinallyStatement.Finally.Statements);
						else
						{
							tryCatchFinallyStatement.CatchClauses.Add(DecompileCatchClause(childBlock.HandlerEndIp, block.HandlerBlocks.Values[i + 1].HandlerStartIp, il, childBlock.ExceptionVariable));
						}
					}
				}

				// Write any code between the last child block and the end of the handler block
				DecompileRange(block.HandlerBlocks.Values[block.HandlerBlocks.Count - 1].HandlerEndIp, block.HandlerEndIp, il, tryCatchFinallyStatement.Finally.Statements);
			}
		}

		private ICatchClause DecompileCatchClause(ushort startIp, ushort endIp, byte[] il, IVariableReference variableDeclaration)
		{
			// Build a storted list of instructions in the range
			SortedList<ushort, IInstruction> instructions = DissasembleInstructionRange(startIp, endIp, il);

			// Generate a control flow graph of the block of instructions
			_controlFlowGraph = new ControlFlowGraph(instructions);

			CatchClause catchClause = new CatchClause(variableDeclaration, null);

			// Push the exception reference onto the stack
			// Construct the variable reference
			Stack<IExpression> stack = new Stack<IExpression>();
			stack.Push(new VariableReferenceExpression(variableDeclaration));

			// Generate instructions based on the graph
			GenerateStatementsForGraph(_controlFlowGraph.RootNode, null, stack, catchClause.Body.Statements);

			return catchClause;
		}

		/// <summary>
		/// Decompiles a range of instructions to statements
		/// </summary>
		/// <param name="startIp"></param>
		/// <param name="endIp"></param>
		/// <param name="il"></param>
		/// <param name="statements"></param>
		private void DecompileRange(ushort startIp, ushort endIp, byte[] il, ICollection<IStatement> statements)
		{
			if (startIp == endIp)
				return;

			// Build a storted list of instructions in the range
			SortedList<ushort, IInstruction> instructions = DissasembleInstructionRange(startIp, endIp, il);

			// Generate a control flow graph of the block of instructions
			_controlFlowGraph = new ControlFlowGraph(instructions);

			// Generate instructions based on the graph
			GenerateStatementsForGraph(_controlFlowGraph.RootNode, null, new Stack<IExpression>(), statements);
		}

		private IExpression GenerateStatementsForGraph(ICallGraphNode node, ICallGraphNode stopNode, Stack<IExpression> stack, ICollection<IStatement> statements)
		{
			if (node == stopNode)
				return null;

			if (((CallGraphNode)node).Traversed)
				return null;

			((CallGraphNode)node).Traversed = true;

			IExpression expression = GenerateStatementsForNode(node, stack, statements);

			switch (node.LoopType)
			{
				case LoopType.Do:
					{
						List<IStatement> bodyStatements = new List<IStatement>();
						bodyStatements.AddRange(statements);
						statements.Clear();
						//GenerateStatementsForGraph((ICallGraphNode)node.LoopHead.OutEdges[0], node.LoopHead, stack, bodyStatements);
						statements.Add(new DoStatement(expression, new BlockStatement(bodyStatements)));

						return GenerateStatementsForGraph(node.LoopFollow, stopNode, stack, statements);
					}

				case LoopType.While:
					{
						List<IStatement> bodyStatements = new List<IStatement>();

						GenerateStatementsForGraph((ICallGraphNode)node.LoopHead.OutEdges[0], node.LoopHead, stack, bodyStatements);
						statements.Add(new WhileStatement(expression, new BlockStatement(bodyStatements)));

						return GenerateStatementsForGraph(node.LoopFollow, stopNode, stack, statements);
					}

				case LoopType.Endless:
					{
						List<IStatement> bodyStatements = new List<IStatement>();
						GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[0], node.LoopFollow, stack, bodyStatements);
						statements.Add(new WhileStatement(new LiteralExpression(true), new BlockStatement(bodyStatements)));
						break;
					}

				case LoopType.None:
					{
						// Not a loop - will be an If or Case statement
						switch (node.NodeType)
						{
							case NodeType.TwoBranch:
								{
									List<IStatement> thenStatements = new List<IStatement>();
									GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[0], node.IfFollow, stack, thenStatements);

									if (node.IfFollow != node.OutEdges[1])
									{
										List<IStatement> elseStatements = new List<IStatement>();
										GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[1], node.IfFollow, stack, elseStatements);
										statements.Add(new ConditionStatement(expression, new BlockStatement(elseStatements), new BlockStatement(thenStatements)));
									}
									else
									{
										statements.Add(new ConditionStatement(expression, new BlockStatement(thenStatements), null));
									}

									return GenerateStatementsForGraph(node.IfFollow, stopNode, stack, statements);
								}

							case NodeType.MultiBranch:
								{
									List<ICaseStatement> cases = new List<ICaseStatement>();

									ushort[] args = (ushort[])node.Instructions[node.Instructions.Count - 1].Argument;

									for (int i = 0; i < args.Length; i++)
									{
										ICallGraphNode caseNode = (ICallGraphNode)node.OutEdges[i];
										List<IStatement> caseStatements = new List<IStatement>();
										GenerateStatementsForGraph(caseNode, node.CaseTail, stack, caseStatements);

										// If no statements were generated then no need to generate a case statement block
										// for this as its a fall through case where the matching label does not
										// do anything (ie a gap in the sequence of matches)
										if (caseStatements.Count == 0)
											continue;

										IStatement lastStatement = caseStatements[caseStatements.Count - 1];
										if ((!(lastStatement is IThrowExceptionStatement)) && (!(lastStatement is IMethodReturnStatement)))
											caseStatements.Add(new BreakStatement());

										cases.Add(new CaseStatement(new LiteralExpression(i), new BlockStatement(caseStatements)));
									}

									if (node.OutEdges.Count > args.Length)
									{
										ICallGraphNode defaultCaseNode = (ICallGraphNode)node.OutEdges[node.OutEdges.Count - 1];
										List<IStatement> defaultStatements = new List<IStatement>();
										GenerateStatementsForGraph(defaultCaseNode, node.CaseTail, stack, defaultStatements);

										if (defaultStatements.Count > 0)
										{
											IStatement lastStatement = defaultStatements[defaultStatements.Count - 1];
											if ((!(lastStatement is IThrowExceptionStatement)) && (!(lastStatement is IMethodReturnStatement)))
												defaultStatements.Add(new BreakStatement());

											cases.Add(new DefaultCaseStatement(new BlockStatement(defaultStatements)));
										}
									}

									statements.Add(new SwitchStatement(expression, cases));
									return GenerateStatementsForGraph(node.CaseTail, stopNode, stack, statements);
								}

							case NodeType.FallThrough:
							case NodeType.OneBranch:
								{
									if (node.OutEdges.Count == 0)
										return expression;

									List<IStatement> branchStatements = new List<IStatement>();
									expression = GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[0], stopNode, stack, branchStatements);

									foreach (IStatement statement in branchStatements)
										statements.Add(statement);

									return expression;
								}
						}
						break;
					}
			}

			return null;
		}

		/// <summary>
		/// Converts the node into a list of statements and returns an expression if the statement is incomplete
		/// </summary>
		/// <param name="node"></param>
		/// <param name="stack"></param>
		/// <param name="statements"></param>
		/// <returns></returns>
		private IExpression GenerateStatementsForNode(ICallGraphNode node, Stack<IExpression> stack, ICollection<IStatement> statements)
		{
			Idioms.Idiom.Process(node.Instructions);

			return GenerateStatementsFromInstructions(node, stack, statements);
		}

		private IExpression GenerateStatementsFromInstructions(ICallGraphNode node, Stack<IExpression> stack, ICollection<IStatement> statements)
		{
			foreach (Instruction instruction in node.Instructions)
			{
				switch ((ushort)instruction.OpCode.Value)
				{
					case 0x00: // Nop
						{
							break;
						}

					case 0x01: // Break
						{
							break;
						}

					/*case 0x02: // Ldarg_0
					{
						stack.Push(new ThisReferenceExpression());
						break;
					}

					case 0x03: // Ldarg_1
					{
						stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[0])));
						break;
					}

					case 0x04: // Ldarg_2
					{
						stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[1])));
						break;
					}

					case 0x05: // Ldarg_3
					{
						stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[2])));
						break;
					}*/

					/*case 0x06: // Ldloc_0
					{
						stack.Push(new VariableReferenceExpression(_variables[0]));
						break;
					}

					case 0x07: // Ldloc_1
					{
						stack.Push(new VariableReferenceExpression(_variables[1]));
						break;
					}

					case 0x08: // Ldloc_2
					{
						stack.Push(new VariableReferenceExpression(_variables[2]));
						break;
					}

					case 0x09: // Ldloc_3
					{
						stack.Push(new VariableReferenceExpression(_variables[3]));
						break;
					}*/

					/*case 0x0a: // Stloc_0
						{
							statements.Add(
								new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[0]))));
							break;
						}

					case 0x0b: // Stloc_1
						{
							statements.Add(
								new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[1]))));
							break;
						}

					case 0x0c: // Stloc_2
						{
							statements.Add(
								new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[2]))));
							break;
						}

					case 0x0d: // Stloc_3
						{
							statements.Add(
								new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[3]))));
							break;
						}

					case 0x0e: // Ldarg_S
					{
						byte index = (byte) instruction.Argument;
						stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index - 1])));

						break;
					}

					case 0x0f: // Ldarga_S
					{
						byte index = (byte) instruction.Argument;
						stack.Push(
							new AddressOfExpression(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index]))));

						break;
					}*/

					/*case 0x10: // Starg_S
					{
						byte index = (byte) instruction.Argument;
						statements.Add(
							new ExpressionStatement(new AssignExpression(stack.Pop(),
							                                             new ArgumentReferenceExpression(
							                                             	new ParameterDeclaration(_parameters[index - 1])))));
						break;
					}*/

					/*case 0x11: // Ldloc_S
					{
						byte index = (byte) instruction.Argument;
						stack.Push(new VariableReferenceExpression(_variables[index]));
						break;
					}

					case 0x12: // Ldloca_s
					{
						byte variableIndex = (byte) instruction.Argument;
						stack.Push(new AddressOfExpression(new VariableReferenceExpression(_variables[variableIndex])));

						break;
					}*/

					/*case 0x13: // Stloc_S
						{
							byte index = (byte)instruction.Argument;
							statements.Add(
								new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[index]))));

							break;
						}*/

					case 0x14: // Ldnull
						{
							stack.Push(new LiteralExpression(null));
							break;
						}

					/*case 0x15: // Ldc_I4_M1
						{
							stack.Push(new LiteralExpression(-1));
							break;
						}

					case 0x16: // Ldc_I4_0
						{
							stack.Push(new LiteralExpression(0));
							break;
						}

					case 0x17: // Ldc_I4_1
						{
							stack.Push(new LiteralExpression(1));
							break;
						}

					case 0x18: // Ldc_I4_2
						{
							stack.Push(new LiteralExpression(2));
							break;
						}

					case 0x19: // Ldc_I4_3
						{
							stack.Push(new LiteralExpression(3));
							break;
						}

					case 0x1a: // Ldc_I4_4
						{
							stack.Push(new LiteralExpression(4));
							break;
						}

					case 0x1b: // Ldc_I4_5
						{
							stack.Push(new LiteralExpression(5));
							break;
						}

					case 0x1c: // Ldc_I4_6
						{
							stack.Push(new LiteralExpression(6));
							break;
						}

					case 0x1d: // Ldc_I4_7
						{
							stack.Push(new LiteralExpression(7));
							break;
						}

					case 0x1e: // Ldc_I4_8
						{
							stack.Push(new LiteralExpression(8));
							break;
						}

					case 0x1f: // Ldc_I4_S
						{
							int value = (byte)instruction.Argument;
							stack.Push(new LiteralExpression(value));
							break;
						}*/

					case 0x20: // Ldc_I4
						{
							int value = (int)instruction.Argument;
							stack.Push(new LiteralExpression(value));
							break;
						}

					case 0x23: // Ldc_r8
						{
							Double value = (Double)instruction.Argument;
							stack.Push(new LiteralExpression(value));
							break;
						}

					case 0x25: // Dup
						{
							IVariableDeclaration declaration = GenerateVariableDeclaration(typeof(Object));
							_variables.Add(declaration);

							IExpression stackValue = stack.Pop();

							_statements.Add(
								new ExpressionStatement(new AssignExpression(stackValue, new VariableReferenceExpression(declaration))));

							stack.Push(new VariableReferenceExpression(declaration));
							stack.Push(new VariableReferenceExpression(declaration));

							break;
						}

					case 0x26: // Pop
						{
							stack.Pop();
							break;
						}

					case 0x28: // Call:
						{
							bool generateStatement = true;
							IMethodReference methodReference = (IMethodReference)instruction.Argument;

							IExpression target;

							List<IExpression> arguments = new List<IExpression>();
							if (methodReference is IConstructorReference)
							{
								target = stack.Pop();
							}
							else
							{
								// Method
								for (int i = 0; i < methodReference.Parameters.Count; i++)
									arguments.Insert(0, stack.Pop());

								if (!methodReference.IsStatic)
									target = stack.Pop();
								else
									target = new TypeReferenceExpression(methodReference.Resolve().DeclaringType);

								if (methodReference.ReturnType != VoidTypeReference)
									generateStatement = false;
							}

							// Substitue cals to Type.GetTypeRefFromHandle with the typeof operator
							ITypeReferenceExpression targetExpr = target as ITypeReferenceExpression;

							IExpression expression;
							if ((targetExpr != null) && (targetExpr.TypeReference == TypeOfTypeReference) && (methodReference == TypeOfMethodReference)
								&& (arguments.Count == 1) && (arguments[0] is ITypeReferenceExpression))
							{
								expression = new TypeOfExpression(((ITypeReferenceExpression)arguments[0]).TypeReference);
							}
							else
							{
								expression = new MethodInvokeExpression(new MethodReferenceExpression(methodReference, target), arguments);
							}

							if (generateStatement)
								statements.Add(new ExpressionStatement(expression));
							else
								stack.Push(expression);

							break;
						}

					case 0x2A: // Ret:
						{
							if (stack.Count > 0)
								statements.Add(new MethodReturnStatement(stack.Pop()));
							else
								statements.Add(new MethodReturnStatement());
							break;
						}

					case 0x2b: // Br_S
						{
							break;
						}

					case 0x2c: // Brfalse_S - branch if value is false, null, or zero
						{
							return new BinaryExpression(stack.Pop(), BinaryOperator.ValueInequality, new LiteralExpression(null));
						}

					case 0x2d: // Brtrue_S - branch if value is true, not null, or non-zero
						{
							return new BinaryExpression(stack.Pop(), BinaryOperator.ValueEquality, new LiteralExpression(null));
						}

					case 0x2e: // Beq_S
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.ValueInequality, value2);
						}

					case 0x2f: // Bge_S
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThan, value2);
						}

					case 0x30: // Bgt_S
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThanOrEqual, value2);
						}

					case 0x31: // Ble_S
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThan, value2);
						}

					case 0x32: // Blt_S
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThanOrEqual, value2);
						}

					case 0x33: // Bne_un_s
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.ValueEquality, value2);
						}

					case 0x34: // Bge_un_s
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThan, value2);
						}

					case 0x35: // Bgt_un_s
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThanOrEqual, value2);
						}

					case 0x36: // Ble_un_s
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThan, value2);
						}

					case 0x37: // Blt_un_s
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThanOrEqual, value2);
						}

					case 0x38: // Br
						{
							return null;
						}

					case 0x39: // Brfalse - branch if value is false, null, or zero
						{
							return new BinaryExpression(stack.Pop(), BinaryOperator.ValueInequality, new LiteralExpression(null));
						}

					case 0x3a: // Brtrue - branch if value is true, not null, or non-zero
						{
							return new BinaryExpression(stack.Pop(), BinaryOperator.ValueEquality, new LiteralExpression(null));
						}

					case 0x3b: // Beq
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.ValueInequality, value2);
						}

					case 0x3d: // Bgt
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThanOrEqual, value2);
						}

					case 0x3e: // Ble
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThan, value2);
						}

					case 0x3f: // Blt
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThanOrEqual, value2);
						}

					case 0x40: // Bne_Un
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.ValueEquality, value2);
						}

					case 0x45: // Switch
						{
							return stack.Pop();
						}

					case 0x46: // Ldind_I1
					case 0x47: // Ldind_U1
					case 0x48: // Ldind_I2
					case 0x49: // Ldind_U2
					case 0x4a: // Ldind_I4
					case 0x4b: // Ldind_U4
					case 0x4c: // Ldind_I8
					case 0x4d: // Ldind_I
					case 0x4e: // Ldind_R4
					case 0x4f: // Ldind_R8
					case 0x50: // Ldind_ref
						{
							stack.Push(new AddressDereferenceExpression(stack.Pop()));

							break;
						}

					case 0x51: // Stind_ref
					case 0x52: // Stind_I1
					case 0x53: // Stind_I2
					case 0x54: // Stind_I4
					case 0x55: // Stind_I8
					case 0x56: // Stind_R4
					case 0x57: // Stind_R8
						{
							IExpression value = stack.Pop();
							IExpression address = stack.Pop();

							statements.Add(new ExpressionStatement(new AssignExpression(value, new AddressDereferenceExpression(address))));
							break;
						}

					case 0x58: // Add 
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Add, right));

							break;
						}

					case 0x59: // Sub 
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Subtract, right));

							break;
						}

					case 0x5a: // Mull 
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Multiply, right));

							break;
						}

					case 0x5b: // Div
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Divide, right));

							break;
						}

					case 0x5d: // Rem
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Modulus, right));

							break;
						}

					case 0x5f: // And
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.BitwiseOr, right));

							break;
						}

					case 0x60: // Or
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.BitwiseAnd, right));

							break;
						}

					case 0x61: // Xor
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.BitwiseExclusiveOr, right));

							break;
						}

					case 0x62: // Shl
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.ShiftLeft, right));

							break;
						}

					case 0x63: // Shr
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.ShiftRight, right));

							break;
						}

					case 0x64: // Shr_un
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.ShiftRight, right));

							break;
						}

					case 0x66: // Not
						{
							stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.BitwiseNot));
							break;
						}

					case 0x67: // Conv_I1
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(sbyte), new Type[0])));
							break;
						}

					case 0x68: // Conv_I2
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(short), new Type[0])));
							break;
						}

					case 0x69: // Conv_I4
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(int), new Type[0])));
							break;
						}

					case 0x6a: // Conv_I8
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(long), new Type[0])));
							break;
						}

					case 0x6d: // Conv_U4
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(uint), new Type[0])));
							break;
						}

					case 0x6e: // Conv_U8
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(ulong), new Type[0])));
							break;
						}

					case 0x6F: // Callvirt 
						{
							bool generateStatement = true;
							IMethodReference methodReference = (IMethodReference)instruction.Argument;

							IExpression target = null;

							List<IExpression> arguments = new List<IExpression>();
							if (!(methodReference is IConstructorReference))
							{
								for (int i = 0; i < methodReference.Parameters.Count; i++)
									arguments.Insert(0, stack.Pop());

								if (!methodReference.IsStatic)
									target = stack.Pop();
								else
									target = new TypeReferenceExpression(methodReference.Resolve().DeclaringType);

								if (methodReference.ReturnType != VoidTypeReference)
									generateStatement = false;
							}

							IExpression expression = new MethodInvokeExpression(new MethodReferenceExpression(methodReference, target), arguments);

							if (generateStatement)
								statements.Add(new ExpressionStatement(expression));
							else
								stack.Push(expression);

							break;
						}

					case 0x71: // Ldobj
						{
							stack.Push(new AddressDereferenceExpression(stack.Pop()));
							break;
						}

					case 0x72: // Ldstr
						{
							stack.Push(new LiteralExpression(instruction.Argument));
							break;
						}

					case 0x73: // Newobj
						{
							IMethodReference methodReference = (IMethodReference)instruction.Argument;

							IConstructorReference constructorReference = methodReference as IConstructorReference;
							if (constructorReference == null)
								throw new ApplicationException("Newobj did not find a constructor method reference.");

							List<IExpression> args = new List<IExpression>();
							for (int i = 0; i < constructorReference.Parameters.Count; i++)
								args.Insert(0, stack.Pop());

							stack.Push(new ObjectCreateExpression(constructorReference, args, constructorReference.Resolve().DeclaringType));

							break;
						}

					case 0x75: // Isinst
						{
							ITypeReference typeReference = (ITypeReference)instruction.Argument;

							stack.Push(new ValueOfTypedReferenceExpression(stack.Pop(), typeReference));
							break;
						}

					case 0x7a: // Throw
						{
							statements.Add(new ThrowExceptionStatement(stack.Pop()));
							break;
						}

					case 0x74: // Castclass
						{
							ITypeReference typeReference = (ITypeReference)instruction.Argument;
							stack.Push(new CastExpression(stack.Pop(), typeReference));
							break;
						}

					case 0x7b: // Ldfld:
						{
							// Get the field in the target object
							IFieldReference fieldReference = (IFieldReference)instruction.Argument;

							stack.Push(new FieldReferenceExpression(fieldReference, stack.Pop()));

							break;
						}

					case 0x7c: // Ldflda:
						{
							// Get the field in the target object
							IFieldReference fieldReference = (IFieldReference)instruction.Argument;

							stack.Push(new AddressOfExpression(new FieldReferenceExpression(fieldReference, stack.Pop())));

							break;
						}

					case 0x7d: // Stfld
						{
							IExpression value = stack.Pop();
							IExpression target = stack.Pop();
							IFieldReference fieldReference = (IFieldReference)instruction.Argument;

							statements.Add(new ExpressionStatement(new AssignExpression(value, new FieldReferenceExpression(fieldReference, target))));

							break;
						}

					case 0x7e: // Ldsfld
						{
							IFieldReference fieldReference = (IFieldReference)instruction.Argument;
							stack.Push(new FieldReferenceExpression(fieldReference, new TypeReferenceExpression(fieldReference.FieldType)));

							break;
						}

					case 0x80: // Stsfld
						{
							IExpression value = stack.Pop();
							IFieldReference fieldReference = (IFieldReference)instruction.Argument;
							statements.Add(new ExpressionStatement(new AssignExpression(value, new FieldReferenceExpression(fieldReference))));
							break;
						}

					case 0x81: // Stobj 
						{
							IExpression value = stack.Pop();
							IExpression address = stack.Pop();
							statements.Add(new ExpressionStatement(new AssignExpression(value, new AddressDereferenceExpression(address))));

							break;
						}

					case 0x8c: // Box
						{
							break;
						}

					case 0x8d: // Newarr
						{
							ITypeReference typeReference = (ITypeReference)instruction.Argument;
							List<IExpression> dimensions = new List<IExpression>();
							dimensions.Add(stack.Pop());

							stack.Push(new ArrayCreateExpression(typeReference, dimensions, null));

							break;
						}

					case 0x8e: // Ldlen
						{
							IExpression arrayRef = stack.Pop();
							IMethodReference methodReference = AssemblyManager.FindMethod(typeof(Array).GetProperty("Length").GetGetMethod());

							stack.Push(new MethodInvokeExpression(new MethodReferenceExpression(methodReference, arrayRef), new List<IExpression>()));
							break;
						}

					case 0x8f: // Ldelema
						{
							IExpression index = stack.Pop();
							IExpression arrayReference = stack.Pop();
							stack.Push(new AddressOfExpression(new ArrayIndexerExpression(arrayReference, index)));
							break;
						}

					case 0x90: // Ldelm_I1
					case 0x91: // Ldelm_U1
					case 0x92: // Ldelm_I2
					case 0x93: // Ldelm_U2
					case 0x94: // Ldelm_I4
					case 0x95: // Ldelm_U4
					case 0x96: // Ldelm_I8
					case 0x97: // Ldelm_I
					case 0x98: // Ldelm_R4
					case 0x99: // Ldelm_R8
					case 0x9a: // Ldelm_Ref
					case 0xa3: // Ldelm
						{
							IExpression index = stack.Pop();
							IExpression arrayReference = stack.Pop();
							stack.Push(new ArrayIndexerExpression(arrayReference, index));
							break;
						}

					case 0x9b: // Stelm_I
					case 0x9c: // Stelm_I1
					case 0x9d: // Stelm_I2
					case 0x9e: // Stelm_I4
					case 0x9f: // Stelm_I8
					case 0xa0: // Stelm_R4
					case 0xa1: // Stelm_R8
					case 0xa2: // Stelm_ref
						{
							IExpression value = stack.Pop();
							IExpression index = stack.Pop();
							IExpression arrayReference = stack.Pop();

							statements.Add(new ExpressionStatement(new AssignExpression(value, new ArrayIndexerExpression(arrayReference, index))));

							break;
						}

					case 0xa4: // Stelem
						{
							IExpression value = stack.Pop();
							IExpression index = stack.Pop();
							IExpression arrayReference = stack.Pop();

							// The type token argument should be the type of the value - There is no need to check
							statements.Add(new ExpressionStatement(new AssignExpression(value, new ArrayIndexerExpression(arrayReference, index))));

							break;
						}

					case 0xa5: // Unbox_any
						{
							break;
						}

					case 0xe0: // Conv_U
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(IntPtr), new Type[0])));
							break;
						}

					case 0xd0: // Ldtoken
						{
							object token = instruction.Argument;
							if (token is IFieldReference)
							{
								IFieldReference fieldReference = (IFieldReference)token;
								if (fieldReference.Resolve().IsStatic)
									stack.Push(new FieldReferenceExpression((IFieldReference)token, new TypeReferenceExpression(fieldReference.Resolve().DeclaringType)));
								else
									stack.Push(new FieldReferenceExpression((IFieldReference)token));
							}
							else if (token is IMethodReference)
								stack.Push(new MethodReferenceExpression((IMethodReference)token, stack.Pop()));
							else if (token is ITypeReference)
								stack.Push(new TypeReferenceExpression((ITypeReference)token));
							else
								throw new ApplicationException("Unknown token type in ldtoken");

							break;
						}

					case 0xd1: // Conv_U2
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(ushort), new Type[0])));
							break;
						}

					case 0xd2: // Conv_U1
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(byte), new Type[0])));
							break;
						}

					case 0xd3: // Conv_I
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(int), new Type[0])));
							break;
						}

					case 0xd6: // Add_Ovf 
					case 0xd7: // Add_Ovf_Un 
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Add, right));

							break;
						}

					case 0xd8: // Mull_Ovf
					case 0xd9: // Mull_Ovf_Un
						{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Multiply, right));

							break;
						}

					case 0xdc: // Endfinally
						{
							stack.Clear();
							break;
						}

					case 0xdd: // Leave
						{
							stack.Clear();
							break;
						}

					case 0xde: // Leave_s
						{
							stack.Clear();
							break;
						}

					case 0xfe00: // ArgList
						{

							break;
						}

					case 0xfe01:  // Ceq
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							stack.Push(new BinaryExpression(value1, BinaryOperator.ValueInequality, value2));

							break;
						}

					case 0xfe02: // Cgt
					case 0xfe03: // Cgt_U
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							stack.Push(new BinaryExpression(value1, BinaryOperator.LessThan, value2));

							break;
						}

					case 0xfe04: // Clt
					case 0xfe05: // Clt_U
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							stack.Push(new BinaryExpression(value1, BinaryOperator.GreaterThan, value2));

							break;
						}

					case 0xfe09: // Ldarg
						{
							ushort index = (ushort)instruction.Argument;

							if (_netMethod.IsStatic)
								stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index])));
							else if (index == 0)
								stack.Push(new ThisReferenceExpression());
							else
								stack.Push(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index - 1])));
							break;
						}

					case 0xfe0a: // Ldarga
						{
							ushort index = (ushort)instruction.Argument;

							if (_netMethod.IsStatic)
								stack.Push(new AddressOfExpression(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index]))));
							else if (index == 0)
								stack.Push(new AddressOfExpression(new ThisReferenceExpression()));
							else
								stack.Push(new AddressOfExpression(new ArgumentReferenceExpression(new ParameterDeclaration(_parameters[index - 1]))));

							break;
						}

					case 0xfe0b: // Starg
						{
							ushort index = (ushort)instruction.Argument;
							if (_netMethod.IsStatic)
								statements.Add(
									new ExpressionStatement(new AssignExpression(stack.Pop(),
																				 new ArgumentReferenceExpression(
																					new ParameterDeclaration(_parameters[index])))));
							else if (index == 0)
								throw new ApplicationException("Cannot change the this reference");
							else
								statements.Add(
									new ExpressionStatement(new AssignExpression(stack.Pop(),
																				 new ArgumentReferenceExpression(
																					new ParameterDeclaration(_parameters[index - 1])))));
							break;
						}

					case 0xfe0c: // Ldloc
						{
							stack.Push(new VariableReferenceExpression(_variables[(ushort)instruction.Argument]));
							break;
						}

					case 0xfe0d: // Ldloca
						{
							stack.Push(new AddressOfExpression(new VariableReferenceExpression(_variables[(ushort)instruction.Argument])));
							break;
						}

					case 0xfe0e: // Stloc
						{
							statements.Add(new ExpressionStatement(new AssignExpression(stack.Pop(), new VariableReferenceExpression(_variables[(ushort)instruction.Argument]))));

							break;
						}

					case 0xfe0f: // Localloc
						{
							stack.Push(new StackAllocateExpression(stack.Pop()));
							break;
						}

					case 0xfe13: // Volatile
						{
							break;
						}

					case 0xfe15: // Initobj
						{
							stack.Push(new ArgumentListExpression());
							break;
						}

					case 0xfe16: // Constrained
						{
							break;
						}

					// CUSTOM OP CODE PROCESSING
					case 0x100: // PreInc
						{
							stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.PreIncrement));
							break;
						}

					case 0x101: // PreDec
						{
							stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.PreDecrement));
							break;
						}

					case 0x102: // PostInc
						{
							if ((int)instruction.Argument == 1)
								stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.PostIncrement));
							else
								stack.Push(new BinaryExpression(stack.Pop(), BinaryOperator.Increment, new LiteralExpression(instruction.Argument)));
							break;
						}

					case 0x103: // PostDec
						{
							if ((int)instruction.Argument == 1)
								stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.PostDecrement));
							else
								stack.Push(new BinaryExpression(stack.Pop(), BinaryOperator.Decrement, new LiteralExpression(instruction.Argument)));
							break;
						}

					case 0x104: // PostIncStmt
						{
							if ((int)instruction.Argument == 1)
								_statements.Add(new ExpressionStatement(new UnaryExpression(stack.Pop(), UnaryOperator.PostIncrement)));
							else
								_statements.Add(new ExpressionStatement(new BinaryExpression(stack.Pop(), BinaryOperator.Increment, new LiteralExpression(instruction.Argument))));
							break;
						}

					case 0x105: // PostDecStmt
						{
							if ((int)instruction.Argument == 1)
								_statements.Add(new ExpressionStatement(new UnaryExpression(stack.Pop(), UnaryOperator.PostDecrement)));
							else
								_statements.Add(new ExpressionStatement(new BinaryExpression(stack.Pop(), BinaryOperator.Decrement, new LiteralExpression(instruction.Argument))));
							break;
						}

					default:
						{
							throw new ApplicationException(String.Format("Unknown opcode {0} at {1:x4}", instruction.OpCode.Name, instruction.IP));
						}
				}
			}

			return null;
		}

		#endregion

	}
}
