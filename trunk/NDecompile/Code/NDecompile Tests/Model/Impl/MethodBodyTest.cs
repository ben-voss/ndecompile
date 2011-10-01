using System;
using System.Collections.Generic;
using System.Reflection;
using LittleNet.NDecompile.Model;
using NUnit.Framework;
using MethodBody=LittleNet.NDecompile.Model.Impl.MethodBody;
using Module=LittleNet.NDecompile.Model.Impl.Module;

namespace LittleNet.NDecompile.Tests.Model.Impl
{
	[TestFixture]
	public class MethodBodyTest
	{
		private static String DecompileMethod(String methodName)
		{
			MethodInfo netMethodInfo = typeof(MethodBodyTest).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
			if (netMethodInfo == null)
				netMethodInfo = typeof(MethodBodyTest).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

			IMethodReference methodReferece = AssemblyManager.FindMethod(netMethodInfo);

			CSharpWriter languageWriter = new CSharpWriter();
			PlainTextWriter formattedCodeWriter = new PlainTextWriter();
			languageWriter.WriteMethodDeclaration(methodReferece.Resolve(), formattedCodeWriter);
			return formattedCodeWriter.ToString();
		}

		private static bool IfTestImpl(bool choice)
		{
			if (choice)
			{
				return true;
			}

			return false;
		}

		[Test]
		public void IfTest()
		{
			MethodBody body = new MethodBody(typeof(MethodBodyTest).GetMethod("IfTestImpl", BindingFlags.NonPublic | BindingFlags.Static), new Module(typeof(MethodBodyTest).Module, null), null);

			body.Decompile();
		}

		private MethodBodyTest TestLdArg_0_Test()
		{
			return this;
		}

		[Test]
		public void TestLdArg_0()
		{
			String method = DecompileMethod("TestLdArg_0_Test");

			string expected;
			
			if (Environment.NewLine == "\r\n")
				expected = "private MethodBodyTest TestLdArg_0_Test()\r\n\r\n{\r\n\t\r\nreturn this;\r\n}\r\n";
			else
				expected = "private MethodBodyTest TestLdArg_0_Test()\n\n{\n\t\nreturn this;\n\n}\n";

			Assert.AreEqual(expected, method);
		}

		[Test]
		public void TestNewarr()
		{
			String method = DecompileMethod("Newarr_Test");

			//const string expected = "private MethodBodyTest TestLdArg_0_Test()\r\n{\r\n\treturn this;\r\n}";

			//Assert.AreEqual(expected, method);
		}

		private String[] Newarr_Test()
		{
			String[] testArray = new string[2];
			testArray[0] = GetType().Name;
			testArray[1] = GetType().Namespace;

			return testArray;
		}

		[Test]
		public void TestLdtoken()
		{
			String method = DecompileMethod("Ldtoken_Test");
		}

		private Type Ldtoken_Test()
		{
			return typeof (String);
		}

		[Test]
		public void TestException()
		{
			DecompileMethod("ExceptionTest");
		}

		private Double ExceptionTest(Double x)
		{
			try
			{
				return 1 / x;
			}
			catch (DivideByZeroException e)
			{
				Console.WriteLine(e);
			}
			return Double.NaN;
		}

		private Double AnonymousExceptionTest(Double x)
		{
			try
			{
				return 1 / x;
			}
			catch (DivideByZeroException)
			{
				return Double.NaN;
			}
		}

		private void GenericList()
		{
			Dictionary<String, int> d = new Dictionary<string,int>();
			d.Add("Hello", 42);
		}

        private int PropertyGet()
        {
            return TestProperty;
        }

        private void PropertySet()
        {
            TestProperty = 42;
        }

        private int _value;

        private int TestProperty
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        private void BuildInTypes()
        {
            bool _bool;
            char _char;
            byte _byte;
            sbyte _sbyte;
            short _short;
            ushort _ushort;
            int _int;
            uint _uint;
            long _long;
            ulong _ulong;
            string _string;
            double _double;
            float _float;
            decimal _decimal;
        }


        public IntPtr TestIntPtr()
        {
            return IntPtr.Zero;
        }
	}
}