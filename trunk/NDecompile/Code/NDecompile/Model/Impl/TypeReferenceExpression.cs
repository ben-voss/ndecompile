
namespace LittleNet.NDecompile.Model.Impl
{
	internal class TypeReferenceExpression : Expression, ITypeReferenceExpression
	{
		private readonly ITypeReference _type;

		public TypeReferenceExpression(ITypeReference type)
		{
			_type = type;
		}

		public ITypeReference TypeReference
		{
			get
			{
				return _type;
			}
		}
	}
}