using System;
using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
    public interface IAttribute
    {
        IMethodReference Constructor
        {
            get;
        }

        IList<IExpression> Arguments
        {
            get;
        }
    }
}
