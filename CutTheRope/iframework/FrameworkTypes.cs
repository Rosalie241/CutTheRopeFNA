using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.platform;
using CutTheRope.ios;
using CutTheRope.windows;

namespace CutTheRope.iframework
{
	internal class FrameworkTypes : MathHelper
	{
		public static float SCREEN_WIDTH = 320f;

		public static float SCREEN_HEIGHT = 480f;

		public static float SCREEN_OFFSET_Y = 0f;

		public static float SCREEN_OFFSET_X = 0f;

		public static float SCREEN_HEIGHT_EXPANDED = SCREEN_HEIGHT;

		public static float SCREEN_WIDTH_EXPANDED = SCREEN_WIDTH;

		public static float VIEW_SCREEN_WIDTH = 480f;

		public static float VIEW_SCREEN_HEIGHT = 800f;

		public static float PORTRAIT_SCREEN_WIDTH = 480f;

		public static float PORTRAIT_SCREEN_HEIGHT = 320f;

		public GLCanvas canvas
		{
			get
			{
				return Application.sharedCanvas();
			}
		}

		public static float[] toFloatArray(Quad2D[] quads)
		{
			float[] array = new float[quads.Count() * 8];
			for (int i = 0; i < quads.Length; i++)
			{
				quads[i].toFloatArray().CopyTo(array, i * 8);
			}
			return array;
		}

		public static float[] toFloatArray(Quad3D[] quads)
		{
			float[] array = new float[quads.Count() * 12];
			for (int i = 0; i < quads.Length; i++)
			{
				quads[i].toFloatArray().CopyTo(array, i * 12);
			}
			return array;
		}

		public static Rectangle MakeRectangle(double xParam, double yParam, double width, double height)
		{
			return MakeRectangle((float)xParam, (float)yParam, (float)width, (float)height);
		}

		public static Rectangle MakeRectangle(float xParam, float yParam, float width, float height)
		{
			return new Rectangle(xParam, yParam, width, height);
		}

		public static string ACHIEVEMENT_STRING(string s)
		{
			return s;
		}

		public static void _LOG(string str)
		{
#if DEBUG
			Console.WriteLine($"[LOG] {str}");
#endif // DEBUG
		}

		public static float WVGAH(double H, double L)
		{
			return (float)H;
		}

		public static float RT(double H, double L)
		{
			return (float)L;
		}

		public static float RTD(double V)
		{
			return (float)V;
		}

		public static float RTPD(double V)
		{
			return (float)V;
		}
	}
}
