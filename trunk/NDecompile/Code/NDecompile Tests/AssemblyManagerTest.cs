using System;
using LittleNet.NDecompile.Model;
using NUnit.Framework;

namespace LittleNet.NDecompile.Tests
{
	[TestFixture]
	public class AssemblyManagerTest
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			IAssembly assembly = AssemblyManager.Load("mscorlib");
			Assert.IsNotNull(assembly);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ParseMemberReferenceNullArg()
		{
			AssemblyManager.ParseMemberReference(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseMemberReferenceZeroLengthArg()
		{
			AssemblyManager.ParseMemberReference(String.Empty);
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void ParseMemberReferenceBadFirstChar()
		{
			AssemblyManager.ParseMemberReference("foo");
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void ParseMemberReferenceBadAssemblyNameChar()
		{
			AssemblyManager.ParseMemberReference("[foo");
		}

		[Test]
		[ExpectedException(typeof(ApplicationException))]
		public void ParseMemberReferenceBadAssemblyName()
		{
			AssemblyManager.ParseMemberReference("[]");
		}

		[Test]
		public void ParseMemberReferenceFindAssemblyName()
		{
			IMemberReference memberReference =  AssemblyManager.ParseMemberReference("[mscorlib]");
			Assert.IsNotNull(memberReference);

			Assert.IsInstanceOf(typeof(IAssemblyReference), memberReference);
			IAssemblyReference assemblyReference = (IAssemblyReference) memberReference;
			Assert.AreEqual("mscorlib", assemblyReference.Name);
			//Assert.AreEqual(assembly, assemblyReference);
		}

		[Test]
		public void ParseMemberReferenceFindTypeName()
		{
			IMemberReference memberReference = AssemblyManager.ParseMemberReference("[mscorlib]System.Text.StringBuilder");
			Assert.IsNotNull(memberReference);
			
			Assert.IsInstanceOf(typeof(ITypeReference), memberReference);
			ITypeReference typeReference = (ITypeReference)memberReference;
			Assert.AreEqual("System.Text", typeReference.Namespace);
			Assert.AreEqual("StringBuilder", typeReference.Name);
		}
	}
}
