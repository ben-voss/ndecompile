
namespace LittleNet.NDecompile.Model
{
	public interface IInstruction
	{
		/// <summary>
		/// Gets the operation code of this instruction
		/// </summary>
		OpCode OpCode
		{
			get;
		}

		/// <summary>
		/// Gets the offset address of this instruction in the method
		/// </summary>
		ushort IP
		{
			get;
		}

		/// <summary>
		/// The instructions arguments
		/// </summary>
		object Argument
		{
			get;
		}
	}
}
