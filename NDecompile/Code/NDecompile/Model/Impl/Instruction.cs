using System;
using NetOpCode = System.Reflection.Emit.OpCode;

namespace LittleNet.NDecompile.Model.Impl
{
	internal class Instruction : IInstruction
	{
		private readonly OpCode _opCode;
		private readonly ushort _ip;
		private readonly Object _argument;

		public Instruction(NetOpCode opCode, ushort ip, Object argument)
		{
			_opCode = OpCodeTable.Get(opCode.Value);
			_ip = ip;
			_argument = argument;
		}

		public Instruction(OpCode opCode, ushort ip, Object argument)
		{
			_opCode = opCode;
			_ip = ip;
			_argument = argument;
		}

		public OpCode OpCode
		{
			get
			{
				return _opCode;
			}
		}

		public ushort IP
		{
			get
			{
				return _ip;
			}
		}

		public Object Argument
		{
			get
			{
				return _argument;
			}
		}
		
		public override string ToString ()
		{
			return string.Format("{0:x4} {1} {2}", _ip, _opCode.Name, _argument);
		}

	}
}
