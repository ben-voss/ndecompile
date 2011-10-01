using System;
using System.Reflection;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class ParameterDeclaration : IParameterDeclaration
	{
        private readonly ParameterInfo _netParameterInfo;
		private ITypeReference _type;
        private IList<IAttribute> _attributes;

		public ParameterDeclaration(ParameterInfo netParameterInfo)
		{
            _netParameterInfo = netParameterInfo;
		}

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                    foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netParameterInfo))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;
            }
        }

		public IParameterDeclaration Resolve()
		{
			return this;
		}

		public String Name
		{
			get
			{
                return _netParameterInfo.Name;
			}
		}

		public ITypeReference ParameterType
		{
			get
			{
                if (_type == null)
                    _type = AssemblyManager.FindType(_netParameterInfo.ParameterType, _netParameterInfo.ParameterType.GetGenericArguments());

                return _type;
			}
		}

        public bool IsOut
        {
            get
            {
                return _netParameterInfo.IsOut;
            }
        }

        public bool IsByRef
        {
            get
            {
                return _netParameterInfo.IsRetval;
            }
        }
	}
}
