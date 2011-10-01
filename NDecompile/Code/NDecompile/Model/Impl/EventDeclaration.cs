using System;
using System.Collections.Generic;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class EventDeclaration : IEventDeclaration
	{
		private readonly ITypeReference _declaringType;
        private readonly EventInfo _netEventInfo;
        private readonly Module _module;
        private IMethodReference _addMethod;
		private IMethodReference _removeMethod;
        private IList<IAttribute> _attributes;
        private ITypeReference _eventType;

		public EventDeclaration(EventInfo netEventInfo, ITypeReference declaringType, Module module)
		{
            _netEventInfo = netEventInfo;
			_declaringType = declaringType;
            _module = module;
		}

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                    foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netEventInfo))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;
            }
        }

		public IEventDeclaration Resolve()
		{
			return this;
		}

		public ITypeReference DeclaringType
		{
			get
			{
				return _declaringType;
			}
		}

		public String Name
		{
			get
			{
                return _netEventInfo.Name;
			}
		}

		public IMethodReference AddMethod
		{
			get
			{
                if (_addMethod == null)
                    _addMethod = _module.FindMethod((uint)_netEventInfo.GetAddMethod(true).MetadataToken, _netEventInfo.DeclaringType.GetGenericArguments(), _netEventInfo.GetAddMethod(true).GetGenericArguments());

				return _addMethod;
			}
		}

		public IMethodReference RemoveMethod
		{
			get
			{
                if (_removeMethod == null)
                    _removeMethod = _module.FindMethod((uint)_netEventInfo.GetRemoveMethod(true).MetadataToken, _netEventInfo.DeclaringType.GetGenericArguments(), _netEventInfo.GetRemoveMethod(true).GetGenericArguments());

				return _removeMethod;
			}
		}

        public ITypeReference EventType
        {
            get
            {
                if (_eventType == null)
                    _eventType = AssemblyManager.FindType(_netEventInfo.EventHandlerType, _netEventInfo.EventHandlerType.GetGenericArguments());

                return _eventType;
            }
        }
	}
}
