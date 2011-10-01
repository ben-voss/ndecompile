using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class MethodDeclaration : MethodDeclarationBase, IMethodDeclaration
	{
		private readonly MethodInfo _netMethodInfo;
		private ITypeReference _returnType;

		public MethodDeclaration(MethodInfo methodInfo, Module module, ITypeReference declaringType)
			: base(methodInfo, module, declaringType)
		{
			_netMethodInfo = methodInfo;
		}

		public IMethodDeclaration Resolve()
		{
			return this;
		}

		public override ITypeReference ReturnType
		{
			get
			{
				if (_returnType == null)
					_returnType = AssemblyManager.FindType(_netMethodInfo.ReturnType, _netMethodInfo.ReturnType.GetGenericArguments());

				return _returnType;
			}
		}

		public override bool Overrides
		{
			get
			{
				return _netMethodInfo.GetBaseDefinition() != null;
			}
		}
	}
}
