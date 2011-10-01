using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace LittleNet.NDecompile.Model
{
	public interface IModuleReference : IMemberReference
	{
		IModule Resolve();

        int MDStreamVersion
        {
            get;
        }

        Guid ModuleVersionId
        {
            get;
        }

        ImageFileMachine ImageFileMachine
        {
            get;
        }

        PortableExecutableKinds PortableExecutableKinds
        {
            get;
        }

        X509Certificate Certificate
        {
            get;
        }
	}
}