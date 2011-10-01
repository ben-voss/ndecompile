
namespace LittleNet.NDecompile.FormsUI.Interfaces
{
	internal interface IObservable
	{
		void AddObserver(IObserver observer);

		void RemoveObserver(IObserver observer);
	}
}