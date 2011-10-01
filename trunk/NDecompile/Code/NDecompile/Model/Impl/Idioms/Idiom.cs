using System.Collections.Generic;

namespace LittleNet.NDecompile.Model.Impl.Idioms
{
	internal abstract class Idiom
	{
		private readonly static Idiom[] Idioms = new Idiom[]{
            new ConstantLoad(),
            new ArgumentLoad(),
			new FieldLoad(),
            new ArgumentSave(),
			new FieldSave(),
            new PreIncrement(),
            new PreDerement(),
            new PostIncrement(),
            new PostDecrement(),
            new RedundantStoreLoad(),
        };

		public static void Process(IList<IInstruction> instructions)
		{
			foreach (Idiom idiom in Idioms)
				for (int index = 0; index < instructions.Count; index++)
					idiom.Process(instructions, index);
		}

		public abstract void Process(IList<IInstruction> instructions, int index);
	}
}