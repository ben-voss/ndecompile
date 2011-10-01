
namespace LittleNet.NDecompile.Model.Impl
{
	internal class DefaultCaseStatement : CaseStatement, IDefaultCaseStatement
	{
		public DefaultCaseStatement(IBlockStatement statement)
			: base(null, statement)
		{
		}
	}
}
