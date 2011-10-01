using System;
using System.Reflection.Emit;
using NetOpCode = System.Reflection.Emit.OpCode;

namespace LittleNet.NDecompile
{
	public class OpCode
	{
		private readonly short _value;
		private readonly String _name;
		private readonly FlowControl _flowControl;
		private readonly OperandType _operandType;

		public OpCode(short value, String name, FlowControl flowControl, OperandType operandType)
		{
			_value = value;
			_name = name;
			_flowControl = flowControl;
			_operandType = operandType;
		}

		public short Value
		{
			get
			{
				return _value;
			}
		}

		public String Name
		{
			get
			{
				return _name;
			}
		}

		public FlowControl FlowControl
		{
			get
			{
				return _flowControl;
			}
		}

		public OperandType OperandType
		{
			get
			{
				return _operandType;
			}
		}

		public static bool operator !=(OpCode a, OpCode b)
		{
			return !(a == b);
		}

		public static bool operator !=(OpCode a, NetOpCode b)
		{
			return !(a == b);
		}

		public static bool operator ==(OpCode a, OpCode b)
		{
			return a.Equals(b);
		}

		public static bool operator ==(OpCode a, NetOpCode b)
		{
			return a.Equals(b);
		}

		public bool Equals(OpCode obj)
		{
			return (obj._value == _value);
		}

		public bool Equals(NetOpCode obj)
		{
			return obj.Value == _value;
		}

		public override bool Equals(object obj)
		{
			return (((obj is OpCode) && Equals((OpCode)obj)) ||
				((obj is NetOpCode) && Equals((NetOpCode)obj)));
		}

		public override int GetHashCode()
		{
			return _value.GetHashCode();
		}

	}
}
