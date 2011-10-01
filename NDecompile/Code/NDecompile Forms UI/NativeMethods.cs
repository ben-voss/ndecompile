namespace LittleNet.NDecompile.FormsUI
{
	internal static class NativeMethods
	{
		public enum OLECMDF
		{
			// Fields
			OLECMDF_DEFHIDEONCTXTMENU = 0x20,
			OLECMDF_ENABLED = 2,
			OLECMDF_INVISIBLE = 0x10,
			OLECMDF_LATCHED = 4,
			OLECMDF_NINCHED = 8,
			OLECMDF_SUPPORTED = 1
		}

		public enum OLECMDID
		{
			// Fields
			OLECMDID_PAGESETUP = 8,
			OLECMDID_PRINT = 6,
			OLECMDID_PRINTPREVIEW = 7,
			OLECMDID_PROPERTIES = 10,
			OLECMDID_SAVEAS = 4,
			// OLECMDID_SHOWSCRIPTERROR = 40
		}
		public enum OLECMDEXECOPT
		{
			// Fields
			OLECMDEXECOPT_DODEFAULT = 0,
			OLECMDEXECOPT_DONTPROMPTUSER = 2,
			OLECMDEXECOPT_PROMPTUSER = 1,
			OLECMDEXECOPT_SHOWHELP = 3
		}

	}
}
