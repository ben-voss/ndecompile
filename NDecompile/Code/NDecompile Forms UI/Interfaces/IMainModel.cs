using System;
using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile.FormsUI.Interfaces
{
	internal interface IMainModel : IObservable
	{
		void LoadAssembly(String fileName);

		IAssembly[] Assemblies
		{
			get;
		}

		IMemberReference CurrentReference
		{
			get;
			set;
		}

		bool CanGoBack
		{
			get;
		}

		bool CanGoForward
		{
			get;
		}

		void Back();

		void Forward();
	}
}