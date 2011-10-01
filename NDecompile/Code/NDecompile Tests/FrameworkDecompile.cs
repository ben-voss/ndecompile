using System;
using System.Collections.Generic;
using System.Text;
using LittleNet.NDecompile.Model;
using NUnit.Framework;

namespace LittleNet.NDecompile.Tests
{
    [TestFixture]
    public class FrameworkDecompile
    {

		[Test]
		public void TestStringSplit() {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(String).GetMethod("Split"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
			String s;
			
		}
		
        [Test]
        public void Test()
        {
            IAssembly assembly = AssemblyManager.Load(typeof(String).Assembly.GetName());
            foreach (IModule module in assembly.Modules)
            {
                foreach (ITypeDeclaration typeDeclaration in module.Types)
                {
                    foreach (IMethodDeclaration methodDeclaration in typeDeclaration.Methods)
                    {
						if ((methodDeclaration.Name != "Split") && (methodDeclaration.Name != "FindNotInTable") && (methodDeclaration.Name != "IndexOfAnyUnchecked")) {
						
	                        IList<IStatement> statements = methodDeclaration.Body.Statements;
						} else {
							
						}
                    }
                }
            }
        }
    }
}
