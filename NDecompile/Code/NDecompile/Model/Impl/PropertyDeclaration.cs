using System;
using System.Collections.Generic;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class PropertyDeclaration : IPropertyDeclaration
	{
        private readonly PropertyInfo _netPropertyInfo;
        private readonly Module _module;

        private ITypeReference _declaringType;
		private ITypeReference _propertyType;
		private IMethodReference _getMethod;
		private IMethodReference _setMethod;
		private List<IParameterDeclaration> _parameters;
        private IList<IAttribute> _attributes;

        public PropertyDeclaration(PropertyInfo netPropertyInfo, Module module)
        {
            _netPropertyInfo = netPropertyInfo;
            _module = module;
		}

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                    foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netPropertyInfo))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;
            }
        }

		public IPropertyDeclaration Resolve()
		{
			return this;
		}

		public ITypeReference DeclaringType
		{
			get
			{
                if (_declaringType == null)
                    _declaringType = AssemblyManager.FindType(_netPropertyInfo.DeclaringType, _netPropertyInfo.DeclaringType.GetGenericArguments());

				return _declaringType;
			}
		}

		public String Name
		{
			get
			{
                return _netPropertyInfo.Name;
			}
		}

		public ITypeReference PropertyType
		{
			get
			{
                if (_propertyType == null)
                    _propertyType = AssemblyManager.FindType(_netPropertyInfo.PropertyType, _netPropertyInfo.PropertyType.GetGenericArguments());

				return _propertyType;
			}
		}

		public IMethodReference GetMethod
		{
			get
			{
                if (_getMethod == null)
                {
                    MethodInfo getMethodInfo = _netPropertyInfo.GetGetMethod(true);
                    if (getMethodInfo != null)
                        _getMethod = _module.FindMethod((uint)getMethodInfo.MetadataToken, _netPropertyInfo.DeclaringType.GetGenericArguments(), getMethodInfo.GetGenericArguments());

                }
                
                return _getMethod;
			}
		}

		public IMethodReference SetMethod
		{
			get
			{
                if (_setMethod == null)
                {
                    MethodInfo setMethodInfo = _netPropertyInfo.GetSetMethod(true);
                    if (setMethodInfo != null)
                        _setMethod = _module.FindMethod((uint)setMethodInfo.MetadataToken, _netPropertyInfo.DeclaringType.GetGenericArguments(), setMethodInfo.GetGenericArguments());
                }

				return _setMethod;
			}
		}

		public List<IParameterDeclaration> Parameters
		{
			get
			{
                if (_parameters == null)
                {
                    _parameters = new List<IParameterDeclaration>();
                    foreach (ParameterInfo parameterInfo in _netPropertyInfo.GetIndexParameters())
                        _parameters.Add(new ParameterDeclaration(parameterInfo));
                }

				return _parameters;
			}
		}
	}
}