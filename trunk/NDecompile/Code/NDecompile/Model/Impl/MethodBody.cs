using System;
using System.Collections.Generic;
using System.Reflection;
using NetMethod = System.Reflection.MethodBase;
using NetMethodBody = System.Reflection.MethodBody;
using LittleNet.NDecompile.Model.Impl.Idioms;
using System.Reflection.Emit;

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
		private readonly Type[] _genericTypeArgs;
		private readonly Type[] _genericMethodArgs;

		private IList<IInstruction> _instructions;
		private IControlFlowGraph _controlFlowGraph;
		private IList<IStatement> _statements;
		private IList<IVariableDeclaration> _variables;
		private readonly IList<String> _variableNames = new List<string>();

		private MethodDeclarationBase _methodDeclarationBase;

		private IMethodInvokeExpression _initialiser;

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
		/// <param name="netMethod"></param>
		/// <param name="module"></param>
		/// <param name="methodDeclarationBase"></param>
		public MethodBody(MethodBase netMethod, Module module, MethodDeclarationBase methodDeclarationBase)
		{
			_netMethod = netMethod;
			_module = module;
			_methodDeclarationBase = methodDeclarationBase;
			_body = netMethod.GetMethodBody();

			// Deal with the arguments
			_parameters = netMethod.GetParameters();
			if (netMethod.IsGenericMethod)
				_genericMethodArgs = netMethod.GetGenericArguments();
			else
				_genericMethodArgs = new Type[0];

			if (_netMethod.DeclaringType.IsGenericType)
				_genericTypeArgs = _netMethod.DeclaringType.GetGenericArguments();
			else
				_genericTypeArgs = new Type[0];
			
			Console.WriteLine(_netMethod.Name);
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

				if (_controlFlowGraph == null)
					_controlFlowGraph = new ControlFlowGraph(_body, _module, _genericTypeArgs, _genericMethodArgs);

				return _controlFlowGraph;
			}
		}

		public IList<IStatement> Statements
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

		public IMethodInvokeExpression Initialiser
		{
			get
			{
				if (_body == null)
					return null;
				
				if (_statements == null)
					Decompile();

				return _initialiser;
			}
		}

		public IList<IVariableDeclaration> Variables
		{
			get
			{
				if (_body == null)
					return null;

				if (_variables == null)
					GenerateLocalVariables();

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

			ushort ip = 0;

			_instructions = new List<IInstruction>();
			while (ip < il.Length)
				_instructions.Add(OpCodeTable.GetInstruction(il, ref ip, _module, _genericTypeArgs, _genericMethodArgs));
		}

		#endregion

		#region Decompile

		public void Decompile()
		{
			// Write the variable declaraions
			_statements = new List<IStatement>();
			foreach (IVariableDeclaration variableDeclaration in Variables)
				_statements.Add(new ExpressionStatement(new VariableDeclarationExpression(variableDeclaration)));

			// Generate instructions based on the graph
			GenerateStatementsForGraph(ControlFlowGraph.RootNode, null, new Stack<IExpression>(), _statements);
		}

		/// <summary>
		/// Generates variable declarations for all the local variables of a method
		/// </summary>
		private void GenerateLocalVariables()
		{
			// Build the variable declaration table and invent some variable names based on type
			_variables = new List<IVariableDeclaration>();
			for (int i = 0; i < _body.LocalVariables.Count; i++)
				_variables.Add(GenerateVariableDeclaration(_body.LocalVariables[i].LocalType));
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
						GenerateStatementsForGraph((ICallGraphNode)node.LoopHead.OutEdges[0], node.LoopHead, stack, bodyStatements);
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
						// Not a loop - will be a Try-Handle, If, or Case statement
						switch (node.NodeType)
						{
							case NodeType.TryCatch:
                            {
                                TryCatchFinallyStatement tryCatchStatement = new TryCatchFinallyStatement();
								
								// Generate the statements in the try handler
								expression = GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[0], node.HandlerNode, stack, tryCatchStatement.Try.Statements);

								// Determine the exception variable
								CatchClause catchClause;
								IInstruction handlerInstruction = node.HandlerNode.Instructions[0];
								if (handlerInstruction.OpCode.Value == OpCodes.Stloc_0.Value)
									catchClause = new CatchClause(_variables[0], expression);
								else if (handlerInstruction.OpCode.Value == OpCodes.Stloc_1.Value)
									catchClause = new CatchClause(_variables[1], expression);
								else if (handlerInstruction.OpCode.Value == OpCodes.Stloc_2.Value)
									catchClause = new CatchClause(_variables[2], expression);
								else if (handlerInstruction.OpCode.Value == OpCodes.Stloc_3.Value)
									catchClause = new CatchClause(_variables[3], expression);
								else if (handlerInstruction.OpCode.Value == OpCodes.Stloc.Value)
									catchClause = new CatchClause(_variables[(int)handlerInstruction.Argument], expression);
								else if (handlerInstruction.OpCode.Value == OpCodes.Stloc_S.Value)
									catchClause = new CatchClause(_variables[(byte)handlerInstruction.Argument], expression);
								else// if (handlerInstruction.OpCode.Value == OpCodes.Pop.Value)
									catchClause = new CatchClause(null, expression);
								//else
								//	throw new ApplicationException("Expected stloc or pop instruction");		
							
								node.HandlerNode.Instructions.RemoveAt(0);			
					
								// Generate the statements in the handler
								GenerateStatementsForGraph(node.HandlerNode, node.FollowNode, stack, catchClause.Body.Statements);

                                tryCatchStatement.CatchClauses.Add(catchClause);
								statements.Add(tryCatchStatement);
					
								// Generate the statements after the try-catch
								return GenerateStatementsForGraph(node.FollowNode, stopNode, stack, statements);	
							}

                            case NodeType.TryFinally:
                            {
                                TryCatchFinallyStatement tryFinallyStatement = new TryCatchFinallyStatement();

                                // Generate the statements in the try handler
                                expression = GenerateStatementsForGraph((ICallGraphNode)node.OutEdges[0], node.HandlerNode, stack, tryFinallyStatement.Try.Statements);
                                
                                // Generate the statements in the handler
                                GenerateStatementsForGraph(node.HandlerNode, node.FollowNode, stack, tryFinallyStatement.Finally.Statements);

                                statements.Add(tryFinallyStatement);

                                // Generate the statements after the try-finally
                                return GenerateStatementsForGraph(node.FollowNode, stopNode, stack, statements);
                            }
					
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

									if (node.IfFollow != null)
										return GenerateStatementsForGraph(node.IfFollow, stopNode, stack, statements);
									else
										return null;
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
			Idiom.Process(node.Instructions);

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

					case 0x14: // Ldnull
						{
							stack.Push(new LiteralExpression(null));
							break;
						}

					case 0x20: // Ldc_I4
						{
							int value = (int)instruction.Argument;
							stack.Push(new LiteralExpression(value));
							break;
						}
					
					case 0x21: // Ldc_I8
						{
							long value = (long)instruction.Argument;
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

							List<IExpression> arguments = new List<IExpression>();
							for (int i = 0; i < methodReference.Parameters.Count; i++)
								arguments.Insert(0, stack.Pop());

							IExpression target;

							if (methodReference.IsStatic)
							{
								// Static method
								target = new TypeReferenceExpression(methodReference.Resolve().DeclaringType);
							}
							else
							{
								// Instance method
								target = stack.Pop();

								// Substitue a this reference for a base reference if the method we are invoking is not declared on the current type
								if ((target is IThisReferenceExpression) && (methodReference.Resolve().DeclaringType != _methodDeclarationBase.DeclaringType))
									target = new BaseReferenceExpression();
							}

							if (methodReference.ReturnType != VoidTypeReference) 
								generateStatement = false;

							if (methodReference is IConstructorReference)
								generateStatement = true;

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

							if ((target is IBaseReferenceExpression) && (methodReference is IConstructorReference))
								_initialiser = (IMethodInvokeExpression)expression;
							else if (generateStatement)
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
					
					case 0x3c: // Bge
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThan, value2);
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

					case 0x41: // Bge_Un
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThanOrEqual, value2);
						}

					case 0x42: // Bgt_Un
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.LessThan, value2);
						}

					case 0x43: // Ble_Un
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThanOrEqual, value2);
						}

					case 0x44: // Blt_Un
						{
							IExpression value2 = stack.Pop();
							IExpression value1 = stack.Pop();

							return new BinaryExpression(value1, BinaryOperator.GreaterThan, value2);
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
									
					case 0x65: // Neg
						{
							stack.Push(new UnaryExpression(stack.Pop(), UnaryOperator.BooleanNot));
							
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

					case 0x6c: // Conv_R8
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(double), new Type[0])));
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

							List<IExpression> arguments = new List<IExpression>();
							for (int i = 0; i < methodReference.Parameters.Count; i++)
								arguments.Insert(0, stack.Pop());

							IExpression target;
							if (methodReference.IsStatic)
							{
								target = new TypeReferenceExpression(methodReference.Resolve().DeclaringType);
							}
							else
							{
								target = stack.Pop();

								// Dereference the target NB - this may be better done in the constrained op code
								if (target is IAddressOfExpression)
									target = ((IAddressOfExpression) target).Expression;
							}

							if (methodReference.ReturnType != VoidTypeReference)
								generateStatement = false;

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

					case 0x7f: // Ldsflda
					{
						IFieldReference fieldReference = (IFieldReference) instruction.Argument;
						stack.Push(
							new AddressOfExpression(new FieldReferenceExpression(fieldReference,
							                                                 new TypeReferenceExpression(fieldReference.FieldType))));

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
						
					case 0xb8: // Conv_Ovf_u4
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(UInt32), new Type[0])));
					
							break;
						}
					
					case 0xba: // Conv_Ovf_u8
						{
							stack.Push(new CastExpression(stack.Pop(), AssemblyManager.FindType(typeof(UInt64), new Type[0])));
					
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
					
					case 0xda: // Sub_Ovf
					{
							IExpression right = stack.Pop();
							IExpression left = stack.Pop();
							stack.Push(new BinaryExpression(left, BinaryOperator.Subtract, right));

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
							throw new ApplicationException(String.Format("Unknown opcode {0} ({1:x4}) at {2:x4}", instruction.OpCode.Name, instruction.OpCode.Value, instruction.IP));
						}
				}
			}

			return null;
		}

		#endregion

	}
}
