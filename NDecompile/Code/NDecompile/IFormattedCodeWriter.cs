using System;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile
{
	public interface IFormattedCodeWriter
	{
		int Indent
		{
			get;
			set;
		}

		void Write(String text);

		void Write(char c);

		void Write(int i);

		void Write(String format, params object[] args);

		void WriteLine();

		void WriteLine(String text);

		void WriteLine(char c);

		void WriteLine(int i);

		void WriteLine(String format, params object[] args);

		void WriteIdentifier(String identifier);

		void WriteKeyword(String keyword);

		void WriteComment(String comment);

		void WriteLiteral(String literal);

		void WriteType(ITypeReference type, String name);

		void WriteMethodReference (IMethodReference methodReference);

        void BeginOutlineBlock();

        void EndOutlineBlock();
	}
}
