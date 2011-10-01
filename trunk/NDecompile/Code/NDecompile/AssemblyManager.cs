using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LittleNet.NDecompile.Model;
using Assembly=LittleNet.NDecompile.Model.Impl.Assembly;
using NetAssembly=System.Reflection.Assembly;
using NetModule = System.Reflection.Module;
using TypeDeclaration=LittleNet.NDecompile.Model.Impl.TypeDeclaration;

namespace LittleNet.NDecompile
{
	public static class AssemblyManager
	{
		private static readonly object _syncObj = new object();

		private static readonly Dictionary<NetAssembly, Assembly> _assemblyList = new Dictionary<NetAssembly, Assembly>();

		private static bool _assemblyListChanged;

		private static int _canFireEvent;

		static AssemblyManager()
		{
			AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomainReflectionOnlyAssemblyResolve;
		}

		public delegate FileInfo AssemblyResolveEvent(object sender, ResolveEventArgs args);
		public delegate void AssemblyListChangedEvent(object sender, EventArgs args);

		static System.Reflection.Assembly CurrentDomainReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
		{
			lock (_syncObj)
			{
				try
				{
					// This event can be invoked from multiple threads so there is a possibility that the assembly
					// has already been loaded by another thread by the time we get here.  Load the assembly to get
					// the framework reference for it and only add it to the load list it its not already on the list.
					NetAssembly netAssembly = NetAssembly.ReflectionOnlyLoad(args.Name);

					if (_assemblyList.ContainsKey(netAssembly))
						return netAssembly;

					_assemblyList.Add(netAssembly, null);
					_assemblyListChanged = true;

					return netAssembly;
				}
				catch (Exception)
				{
					if (AssemblyResolve != null)
					{
						foreach (AssemblyResolveEvent eventHandler in AssemblyResolve.GetInvocationList())
						{
							try
							{
								FileInfo fileInfo = eventHandler.Invoke(sender, args);
								if (fileInfo != null)
								{
									NetAssembly netAssembly = NetAssembly.ReflectionOnlyLoadFrom(fileInfo.FullName);
									_assemblyList.Add(netAssembly, null);
									_assemblyListChanged = true;

									return netAssembly;
								}
							}
							catch (Exception)
							{

							}
						}
					}

					throw;
				}
			}
		}

		public static event AssemblyResolveEvent AssemblyResolve;

		public static event AssemblyListChangedEvent AssemblyListChanged;

		private static void OnAssemblyListChanged()
		{
			if (_canFireEvent != 0)
				return;

			if (!_assemblyListChanged)
				return;

			if (AssemblyListChanged != null)
				AssemblyListChanged(null, EventArgs.Empty);

			_assemblyListChanged = false;
		}

		public static IAssembly Load(FileInfo fileInfo)
		{
			lock (_syncObj)
			{
				NetAssembly netAssembly = NetAssembly.ReflectionOnlyLoadFrom(fileInfo.FullName);

				Assembly assembly;
				if (_assemblyList.TryGetValue(netAssembly, out assembly))
					return assembly;

				if (!_assemblyList.ContainsKey(netAssembly))
				{
					_assemblyList.Add(netAssembly, null);
					_assemblyListChanged = true;
				}

				return InternalLoad(netAssembly);
			}
		}

		public static IAssembly Load(String name)
		{
			lock (_syncObj)
			{
				NetAssembly netAssembly = NetAssembly.ReflectionOnlyLoad(name);

				Assembly assembly;
				if (_assemblyList.TryGetValue(netAssembly, out assembly))
					return assembly;

				if (!_assemblyList.ContainsKey(netAssembly))
				{
					_assemblyList.Add(netAssembly, null);
					_assemblyListChanged = true;
				}

				return InternalLoad(netAssembly);
			}
		}

		internal static IAssembly Load(AssemblyName assemblyName)
		{
			lock (_syncObj)
			{
				NetAssembly netAssembly = NetAssembly.ReflectionOnlyLoad(assemblyName.FullName);

				Assembly assembly;
				if (_assemblyList.TryGetValue(netAssembly, out assembly))
					return assembly;

				if (!_assemblyList.ContainsKey(netAssembly))
				{
					_assemblyList.Add(netAssembly, null);
					_assemblyListChanged = true;
				}

				return InternalLoad(netAssembly);
			}
		}

		private static Assembly InternalLoad(System.Reflection.Assembly netAssembly)
		{
			_canFireEvent++;
			Assembly assembly = new Assembly(netAssembly);
			_canFireEvent--;

			_assemblyList[netAssembly] = assembly;
			_assemblyListChanged = true;

			OnAssemblyListChanged();

			return assembly;
		}

		public static IAssembly[] Assemblies
		{
			get
			{
				lock (_syncObj)
				{
					_canFireEvent++;

					bool changed;
					do
					{
						changed = false;

						NetAssembly[] keys = new List<NetAssembly>(_assemblyList.Keys).ToArray();
						foreach (NetAssembly key in keys)
							if (_assemblyList[key] == null)
							{
								InternalLoad(key);
								changed = true;
							}
					} while (changed);

					_canFireEvent--;

					return new List<Assembly>(_assemblyList.Values).ToArray();
				}
			}
		}

		internal static ITypeReference FindType(Type netType, Type[] genericArguments)
		{
			if ((netType.IsByRef) || (netType.IsArray) || (netType.IsPointer) || (netType.IsGenericParameter))
			{
				IAssembly assembly = FindAssembly(netType.Assembly);

				return new TypeDeclaration(netType, (Model.Impl.Module)assembly.Modules[0], assembly, null);
			}

			return FindAssembly(netType.Assembly).FindType(netType, genericArguments);
		}

		internal static IFieldReference FindField(FieldInfo netFieldInfo, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			return FindAssembly(netFieldInfo.DeclaringType.Assembly).FindField(netFieldInfo, genericTypeArguments, genericMethodArguments);
		}

		internal static IMethodReference FindMethod(MethodBase netMethodInfo)
		{
			return FindAssembly(netMethodInfo.DeclaringType.Assembly).FindMethod(netMethodInfo);
		}

        internal static IPropertyReference FindProperty(PropertyInfo netPropertyInfo)
        {
            return FindAssembly(netPropertyInfo.DeclaringType.Assembly).FindProperty(netPropertyInfo);
        }

		internal static IMemberReference FindMember(MemberInfo netMemberInfo)
		{
			return FindAssembly(netMemberInfo.DeclaringType.Assembly).FindMember(netMemberInfo);
		}

		private static Assembly FindAssembly(NetAssembly netAssembly)
		{
			Assembly assembly;
			if (_assemblyList.TryGetValue(netAssembly, out assembly) && (assembly != null))
				return assembly;

			return InternalLoad(netAssembly);
		}

		public static IMemberReference ParseMemberReference(String reference)
		{
			if (reference == null)
				throw new ArgumentNullException("reference", "The value specified for the 'reference' argument must not be null.");

			if (reference.Length == 0)
				throw new ArgumentException("The value specified for the 'reference' argument must not be zero length.", "reference");

			// Parse the assembly
			if (reference[0] != '[')
				throw new ApplicationException("Expected '[' character at position 0");

			int position = 1;
			while (reference[position++] != ']')
				if (position == reference.Length)
					throw new ApplicationException("Unexpected end of string while parsing assembly name");

			String assemblyName = reference.Substring(1, position - 2);
			if (assemblyName.Length == 0)
				throw new ApplicationException("Unable to parse assembly name.");

			IAssembly[] assemblies = Assemblies;

			IAssembly assembly = null;
			for (int i = 0; i < assemblies.Length; i++)
				if (assemblyName.Equals(assemblies[i].Name, StringComparison.InvariantCulture))
				{
					assembly = assemblies[i];
					break;
				}

			if ((position == reference.Length) || (assembly == null))
				return assembly;

			// Parse the namespace
			int startPosition = position;
			while ((position < reference.Length) && (reference[position++] != ':'))
			{}

			if (position < reference.Length)
				position--;

			String typeName = reference.Substring(startPosition, position - startPosition);
			ITypeReference typeReference = null;
			foreach (IModule module in assembly.Modules)
			{
				foreach (ITypeDeclaration type in module.Types)
				{
					if (typeName == type.Namespace + "." + type.Name)
					{
						typeReference = type;
						break;
					}
				}
			}

			if ((position == reference.Length) || (typeReference == null))
				return typeReference;

			String memberName = reference.Substring(position + 2);

			foreach(IMethodDeclaration methodReference in typeReference.Resolve().Methods)
				if (methodReference.Name == memberName)
					return methodReference;

			foreach (IPropertyDeclaration propertyDeclaration in typeReference.Resolve().Properties)
			{
				if (propertyDeclaration.Name == memberName)
					return propertyDeclaration;

				if ((propertyDeclaration.GetMethod != null) && propertyDeclaration.GetMethod.Name == memberName)
					return propertyDeclaration.GetMethod;

				if ((propertyDeclaration.SetMethod != null) && propertyDeclaration.SetMethod.Name == memberName)
					return propertyDeclaration.SetMethod;
			}

			return null;
		}
	}
}
