using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LittleNet.NDecompile.FormsUI
{
    internal abstract class HtmlFormattedCodeWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();
		private int _indent;
		private bool _writeIndent;

        public HtmlFormattedCodeWriter()
		{
			_builder.AppendLine("<html>");
            _builder.AppendLine("<body style=\"overflow: auto; background: #" + (SystemColors.Info.ToArgb() & 0xffffff).ToString("x6") + "; font-family: Tahoma; font-size :8.25pt\">");
            _builder.AppendLine("<style type=\"text/css\">");
            _builder.AppendLine(".b {position:relative;left:-8px;font-weight:bold;text-decoration:none;border:1px}");
            _builder.AppendLine("SPAN.outlineExpanded {}");
            _builder.AppendLine("</style>");

            _builder.AppendLine("<script>");
            /*function f(e){
                if (e.className=="ci"){
                    if (e.children(0).innerText.indexOf("\n")>0)
                        fix(e,"cb");
                }
                if (e.className=="di"){
                    if (e.children(0).innerText.indexOf("\n")>0)
                        fix(e,"db");
                }
                e.id="";
            }
            
            function fix(e,cl){
                e.className=cl;
                e.style.display="block";
                j=e.parentElement.children(0);
                j.className="c";
                k=j.children(0);
                k.style.visibility="visible";
                k.href="#";
            }
            
            function ch(e){
                mark=e.children(0).children(0);
                if (mark.innerText=="+"){
                    mark.innerText="-"; 
                    for (var i=1;i<e.children.length;i++)
                        e.children(i).style.display="block";
                } else if (mark.innerText=="-"){
                    mark.innerText="+"; 
                    for (var i=1;i<e.children.length;i++)
                        e.children(i).style.display="none"; 
                }
            }
            
            function ch2(e){
                mark=e.children(0).children(0); 
                contents=e.children(1);
                if (mark.innerText=="+"){
                    mark.innerText="-";
                    if (contents.className=="db"||contents.className=="cb") 
                        contents.style.display="block";
                    else contents.style.display="inline"; 
                } else if (mark.innerText=="-"){
                    mark.innerText="+";
                    contents.style.display="none";
                }
            }
            
            function cl(){
                e=window.event.srcElement; 
                if (e.className!="c"){
                    e=e.parentElement;
                    if (e.className!="c"){
                        return;
                    }
                }
    
                e=e.parentElement;
                if (e.className=="e") 
                    ch(e);
                if (e.className=="k")
                    ch2(e);
            }

            function ex() {
            }*/

            _builder.Append("function h(){window.status=\" \";}");
            /*
            document.onclick=cl;*/

            _builder.AppendLine("</script>");

		}

		public String Html
		{
			get
			{
				return _builder + "</font></body></html>";
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

        protected void Append(String text)
        {
            _builder.Append(text);
        }

        protected void Append(Char c)
        {
            _builder.Append(c);
        }

		public void Write(String text)
		{
			WriteIndent();

			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '<')
					_builder.Append("&lt;");
				else if (text[i] == '>')
					_builder.Append("&gt;");
				else
					_builder.Append(text[i]);
			}
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

		protected void WriteIndent()
		{
			if (!_writeIndent)
				return;

			for (int i = 0; i < _indent; i++)
				_builder.Append("&nbsp;&nbsp;&nbsp;&nbsp;");

			_builder.AppendLine();

			_writeIndent = false;
		}

		public void WriteLine()
		{
			_builder.Append("<br>");
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
    }
}
