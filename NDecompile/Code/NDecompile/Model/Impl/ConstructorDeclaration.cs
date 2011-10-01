
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ConstructorDeclaration : MethodDeclarationBase, IConstructorDeclaration
	{
		public ConstructorDeclaration(ConstructorInfo constructorInfo, Module module, ITypeReference declaringType)
			: base(constructorInfo, module, declaringType)
		{
		}

		public IMethodDeclaration Resolve()
		{
			return this;
		}

		public override ITypeReference ReturnType
		{
			get
			{
				return null;
			}
		}

		public override bool Overrides
		{
			get {
				return false;
			}
		}
	}
}
