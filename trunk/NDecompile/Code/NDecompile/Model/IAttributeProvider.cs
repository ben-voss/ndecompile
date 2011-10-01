using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
    public interface IAttributeProvider
    {
        IList<IAttribute> Attributes
        {
            get;
        }
    }
}
