using System;

namespace LittleNet.NDecompile.Tests.Model.Impl.Idioms
{
	public class PostDecrementTest
	{
		public int PostDecrement1(int a)
		{
			return a--;
		}

		public int PostDecrement2(int a)
		{
			return a -= 2;
		}

		public void PostDecrement3(int a)
		{
			a--;
		}

		public void PostDecrement4(int a)
		{
			a -= 2;
		}
	}
}