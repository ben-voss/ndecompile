using System;
using System.Collections.Generic;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile
{
	public class ILWriter : ILanguageWriter
	{
		public void WriteAssembly(IAssemblyReference assembly, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".assembly ");
			writer.WriteIdentifier(assembly.Name);
			writer.WriteLine();
			writer.WriteLine("{");
			writer.Indent++;
			writer.WriteKeyword(".ver ");
			writer.WriteLiteral(assembly.Version.Major.ToString());
			writer.Write(':');
			writer.WriteLiteral(assembly.Version.Minor.ToString());
			writer.Write(':');
			writer.WriteLiteral(assembly.Version.Build.ToString());
			writer.Write(':');
			writer.WriteLiteral(assembly.Version.Revision.ToString());
			writer.WriteLine();

			writer.WriteKeyword(".hash algorithm ");
			writer.WriteLiteral(String.Format("0x{0:x8}", assembly.HashAlgorithm));
			writer.WriteLine();

			if (assembly.PublicKey != null)
			{
				writer.WriteKeyword(".publickey ");
				writer.Write('(');

				writer.WriteLiteral(assembly.PublicKey[0].ToString("X2"));
				for (int i = 1; i < assembly.PublicKey.Length; i++)
				{
					writer.Write(' ');
					writer.WriteLiteral(assembly.PublicKey[i].ToString("X2"));
				}

				writer.WriteLine(')');
			}

			writer.Indent--;
			writer.WriteLine("}");
		}

		public void WriteModule(IModule module, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".module ");
			writer.WriteIdentifier(module.Name);
			writer.WriteLine();

			writer.WriteComment("// Meta Data Stream Version: " + (module.MDStreamVersion >> 16 & 0xffff) + "." + (module.MDStreamVersion & 0xffff));
			writer.WriteLine();
			writer.WriteComment("// Module Version ID: " + module.ModuleVersionId.ToString("B").ToUpperInvariant());
			writer.WriteLine();
			writer.WriteComment("// Image File Machine: " + module.ImageFileMachine);
			writer.WriteLine();
			writer.WriteComment("// Portable Executable Kind: " + module.PortableExecutableKinds);
			writer.WriteLine();

			if (module.Certificate != null)
				writer.Write(module.Certificate.GetRawCertDataString());
		}

		public void WriteTypeDeclaration(ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".class");
            writer.Write(' ');
            writer.WriteKeyword("explicit");
            writer.Write(' ');
            writer.WriteKeyword("ansi");
            writer.Write(' ');
            writer.WriteKeyword("sealed");
            writer.Write(' ');
            writer.WriteKeyword("nested");
            writer.Write(' ');
            writer.WriteKeyword("private");
            writer.Write(' ');
            writer.WriteIdentifier(typeDeclaration.Name);
            writer.WriteLine();

            if (typeDeclaration.BaseType != null)
            {
                writer.Indent++;
                writer.WriteKeyword("extends");
                writer.Write(' ');
                WriteTypeReference(typeDeclaration.BaseType, typeDeclaration.BaseType.Assembly, writer);
                writer.Indent--;
            }

            writer.WriteLine("{");
            writer.Indent++;

            foreach (ITypeDeclaration nestedTypeDeclaration in typeDeclaration.Types)
            {
                writer.WriteLine();
                WriteTypeDeclaration(nestedTypeDeclaration, writer);
            }

            foreach (IFieldDeclaration fieldDeclaration in typeDeclaration.Fields)
            {
                writer.WriteLine();
                WriteFieldDeclaration(fieldDeclaration, writer);
            }

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
            writer.WriteLine("}");
        }
		
		public void WriteEventDeclaration(IEventDeclaration eventDeclaration, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".event ");

			writer.WriteLine(eventDeclaration.Name);
			writer.WriteLine('{');
			writer.Indent++;
			if (eventDeclaration.AddMethod != null)
			{
				writer.Write(".addon ");

			}

			if (eventDeclaration.RemoveMethod != null)
			{
				writer.Write(".removeon ");
				
			}
			writer.Indent--;
			writer.Write('}');
		}

		public void WritePropertyDeclaration(IPropertyDeclaration propertyDeclaration, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".property ");
			writer.WriteLine(propertyDeclaration.Name);
			writer.WriteLine('{');
			writer.Indent++;

			if (propertyDeclaration.GetMethod != null)
			{
				writer.WriteKeyword(".get ");

				if (!propertyDeclaration.GetMethod.IsStatic)
					writer.Write("instance ");

				WriteTypeReference(propertyDeclaration.PropertyType, propertyDeclaration.PropertyType.Assembly, writer);
                writer.Write(' ');
                writer.WriteLine(propertyDeclaration.GetMethod.Name);
			}

            if (propertyDeclaration.SetMethod != null)
            {
                writer.WriteKeyword(".set ");

                if (!propertyDeclaration.SetMethod.IsStatic)
                    writer.Write("instance ");

                WriteTypeReference(propertyDeclaration.PropertyType, propertyDeclaration.PropertyType.Assembly, writer);
                writer.Write(' ');
                writer.WriteLine(propertyDeclaration.SetMethod.Name);
            }

			writer.Indent--;
			writer.WriteLine('}');
		}

		public void WriteFieldDeclaration(IFieldDeclaration fieldDeclaration, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".field ");

			if (fieldDeclaration.Visibility == FieldVisibility.Public)
				writer.WriteKeyword("public ");
			else if (fieldDeclaration.Visibility == FieldVisibility.Private)
				writer.WriteKeyword("private ");
			else if (fieldDeclaration.Visibility == FieldVisibility.Assembly)
				writer.WriteKeyword("assembly ");
			else if (fieldDeclaration.Visibility == FieldVisibility.Family)
				writer.WriteKeyword("family ");
			else if (fieldDeclaration.Visibility == FieldVisibility.FamilyAndAssembly)
				writer.WriteKeyword("family assembly ");
			else if (fieldDeclaration.Visibility == FieldVisibility.FamilyOrAssembly)
				writer.WriteKeyword("famorassem ");

			if (fieldDeclaration.IsStatic)
				writer.WriteKeyword("static ");

			if (fieldDeclaration.IsConst)
				writer.WriteKeyword("literal ");
			else if (fieldDeclaration.IsReadOnly)
				writer.WriteKeyword("initonly ");


			WriteTypeReference(fieldDeclaration.FieldType, fieldDeclaration.FieldType.Assembly, writer);
			writer.Write(' ');
			writer.WriteIdentifier(fieldDeclaration.Name);

			if (fieldDeclaration.IsConst)
			{
				writer.Write(" = ");
				WriteTypeReference(fieldDeclaration.FieldType, fieldDeclaration.FieldType.Assembly, writer);
				writer.Write('(');
				writer.Write(fieldDeclaration.Initialiser.Value.ToString());
				writer.Write(')');
			}

            writer.WriteLine();
		}

		public void WriteMethodDeclaration(IMethodDeclaration methodDeclaration, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".method");
            writer.Write(' ');

            if (methodDeclaration.Visibility == MethodVisibility.Public)
            {
                writer.WriteKeyword("public");
                writer.Write(' ');
            }
            else if (methodDeclaration.Visibility == MethodVisibility.Private)
            {
                writer.WriteKeyword("private");
                writer.Write(' ');
            }

            if (methodDeclaration.HideBySignature)
            {
                writer.WriteKeyword("hidebysig");
                writer.Write(' ');
            }

            if (methodDeclaration.SpecialName)
            {
                writer.WriteKeyword("specialname");
                writer.Write(' ');
            }

            if (methodDeclaration.NewSlot)
            {
                writer.WriteKeyword("newslot");
                writer.Write(' ');
            }

            if (methodDeclaration.Virtual)
            {
                writer.WriteKeyword("virtual");
                writer.Write(' ');
            }

            if (methodDeclaration.Final)
            {
                writer.WriteKeyword("final");
                writer.Write(' ');
            }

			if (methodDeclaration.IsStatic)
				writer.WriteKeyword("static");
			else
				writer.WriteKeyword("instance");
            writer.Write(' ');

            if (!(methodDeclaration is IConstructorDeclaration))
            {
                WriteTypeReference(methodDeclaration.ReturnType, methodDeclaration.DeclaringType.Assembly, writer);
                writer.Write(' ');
            }

			writer.WriteIdentifier(methodDeclaration.Name);
			writer.Write('(');

			if (methodDeclaration.Parameters.Count > 0)
			{
				WriteParameterDeclaration(methodDeclaration.Parameters[0], methodDeclaration.DeclaringType.Assembly, writer);

				for (int i = 1; i < methodDeclaration.Parameters.Count; i++)
				{
					writer.Write(", ");
					WriteParameterDeclaration(methodDeclaration.Parameters[i], methodDeclaration.DeclaringType.Assembly, writer);
				}
			}

			writer.Write(") ");
			writer.WriteKeyword("cli managed");

			if (!methodDeclaration.Abstract && methodDeclaration.Body.Instructions == null)
				writer.WriteKeyword(" internalcall");

			writer.WriteLine();
			writer.WriteLine("{");

			if (!methodDeclaration.Abstract && methodDeclaration.Body.Instructions != null)
			{
				writer.Indent++;
				WriteMethodBody(methodDeclaration, methodDeclaration.Body, methodDeclaration.DeclaringType.Assembly, methodDeclaration.Parameters, writer);
				writer.Indent--;
			}

			writer.WriteLine('}');
		}

		private static void WriteMethodBody(IMethodDeclaration methodDeclaration, IMethodBody methodBody, IAssembly assembly, IList<IParameterDeclaration> parameters, IFormattedCodeWriter writer)
		{
			writer.WriteKeyword(".maxstack ");
			writer.WriteLine(methodBody.MaxStack);

			if (methodBody.InitVariables)
			{
				writer.WriteKeyword(".locals init ");
				writer.WriteLine("(");
				writer.Indent++;
				if (methodBody.Variables.Count > 0)
					WriteVariableDeclaration(0, methodBody.Variables[0], assembly, writer);

				for (int i = 1; i < methodBody.Variables.Count; i++)
				{
					writer.WriteLine(",");
					WriteVariableDeclaration(i, methodBody.Variables[i], assembly, writer);
				}
				writer.Indent--;
				writer.WriteLine(")");
			}

			foreach (IInstruction instruction in methodBody.Instructions)
                WriteInstruction(instruction, assembly, methodBody.Variables, methodDeclaration.IsStatic, parameters, writer);
		}

		private static void WriteVariableDeclaration(int index, IVariableDeclaration variableDeclaration, IAssembly assembly, IFormattedCodeWriter writer)
		{
			writer.Write('[');
			writer.WriteLiteral(index.ToString());
			writer.Write("] ");
			WriteTypeReference(variableDeclaration.TypeReference, assembly, writer);
			writer.Write(' ');
			writer.WriteIdentifier(variableDeclaration.Name);
		}

		private static void WriteInstruction(IInstruction instruction, IAssembly assembly, IList<IVariableDeclaration> variables, bool isStatic, IList<IParameterDeclaration> parameters, IFormattedCodeWriter writer)
		{
			writer.Write("L_{0:x4}: ", instruction.IP);

			writer.Write(instruction.OpCode.Name);

			switch (instruction.OpCode.Value)
			{
				case 0x0e: // Ldarg_S
				case 0x0f: // Ldarga_S
				case 0x10: // Starg_S
					{
						writer.Write(' ');
						byte index = (byte)instruction.Argument;

                        if (!isStatic)
                            index--;

						writer.WriteIdentifier(parameters[index].Name);

						break;
					}

				case 0x11: // Ldloc_S
				case 0x12: // Ldloca_s
				case 0x13: // Stloc_S
					{
						writer.Write(' ');
						byte index = (byte)instruction.Argument;
						writer.WriteIdentifier(variables[index].Name);

						break;
					}


				case 0x28: // Call
				case 0x6F: // Callvirt 
				case 0x73: // Newobj
				{
					IMethodReference methodReference = (IMethodReference) instruction.Argument;

					if (methodReference.IsStatic)
						writer.Write(' ');
					else
						writer.Write(" instance ");

					WriteMethodReference(methodReference, assembly, writer);

					break;
				}

				case 0x2b: // Br_S
				case 0x2c: // Brfalse_S - branch if value is false, null, or zero
				case 0x2d: // Brtrue_S - branch if value is true, not null, or non-zero
				case 0x2e: // Beq_S
				case 0x2f: // Bge_S
				case 0x30: // Bgt_S
				case 0x31: // Ble_S
				case 0x32: // Blt_S
				case 0x33: // Bne_un_s
				case 0x34: // Bge_un_s
				case 0x35: // Bgt_un_s
				case 0x37: // Blt_un_s
				case 0x38: // Br
				case 0x39: // Brfalse - branch if value is false, null, or zero
				case 0x3a: // Brtrue - branch if value is true, not null, or non-zero
				case 0x3b: // Beq
				case 0x3d: // Bgt
				case 0x3e: // Ble
				case 0x3f: // Blt
				case 0x40: // Bne_Un
				{
					writer.Write(String.Format(" L_{0:x4}", instruction.Argument));
					break;
				}

				case 0x45: // Switch
				{
					writer.Write(" (");
					short[] caseLabels = (short[])instruction.Argument;
					if (caseLabels.Length > 0)
						writer.Write(String.Format("L_{0:x4}", caseLabels[0]));

					for (int i = 1; i < caseLabels.Length; i++)
						writer.Write(String.Format(", L_{0:x4}", caseLabels[i]));

					writer.Write(')');

					break;
				}

				case 0x72: // Ldstr
				{
					writer.WriteLiteral(" \"");
					writer.WriteLiteral((String)instruction.Argument);
					writer.WriteLiteral("\"");
					break;
				}


				case 0x75: // Isinst
				case 0x74: // Castclass
				case 0x8d: // Newarr
				{
					ITypeReference typeReference = (ITypeReference) instruction.Argument;
					writer.Write(' ');
					WriteTypeReference(typeReference, assembly, writer);
					break;
				}

				case 0x7b: // Ldfld:
				case 0x7c: // Ldflda:
				case 0x7d: // Stfld
				case 0x7e: // Ldsfld
				case 0x80: // Stsfld
				{
					// Get the field in the target object
					IFieldReference fieldReference = (IFieldReference) instruction.Argument;
					writer.Write(' ');
					WriteFieldReference(fieldReference, assembly, writer);
					break;
				}

				case 0xd0: // Ldtoken
				{
					writer.Write(' ');
					object token = instruction.Argument;
					if (token is IFieldReference)
						WriteFieldReference((IFieldReference)token, assembly, writer);
					else if (token is IMethodReference)
						WriteMethodReference((IMethodReference)token, assembly, writer);
					else if (token is ITypeReference)
						WriteTypeReference((ITypeReference)token, assembly, writer);
					else
						throw new ApplicationException("Unknown token type in ldtoken");
					break;
				}

				case 0x1f: // Lcd_I4_S
				case 0x20:
				{
					writer.Write(' ');
					writer.WriteLiteral(instruction.Argument.ToString());
					break;
				}

                case 0xa3: // Ldelem.any
                {
                    writer.Write(' ');
                    WriteTypeReference((ITypeReference)instruction.Argument, assembly, writer);
                    break;
                }

			}

			writer.WriteLine();
		}

		private static void WriteFieldReference(IFieldReference fieldReference, IAssembly assembly, IFormattedCodeWriter writer)
		{
			WriteTypeReference(fieldReference.FieldType, assembly, writer);

            if (fieldReference.Resolve().DeclaringType != null)
            {
                writer.Write(' ');
                WriteTypeReference(fieldReference.Resolve().DeclaringType, assembly, writer);
            }
			writer.Write("::");
			writer.Write(fieldReference.Name);
		}

		private static void WriteMethodReference(IMethodReference methodReference, IAssembly assembly, IFormattedCodeWriter writer)
		{
			if (!(methodReference is IConstructorReference))
			{
				WriteTypeReference(methodReference.ReturnType, assembly, writer);
				writer.Write(' ');
			}

			WriteTypeReference(methodReference.Resolve().DeclaringType, assembly, writer);
			writer.Write("::");
			writer.Write(methodReference.Name);
			writer.Write('(');
			if (methodReference.Parameters.Count > 0)
			{
				WriteTypeReference(methodReference.Parameters[0].ParameterType, assembly, writer);

				for (int i = 1; i < methodReference.Parameters.Count; i++)
				{
					writer.Write(", ");
					WriteTypeReference(methodReference.Parameters[i].ParameterType, assembly, writer);
				}
			}
			writer.Write(')');
		}

        private static void WriteTypeReference(ITypeReference type, IAssembly assembly, IFormattedCodeWriter writer)
        {
            String typeName;
            if (MsilBuiltInTypeNameTable.TryLookup(type, out typeName))
            {
                writer.Write(typeName);
                return;
            }

            if (type.IsGenericParameter)
            {
                writer.Write('!');
            }
            else
            {

                if (type.IsValueType)
                    writer.Write("valuetype ");
                else
                    writer.Write("class ");

                if (assembly != type.Assembly)
                {
                    writer.Write('[');
                    writer.Write(type.Assembly.Name);
                    writer.Write(']');
                }

                if (type.Namespace != null)
                {
                    writer.Write(type.Namespace);
                    writer.Write('.');
                }
            }

            writer.Write(type.Name);

            if (type.IsGeneric)
            {
                writer.Write('<');

                if (type.GenericArguments.Count > 0)
                {
                    WriteTypeReference(type.GenericArguments[0], assembly, writer);

                    for (int i = 1; i < type.GenericArguments.Count; i++)
                    {
                        writer.Write(", ");
                        WriteTypeReference(type.GenericArguments[i], assembly, writer);
                    }
                }

                writer.Write('>');
            }

        }

		private static void WriteParameterDeclaration(IParameterDeclaration parameterDeclaration, IAssembly assembly, IFormattedCodeWriter writer)
		{
			WriteTypeReference(parameterDeclaration.ParameterType, assembly, writer);
			writer.Write(' ');
			writer.Write(parameterDeclaration.Name);
		}

	}
}
