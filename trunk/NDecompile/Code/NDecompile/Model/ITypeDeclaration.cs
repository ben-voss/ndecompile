using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
    public interface ITypeDeclaration : ITypeReference, IAttributeProvider
	{
		IModule Module
		{
			get;
		}

        IList<IConstructorDeclaration> Constructors
        {
            get;
        }

        ITypeReference DeclaringType
        {
            get;
        }

		bool IsInterface
		{
			get;
		}

		bool Abstract
		{
			get;
		}

		bool IsEnum
		{
			get;
		}

        ITypeReference BaseType
        {
            get;
        }

        IList<ITypeDeclaration> Types
        {
            get;
        }

        IList<ITypeReference> Interfaces
        {
            get;
        }

		IList<IFieldDeclaration> Fields
		{
			get;
		}

		IList<IEventDeclaration> Events
		{
			get;
		}

		IList<IMethodDeclaration> Methods
		{
			get;
		}

		IList<IPropertyDeclaration> Properties
		{
			get;
		}

		TypeVisibility Visibility
		{
			get;
		}
	}
}
