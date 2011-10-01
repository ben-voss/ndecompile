using System;
using System.Collections.Generic;
using System.Reflection;

namespace LittleNet.NDecompile.Model.Impl
{
    internal class Attribute : IAttribute
    {
        private readonly CustomAttributeData _netAttribute;

        private IMethodReference _constructor;
        private IList<IExpression> _arguments;

        public Attribute(CustomAttributeData netAttribute)
        {
            _netAttribute = netAttribute;
        }

        public IMethodReference Constructor
        {
            get
            {
                if (_constructor == null)
                    _constructor = AssemblyManager.FindMethod(_netAttribute.Constructor);

                return _constructor;
            }
        }

        public IList<IExpression> Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    _arguments = new List<IExpression>();

                    foreach (CustomAttributeTypedArgument argument in _netAttribute.ConstructorArguments)
                        if (argument.ArgumentType == typeof(Type))
                            _arguments.Add(new TypeOfExpression(AssemblyManager.FindType((Type)argument.Value, new Type[0])));
                        else
                            _arguments.Add(new LiteralExpression(argument.Value));

                    foreach (CustomAttributeNamedArgument namedArgument in _netAttribute.NamedArguments)
                    {
                        IExpression valueExpression;
                        if (namedArgument.TypedValue.ArgumentType == typeof(Type))
                            valueExpression = new TypeOfExpression(AssemblyManager.FindType((Type)namedArgument.TypedValue.Value, new Type[0]));
                        else
                            valueExpression = new LiteralExpression(namedArgument.TypedValue.Value);

                        if (namedArgument.MemberInfo.MemberType == MemberTypes.Property)
                            _arguments.Add(new AssignExpression(valueExpression, new PropertyReferenceExpression(AssemblyManager.FindProperty((PropertyInfo)namedArgument.MemberInfo), null)));
                        else if (namedArgument.MemberInfo.MemberType == MemberTypes.Field)
                            _arguments.Add(new AssignExpression(valueExpression, new FieldReferenceExpression(AssemblyManager.FindField((FieldInfo)namedArgument.MemberInfo, namedArgument.MemberInfo.DeclaringType.GetGenericArguments(), null), null)));
                    }
                }

                return _arguments;
            }
        }
    }
}
