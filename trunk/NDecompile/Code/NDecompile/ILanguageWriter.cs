using LittleNet.NDecompile.Model;

namespace LittleNet.NDecompile
{
	public interface ILanguageWriter
	{
		void WriteMethodDeclaration(IMethodDeclaration methodDeclaration, IFormattedCodeWriter writer);

		void WriteTypeDeclaration (ITypeDeclaration typeDeclaration, IFormattedCodeWriter writer);

		void WriteAssembly(IAssemblyReference assembly, IFormattedCodeWriter writer);

		void WriteModule (IModule module, IFormattedCodeWriter writer);

		void WriteEventDeclaration (IEventDeclaration eventDeclaration, IFormattedCodeWriter writer);

		void WritePropertyDeclaration (IPropertyDeclaration propertyDeclaration, IFormattedCodeWriter writer);

		void WriteFieldDeclaration (IFieldDeclaration fieldDeclaration, IFormattedCodeWriter writer);
	}
}
