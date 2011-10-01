using System;
using System.Collections.Generic;
using LittleNet.NDecompile.FormsUI.Interfaces;

namespace LittleNet.NDecompile.FormsUI.Models
{
	internal class ModelBase : IObservable, IDisposable
	{
		private readonly List<IObserver> _observers = new List<IObserver>();

		void IObservable.AddObserver(IObserver observer)
		{
			_observers.Add(observer);

			observer.Notify(null);
		}

		void IObservable.RemoveObserver(IObserver observer)
		{
			_observers.Remove(observer);
		}

		protected void NotifyObservers(Object hint)
		{
			foreach (IObserver observer in _observers)
				observer.Notify(hint);
		}

		public void Dispose()
		{
			Dispose(true);
			_observers.Clear();
		}

		protected virtual void Dispose(bool disposing)
		{
			
		}
	}
}