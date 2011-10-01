
namespace LittleNet.NDecompile.FormsUI.Models
{
	internal class MainModelChangeHint
	{
		public enum HintType {
			AssemblyList,
			CurrentReference
		}

		private readonly HintType _hint;

		public MainModelChangeHint(HintType hint) {
			_hint = hint;
		}

		public HintType Hint {
			get {
				return _hint;
			}
		}
	}
}