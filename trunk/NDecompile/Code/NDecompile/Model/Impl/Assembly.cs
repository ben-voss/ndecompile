using System;
using System.Collections.Generic;
using System.Reflection;
using NetAssembly = System.Reflection.Assembly;
using NetModule = System.Reflection.Module;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class Assembly : AssemblyReference, IAssembly
	{
        private readonly NetAssembly _netAssembly;
        private IList<IModule> _modules;
		private IList<IResource> _resources;
		private IList<IAssemblyReference> _assemblyReferences;
        private IList<IAttribute> _attributes;

		public Assembly(NetAssembly assembly) : base(assembly.GetName())
		{
			_netAssembly = assembly;
		}

        public IList<IAttribute> Attributes
        {
            get
            {
                if (_attributes == null)
                {
                    _attributes = new List<IAttribute>();

                    foreach (CustomAttributeData netAttribute in CustomAttributeData.GetCustomAttributes(_netAssembly))
                        _attributes.Add(new Attribute(netAttribute));
                }

                return _attributes;
            }
        }

        public String FileName
        {
            get
            {
                return _netAssembly.CodeBase;
            }
        }

		public override IAssembly Resolve()
		{
			return this;
		}

		public IList<IAssemblyReference> ReferencedAssemblies
		{
			get
			{
                if (_assemblyReferences == null)
                {
                    _assemblyReferences = new List<IAssemblyReference>();
                    foreach (AssemblyName name in _netAssembly.GetReferencedAssemblies())
                        _assemblyReferences.Add(new AssemblyReference(name));
                }

				return _assemblyReferences;
			}
		}

        private Module FindModule(NetModule netModule)
        {
            if (_modules == null)
                LoadModules();

            foreach (Module module in _modules)
                if (module.Internal == netModule)
                    return module;

            throw new ApplicationException("Unable to find module.");
        }

		internal ITypeReference FindType(Type netType, Type[] genericTypeArguments)
		{
            return FindModule(netType.Module).FindType((uint)netType.MetadataToken, genericTypeArguments, null);
		}

        internal IFieldReference FindField(FieldInfo netFieldInfo, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            return FindModule(netFieldInfo.Module).FindField((uint)netFieldInfo.MetadataToken, genericTypeArguments, genericMethodArguments);
        }

        internal IPropertyReference FindProperty(PropertyInfo netPropertyInfo)
        {
            return FindModule(netPropertyInfo.Module).FindProperty(netPropertyInfo);
        }

        internal IMethodReference FindMethod(MethodBase netMethodInfo)
        {
            if (netMethodInfo.IsGenericMethod)
                return FindModule(netMethodInfo.Module).FindMethod((uint)netMethodInfo.MetadataToken, netMethodInfo.DeclaringType.GetGenericArguments(), netMethodInfo.GetGenericArguments());
            else
                return FindModule(netMethodInfo.Module).FindMethod((uint)netMethodInfo.MetadataToken, new Type[0], new Type[0]);
        }

        internal IMemberReference FindMember(MemberInfo netMemberInfo)
        {
            return FindModule(netMemberInfo.Module).FindMember((uint)netMemberInfo.MetadataToken, netMemberInfo.DeclaringType.GetGenericArguments(), new Type[0]);
        }

		public IList<IModule> Modules
		{
			get
			{
				if (_modules == null)
					LoadModules();

				return _modules;
			}
		}

		public IList<IResource> Resources
		{
			get
			{
				if (_resources == null)
					LoadResources();

				return _resources;
			}
		}

		private void LoadModules()
		{
            _modules = new List<IModule>();
			foreach (NetModule netModule in _netAssembly.GetModules(true))
				if (!netModule.IsResource())
					_modules.Add(new Module(netModule, this));
		}

		private void LoadResources()
        {
            _resources = new List<IResource>();
			foreach (String resourceName in _netAssembly.GetManifestResourceNames())
				_resources.Add(new Resource(resourceName, _netAssembly, this));
		}
	}
}