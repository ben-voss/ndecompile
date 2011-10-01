using System;
using System.Reflection;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class FieldDeclaration : IFieldDeclaration
	{
        private readonly FieldInfo _netFieldInfo;
        private ITypeReference _declaringType;
		private ITypeReference _fieldType;
		private ILiteralExpression _initialiser;
        private IList<IAttribute> _attributes;

        public FieldDeclaration(FieldInfo netFieldInfo)
        {
            _netFieldInfo = netFieldInfo;
        }

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                    foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netFieldInfo))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;
            }
        }

		public FieldVisibility Visibility
		{
			get
			{
				if (_netFieldInfo.IsPrivate)
					return FieldVisibility.Private;
				if (_netFieldInfo.IsPublic)
					return FieldVisibility.Public;
				if (_netFieldInfo.IsFamilyAndAssembly)
					return FieldVisibility.FamilyAndAssembly;
				if (_netFieldInfo.IsFamilyOrAssembly)
					return FieldVisibility.FamilyOrAssembly;
				if (_netFieldInfo.IsFamily)
					return FieldVisibility.Family;
				if (_netFieldInfo.IsAssembly)
					return FieldVisibility.Assembly;

				return 0;
			}
		}

		public IFieldDeclaration Resolve()
		{
			return this;
		}

        public ILiteralExpression Initialiser
        {
            get
            {
                if ((_initialiser == null) && (_netFieldInfo.IsLiteral) && (_netFieldInfo.GetRawConstantValue() != null))
                    _initialiser = new LiteralExpression(_netFieldInfo.GetRawConstantValue());

                return _initialiser;
            }
        }

		public bool IsStatic
		{
			get
			{
				return _netFieldInfo.IsStatic;
			}
		}

		public bool IsConst
		{
			get
			{
				return _netFieldInfo.IsLiteral;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return _netFieldInfo.IsInitOnly;
			}
		}

		public String Name
		{
			get
			{
				return _netFieldInfo.Name;
			}
		}

		public ITypeReference DeclaringType
		{
			get
			{
                if (_declaringType == null)
                    _declaringType = AssemblyManager.FindType(_netFieldInfo.DeclaringType, _netFieldInfo.DeclaringType.GetGenericArguments());

				return _declaringType;
			}
		}

		public ITypeReference FieldType
		{
			get
			{
                if (_fieldType == null)
                    _fieldType = AssemblyManager.FindType(_netFieldInfo.FieldType, _netFieldInfo.FieldType.GetGenericArguments());

				return _fieldType;
			}
		}
	}
}
