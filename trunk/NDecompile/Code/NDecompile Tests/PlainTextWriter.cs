using System;
using System.Text;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile.Tests
{
	internal class PlainTextWriter : IFormattedCodeWriter
	{
		private readonly StringBuilder _builder = new StringBuilder();
		private int _indent;
		private bool _writeIndent;

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

		public void Write(string text)
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
			_builder.AppendLine();
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
			Write(literal);
		}

		public void WriteKeyword(String keyword)
		{
			Write(keyword);
		}

		public void WriteComment(String comment)
		{
			Write(comment);
		}

		public override string ToString()
		{
			return _builder.ToString();
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
}