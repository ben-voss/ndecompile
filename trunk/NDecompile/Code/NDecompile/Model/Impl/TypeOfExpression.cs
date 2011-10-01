
namespace LittleNet.NDecompile.Model.Impl
{
	internal class TypeOfExpression : Expression, ITypeOfExpression
	{
		private readonly ITypeReference _type;

		public TypeOfExpression(ITypeReference type)
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
