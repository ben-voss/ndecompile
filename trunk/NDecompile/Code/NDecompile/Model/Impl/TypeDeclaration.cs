using System;
using System.Collections.Generic;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class TypeDeclaration : ITypeDeclaration
	{
		private const BindingFlags ResolutionBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        private readonly IAssembly _assembly;
        private readonly Type _netType;
        private readonly Module _module;

        private ITypeReference _baseType;
        private IList<ITypeReference> _interfaces;
        private IList<ITypeDeclaration> _types;
		private IList<IFieldDeclaration> _fields;
		private IList<IEventDeclaration> _events;
		private IList<IPropertyDeclaration> _properties;
		private IList<IMethodDeclaration> _methods;
		private IList<ITypeReference> _genericArguments;
        private IList<IAttribute> _attributes;
        private IList<IConstructorDeclaration> _constructors;

		public TypeDeclaration(Type netType, Module module, IAssembly assembly, IList<ITypeReference> genericArguments)
		{
			_netType = netType;
			_assembly = assembly;
			_module = module;
			_genericArguments = genericArguments;
		}

		public IModule Module
		{
			get
			{
				return _module;
			}
		}

        public bool IsGenericParameter
        {
            get
            {
                return _netType.IsGenericParameter;
            }
        }

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                     foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netType))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;                
            }
        }

        public ITypeReference DeclaringType
        {
            get
            {
                if (_netType.DeclaringType == null)
                    return null;

                return AssemblyManager.FindType(_netType.DeclaringType, _netType.GetGenericArguments());
            }
        }

        public ITypeReference BaseType
        {
            get
            {
                if ((_netType.BaseType != null) && (_baseType == null))
                    _baseType = AssemblyManager.FindType(_netType.BaseType, _netType.BaseType.GetGenericArguments());

                return _baseType;
            }
        }

        public bool IsGeneric
		{
			get
			{
				return _netType.IsGenericType;
			}
		}

		public bool Abstract
		{
			get
			{
				return _netType.IsAbstract;
			}
		}

		public IList<ITypeReference> GenericArguments
		{
			get
			{
				return _genericArguments;
			}
		}

		public bool IsValueType
		{
			get
			{
				return _netType.IsValueType;
			}
		}

		public IAssembly Assembly
		{
			get
			{
				return _assembly;
			}
		}

		public ITypeDeclaration Resolve()
		{
			return this;
		}

		public String Name
		{
			get
			{
				return _netType.Name;
			}
		}

		public String Namespace
		{
			get
			{
				return _netType.Namespace;
			}
		}

		public bool IsInterface
		{
			get
			{
				return _netType.IsInterface;
			}
		}

		public bool IsEnum
		{
			get
			{
				return _netType.IsEnum;
			}
		}

		public TypeVisibility Visibility
		{
			get
			{
				if (_netType.IsPublic)
					return TypeVisibility.Public;

				return TypeVisibility.Private;
			}
		}

        public IList<ITypeReference> Interfaces
        {
            get
            {
                if (_interfaces == null)
                {
                    _interfaces = new List<ITypeReference>();

                    foreach (Type type in _netType.GetInterfaces())
                    {
                        ITypeReference typeReference = AssemblyManager.FindType(type, type.GetGenericArguments());

                        _interfaces.Add(typeReference);
                    }
                }

                return _interfaces;
            }
        }

        public IList<ITypeDeclaration> Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new List<ITypeDeclaration>();

                    // Add the types nested child types
                    foreach (Type type in _netType.GetNestedTypes(ResolutionBindingFlags))
                    {
                        if (type.DeclaringType != _netType)
                            continue;

                        ITypeReference typeReference = _module.FindType((uint)type.MetadataToken, type.GetGenericArguments(), null);

                        _types.Add(typeReference.Resolve());
                    }
                }

                return _types;
            }
        }

		public IList<IFieldDeclaration> Fields
		{
			get
			{
				if (_fields == null)
				{
					_fields = new List<IFieldDeclaration>();

					// Add the types fields
					foreach (FieldInfo fieldInfo in _netType.GetFields(ResolutionBindingFlags))
					{
						if (fieldInfo.DeclaringType != _netType)
							continue;

						IFieldReference fieldDeclaration = _module.FindField((uint)fieldInfo.MetadataToken, _netType.GetGenericArguments(), null);

						_fields.Add(fieldDeclaration.Resolve());
					}
				}

				return _fields;
			}
		}

		public IList<IEventDeclaration> Events
		{
			get
			{
				if (_events == null)
				{
					_events = new List<IEventDeclaration>();
					foreach (EventInfo eventInfo in _netType.GetEvents(ResolutionBindingFlags))
					{
						if (eventInfo.DeclaringType != _netType)
							continue;

						_events.Add(new EventDeclaration(eventInfo, this, _module));
					}
				}

				return _events;
			}
		}

		public IList<IPropertyDeclaration> Properties
		{
			get
			{
				if (_properties == null)
				{
					_properties = new List<IPropertyDeclaration>();

					foreach (PropertyInfo propertyInfo in _netType.GetProperties(ResolutionBindingFlags))
					{
						if (propertyInfo.DeclaringType != _netType)
							continue;

                        IPropertyReference propertyReference = _module.FindProperty(propertyInfo);
						_properties.Add(propertyReference.Resolve());
					}
				}

				return _properties;
			}
		}

		public IList<IMethodDeclaration> Methods
		{
			get
			{
				if (_methods == null)
				{
					_methods = new List<IMethodDeclaration>();

					foreach (MethodInfo methodInfo in _netType.GetMethods(ResolutionBindingFlags))
					{
						if (methodInfo.DeclaringType != _netType)
							continue;

						if (methodInfo.IsSpecialName)
							continue;

						IMethodReference methodReference = _module.FindMethod((uint)methodInfo.MetadataToken, methodInfo.GetGenericArguments(), _netType.GetGenericArguments());
						_methods.Add(methodReference.Resolve());
					}
				}

				return _methods;
			}
		}

        public IList<IConstructorDeclaration> Constructors
        {
            get
            {
                if (_constructors == null)
                {
                    _constructors = new List<IConstructorDeclaration>();

                    foreach (ConstructorInfo constructorInfo in _netType.GetConstructors(ResolutionBindingFlags))
                    {
                        if (constructorInfo.DeclaringType != _netType)
                            continue;

                        //if (constructorInfo.IsSpecialName)
                        //    continue;

                        IMethodReference constructorReference;
                        if (constructorInfo.IsGenericMethod)
                            constructorReference = _module.FindMethod((uint)constructorInfo.MetadataToken, constructorInfo.GetGenericArguments(), _netType.GetGenericArguments());
                        else
                            constructorReference = _module.FindMethod((uint)constructorInfo.MetadataToken, new Type[0], new Type[0]);

                        _constructors.Add((IConstructorDeclaration)constructorReference.Resolve());
                    }
                }

                return _constructors;
            }
        }

		public bool IsArray
		{
			get
			{
				return _netType.IsArray;
			}
		}
		
		public override string ToString ()
		{
			return Name;
		}

	}
}