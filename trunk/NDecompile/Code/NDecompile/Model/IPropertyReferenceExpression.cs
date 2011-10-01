using System;

namespace LittleNet.NDecompile.Model
{
    public interface IPropertyReferenceExpression : IExpression
    {
        IPropertyReference Property
        {
            get;
        }

        IExpression Target
        {
            get;
        }
    }
}
