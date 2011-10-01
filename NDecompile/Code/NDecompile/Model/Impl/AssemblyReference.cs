using System;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class AssemblyReference : IAssemblyReference
	{
		private readonly Version _version;
		private readonly String _name;
		private readonly int _hashAlgorithm;
		private readonly byte[] _publicKey;
		private readonly AssemblyName _assemblyName;

		public AssemblyReference(AssemblyName assemblyName)
		{
			_assemblyName = assemblyName;
			
			_name = assemblyName.Name;
			_version = assemblyName.Version;
			_hashAlgorithm = (int)assemblyName.HashAlgorithm;
			_publicKey = assemblyName.GetPublicKey();
		}

		public Version Version
		{
			get
			{
				return _version;
			}
		}

		public String Name
		{
			get
			{
				return _name;
			}
		}

		public int HashAlgorithm
		{
			get
			{
				return _hashAlgorithm;
			}
		}

		public byte[] PublicKey
		{
			get
			{
				return _publicKey;
			}
		}

		public virtual IAssembly Resolve()
		{
			return AssemblyManager.Load(_assemblyName);
		}
	}
}
