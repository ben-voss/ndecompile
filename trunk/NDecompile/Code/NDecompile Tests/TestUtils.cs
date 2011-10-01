using System;
using System.Reflection;
using System.Text;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;

namespace LittleNet.NDecompile.Tests
{
	internal static class TestUtils
	{
		private class TestWriter : IFormattedCodeWriter
		{
			private readonly StringBuilder _builder = new StringBuilder();
			private int _indent;
			private bool _writeIndent;

			public String String
			{
				get
				{
					return _builder.ToString();
				}
			}

			public int Indent
			{
				get
				{
					return _indent;
				}
				set
				{
					_indent = value;
				}
			}

			public void Write(String text)
			{
				WriteIndent();

				_builder.Append(text);
			}

			public void Write(char c)
			{
				Write(c.ToString());
			}

			public void Write(int i)
			{
				Write(i.ToString());
			}

			public void Write(String format, params object[] args)
			{
				Write(String.Format(format, args));
			}

			private void WriteIndent()
			{
				if (!_writeIndent)
					return;

				for (int i = 0; i < _indent; i++)
					_builder.Append("\t");

				_builder.AppendLine();

				_writeIndent = false;
			}

			public void WriteLine()
			{
				_builder.Append("\r\n");
				_writeIndent = true;
			}

			public void WriteLine(String text)
			{
				Write(text);
				WriteLine();
			}

			public void WriteLine(char text)
			{
				WriteLine(text.ToString());
			}

			public void WriteLine(int i)
			{
				WriteLine(i.ToString());
			}

			public void WriteLine(String format, params object[] args)
			{
				WriteLine(String.Format(format, args));
			}

			public void WriteIdentifier(String identifier)
			{
				Write(identifier);
			}

			public void WriteType(ITypeReference type, String name)
			{
				Write(type.Name);
			}

			public void WriteLiteral(String literal)
			{
				if (literal[0] == '"')
				{
					Write(literal);
				}
				else
				{
					Write(literal);
				}
			}

			public void WriteKeyword(String keyword)
			{
				Write(keyword);
			}

			public void WriteComment(String comment)
			{
				Write(comment);
			}

			public void WriteMethodReference(IMethodReference methodReference)
			{
				Write(methodReference.Name);
			}

			public void BeginOutlineBlock()
			{
			}

			public void EndOutlineBlock()
			{
			}
		}

		public static String WriteExpression(Expression expression)
		{
			TestWriter writer = new TestWriter();
			CSharpWriter cSharpWriter = new CSharpWriter();
			cSharpWriter.GetType().GetMethod("WriteExpression", BindingFlags.NonPublic | BindingFlags.Static).Invoke(cSharpWriter,
																												new object[]
			                                                                                                    	{
			                                                                                                    		expression,
			                                                                                                    		writer
			                                                                                                    	});
			return writer.String;
		}

		public static String WriteStatement(IStatement statement)
		{
			TestWriter writer = new TestWriter();
			CSharpWriter cSharpWriter = new CSharpWriter();
			cSharpWriter.GetType().GetMethod("WriteStatement", BindingFlags.NonPublic | BindingFlags.Static).Invoke(cSharpWriter,
																												new object[]
			                                                                                                    	{
			                                                                                                    		statement,
			                                                                                                    		writer
			                                                                                                    	});
			return writer.String;
		}

		public static bool IsDebug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}
	}
}
