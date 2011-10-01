using System.Collections.Generic;

namespace LittleNet.NDecompile.Model
{
    public interface IMethodDeclaration : IMethodReference, IMemberDeclaration
	{

		ITypeReference DeclaringType
		{
			get;
		}

		IMethodBody Body
		{
			get;
		}

		MethodVisibility Visibility
		{
			get;
		}

		bool HideBySignature
		{
			get;
		}

		bool Final
		{
			get;
		}

		bool NewSlot
		{
			get;
		}

        bool Virtual
        {
            get;
        }

		bool Overrides
		{
			get;
		}

		bool Abstract
		{
			get;
		}

		bool SpecialName
		{
			get;
		}

		bool RuntimeSpecialName
		{
			get;
		}
	}
}
