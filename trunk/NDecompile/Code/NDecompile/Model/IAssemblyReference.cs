
using System;

namespace LittleNet.NDecompile.Model
{
	public interface IAssemblyReference : IMemberReference
	{
		IAssembly Resolve ();

		Version Version
		{
			get;
		}

		int HashAlgorithm
		{
			get;
		}

		byte[] PublicKey
		{
			get;
		}
	}
}
