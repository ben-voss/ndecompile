using System;

namespace LittleNet.NDecompile.Tests.Model.Impl
{
    class ArgumentTests
    {
        public void RefArgTest(string norm, out string outarg, ref string foo)
        {
            foo = "Hello World";
            outarg = "bar";
        }

    }
}
