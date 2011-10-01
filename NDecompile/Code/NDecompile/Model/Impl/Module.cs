using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using NetModule = System.Reflection.Module;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class Module : IModule
	{
		#region Class - GenericReference

		private class GenericReference<T> where T : IMemberReference
		{
			private readonly Type[] _genericArgs;
			private readonly T _reference;

			public GenericReference(T reference, Type[] genericArgs)
			{
				_genericArgs = genericArgs;
				_reference = reference;
			}

			public Type[] GenericArgs
			{
				get
				{
					return _genericArgs;
				}
			}

			public T Reference
			{
				get
				{
					return _reference;
				}
			}
		}

		#endregion

		private readonly NetModule _netModule;

		private readonly IAssembly _assembly;

		private List<ITypeDeclaration> _types;

		private readonly Dictionary<uint, IFieldDeclaration> _tokenFieldMap = new Dictionary<uint, IFieldDeclaration>();
		private readonly Dictionary<uint, IPropertyReference> _tokenPropertyMap = new Dictionary<uint, IPropertyReference>();

		private readonly Dictionary<uint, List<GenericReference<ITypeReference>>> _genericTokenTypeMap = new Dictionary<uint, List<GenericReference<ITypeReference>>>();

		private readonly Dictionary<uint, List<GenericReference<IMethodReference>>> _genericTokenMethodMap = new Dictionary<uint, List<GenericReference<IMethodReference>>>();


		public Module(NetModule netModule, IAssembly assembly)
		{
			_netModule = netModule;
			_assembly = assembly;
		}

		public IAssemblyReference Assembly
		{
			get
			{
				return _assembly;
			}
		}

		public IModule Resolve()
		{
			return this;
		}

		public int MDStreamVersion
		{
			get
			{
				return _netModule.MDStreamVersion;
			}
		}

		public Guid ModuleVersionId
		{
			get
			{
				return _netModule.ModuleVersionId;
			}
		}

		public ImageFileMachine ImageFileMachine
		{
			get
			{
				PortableExecutableKinds peKind;
				ImageFileMachine machine;
				_netModule.GetPEKind(out peKind, out machine);
				return machine;
			}
		}

		public PortableExecutableKinds PortableExecutableKinds
		{
			get
			{
				PortableExecutableKinds peKind;
				ImageFileMachine machine;
				_netModule.GetPEKind(out peKind, out machine);
				return peKind;
			}
		}

		public X509Certificate Certificate
		{
			get
			{
				return _netModule.GetSignerCertificate();
			}
		}

		public String Name
		{
			get
			{
				return _netModule.ScopeName;
			}
		}

		public NetModule Internal
		{
			get
			{
				return _netModule;
			}
		}

		public List<ITypeDeclaration> Types
		{
			get
			{
				if (_types == null)
				{
					_types = new List<ITypeDeclaration>();

					foreach (Type netType in _netModule.GetTypes())
					{
						if (netType.DeclaringType == null)
						{
							// Create the new type declaration and add it to the meta data tables
							ITypeReference typeReference = FindType((uint)netType.MetadataToken, netType.GetGenericArguments(), null);
							_types.Add(typeReference.Resolve());
						}
					}
				}

				return _types;
			}
		}

		public IMemberReference FindToken(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			if ((metadataToken & 0xff000000) == 0x01000000)
				return FindTypeRef(metadataToken, genericTypeArguments, genericMethodArguments, genericTypeArguments);

			if ((metadataToken & 0xff000000) == 0x02000000)
				return FindType(metadataToken, genericTypeArguments, genericMethodArguments);

			if ((metadataToken & 0xff000000) == 0x04000000)
				return FindField(metadataToken, genericTypeArguments, genericMethodArguments);

			if ((metadataToken & 0xff000000) == 0x06000000)
				return FindMethod(metadataToken, genericTypeArguments, genericMethodArguments);

			if ((metadataToken & 0xff000000) == 0x0a000000)
				return FindMemberRef(metadataToken, genericTypeArguments, genericMethodArguments);

			if ((metadataToken & 0xff000000) == 0x1b000000)  // Array type refs
				return FindTypeRef(metadataToken, genericTypeArguments, genericMethodArguments, genericTypeArguments);

			//if ((metadataToken & 0xff000000) == 0x0a000000)
			//	return FindMethodRef(metadataToken);

			//if ((metadataToken & 0xff000000) == 0x14000000)
			//	return FindEvent(metadataToken);

			//if ((metadataToken & 0xff000000) == 0x17000000)
			//	return FindProperty(metadataToken);

			throw new ApplicationException(String.Format("Unable to find token {0:x8} in module '{1}'.", metadataToken, _netModule.ScopeName));
		}

		private IMemberReference FindMemberRef(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return AssemblyManager.FindMember(_netModule.ResolveMember((int)metadataToken, genericTypeArguments, genericMethodArguments));
		}

//		private IMethodReference FindMethodRef(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
//		{
//			return AssemblyManager.FindMethod(_netModule.ResolveMethod((int)metadataToken, genericTypeArguments, genericMethodArguments));
//		}

		private ITypeReference FindTypeRef(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments, Type[] genericArguments)
		{
			return AssemblyManager.FindType(_netModule.ResolveType((int)metadataToken, genericTypeArguments, genericMethodArguments), genericArguments);
		}

		public IMemberReference FindMember(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			// Resolve the member to find out what type it is and then re-resolve it via the
			// appropriate cache lookup method
			MemberInfo netMemberInfo = _netModule.ResolveMember((int)metadataToken, genericTypeArguments, genericMethodArguments);

			if (netMemberInfo.MemberType == MemberTypes.Constructor)
				return FindMethod((uint)netMemberInfo.MetadataToken, genericTypeArguments, genericMethodArguments);

			if (netMemberInfo.MemberType == MemberTypes.Field)
				return FindField((uint)netMemberInfo.MetadataToken, genericTypeArguments, genericMethodArguments);

			if (netMemberInfo.MemberType == MemberTypes.Method)
				return FindMethod((uint)netMemberInfo.MetadataToken, genericTypeArguments, genericMethodArguments);

			throw new ApplicationException("dont know what to do with a " + netMemberInfo.MemberType + " member.");
		}

		private static bool CompareArrays<T, V>(T[] array1, V[] array2)
		{
			if (array1.Length != array2.Length)
				return false;

			for (int i = 0; i < array1.Length; i++)
			{
				if (!array1[i].Equals(array2[i]))
					return false;
			}

			return true;
		}

		public ITypeReference FindType(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			// Lookup the type in the cache
			List<GenericReference<ITypeReference>> genericTypeReferences;
			if (_genericTokenTypeMap.TryGetValue(metadataToken, out genericTypeReferences))
				foreach (GenericReference<ITypeReference> r in genericTypeReferences)
					if (CompareArrays(r.GenericArgs, genericTypeArguments))
						return r.Reference;

			IList<ITypeReference> genericArguments = new List<ITypeReference>();
			foreach (Type type in genericTypeArguments)
				genericArguments.Add(AssemblyManager.FindType(type, type.GetGenericArguments()));

			TypeDeclaration typeDeclaration = new TypeDeclaration(_netModule.ResolveType((int)metadataToken, genericTypeArguments, genericMethodArguments), this, _assembly, genericArguments);

			if (genericTypeReferences == null)
			{
				genericTypeReferences = new List<GenericReference<ITypeReference>>();
				_genericTokenTypeMap.Add(metadataToken, genericTypeReferences);
			}

			genericTypeReferences.Add(new GenericReference<ITypeReference>(typeDeclaration, genericTypeArguments));

			return typeDeclaration;
		}

		public IFieldReference FindField(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			IFieldDeclaration fieldReference;
			if (_tokenFieldMap.TryGetValue(metadataToken, out fieldReference))
				return fieldReference;

			fieldReference = new FieldDeclaration(_netModule.ResolveField((int)metadataToken, genericTypeArguments, genericMethodArguments));

			_tokenFieldMap.Add(metadataToken, fieldReference);
			return fieldReference;
		}

		public IPropertyReference FindProperty(PropertyInfo netPropertyInfo)
		{
			IPropertyReference propertyReference;
			if (_tokenPropertyMap.TryGetValue((uint)netPropertyInfo.MetadataToken, out propertyReference))
				return propertyReference;

			propertyReference = new PropertyDeclaration(netPropertyInfo, this);

			_tokenPropertyMap.Add((uint)netPropertyInfo.MetadataToken, propertyReference);
			return propertyReference;
		}

		public IMethodReference FindMethod(uint metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			// Lookup the type in the cache
			List<GenericReference<IMethodReference>> genericMethodReferences;
			if (_genericTokenMethodMap.TryGetValue(metadataToken, out genericMethodReferences))
				foreach (GenericReference<IMethodReference> r in genericMethodReferences)
					if (CompareArrays(r.GenericArgs, genericMethodArguments))
						return r.Reference;

			IList<ITypeReference> genericArguments = new List<ITypeReference>();
			foreach (Type type in genericTypeArguments)
				genericArguments.Add(AssemblyManager.FindType(type, type.GetGenericArguments()));

			MethodBase methodBase = _netModule.ResolveMethod((int)metadataToken, genericTypeArguments, genericMethodArguments);

			IMethodDeclaration methodDeclaration;
			if (methodBase is ConstructorInfo)
				methodDeclaration = new ConstructorDeclaration((ConstructorInfo)methodBase, this,
															   AssemblyManager.FindType(methodBase.DeclaringType, methodBase.DeclaringType.GetGenericArguments()));
			else
				methodDeclaration = new MethodDeclaration((MethodInfo)methodBase, this,
														  AssemblyManager.FindType(methodBase.DeclaringType, methodBase.DeclaringType.GetGenericArguments()));

			if (genericMethodReferences == null)
			{
				genericMethodReferences = new List<GenericReference<IMethodReference>>();
				_genericTokenMethodMap.Add(metadataToken, genericMethodReferences);
			}

			genericMethodReferences.Add(new GenericReference<IMethodReference>(methodDeclaration, genericTypeArguments));

			return methodDeclaration;
		}

		public String FindString(uint metadataToken)
		{
			return _netModule.ResolveString((int)metadataToken);
		}
	}
}