using System;
using System.Collections.Generic;
using System.Text;

namespace LittleNet.NDecompile.Tests
{
    public class GuiImages
    {
    	public int _publicField;

    	protected int _protectedField;

    	private int _privateField;

		internal int _internalField;

		internal protected int _internalProtectedField;

    	public static int _publicStaticField;

		protected static int _protectedStaticField;

    	private static int _privateStaticField;

		internal static int _internalStaticField;

		internal protected static int _internalProtectedStaticField;

		public const int _publicConstField = 1;

    	protected const int _protectedConstField = 2;

    	private const int _privateConstField = 3;

		internal const int _internalConstField = 3;

		internal protected const int _internalProtectedConstField = 3;
		
		public readonly int _publicReadOnlyField = 4;

		protected readonly int _protectedReadOnlyField = 5;

		private readonly int _privateReadOnlyField = 6;

		internal readonly int _internalReadOnlyField = 6;

		internal protected readonly int _internalProtectedReadOnlyField = 6;
		
		public static readonly int _publicStaticReadOnlyField = 7;

		protected static readonly int _protectedStaticReadOnlyField = 8;

		private static readonly int _privateStaticReadOnlyField = 9;

		internal static readonly int _internalStaticReadOnlyField = 9;

		internal protected static readonly int _internalProtectedStaticReadOnlyField = 9;

		public void PublicMethod()
        {
        }

        private void PrivateMethod()
        {
        }

        protected void ProtectedMethod()
        {
        }

        internal void InternalMethod()
        {
        }

        internal protected void InternalProtectedMethod()
        {
        }

        public static void PublicStaticMethod()
        {
        }

        private static void PrivateStaticMethod()
        {
        }

        protected static void ProtectedStaticMethod()
        {
        }

        internal static void InternalStaticMethod()
        {
        }

        internal protected static void InternalProtectedStaticMethod()
        {
        }

        public int PublicProperty
        {
            get
            {
                return 0;
            }
			set
			{
				_publicField = value;
			}
        }

        private int PrivateProperty
        {
            get
            {
                return 0;
            }
        }

        protected int ProtectedProperty
        {
            get {
                return 0;
            }
        }

        internal int InternalProperty
        {
            get
            {
                return 0;
            }
        }

        public delegate void PublicDelegate();

        private delegate void PrivateDelegate();

        internal delegate void InternalDelegate();

        public event PublicDelegate OnPublicEvent;

        private event PrivateDelegate OnPrivateEvent;

        internal event InternalDelegate OnInternalEvent;
    }
}
