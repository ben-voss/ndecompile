using System;
using System.Collections.Generic;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal abstract class MethodDeclarationBase
	{
		private readonly Module _module;
		private readonly MethodBase _netMethodInfo;
		private readonly ITypeReference _declaringType;
		private List<IParameterDeclaration> _parameterDeclaration;
		private IMethodBody _methodBody;
		private IList<IAttribute> _attributes;

		protected MethodDeclarationBase(MethodBase netMethodInfo, Module module, ITypeReference declaringType)
		{
			_module = module;
			_netMethodInfo = netMethodInfo;
			_declaringType = declaringType;
		}

		public IList<IAttribute> Attributes
		{
			get
			{
				if (_attributes == null)
				{
					_attributes = new List<IAttribute>();

					foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netMethodInfo))
						_attributes.Add(new Attribute(netAttribute));
				}

				return _attributes;
			}
		}

		public abstract ITypeReference ReturnType
		{
			get;
		}

		public abstract bool Overrides
		{
			get;
		}

		public List<IParameterDeclaration> Parameters
		{
			get
			{
				if (_parameterDeclaration == null)
				{
					_parameterDeclaration = new List<IParameterDeclaration>();
					foreach (ParameterInfo parameterInfo in _netMethodInfo.GetParameters())
						_parameterDeclaration.Add(new ParameterDeclaration(parameterInfo));
				}

				return _parameterDeclaration;
			}
		}

		public IMethodBody Body
		{
			get
			{
				if (_methodBody == null)
					_methodBody = new MethodBody(_netMethodInfo, _module, this);

				return _methodBody;
			}
		}

		public String Name
		{
			get
			{
				return _netMethodInfo.Name;
			}
		}

		public MethodVisibility Visibility
		{
			get
			{
				if (_netMethodInfo.IsPrivate)
					return MethodVisibility.Private;
				if (_netMethodInfo.IsPublic)
					return MethodVisibility.Public;
				if (_netMethodInfo.IsFamilyAndAssembly)
					return MethodVisibility.FamilyAndAssembly;
				if (_netMethodInfo.IsFamilyOrAssembly)
					return MethodVisibility.FamilyOrAssembly;
				if (_netMethodInfo.IsFamily)
					return MethodVisibility.Family;
				if (_netMethodInfo.IsAssembly)
					return MethodVisibility.Assembly;

				return 0;
			}
		}

		public bool IsStatic
		{
			get
			{
				return _netMethodInfo.IsStatic;
			}
		}

		public bool HideBySignature
		{
			get
			{
				return _netMethodInfo.IsHideBySig;
			}
		}

		public bool Final
		{
			get
			{
				return _netMethodInfo.IsFinal;
			}
		}

		public bool NewSlot
		{
			get
			{
				return (_netMethodInfo.Attributes & MethodAttributes.NewSlot) != 0;
			}
		}

		public bool Virtual
		{
			get
			{
				return _netMethodInfo.IsVirtual;
			}
		}

		public bool Abstract
		{
			get
			{
				return _netMethodInfo.IsAbstract;
			}
		}

		public bool SpecialName
		{
			get
			{
				return _netMethodInfo.IsSpecialName;
			}
		}

		public bool RuntimeSpecialName
		{
			get
			{
				return false;
			}
		}

		public ITypeReference DeclaringType
		{
			get
			{
				return _declaringType;
			}
		}

		public override string ToString()
		{
			return DeclaringType + "." + Name;
		}

	}
}