using System;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
    class PreIncrementTest
    {

        public int PreIncArg0(int a)
        {
            return ++a;
        }

        public int PreIncArg0Store(int a)
        {
            int b;
            b = ++a;
            return b;
        }

        public int PreIncArg1(int a0, int a)
        {
            return ++a;
        }

        public int PreIncArg2(int a0, int a1, int a)
        {
            return ++a;
        }

        public int PreIncArg3(int a0, int a1, int a2, int a)
        {
            return ++a;
        }

        public int PreIncArg4(int a0, int a1, int a2, int a3, int a)
        {
            return ++a;
        }

        public int PreIncArg5(int a0, int a1, int a2, int a3, int a4, int a)
        {
            return ++a;
        }

        public int PreIncArg6(int a0, int a1, int a2, int a3, int a4, int a5, int a)
        {
            return ++a;
        }

        public int PreIncArg7(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a)
        {
            return ++a;
        }

        public int PreIncArg8(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a)
        {
            return ++a;
        }

        public int PreIncArg9(int a0, int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a)
        {
            return ++a;
        }
    }
}
