using System;
using System.Collections.Generic;
using System.Text;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile
{
	public class CSharpWriter : ILanguageWriter
	{
		private static ITypeReference ObjectType = AssemblyManager.FindType(typeof(Object), new Type[0]);

		#region ILanguageWriter Members

		public void WriteMethodDeclaration(IMethodDeclaration methodDeclaration, IFormattedCodeWriter writer)
		{
			WriteAttributes(methodDeclaration, writer, false);

			if (!methodDeclaration.DeclaringType.Resolve().IsInterface)
			{
				if (methodDeclaration.Visibility == MethodVisibility.Public)
					writer.WriteKeyword("public");
				if (methodDeclaration.Visibility == MethodVisibility.Private)
					writer.WriteKeyword("private");
				if (methodDeclaration.Visibility == MethodVisibility.Assembly)
					writer.WriteKeyword("internal");
				if (methodDeclaration.Visibility == MethodVisibility.Family)
					writer.WriteKeyword("protected");
				if (methodDeclaration.Visibility == MethodVisibility.FamilyAndAssembly)
					writer.WriteKeyword("protected");
				if (methodDeclaration.Visibility == MethodVisibility.FamilyOrAssembly)
					writer.WriteKeyword("protected");

				writer.Write(' ');
			}

			if (methodDeclaration.IsStatic)
			{
				writer.WriteKeyword("static");
				writer.Write(' ');
			}

			if ((methodDeclaration.Body.Statements == null) && (!methodDeclaration.Abstract))
			{
				writer.WriteKeyword("extern");
				writer.Write(' ');
			}

			if (!(methodDeclaration is IConstructorDeclaration))
			{
				WriteTypeReference(methodDeclaration.ReturnType, writer);
				writer.Write(' ');
			}

			// Substitue the name of the class for constructors
			if ((methodDeclaration.Name == ".ctor") || (methodDeclaration.Name == ".cctor"))
				writer.Write(methodDeclaration.DeclaringType.Name);
			else
				writer.Write(methodDeclaration.Name);

			writer.Write("(");

			if (methodDeclaration.Parameters.Count > 0)
				WriteParameterDeclaration(methodDeclaration.Parameters[0], writer);

			for (int i = 1; i < methodDeclaration.Parameters.Count; i++)
			{
				writer.Write(", ");
				WriteParameterDeclaration(methodDeclaration.Parameters[i], writer);
			}

			if ((methodDeclaration.Abstract) || (methodDeclaration.Body.Statements == null))
			{
				writer.WriteLine(");");
			}
			else
			{

				if (methodDeclaration.Body.Initialiser != null)
				{
					writer.Write(") : ");
					writer.WriteKeyword("base");
					writer.Write('(');
					if (methodDeclaration.Body.Initialiser.Arguments.Count > 0)
						WriteExpression(methodDeclaration.Body.Initialiser.Arguments[0], writer);

					for (int i = 1; i < methodDeclaration.Body.Initialiser.Arguments.Count; i++)
					{
						writer.Write(", ");
						WriteExpression(methodDeclaration.Body.Initialiser.Arguments[i], writer);
					}

					writer.WriteLine(") {");
				} else
					writer.WriteLine(')');

				writer.WriteLine('{');
				writer.Indent++;
				WriteStatementList(methodDeclaration.Body.Statements, writer);
				writer.Indent--;
				writer.WriteLine('}');
			}
		}

		private static void WriteAttribute(IAttribute attribute, IFormattedCodeWriter writer, bool inline)
		{
			if (!inline)
				writer.Write('[');

			writer.WriteType(attribute.Constructor.Resolve().DeclaringType, attribute.Constructor.Resolve().DeclaringType.Name.Substring(0, attribute.Constructor.Resolve().DeclaringType.Name.Length - 9));

			if (attribute.Arguments.Count > 0)
			{
				writer.Write('(');
				WriteExpression(attribute.Arguments[0], writer);

				for (int i = 1; i < attribute.Arguments.Count; i++)
				{
					writer.Write(", ");
					WriteExpression(attribute.Arguments[i], writer);
				}

				writer.Write(')');
			}

			if (!inline)
				writer.WriteLine(']');
		}

		private static void WriteAttributes(IAttributeProvider attributeProvider, IFormattedCodeWriter writer, bool inline)
		{
			if (attributeProvider.Attributes.Count == 0)
				return;

			if (inline)
			{
				writer.Write('[');
				WriteAttribute(attributeProvider.Attributes[0], writer, true);

				for (int i = 1; i < attributeProvider.Attributes.Count; i++)
				{
					writer.Write(", ");
					WriteAttribute(attributeProvider.Attributes[i], writer, true);
				}
				writer.Write(']');
			}
			else
			{
				foreach (IAttribute attribute in attributeProvider.Attributes)
					WriteAttribute(attribute, writer, false);
			}
		}

		public void WriteTypeDeclaration(ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer)
		{
			if ((typeDeclaration.Namespace != null) && (typeDeclaration.DeclaringType == null))
			{
				writer.WriteKeyword("namespace");
				writer.Write(' ');
				writer.WriteIdentifier(typeDeclaration.Namespace);
				writer.WriteLine();
				writer.WriteLine('{');
				writer.Indent++;
			}

			WriteAttributes(typeDeclaration, writer, false);

			if (typeDeclaration.IsEnum)
				WriteEnumDeclaration(typeDeclaration, writer);
			else if (typeDeclaration.IsInterface)
				WriteInterfaceDeclaration(typeDeclaration, writer);
			else
				WriteClassDeclaration(typeDeclaration, writer);

			if ((typeDeclaration.Namespace != null) && (typeDeclaration.DeclaringType == null))
			{
				writer.Indent--;
				writer.WriteLine('}');
			}
		}

		private static void WriteEnumDeclaration(ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer)
		{
			if (typeDeclaration.Visibility == TypeVisibility.Public)
			{
				writer.WriteKeyword("public");
				writer.Write(' ');
			}
			else if (typeDeclaration.Visibility == TypeVisibility.Private)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
			}

			writer.WriteKeyword("enum");
			writer.WriteLine();
			writer.WriteLine('{');
			writer.Indent++;

			bool first = true;
			foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
			{
				if (!fieldDeclaration.IsConst)
					continue;

				if (first)
					first = false;
				else
					writer.WriteLine(',');

				writer.WriteIdentifier(fieldDeclaration.Name);

				if (fieldDeclaration.Initialiser != null)
				{
					writer.Write(" = ");
					WriteLiteralExpression(fieldDeclaration.Initialiser, writer);
				}
			}

			writer.WriteLine();
			writer.Indent--;
			writer.WriteLine('}');
		}

		private void WriteInterfaceDeclaration(ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer)
		{
			if (typeDeclaration.Visibility == TypeVisibility.Public)
			{
				writer.WriteKeyword("public");
				writer.Write(' ');
			}
			else if (typeDeclaration.Visibility == TypeVisibility.Private)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
			}

			writer.WriteKeyword("interface");
			writer.Write(' ');

			if ((typeDeclaration.IsGeneric) && (typeDeclaration.Name.LastIndexOf('`') > 0))
			{
				String name = typeDeclaration.Name.Substring(0, typeDeclaration.Name.LastIndexOf('`'));
				writer.Write(name);

				writer.Write('<');
				if (typeDeclaration.GenericArguments.Count > 0)
				{
					WriteTypeReference(typeDeclaration.GenericArguments[0], writer);
					for (int i = 1; i < typeDeclaration.GenericArguments.Count; i++)
					{
						writer.Write(", ");
						WriteTypeReference(typeDeclaration.GenericArguments[1], writer);
					}
				}
				writer.Write('>');
			}
			else
			{
				writer.Write(typeDeclaration.Name);
			}

			if (typeDeclaration.Interfaces.Count > 0)
			{
				writer.Write(" : ");
				WriteTypeReference(typeDeclaration.Interfaces[0], writer);

				for (int i = 1; i < typeDeclaration.Interfaces.Count; i++)
				{
					writer.Write(", ");
					WriteTypeReference(typeDeclaration.Interfaces[i], writer);
				}
			}

			writer.WriteLine();
			writer.WriteLine('{');
			writer.Indent++;

			foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
			{
				writer.WriteLine();
				WriteMethodDeclaration(methodDeclaration, writer);
			}

			foreach (IPropertyDeclaration propertyDeclaration in typeDeclaration.Properties)
			{
				writer.WriteLine();
				WritePropertyDeclaration(propertyDeclaration, writer);
			}

			foreach (IEventDeclaration eventDeclaration in typeDeclaration.Events)
			{
				writer.WriteLine();
				WriteEventDeclaration(eventDeclaration, writer);
			}

			writer.Indent--;
			writer.WriteLine('}');
		}

		private void WriteClassDeclaration(ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer)
		{
			if (typeDeclaration.Visibility == TypeVisibility.Public)
			{
				writer.WriteKeyword("public");
				writer.Write(' ');
			}
			else if (typeDeclaration.Visibility == TypeVisibility.Private)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
			}

			writer.WriteKeyword("class");
			writer.Write(" ");

			if ((typeDeclaration.IsGeneric) && (typeDeclaration.Name.LastIndexOf('`') > 0))
			{
				String name = typeDeclaration.Name.Substring(0, typeDeclaration.Name.LastIndexOf('`'));
				writer.Write(name);

				writer.Write('<');
				if (typeDeclaration.GenericArguments.Count > 0)
				{
					WriteTypeReference(typeDeclaration.GenericArguments[0], writer);
					for (int i = 1; i < typeDeclaration.GenericArguments.Count; i++)
					{
						writer.Write(", ");
						WriteTypeReference(typeDeclaration.GenericArguments[1], writer);
					}
				}
				writer.Write('>');
			}
			else
			{
				writer.Write(typeDeclaration.Name);
			}

			if (((typeDeclaration.BaseType != null) && (typeDeclaration.BaseType != ObjectType)) || (typeDeclaration.Interfaces.Count > 0))
				writer.Write(" : ");

			if ((typeDeclaration.BaseType != null) && (typeDeclaration.BaseType != ObjectType))
				WriteTypeReference(typeDeclaration.BaseType, writer);

			if (typeDeclaration.Interfaces.Count > 0)
			{
				if ((typeDeclaration.BaseType != null) && (typeDeclaration.BaseType != ObjectType))
					writer.Write(", ");

				WriteTypeReference(typeDeclaration.Interfaces[0], writer);

				for (int i = 1; i < typeDeclaration.Interfaces.Count; i++)
				{
					writer.Write(", ");
					WriteTypeReference(typeDeclaration.Interfaces[i], writer);
				}
			}

			writer.WriteLine();
			writer.WriteLine('{');
			writer.Indent++;

			foreach (ITypeDeclaration nestedTypeDeclaration in typeDeclaration.Types)
			{
				writer.WriteLine();
				writer.BeginOutlineBlock();
				WriteTypeDeclaration(nestedTypeDeclaration, writer);
				writer.EndOutlineBlock();
			}

			foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
			{
				writer.WriteLine();
				WriteFieldDeclaration(fieldDeclaration, writer);
			}

			foreach (IConstructorDeclaration constructorDeclaration in typeDeclaration.Constructors)
			{
				writer.WriteLine();
				writer.BeginOutlineBlock();
				WriteMethodDeclaration(constructorDeclaration, writer);
				writer.EndOutlineBlock();
			}

			foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
			{
				writer.WriteLine();
				writer.BeginOutlineBlock();
				WriteMethodDeclaration(methodDeclaration, writer);
				writer.EndOutlineBlock();
			}

			foreach (IPropertyDeclaration propertyDeclaration in typeDeclaration.Properties)
			{
				writer.WriteLine();
				WritePropertyDeclaration(propertyDeclaration, writer);
			}

			foreach (IEventDeclaration eventDeclaration in typeDeclaration.Events)
			{
				writer.WriteLine();
				WriteEventDeclaration(eventDeclaration, writer);
			}

			writer.Indent--;
			writer.WriteLine('}');
		}

		public void WriteFieldDeclaration(IFieldDeclaration fieldDeclaration, IFormattedCodeWriter writer)
		{
			WriteAttributes(fieldDeclaration, writer, false);

			if (fieldDeclaration.Visibility == FieldVisibility.Public)
			{
				writer.WriteKeyword("public");
				writer.Write(' ');
			}
			else if (fieldDeclaration.Visibility == FieldVisibility.Private)
			{
				writer.WriteKeyword("private");
				writer.Write(' ');
			}
			else if (fieldDeclaration.Visibility == FieldVisibility.Assembly)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
			}
			else if (fieldDeclaration.Visibility == FieldVisibility.FamilyAndAssembly)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
				writer.WriteKeyword("protected");
				writer.Write(' ');
			}
			else if (fieldDeclaration.Visibility == FieldVisibility.FamilyOrAssembly)
			{
				writer.WriteKeyword("internal");
				writer.Write(' ');
				writer.WriteKeyword("protected");
				writer.Write(' ');
			}
			else if (fieldDeclaration.Visibility == FieldVisibility.Family)
			{
				writer.WriteKeyword("protected");
				writer.Write(' ');
			}

			if (fieldDeclaration.IsConst)
				writer.WriteKeyword("const ");
			else
			{
				if (fieldDeclaration.IsStatic)
					writer.WriteKeyword("static ");

				if (fieldDeclaration.IsReadOnly)
					writer.WriteKeyword("readonly ");
			}

			WriteTypeReference(fieldDeclaration.FieldType, writer);
			writer.Write(' ');
			writer.WriteIdentifier(fieldDeclaration.Name);

			if (fieldDeclaration.Initialiser != null)
			{
				writer.Write(" = ");
				WriteLiteralExpression(fieldDeclaration.Initialiser, writer);
			}

			writer.WriteLine(';');
		}

		public void WritePropertyDeclaration(IPropertyDeclaration propertyDeclaration, IFormattedCodeWriter writer)
		{
			WriteAttributes(propertyDeclaration, writer, false);
			if (propertyDeclaration.Attributes.Count > 0)
				writer.Write(' ');

			if (!propertyDeclaration.DeclaringType.Resolve().IsInterface)
			{
				IMethodReference methodReference = propertyDeclaration.GetMethod;
				if (methodReference == null)
					methodReference = propertyDeclaration.SetMethod;

				MethodVisibility methodVisibility = methodReference.Resolve().Visibility;

				if (methodVisibility == MethodVisibility.Public)
				{
					writer.WriteKeyword("public");
					writer.Write(' ');
				}
				if (methodVisibility == MethodVisibility.Private)
				{
					writer.WriteKeyword("private");
					writer.Write(' ');
				}
				if (methodVisibility == MethodVisibility.Assembly)
				{
					writer.WriteKeyword("internal");
					writer.Write(' ');
				}
			}


			WriteTypeReference(propertyDeclaration.PropertyType, writer);

			writer.Write(' ');
			writer.WriteIdentifier(propertyDeclaration.Name);
			writer.WriteLine(" {");
			writer.Indent++;
			if (propertyDeclaration.GetMethod != null)
			{
				writer.WriteKeyword("get");

				if (propertyDeclaration.GetMethod.Resolve().Body.Statements == null)
					writer.WriteLine(';');
				else
				{
					writer.WriteLine(" {");
					writer.Indent++;
					WriteStatementList(propertyDeclaration.GetMethod.Resolve().Body.Statements, writer);
					writer.Indent--;
					writer.WriteLine('}');
				}
			}
			if (propertyDeclaration.SetMethod != null)
			{
				writer.WriteKeyword("set");

				if (propertyDeclaration.SetMethod.Resolve().Body.Statements == null)
					writer.WriteLine(';');
				else
				{
					writer.WriteLine(" {");
					writer.Indent++;
					WriteStatementList(propertyDeclaration.SetMethod.Resolve().Body.Statements, writer);
					writer.Indent--;
					writer.WriteLine('}');
				}
			}
			writer.Indent--;
			writer.WriteLine('}');
		}

		public void WriteEventDeclaration(IEventDeclaration eventDeclaration, IFormattedCodeWriter writer)
		{
			WriteAttributes(eventDeclaration, writer, false);

			writer.WriteKeyword("event");
			writer.Write(' ');
			WriteTypeReference(eventDeclaration.EventType, writer);
			writer.Write(' ');
			writer.WriteIdentifier(eventDeclaration.Name);
			writer.WriteLine();
		}

		public void WriteAssembly(IAssemblyReference assembly, IFormattedCodeWriter writer)
		{
			StringBuilder line = new StringBuilder();
			line.Append("// Version: ");
			line.Append(assembly.Version.Major);
			line.Append(':');
			line.Append(assembly.Version.Minor);
			line.Append(':');
			line.Append(assembly.Version.Build);
			line.Append(':');
			line.Append(assembly.Version.Revision);
			writer.WriteComment(line.ToString());

			WriteAttributes(assembly.Resolve(), writer, false);
		}

		public void WriteModule(IModule module, IFormattedCodeWriter writer)
		{
			writer.WriteComment("// Meta Data Stream Version: " + (module.MDStreamVersion >> 16 & 0xffff) + "." + (module.MDStreamVersion & 0xffff));
			writer.WriteLine();
			writer.WriteComment("// Module Version ID: " + module.ModuleVersionId.ToString("B").ToUpperInvariant());
			writer.WriteLine();
			writer.WriteComment("// Image File Machine: " + module.ImageFileMachine);
			writer.WriteLine();
			writer.WriteComment("// Portable Executable Kind: " + module.PortableExecutableKinds);
			writer.WriteLine();
		}

		#endregion

		private static void WriteParameterDeclaration(IParameterDeclaration parameterDeclaration, IFormattedCodeWriter writer)
		{
			WriteAttributes(parameterDeclaration, writer, true);

			if (parameterDeclaration.Attributes.Count > 0)
				writer.Write(' ');

			if (parameterDeclaration.IsOut)
			{
				writer.WriteKeyword("out");
				writer.Write(' ');
			}

			if (parameterDeclaration.IsByRef)
			{
				writer.WriteKeyword("ref");
				writer.Write(' ');
			}

			WriteTypeReference(parameterDeclaration.ParameterType, writer);
			writer.Write(' ');
			writer.WriteIdentifier(parameterDeclaration.Name);
		}

		private static void WriteTypeReference(ITypeReference typeReference, IFormattedCodeWriter writer)
		{
			if ((typeReference.IsGeneric) && (typeReference.Name.LastIndexOf('`') > 0))
			{
				String name = typeReference.Name.Substring(0, typeReference.Name.LastIndexOf('`'));
				writer.WriteType(typeReference, name);

				writer.Write('<');
				if (typeReference.GenericArguments.Count > 0)
				{
					WriteTypeReference(typeReference.GenericArguments[0], writer);
					for (int i = 1; i < typeReference.GenericArguments.Count; i++)
					{
						writer.Write(", ");
						WriteTypeReference(typeReference.GenericArguments[1], writer);
					}
				}
				writer.Write('>');
			}
			else
			{
				writer.WriteType(typeReference, CSharpBuiltInTypeNameTable.Lookup(typeReference));
			}
		}

		private static void WriteStatementList(IEnumerable<IStatement> statements, IFormattedCodeWriter writer)
		{
			foreach (IStatement statement in statements)
				WriteStatement(statement, writer);
		}

		private static void WriteStatement(IStatement statement, IFormattedCodeWriter writer)
		{
			if (statement is IBlockStatement)
			{
				WriteBlockStatement((IBlockStatement)statement, writer);
				return;
			}

			if (statement is ICaseStatement)
			{
				WriteCaseStatement((ICaseStatement)statement, writer);
				return;
			}

			if (statement is IConditionStatement)
			{
				WriteConditionStatement((IConditionStatement)statement, writer);
				return;
			}

			if (statement is IExpressionStatement)
			{
				WriteExpressionStatement((IExpressionStatement)statement, writer);
				return;
			}

			if (statement is IMethodReturnStatement)
			{
				WriteMethodReturnStatement((IMethodReturnStatement)statement, writer);
				return;
			}

			if (statement is ISwitchStatement)
			{
				WriteSwitchStatement((ISwitchStatement)statement, writer);
				return;
			}

			if (statement is IThrowExceptionStatement)
			{
				WriteThrowExceptionStatement((IThrowExceptionStatement)statement, writer);
				return;
			}

			if (statement is IWhileStatement)
			{
				WriteWhileStatement((IWhileStatement)statement, writer);
				return;
			}

			if (statement is ITryCatchFinallyStatement)
			{
				WriteTryCatchFinallyStatement((ITryCatchFinallyStatement)statement, writer);
				return;
			}

			if (statement is IBreakStatement)
			{
				WriteBreakStatement((IBreakStatement)statement, writer);
				return;
			}

			if (statement is IDoStatement)
			{
				WriteDoStatement((IDoStatement)statement, writer);
				return;
			}

			throw new ApplicationException("Unknown statement type " + statement.GetType().Name);
		}

		private static void WriteDoStatement(IDoStatement doStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("do");
			writer.WriteLine(" {");
			writer.Indent++;
			WriteStatementList(doStatement.Body.Statements, writer);
			writer.Indent--;
			writer.Write("} ");
			writer.WriteKeyword("while");
			writer.Write(" (");
			WriteExpression(doStatement.Condition, writer);
			writer.WriteLine(')');
		}

		private static void WriteBreakStatement(IBreakStatement breakStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("break");
			writer.WriteLine(";");
		}

		private static void WriteTryCatchFinallyStatement(ITryCatchFinallyStatement tryCatchFinallyStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("try");
			writer.Write(' ');
			WriteBlockStatement(tryCatchFinallyStatement.Try, writer);

			foreach (ICatchClause catchClause in tryCatchFinallyStatement.CatchClauses)
			{
				writer.WriteKeyword("catch");

                if (catchClause.Variable != null)
                {
                    writer.Write(" (");
                    WriteVariableDeclaration(catchClause.Variable.Resolve(), writer);
                    writer.Write(") ");
                }
                else
                    writer.Write(' ');

				WriteBlockStatement(catchClause.Body, writer);
			}

			if ((tryCatchFinallyStatement.Finally != null) && (tryCatchFinallyStatement.Finally.Statements.Count > 0))
			{
				writer.WriteKeyword("finally");
				writer.Write(' ');
				WriteBlockStatement(tryCatchFinallyStatement.Finally, writer);
			}
		}

		private static void WriteExpressionStatement(IExpressionStatement expressionStatement, IFormattedCodeWriter writer)
		{
			WriteExpression(expressionStatement.Expression, writer);
			writer.WriteLine(';');
		}

		private static void WriteMethodReturnStatement(IMethodReturnStatement methodReturnStatement, IFormattedCodeWriter writer)
		{
			if (methodReturnStatement.Expression != null)
			{
				writer.WriteKeyword("return");
				writer.Write(' ');
				WriteExpression(methodReturnStatement.Expression, writer);
			}
			else
			{
				writer.WriteKeyword("return");
			}
			writer.WriteLine(';');
		}

		private static void WriteBlockStatement(IBlockStatement blockStatement, IFormattedCodeWriter writer)
		{
			writer.WriteLine("{");
			writer.Indent++;
			WriteStatementList(blockStatement.Statements, writer);
			writer.Indent--;
			writer.WriteLine('}');
		}

		private static void WriteCaseStatement(ICaseStatement caseStatement, IFormattedCodeWriter writer)
		{
			if (caseStatement is IDefaultCaseStatement)
			{
				writer.WriteKeyword("default");
			}
			else
			{
				writer.WriteKeyword("case");
				writer.Write(' ');
				WriteExpression(caseStatement.Label, writer);
			}

			writer.Write(": ");
			WriteStatement(caseStatement.Statement, writer);
		}

		private static void WriteConditionStatement(IConditionStatement conditionStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("if");
			writer.Write(" (");
			WriteExpression(conditionStatement.Expression, writer);
			writer.WriteLine(')');
			WriteBlockStatement(conditionStatement.Then, writer);

			if ((conditionStatement.Else != null) && (conditionStatement.Else.Statements.Count > 0))
			{
				writer.WriteKeyword("else");
				writer.WriteLine();
				WriteBlockStatement(conditionStatement.Else, writer);
			}
		}

		private static void WriteSwitchStatement(ISwitchStatement switchStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("switch");
			writer.Write(" (");
			WriteExpression(switchStatement.Condition, writer);
			writer.WriteLine(")");
			writer.WriteLine('{');
			writer.Indent++;
			foreach (ICaseStatement caseStatement in switchStatement.Cases)
				WriteCaseStatement(caseStatement, writer);
			writer.Indent--;
			writer.WriteLine('}');
		}

		private static void WriteThrowExceptionStatement(IThrowExceptionStatement throwExceptionStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("throw");
			writer.Write(' ');
			WriteExpression(throwExceptionStatement.Expression, writer);
			writer.WriteLine(';');
		}

		private static void WriteWhileStatement(IWhileStatement whileStatement, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("while");
			writer.Write(" (");
			WriteExpression(whileStatement.Condition, writer);
			writer.WriteLine(')');
			WriteBlockStatement(whileStatement.Body, writer);
		}

		private static void WriteExpression(IExpression expression, IFormattedCodeWriter writer)
		{
			if (expression is IAssignExpression)
			{
				WriteAssignExpression((IAssignExpression)expression, writer);
				return;
			}

			if (expression is IArgumentReferenceExpression)
			{
				WriteArgumentReferenceExpression((IArgumentReferenceExpression)expression, writer);
				return;
			}

			if (expression is IArrayCreateExpression)
			{
				WriteArrayCreateExpression((IArrayCreateExpression)expression, writer);
				return;
			}

			if (expression is IBinaryExpression)
			{
				WriteBinaryExpression((IBinaryExpression)expression, writer);
				return;
			}

			if (expression is IBlockExpression)
			{
				WriteBlockExpression((IBlockExpression)expression, writer);
				return;
			}

			if (expression is ICastExpression)
			{
				WriteCastExpression((ICastExpression)expression, writer);
				return;
			}

			if (expression is IConditionExpression)
			{
				WriteConditionExpression((IConditionExpression)expression, writer);
				return;
			}

			if (expression is IFieldReferenceExpression)
			{
				WriteFieldReferenceExpression((IFieldReferenceExpression)expression, writer);
				return;
			}

			if (expression is ILiteralExpression)
			{
				WriteLiteralExpression((ILiteralExpression)expression, writer);
				return;
			}

			if (expression is IMethodInvokeExpression)
			{
				WriteMethodInvokeExpression((IMethodInvokeExpression)expression, writer);
				return;
			}

			if (expression is IMethodReferenceExpression)
			{
				WriteMethodReferenceExpression((IMethodReferenceExpression)expression, writer);
				return;
			}

			if (expression is IThisReferenceExpression)
			{
				WriteThisReferenceExpression((IThisReferenceExpression)expression, writer);
				return;
			}

			if (expression is IBaseReferenceExpression)
			{
				WriteBaseReferenceExpression((IBaseReferenceExpression)expression, writer);
				return;
			}

			if (expression is ITypeOfExpression)
			{
				WriteTypeOfExpression((ITypeOfExpression)expression, writer);
				return;
			}

			if (expression is IUnaryExpression)
			{
				WriteUnaryExpression((IUnaryExpression)expression, writer);
				return;
			}

			if (expression is IVariableDeclarationExpression)
			{
				WriteVariableDeclarationExpression((IVariableDeclarationExpression)expression, writer);
				return;
			}

			if (expression is IVariableReferenceExpression)
			{
				WriteVariableReferenceExpression((IVariableReferenceExpression)expression, writer);
				return;
			}

			if (expression is ITypeReferenceExpression)
			{
				WriteTypeReferenceExpression((ITypeReferenceExpression)expression, writer);
				return;
			}


			if (expression is IObjectCreateExpression)
			{
				WriteObjectCreateExpression((IObjectCreateExpression)expression, writer);
				return;
			}

			if (expression is IAddressOfExpression)
			{
				WriteAddressOfExpression((IAddressOfExpression)expression, writer);
				return;
			}

			if (expression is IArrayIndexerExpression)
			{
				WriteArrayIndexerExpression((IArrayIndexerExpression)expression, writer);
				return;
			}

			if (expression is IValueOfTypedReferenceExpression)
			{
				WriteValueOfTypedReferenceExpression((IValueOfTypedReferenceExpression)expression, writer);
				return;
			}

			if (expression is IAddressDereferenceExpression)
			{
				WriteAddressDereferenceExpression((IAddressDereferenceExpression)expression, writer);
				return;
			}

			if (expression is IPropertyReferenceExpression)
			{
				WritePropertyReferenceExpression((IPropertyReferenceExpression)expression, writer);
				return;
			}

			if (expression is IStackAllocateExpression)
			{
				WriteStackAllocateExpression((IStackAllocateExpression)expression, writer);
				return;
			}

			throw new ApplicationException("Unexpected expression type " + expression.GetType());
		}
		
		private static void WriteStackAllocateExpression(IStackAllocateExpression stackAllocateExpression, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("stackalloc");
			writer.Write(' ');
			WriteExpression(stackAllocateExpression.Expression, writer);
		}

		private static void WritePropertyReferenceExpression(IPropertyReferenceExpression propertyReferenceExpression, IFormattedCodeWriter writer)
		{
			if (propertyReferenceExpression.Target != null)
			{
				WriteExpression(propertyReferenceExpression.Target, writer);
				writer.Write('.');
			}

			WritePropertyReference(propertyReferenceExpression.Property, writer);
		}

		private static void WriteAddressDereferenceExpression(IAddressDereferenceExpression addressDereferenceExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(addressDereferenceExpression.Expression, writer);
			writer.Write('*');
		}

		private static void WriteValueOfTypedReferenceExpression(IValueOfTypedReferenceExpression valueOfTypedReferenceExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(valueOfTypedReferenceExpression.Expression, writer);
			writer.Write(' ');
			writer.WriteKeyword("is");
			writer.Write(' ');
			WriteTypeReference(valueOfTypedReferenceExpression.TargetType, writer);
		}

		private static void WriteArrayIndexerExpression(IArrayIndexerExpression arrayIndexerExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(arrayIndexerExpression.Array, writer);

			writer.Write('[');

			if (arrayIndexerExpression.Indexers.Count > 0)
			{
				WriteExpression(arrayIndexerExpression.Indexers[0], writer);

				for (int i = 1; i < arrayIndexerExpression.Indexers.Count; i++)
				{
					writer.Write(", ");
					WriteExpression(arrayIndexerExpression.Indexers[1], writer);
				}
			}

			writer.Write(']');
		}

		private static void WriteAddressOfExpression(IAddressOfExpression addressOfExpression, IFormattedCodeWriter writer)
		{
			writer.Write("&");
			WriteExpression(addressOfExpression.Expression, writer);
		}

		private static void WriteArrayCreateExpression(IArrayCreateExpression arrayCreateExpression, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("new");
			writer.Write(' ');
			WriteTypeReference(arrayCreateExpression.TypeReference, writer);
			writer.Write('[');
			if (arrayCreateExpression.Dimensions.Count > 0)
			{
				WriteExpression(arrayCreateExpression.Dimensions[0], writer);

				for (int i = 1; i < arrayCreateExpression.Dimensions.Count; i++)
				{
					writer.Write(", ");
					WriteExpression(arrayCreateExpression.Dimensions[i], writer);
				}
			}
			writer.Write(']');

			if (arrayCreateExpression.Initializer != null)
				WriteBlockExpression(arrayCreateExpression.Initializer, writer);
		}

		private static void WriteObjectCreateExpression(IObjectCreateExpression objectCreateExpression, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("new");
			writer.Write(' ');
			WriteTypeReference(objectCreateExpression.TypeReference, writer);

			writer.Write('(');

			if (objectCreateExpression.Arguments.Count > 0)
				WriteExpression(objectCreateExpression.Arguments[0], writer);

			for (int i = 1; i < objectCreateExpression.Arguments.Count; i++)
			{
				writer.Write(", ");
				WriteExpression(objectCreateExpression.Arguments[i], writer);
			}

			writer.Write(')');
		}

		private static void WriteArgumentReferenceExpression(IArgumentReferenceExpression argumentReferenceExpression, IFormattedCodeWriter writer)
		{
			writer.Write(argumentReferenceExpression.Parameter.Name);
		}

		private static void WriteTypeReferenceExpression(ITypeReferenceExpression typeReferenceExpression, IFormattedCodeWriter writer)
		{
			WriteTypeReference(typeReferenceExpression.TypeReference, writer);
		}

		private static void WriteAssignExpression(IAssignExpression assignExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(assignExpression.Target, writer);
			writer.Write(" = ");
			WriteExpression(assignExpression.Expression, writer);
		}

		private static void WriteBinaryExpression(IBinaryExpression binaryExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(binaryExpression.Left, writer);

			switch (binaryExpression.Operator)
			{
				case BinaryOperator.Add:
					writer.Write(" + ");
					break;

				case BinaryOperator.BitwiseAnd:
					writer.Write(" & ");
					break;

				case BinaryOperator.BitwiseExclusiveOr:
					writer.Write(" ^ ");
					break;

				case BinaryOperator.BitwiseOr:
					writer.Write(" | ");
					break;

				case BinaryOperator.Decrement:
					writer.Write(" -= ");
					break;

				case BinaryOperator.Divide:
					writer.Write(" / ");
					break;

				case BinaryOperator.GreaterThan:
					writer.Write(" > ");
					break;

				case BinaryOperator.GreaterThanOrEqual:
					writer.Write(" >= ");
					break;

				case BinaryOperator.Increment:
					writer.Write(" += ");
					break;

				case BinaryOperator.LessThan:
					writer.Write(" < ");
					break;

				case BinaryOperator.LessThanOrEqual:
					writer.Write(" <= ");
					break;

				case BinaryOperator.Modulus:
					writer.Write(" % ");
					break;

				case BinaryOperator.Multiply:
					writer.Write(" * ");
					break;

				case BinaryOperator.ShiftLeft:
					writer.Write(" << ");
					break;

				case BinaryOperator.ShiftRight:
					writer.Write(" >> ");
					break;

				case BinaryOperator.Subtract:
					writer.Write(" - ");
					break;

				case BinaryOperator.ValueEquality:
					writer.Write(" == ");
					break;

				case BinaryOperator.ValueInequality:
					writer.Write(" != ");
					break;
			}

			WriteExpression(binaryExpression.Right, writer);
		}

		private static void WriteBlockExpression(IBlockExpression blockExpression, IFormattedCodeWriter writer)
		{
			writer.WriteLine('{');
			writer.Indent--;
			foreach (IExpression expression in blockExpression.Expressions)
				WriteExpression(expression, writer);

			writer.Indent--;
			writer.WriteLine('}');
		}

		private static void WriteCastExpression(ICastExpression castExpression, IFormattedCodeWriter writer)
		{
			writer.Write('(');
			WriteTypeReference(castExpression.TargetType, writer);
			writer.Write(')');
			WriteExpression(castExpression.Expression, writer);
		}

		private static void WriteConditionExpression(IConditionExpression conditionExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(conditionExpression.Condition, writer);
			writer.Write(" ? ");
			WriteExpression(conditionExpression.Then, writer);
			writer.Write(" : ");
			WriteExpression(conditionExpression.Else, writer);
		}

		private static void WriteFieldReferenceExpression(IFieldReferenceExpression fieldReferenceExpression, IFormattedCodeWriter writer)
		{
			if (fieldReferenceExpression.Target != null)
			{
				WriteExpression(fieldReferenceExpression.Target, writer);
				writer.Write('.');
			}

			WriteFieldReference(fieldReferenceExpression.Field, writer);
		}

		private static void WriteFieldReference(IFieldReference fieldReference, IFormattedCodeWriter writer)
		{
			writer.Write(fieldReference.Name);
		}

		private static void WriteLiteralExpression(ILiteralExpression literalExpression, IFormattedCodeWriter writer)
		{
			if (literalExpression.Value == null)
			{
				writer.WriteLiteral("null");
			}
			else if (literalExpression.Value is char)
			{
				writer.WriteLiteral("'" + literalExpression.Value + "'");
			}
			else if (literalExpression.Value is string)
			{
				writer.WriteLiteral("\"" + literalExpression.Value.ToString().Replace("\"", "\\\"") + "\"");
			}
			else if (literalExpression.Value is bool)
			{
				writer.WriteKeyword((bool)literalExpression.Value ? "true" : "false");
			}
			else
			{
				writer.WriteLiteral(literalExpression.Value.ToString());
			}
		}

		private static void WriteMethodInvokeExpression(IMethodInvokeExpression methodInvokeExpression, IFormattedCodeWriter writer)
		{
			IMethodReference methodReference = methodInvokeExpression.Method.Method;

			// Substitue method references with property references if the token is a property method call
			if (methodReference.Resolve().SpecialName)
			{
				foreach (IPropertyReference propertyReference in methodReference.Resolve().DeclaringType.Resolve().Properties)
				{
					if (propertyReference.Name == methodReference.Name.Substring(4))
					{
						WriteExpression(methodInvokeExpression.Method.Target, writer);
						writer.Write('.');
						WritePropertyReference(propertyReference, writer);
						return;
					}
				}
			}

			WriteMethodReferenceExpression(methodInvokeExpression.Method, writer);
			writer.Write('(');

			if (methodInvokeExpression.Arguments.Count > 0)
				WriteExpression(methodInvokeExpression.Arguments[0], writer);

			for (int i = 1; i < methodInvokeExpression.Arguments.Count; i++)
			{
				writer.Write(", ");
				WriteExpression(methodInvokeExpression.Arguments[i], writer);
			}

			writer.Write(')');
		}

		private static void WritePropertyReference(IPropertyReference propertyReference, IFormattedCodeWriter writer)
		{
			writer.Write(propertyReference.Name);
		}

		private static void WriteMethodReferenceExpression(IMethodReferenceExpression methodReferenceExpression, IFormattedCodeWriter writer)
		{
			WriteExpression(methodReferenceExpression.Target, writer);
			writer.Write('.');
			WriteMethodReference(methodReferenceExpression.Method, writer);
		}

		private static void WriteMethodReference(IMethodReference methodReference, IFormattedCodeWriter writer)
		{
			writer.WriteMethodReference(methodReference);
		}

		private static void WriteBaseReferenceExpression(IBaseReferenceExpression baseReferenceExpression, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("base");
		}

		private static void WriteThisReferenceExpression(IThisReferenceExpression thisReferenceExpression, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("this");
		}

		private static void WriteTypeOfExpression(ITypeOfExpression typeOfExpresson, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword("typeof");
			writer.Write('(');
			WriteTypeReference(typeOfExpresson.TypeReference, writer);
			writer.Write(')');
		}

		private static void WriteUnaryExpression(IUnaryExpression unaryExpression, IFormattedCodeWriter writer)
		{
			switch (unaryExpression.Operator)
			{
				case UnaryOperator.BitwiseNot:
					{
						writer.Write('!');
						WriteExpression(unaryExpression.Expression, writer);
						break;
					}

				case UnaryOperator.BooleanNot:
					{
						writer.Write('!');
						WriteExpression(unaryExpression.Expression, writer);
						break;
					}

				case UnaryOperator.PostDecrement:
					{
						WriteExpression(unaryExpression.Expression, writer);
						writer.Write("--");
						break;
					}

				case UnaryOperator.PostIncrement:
					{
						WriteExpression(unaryExpression.Expression, writer);
						writer.Write("++");
						break;
					}

				case UnaryOperator.PreDecrement:
					{
						writer.Write("--");
						WriteExpression(unaryExpression.Expression, writer);
						break;
					}

				case UnaryOperator.PreIncrement:
					{
						writer.Write("++");
						WriteExpression(unaryExpression.Expression, writer);
						break;
					}

				default:
					{
						throw new ApplicationException("Unexpected unary operator");
					}
			}

		}

		private static void WriteVariableDeclarationExpression(IVariableDeclarationExpression variableDeclarationExpression, IFormattedCodeWriter writer)
		{
			WriteVariableDeclaration(variableDeclarationExpression.Variable, writer);
		}

		private static void WriteVariableDeclaration(IVariableDeclaration variableDeclaration, IFormattedCodeWriter writer)
		{
			WriteTypeReference(variableDeclaration.TypeReference, writer);
			writer.Write(' ');
			writer.Write(variableDeclaration.Name);
		}

		private static void WriteVariableReferenceExpression(IVariableReferenceExpression variableReferenceExpression, IFormattedCodeWriter writer)
		{
			WriteVariableReference(variableReferenceExpression.VariableReference, writer);
		}

		private static void WriteVariableReference(IVariableReference variableReference, IFormattedCodeWriter writer)
		{
			writer.Write(variableReference.Name);
		}

	}
}