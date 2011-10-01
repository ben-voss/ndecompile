using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Resources;
using NetAssembly =System.Reflection.Assembly;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class Resource : IResource
	{
		private readonly String _name;

		private readonly NetAssembly _netAssembly;

		private readonly Object _resourceData;

		private readonly IAssemblyReference _assembly;

		public Resource(String name, NetAssembly netAssembly, IAssemblyReference assembly)
		{
			_name = name;
			_netAssembly = netAssembly;
			_assembly = assembly;

			try
			{
				ResourceReader r = new ResourceReader(_netAssembly.GetManifestResourceStream(_name));
				Dictionary<String, String> data = new Dictionary<string, string>();
				foreach (DictionaryEntry dictionaryEntry in r)
					data.Add(dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString());

				_resourceData = data;

				return;
			}
			catch (Exception)
			{

			}

			try
			{
				_resourceData = Image.FromStream(_netAssembly.GetManifestResourceStream(_name));
				return;
			}
			catch (Exception)
			{

			}

			Stream stream = _netAssembly.GetManifestResourceStream(_name);
			byte[] buffer = new byte[stream.Length];
			stream.Read(buffer, 0, buffer.Length);
			stream.Close();
			_resourceData = buffer;
		}

		public IResource Resolve()
		{
			return this;
		}

		public IAssemblyReference Assembly
		{
			get
			{
				return _assembly;
			}
		}

		public Object Data
		{
			get
			{
				return _resourceData;
			}
		}

		public String Name
		{
			get
			{
				return _name;
			}
		}
	}
}