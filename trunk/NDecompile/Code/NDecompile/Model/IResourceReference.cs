
namespace LittleNet.NDecompile.Model
{
	public interface IResourceReference : IMemberReference
	{
		IResource Resolve ();
	}
}
