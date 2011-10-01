using System;
using System.Text;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile.FormsUI
{
	internal class ILHtmlFormattedCodeWriter : HtmlFormattedCodeWriter, IFormattedCodeWriter
	{
		public void WriteIdentifier(String identifier)
		{
			Append("<b>");
			Write(identifier);
		    Append("</b>");
		}

		public void WriteMethodReference(IMethodReference methodReference)
		{
			Write(methodReference.Name);
		}

		public void WriteKeyword(String keyword)
		{
			Append("<font color=\"Blue\">");
			Write(keyword);
			Append("</font>");
		}

		public void WriteComment(String comment)
		{
			Write(comment);
		}

		public void WriteLiteral(String literal)
		{
			Write(literal);
		}

		public void WriteType(ITypeReference type, String name)
		{
			Write(type.Name);
		}

        public void BeginOutlineBlock()
        {
        }

        public void EndOutlineBlock()
        {
        }

	}
}
