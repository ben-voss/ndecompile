using System;
using System.Text;
using LittleNet.NDecompile.Model;
using System.Drawing;

namespace LittleNet.NDecompile.FormsUI
{
	internal class CSharpHtmlFormattedCodeWriter : HtmlFormattedCodeWriter, IFormattedCodeWriter
	{

		public void WriteIdentifier(String identifier)
		{
			WriteIndent();
			Append("<font color=\"darkgreen\">");
			Write(identifier);
			Append("</font>");
		}

		public void WriteType(ITypeReference type, String name)
		{
			WriteIndent();
			Append("<font color=\"darkgreen\">");
			Append("<a href=\"resource://[");
			Append(type.Assembly.Name);
			Append(']');
			Append(type.Namespace);
			Append('.');
			Append(type.Name);
			Append("\">");
			Write(name);
			Append("</a>");
			Append("</font>");
		}

		public void WriteMethodReference(IMethodReference methodReference)
		{
			WriteIndent();
			Append("<font color=\"darkgreen\">");
			Append("<a href=\"resource://[");
			Append(methodReference.Resolve().DeclaringType.Assembly.Name);
			Append(']');
			Append(methodReference.Resolve().DeclaringType.Namespace);
			Append('.');
			Append(methodReference.Resolve().DeclaringType.Name);
			Append("::");
			Append(methodReference.Name);
			Append("\">");

			if ((methodReference.Name == ".ctor") || (methodReference.Name == ".cctor"))
				Write(methodReference.Resolve().DeclaringType.Name);
			else
				Write(methodReference.Name);

			Append("</a>");
			Append("</font>");
		}

		public void WriteLiteral(String literal)
		{
			WriteIndent();
			if (literal[0] == '"')
			{
				Append("<font color=\"darkred\">");
				Write(literal);
				Append("</font>");
			}
			else
			{
				Append("<font color=\"darkred\">");
				Write(literal);
				Append("</font>");
			}
		}

		public void WriteKeyword(String keyword)
		{
			WriteIndent();
			Append("<font color=\"darkblue\">");
			Write(keyword);
			Append("</font>");
		}

		public void WriteComment(String comment)
		{
			WriteIndent();
			Append("<font color=\"darkgreen\">");
			Write(comment);
			Append("</font>");
		}

        public void BeginOutlineBlock()
        {
            //Append("<A href=\"#\" onclick=\"return false\" onfocus=\"h()\" class=\"b\">-</A>");
            //Append("<span class=\"outlineExpanded\">");
        }

        public void EndOutlineBlock()
        {
            //Append("</span>");
        }

	}
}
