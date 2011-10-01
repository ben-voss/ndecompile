using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using LittleNet.NDecompile.FormsUI.Interfaces;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.FormsUI.Properties;
using System.Collections.Specialized;

namespace LittleNet.NDecompile.FormsUI.Models
{
	internal class MainModel : ModelBase, IMainModel
	{
		private IMemberReference _currentReference;
		private readonly Stack<IMemberReference> _backList = new Stack<IMemberReference>();
		private readonly Stack<IMemberReference> _forwardList = new Stack<IMemberReference>();

		public MainModel()
		{
			AssemblyManager.AssemblyListChanged += HandleAssemblyListChanged;

			// Start a background thread to load the assemblies
			ThreadPool.QueueUserWorkItem(AssemblyLoadCallback);
		}

        private static void AssemblyLoadCallback(object arg)
        {
            if (Settings.Default.Assemblies != null)
                foreach (String assemblyFileName in Settings.Default.Assemblies)
                    AssemblyManager.Load(new FileInfo(Uri.UnescapeDataString(new Uri(assemblyFileName).PathAndQuery)));
        }

		protected override void Dispose(bool disposing)
		{
			if (disposing)
				AssemblyManager.AssemblyListChanged -= HandleAssemblyListChanged;

            StringCollection assemblyFileNames = new StringCollection();
            foreach (IAssembly assembly in AssemblyManager.Assemblies)
                assemblyFileNames.Add(assembly.FileName);

            Settings.Default.Assemblies = assemblyFileNames;
            Settings.Default.Save();

			base.Dispose(disposing);
		}

		void HandleAssemblyListChanged(object sender, EventArgs args)
		{
			NotifyObservers(new MainModelChangeHint(MainModelChangeHint.HintType.AssemblyList));
		}

		public void LoadAssembly(String fileName)
		{
			AssemblyManager.Load(new FileInfo(fileName));

			NotifyObservers(new MainModelChangeHint(MainModelChangeHint.HintType.AssemblyList));
		}

		public IAssembly[] Assemblies
		{
			get
			{
				return AssemblyManager.Assemblies;
			}
		}

		public IMemberReference CurrentReference
		{
			get
			{
				return _currentReference;
			}
			set
			{
				if (_currentReference == value)
					return;

				if (_currentReference != null)
					_backList.Push(_currentReference);

				_currentReference = value;
				NotifyObservers(new MainModelChangeHint(MainModelChangeHint.HintType.CurrentReference));
			}
		}

		public void Back()
		{
			if (_backList.Count == 0)
				return;

			_forwardList.Push(CurrentReference);
			_currentReference = _backList.Pop();
			NotifyObservers(new MainModelChangeHint(MainModelChangeHint.HintType.CurrentReference));
		}

		public bool CanGoBack
		{
			get
			{
				return _backList.Count != 0;
			}
		}

		public void Forward()
		{
			if (_forwardList.Count == 0)
				return;

			_backList.Push(CurrentReference);
			_currentReference = _forwardList.Pop();
			NotifyObservers(new MainModelChangeHint(MainModelChangeHint.HintType.CurrentReference));
		}

		public bool CanGoForward
		{
			get
			{
				return _forwardList.Count != 0;
			}
		}
	}
}