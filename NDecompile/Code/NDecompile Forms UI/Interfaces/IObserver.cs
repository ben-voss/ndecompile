using System;

namespace LittleNet.NDecompile.FormsUI.Interfaces
{
	internal interface IObserver
	{
		void Notify(Object hint);
	}
}