using System;
using NUnit.Framework;
using LittleNet.NDecompile.Model.Impl;
using LittleNet.NDecompile.Model;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Tests
{
    [TestFixture]
	public class FlowControl
	{
        [Test]
        public void WhileTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("While"));            
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void While(int i)
		{
			Console.WriteLine("1");

			while (i > 0)
			{
				Console.WriteLine("2");
				i--;
			}

			Console.WriteLine("3");
		}

        [Test]
        public void NestedWhileTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("NestedWhile"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void NestedWhile(int i)
		{
			Console.WriteLine("1");

			while (i > 0)
			{
				Console.WriteLine("2");
				i--;

				int j = 0;
				while (j < i)
				{
					Console.WriteLine("3");
					j++;
				}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void DoTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("Do"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void Do(int i)
		{
			Console.WriteLine("1");


			do
			{
				Console.WriteLine("2");
				i--;
			} while (i > 0);

			Console.WriteLine("3");
		}

        [Test]
        public void ForTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("For"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
       }

		public void For()
		{
			Console.WriteLine("1");

			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine("2");
			}

			Console.WriteLine("3");
		}

        [Test]
        public void ForWithNestedIfTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("ForWithNestedIf"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void ForWithNestedIf()
		{
			Console.WriteLine("1");

			for (int i = 0; i < 10; i++)
			{
				if (i == 2)
					Console.WriteLine("2");
			}

			Console.WriteLine("3");
		}

        [Test]
        public void CaseGapTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseGap"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void CaseGap(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						break;
					}

				case 3:
					{
						Console.WriteLine("3");
						break;
					}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void CaseDefaultTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseDefault"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void CaseDefault(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						break;
					}
				case 2:
					{
						Console.WriteLine("3");
						break;
					}

				default:
					{
						Console.WriteLine("4");
						break;
					}
			}

			Console.WriteLine("5");
		}

        [Test]
        public void NestedCaseTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("NestedCase"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void NestedCase(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");

						switch (i)
						{
							case 1:
								{
									Console.WriteLine("3");
									break;
								}
							case 2:
								{
									Console.WriteLine("4");
									break;
								}
						}

						break;
					}
				case 2:
					{
						Console.WriteLine("5");
						break;
					}
			}

			Console.WriteLine("6");
		}

        [Test]
        public void CaseTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("Case"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void Case(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						break;
					}
				case 2:
					{
						Console.WriteLine("3");
						break;
					}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void CaseEmptyDefaultTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseEmptyDefault"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void CaseEmptyDefault(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						break;
					}
				case 2:
					{
						Console.WriteLine("3");
						break;
					}
				default:
					{
						return;
					}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void CaseThrowTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseThrow"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void CaseThrow(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						throw new ApplicationException();
					}
				case 2:
					{
						Console.WriteLine("3");
						break;
					}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void CaseNoExitTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseNoExit"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }
        
        public void CaseNoExit(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
					{
						Console.WriteLine("2");
						return;
					}
				case 2:
					{
						Console.WriteLine("3");
						return;
					}
			}

			Console.WriteLine("4");
		}

        [Test]
        public void CaseNoExit2Test()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("CaseNoExit2"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void CaseNoExit2(int i)
		{
			Console.WriteLine("1");

			switch (i)
			{
				case 1:
				{
					Console.WriteLine("2");
					break;
				}
				case 2:
				{
					Console.WriteLine("3");
					break;
				}

				default:
				{
					Console.WriteLine("4");
					break;
				}
			}

			return;
		}

        [Test]
        public void OneWayIfTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("OneWayIf"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
       }

		public void OneWayIf(bool arg)
		{
			Console.WriteLine("1");

			if (arg)
				Console.WriteLine("2");

			Console.WriteLine("3");
		}

        [Test]
        public void OneWayIf2Test()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("OneWayIf2"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

        public String OneWayIf2(bool arg)
        {
            Console.WriteLine("1");

            if (arg)
                Console.WriteLine("2");

            return "3";
        }

        [Test]
        public void NestedOneWayIfTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("NestedOneWayIf"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void NestedOneWayIf(bool arg)
		{
			Console.WriteLine("1");

			if (arg)
			{
				Console.WriteLine("2");

				if (!arg)
					Console.WriteLine("3");

			}

			Console.WriteLine("4");
		}

        [Test]
        public void TwoWayIfTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("TwoWayIf"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void TwoWayIf(bool arg)
		{
			Console.WriteLine("1");

			if (arg)
				Console.WriteLine("2");
			else
				Console.WriteLine("3");

			Console.WriteLine("4");
		}

        [Test]
        public void NestedTwoWayIfTest()
        {
            IMethodReference methodReference = AssemblyManager.FindMethod(typeof(FlowControl).GetMethod("NestedTwoWayIf"));
            IList<IStatement> statements = methodReference.Resolve().Body.Statements;
        }

		public void NestedTwoWayIf(bool arg)
		{
			Console.WriteLine("1");

			if (arg)
			{
				Console.WriteLine("2");

				if (!arg)
					Console.WriteLine("3");
				else
					Console.WriteLine("4");
			}
			else
			{
				Console.WriteLine("5");

				if (!arg)
					Console.WriteLine("6");
				else
					Console.WriteLine("7");

			}

			Console.WriteLine("4");
		}
	}
}