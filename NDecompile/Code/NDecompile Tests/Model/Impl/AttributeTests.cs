using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LittleNet.NDecompile.Tests.Model.Impl
{
    [Serializable]
    public class AttributeTests
    {
    }

    [CLSCompliant(false)]
    public class AttributeTests2
    {
    }

    [Serializable, ClassInterface(ClassInterfaceType.None), ComVisible(true)]
    public class AttributeTests3
    {
    }
}
