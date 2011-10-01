using System;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class VariableDeclaration : IVariableDeclaration
	{
		private readonly String _name;
		private readonly ITypeReference _typeReference;

		public VariableDeclaration(String name, ITypeReference typeReference)
		{
			_name = name;
			_typeReference = typeReference;
		}

		public IVariableDeclaration Resolve()
		{
			return this;
		}

		public String Name
		{
			get
			{
				return _name;
			}
		}

		public ITypeReference TypeReference
		{
			get
			{
				return _typeReference;
			}
		}
	}
}
