using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using LittleNet.NDecompile.Model;
using LittleNet.NDecompile.Model.Impl;
using NetOpCode = System.Reflection.Emit.OpCode;

namespace LittleNet.NDecompile
{
	internal static class OpCodeTable
	{
		private static readonly Dictionary<short, OpCode> _table = new Dictionary<short, OpCode>();
		private static readonly OpCode _preIncrementOpCode;
		private static readonly OpCode _preDecrementOpCode;
		private static readonly OpCode _postIncrementOpCode;
		private static readonly OpCode _postDecrementOpCode;
		private static readonly OpCode _postIncrementStatementOpCode;
		private static readonly OpCode _postDecrementStatementOpCode;

		static OpCodeTable()
		{
			short maxOpCodeValue = 0;

			foreach (FieldInfo field in typeof(OpCodes).GetFields())
			{
				NetOpCode netOpCode = (NetOpCode)field.GetValue(null);
				OpCode opCode = new OpCode(netOpCode.Value, netOpCode.Name, netOpCode.FlowControl, netOpCode.OperandType);

				_table.Add(opCode.Value, opCode);

				maxOpCodeValue = Math.Max(opCode.Value, maxOpCodeValue);
			}

			_preIncrementOpCode = new OpCode(++maxOpCodeValue, "preInc", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_preIncrementOpCode.Value, _preIncrementOpCode);

			_preDecrementOpCode = new OpCode(++maxOpCodeValue, "preDec", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_preDecrementOpCode.Value, _preDecrementOpCode);

			_postIncrementOpCode = new OpCode(++maxOpCodeValue, "postInc", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_postIncrementOpCode.Value, _postIncrementOpCode);

			_postDecrementOpCode = new OpCode(++maxOpCodeValue, "postDec", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_postDecrementOpCode.Value, _postDecrementOpCode);

			_postIncrementStatementOpCode = new OpCode(++maxOpCodeValue, "postIncStmt", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_postIncrementStatementOpCode.Value, _postIncrementStatementOpCode);

			_postDecrementStatementOpCode = new OpCode(++maxOpCodeValue, "postDecStmt", FlowControl.Next, OperandType.InlineNone);
			_table.Add(_postDecrementStatementOpCode.Value, _postDecrementStatementOpCode);
		}

		public static OpCode PostIncrementStatementOpCode
		{
			get
			{
				return _postIncrementStatementOpCode;
			}
		}

		public static OpCode PostDecrementStatementOpCode
		{
			get
			{
				return _postDecrementStatementOpCode;
			}
		}

		public static OpCode PreIncrementOpCode
		{
			get
			{
				return _preIncrementOpCode;
			}
		}

		public static OpCode PreDecrementOpCode
		{
			get
			{
				return _preDecrementOpCode;
			}
		}

		public static OpCode PostIncrementOpCode
		{
			get
			{
				return _postIncrementOpCode;
			}
		}

		public static OpCode PostDecrementOpCode
		{
			get
			{
				return _postDecrementOpCode;
			}
		}

		public static OpCode Switch
		{
			get
			{
				return _table[OpCodes.Switch.Value];
			}
		}

		private static OpCode GetOpCode(byte[] il, ref ushort ilp)
		{
			ushort op = il[ilp++];

			if (op == 0xfe)
			{
				op <<= 8;
				op |= il[ilp++];
			}

			OpCode opCode;
			if (_table.TryGetValue((short)op, out opCode))
				return opCode;

			return _table[OpCodes.Nop.Value];
		}

		public static OpCode Get(short value)
		{
			return _table[value];
		}

		public static IInstruction GetInstruction(byte[] il, ref ushort ilp, Model.Impl.Module module, Type[] genericTypeArguments, Type[] genericMethodArguments)
		{
			ushort ip = ilp;

			// Get the opcode
			OpCode opCode = GetOpCode(il, ref ilp);

			// Get the argument
			Object argument = null;
			switch (opCode.OperandType)
			{
				case OperandType.InlineBrTarget:
					argument = (ushort)(GetArgument(il, ref ilp, 4, true) + ilp);
					break;

				case OperandType.InlineField:
					argument = module.FindField((uint)GetArgument(il, ref ilp, 4, false), genericTypeArguments, genericMethodArguments);
					break;

				case OperandType.InlineI:
					argument = (int)GetArgument(il, ref ilp, 4, false);
					break;

				case OperandType.InlineI8:
					argument = GetArgument(il, ref ilp, 8, false);
					break;

				case OperandType.InlineMethod:
					argument = module.FindMethod((uint)GetArgument(il, ref ilp, 4, false), genericTypeArguments, genericMethodArguments);
					break;

				case OperandType.InlineNone:
					break;

#pragma warning disable 612,618
				case OperandType.InlinePhi:
#pragma warning restore 612,618
					throw new ApplicationException("Reserved Operand Type");

				case OperandType.InlineR:
					argument = BitConverter.Int64BitsToDouble(GetArgument(il, ref ilp, 8, false));
					break;

				case OperandType.InlineSig:
					argument = module.FindToken((uint)GetArgument(il, ref ilp, 4, false), genericTypeArguments, genericMethodArguments);
					break;

				case OperandType.InlineString:
					argument = module.FindString((uint)GetArgument(il, ref ilp, 4, false));
					break;

				case OperandType.InlineSwitch:
					int numTargets = (int)GetArgument(il, ref ilp, 4, false);

					short[] offsets = new short[numTargets];
					for (int i = 0; i < numTargets; i++)
						offsets[i] = (short)GetArgument(il, ref ilp, 4, true);

					ushort[] targets = new ushort[numTargets];
					for (int i = 0; i < numTargets; i++)
						targets[i] = (ushort)(ilp + offsets[i]);

					argument = targets;
					break;

				case OperandType.InlineTok:
					argument = module.FindToken((uint)GetArgument(il, ref ilp, 4, false), genericTypeArguments, genericMethodArguments);

					break;

				case OperandType.InlineType:
					argument = module.FindType((uint) GetArgument(il, ref ilp, 4, false), genericTypeArguments, genericMethodArguments);
					break;

				case OperandType.InlineVar:
					argument = (ushort)GetArgument(il, ref ilp, 2, false);
					break;

				case OperandType.ShortInlineBrTarget:
					argument = (ushort)(GetArgument(il, ref ilp, 1, true) + ilp);
					break;

				case OperandType.ShortInlineI:
					argument = (byte)GetArgument(il, ref ilp, 1, false);
					break;

				case OperandType.ShortInlineR:
					argument = GetArgument(il, ref ilp, 4, false);
					break;

				case OperandType.ShortInlineVar:
					argument = (byte)GetArgument(il, ref ilp, 1, false);
					break;

				default:
					throw new ApplicationException("Unknown operand type");

			}

			return new Instruction(opCode, ip, argument);
		}

		private static long GetArgument(byte[] il, ref ushort ilp, int length, bool signed)
		{
			if (signed)
			{
				switch (length)
				{
					case 1:
						return (sbyte)il[ilp++];

					case 2:
						short i = BitConverter.ToInt16(il, ilp);
						ilp += 2;
						return i;

					case 4:
						int j = BitConverter.ToInt32(il, ilp);
						ilp += 4;
						return j;

					case 8:
						long k = BitConverter.ToInt64(il, ilp);
						ilp += 8;
						return k;

					default:
						throw new ApplicationException("Unexpected argument length");
				}

			}

			switch (length)
			{
				case 1:
					return il[ilp++];

				case 2:
					short i = BitConverter.ToInt16(il, ilp);
					ilp += 2;
					return i;

				case 4:
					int j = BitConverter.ToInt32(il, ilp);
					ilp += 4;
					return j;

				case 8:
					long k = BitConverter.ToInt64(il, ilp);
					ilp += 8;
					return k;

				default:
					throw new ApplicationException("Unexpected argument length");
			}
		}

	}
}
