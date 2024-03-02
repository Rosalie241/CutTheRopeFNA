using System;

namespace CutTheRope
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			using (CutTheRopeGame game = new CutTheRopeGame())
			{
                game.Run();
			}
		}
	}
}
