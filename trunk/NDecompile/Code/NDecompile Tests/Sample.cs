using System;
using System.Collections.Generic;
using System.Reflection;
using LittleNet.NDecompile.Model;
using NUnit.Framework;

namespace LittleNet.NDecompile.Tests
{
	[TestFixture]
	public class Sample
	{
		[Test]
		public void Test()
		{
			// Get method body information.
			MethodInfo mi = typeof(Sample).GetMethod("MethodBodyExample", BindingFlags.Instance | BindingFlags.NonPublic);
			IMethodReference methodReference = AssemblyManager.FindMethod(mi);
			IList<IStatement> statements = methodReference.Resolve().Body.Statements;
		}

		// The Main method contains code to analyze this method, using
		// the properties and methods of the MethodBody class.
		protected void MethodBodyExample(object arg)
		{
			// Define some local variables. In addition to these variables,
			// the local variable list includes the variables scoped to 
			// the catch clauses.
			int var1 = 42;
			try
			{
				// Depending on the input value, throw an ArgumentException or 
				// an ArgumentNullException to test the Catch clauses.
				if (arg == null)
				{
					throw new ArgumentNullException("arg", "The argument cannot be null.");
				}
			}

			// There is no Filter clause in this code example. See the Visual 
			// Basic code for an example of a Filter clause.

			// This catch clause handles the ArgumentException class, and
			// any other class derived from Exception.
			catch (Exception ex)
			{
				Console.WriteLine("Ordinary exception-handling clause caught: {0}",
					ex.GetType());
			}
			finally
			{
				var1 = 3033;
			}

			Console.WriteLine(var1);
		}

	}
}
