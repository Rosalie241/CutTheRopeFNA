using System;
using CutTheRope.game;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.iframework.helpers
{
	internal class MathHelper : ResDataPhoneFull
	{
		private const int fmSinCosSize = 1024;

		public const double M_PI = Math.PI;

		private const int COHEN_LEFT = 1;

		private const int COHEN_RIGHT = 2;

		private const int COHEN_BOT = 4;

		private const int COHEN_TOP = 8;

		private static Random random_ = new Random();

		private static long ARC4RANDOM_MAX = 4294967296L;

		private static float[] fmSins;

		private static float[] fmCoss;

		public static readonly Vector vectZero = new Vector(0f, 0f);

		public const float vectUndefinedValue = 2.1474836E+09f;

		public static readonly Vector vectUndefined = new Vector(vectUndefinedValue, vectUndefinedValue);

		public static float RND_MINUS1_1
		{
			get
			{
				return (float)((double)arc4random() / (double)ARC4RANDOM_MAX * 2.0 - 1.0);
			}
		}

		public static float RND_0_1
		{
			get
			{
				return (float)((double)arc4random() / (double)ARC4RANDOM_MAX);
			}
		}

		public static int MIN(int a, int b)
		{
			return Math.Min(a, b);
		}

		public static float MIN(float a, float b)
		{
			return Math.Min(a, b);
		}

		public static float MIN(double a, double b)
		{
			return (float)Math.Min(a, b);
		}

		public static int MAX(int a, int b)
		{
			return Math.Max(a, b);
		}

		public static float MAX(float a, float b)
		{
			return Math.Max(a, b);
		}

		public static float MAX(double a, double b)
		{
			return (float)Math.Max(a, b);
		}

		public static int ABS(int a)
		{
			return Math.Abs(a);
		}

		public static float ABS(float a)
		{
			return Math.Abs(a);
		}

		public static float ABS(double a)
		{
			return (float)Math.Abs(a);
		}

		public static int RND(int n)
		{
			return RND_RANGE(0, n);
		}

		public static int RND_RANGE(int n, int m)
		{
			return random_.Next(n, m + 1);
		}

		public static uint arc4random()
		{
			return (uint)random_.Next(int.MinValue, int.MaxValue);
		}

		public static float FIT_TO_BOUNDARIES(double V, double MINV, double MAXV)
		{
			return FIT_TO_BOUNDARIES((float)V, (float)MINV, (float)MAXV);
		}

		public static float FIT_TO_BOUNDARIES(float V, float MINV, float MAXV)
		{
			return Math.Max(Math.Min(V, MAXV), MINV);
		}

		public static float ceil(double value)
		{
			return (float)Math.Ceiling(value);
		}

		public static float round(double value)
		{
			return (float)Math.Round(value);
		}

		public static float cosf(float x)
		{
			return (float)Math.Cos(x);
		}

		public static float sinf(float x)
		{
			return (float)Math.Sin(x);
		}

		public static float tanf(float x)
		{
			return (float)Math.Tan(x);
		}

		public static float acosf(float x)
		{
			return (float)Math.Acos(x);
		}

		public static void fmInit()
		{
			if (fmSins == null)
			{
				fmSins = new float[fmSinCosSize];
				for (int i = 0; i < fmSinCosSize; i++)
				{
					fmSins[i] = (float)Math.Sin((double)(i * 2) * Math.PI / fmSinCosSize);
				}
			}
			if (fmCoss == null)
			{
				fmCoss = new float[fmSinCosSize];
				for (int j = 0; j < fmSinCosSize; j++)
				{
					fmCoss[j] = (float)Math.Cos((double)(j * 2) * Math.PI / fmSinCosSize);
				}
			}
		}

		public static float fmSin(float angle)
		{
			int num = (int)((double)(angle * fmSinCosSize) / Math.PI / 2.0);
			num &= 0x3FF;
			return fmSins[num];
		}

		public static float fmCos(float angle)
		{
			int num = (int)((double)(angle * fmSinCosSize) / Math.PI / 2.0);
			num &= 0x3FF;
			return fmCoss[num];
		}

		public static bool sameSign(float a, float b)
		{
			if (!(a >= 0f) || !(b >= 0f))
			{
				if (a < 0f)
				{
					return b < 0f;
				}
				return false;
			}
			return true;
		}

		public static bool pointInRect(float x, float y, float checkX, float checkY, float checkWidth, float checkHeight)
		{
			if (x >= checkX && x < checkX + checkWidth && y >= checkY)
			{
				return y < checkY + checkHeight;
			}
			return false;
		}

		public static bool rectInRect(float x1l, float y1t, float x1r, float y1b, float x2l, float y2t, float x2r, float y2b)
		{
			if (!(x1l > x2r) && !(x1r < x2l) && !(y1t > y2b))
			{
				return !(y1b < y2t);
			}
			return false;
		}

		public static bool obbInOBB(Vector tl1, Vector tr1, Vector br1, Vector bl1, Vector tl2, Vector tr2, Vector br2, Vector bl2)
		{
			Vector[] array = new Vector[4];
			Vector[] array2 = new Vector[4];
			array[0] = tl1;
			array[1] = tr1;
			array[2] = br1;
			array[3] = bl1;
			array2[0] = tl2;
			array2[1] = tr2;
			array2[2] = br2;
			array2[3] = bl2;
			if (overlaps1Way(array, array2))
			{
				return overlaps1Way(array2, array);
			}
			return false;
		}

		public static float DEGREES_TO_RADIANS(float D)
		{
			return (float)((double)D * Math.PI / 180.0);
		}

		public static float RADIANS_TO_DEGREES(float R)
		{
			return (float)((double)(R * 180f) / Math.PI);
		}

		private static bool overlaps1Way(Vector[] corner, Vector[] other)
		{
			Vector[] array = new Vector[2];
			float[] array2 = new float[2];
			array[0] = vectSub(corner[1], corner[0]);
			array[1] = vectSub(corner[3], corner[0]);
			for (int i = 0; i < 2; i++)
			{
				array[i] = vectDiv(array[i], vectLengthsq(array[i]));
				array2[i] = vectDot(corner[0], array[i]);
			}
			for (int j = 0; j < 2; j++)
			{
				float num = vectDot(other[0], array[j]);
				float num2 = num;
				float num3 = num;
				for (int k = 1; k < 4; k++)
				{
					num = vectDot(other[k], array[j]);
					if (num < num2)
					{
						num2 = num;
					}
					else if (num > num3)
					{
						num3 = num;
					}
				}
				if (num2 > 1f + array2[j] || num3 < array2[j])
				{
					return false;
				}
			}
			return true;
		}

		public static Rectangle rectInRectIntersection(Rectangle r1, Rectangle r2)
		{
			Rectangle result = r2;
			result.x = r2.x - r1.x;
			result.y = r2.y - r1.y;
			if (result.x < 0f)
			{
				result.w += result.x;
				result.x = 0f;
			}
			if (result.x + result.w > r1.w)
			{
				result.w = r1.w - result.x;
			}
			if (result.y < 0f)
			{
				result.h += result.y;
				result.y = 0f;
			}
			if (result.y + result.h > r1.h)
			{
				result.h = r1.h - result.y;
			}
			return result;
		}

		public static float angleTo0_360(float angle)
		{
			float num = angle;
			while (Math.Abs(num) > 360f)
			{
				num -= ((num > 0f) ? 360f : (-360f));
			}
			if (num < 0f)
			{
				num += 360f;
			}
			return num;
		}

		public static Vector vect(float x, float y)
		{
			return new Vector(x, y);
		}

		public static bool vectEqual(Vector v1, Vector v2)
		{
			if (v1.x == v2.x)
			{
				return v1.y == v2.y;
			}
			return false;
		}

		public static Vector vectAdd(Vector v1, Vector v2)
		{
			return new Vector(v1.x + v2.x, v1.y + v2.y);
		}

		public static Vector vectNeg(Vector v)
		{
			return new Vector(0f - v.x, 0f - v.y);
		}

		public static Vector vectSub(Vector v1, Vector v2)
		{
			return new Vector(v1.x - v2.x, v1.y - v2.y);
		}

		public static Vector vectMult(Vector v, double s)
		{
			return vectMult(v, (float)s);
		}

		public static Vector vectMult(Vector v, float s)
		{
			return new Vector(v.x * s, v.y * s);
		}

		public static Vector vectDiv(Vector v, float s)
		{
			return new Vector(v.x / s, v.y / s);
		}

		public static float vectDot(Vector v1, Vector v2)
		{
			return v1.x * v2.x + v1.y * v2.y;
		}

		private static float vectCross(Vector v1, Vector v2)
		{
			return v1.x * v2.y - v1.y * v2.x;
		}

		public static Vector vectPerp(Vector v)
		{
			return new Vector(0f - v.y, v.x);
		}

		public static Vector vectRperp(Vector v)
		{
			return new Vector(v.y, 0f - v.x);
		}

		private static Vector vectProject(Vector v1, Vector v2)
		{
			return vectMult(v2, vectDot(v1, v2) / vectDot(v2, v2));
		}

		private static Vector vectRotateByVector(Vector v1, Vector v2)
		{
			return new Vector(v1.x * v2.x - v1.y * v2.y, v1.x * v2.y + v1.y * v2.x);
		}

		private static Vector vectUnrotateByVector(Vector v1, Vector v2)
		{
			return new Vector(v1.x * v2.x + v1.y * v2.y, v1.y * v2.x - v1.x * v2.y);
		}

		public static float vectAngle(Vector v)
		{
			return (float)Math.Atan(v.y / v.x);
		}

		public static float vectAngleNormalized(Vector v)
		{
			return (float)Math.Atan2(v.y, v.x);
		}

		public static float vectLength(Vector v)
		{
			return (float)Math.Sqrt(vectDot(v, v));
		}

		public static float vectLengthsq(Vector v)
		{
			return vectDot(v, v);
		}

		public static Vector vectNormalize(Vector v)
		{
			return vectMult(v, 1f / vectLength(v));
		}

		public static Vector vectForAngle(float a)
		{
			return new Vector(fmCos(a), fmSin(a));
		}

		public static float vectDistance(Vector v1, Vector v2)
		{
			Vector v3 = vectSub(v1, v2);
			return vectLength(v3);
		}

		public static Microsoft.Xna.Framework.Vector2 vectRotate(Microsoft.Xna.Framework.Vector2 v, double rad)
		{
			float num = fmCos((float)rad);
			float num2 = fmSin((float)rad);
			float xParam = v.X * num - v.Y * num2;
			float yParam = v.X * num2 + v.Y * num;
			return new Microsoft.Xna.Framework.Vector2(xParam, yParam);
		}

		public static Vector vectRotate(Vector v, double rad)
		{
			float num = fmCos((float)rad);
			float num2 = fmSin((float)rad);
			float xParam = v.x * num - v.y * num2;
			float yParam = v.x * num2 + v.y * num;
			return new Vector(xParam, yParam);
		}

		public static Microsoft.Xna.Framework.Vector2 vectRotateAround(Microsoft.Xna.Framework.Vector2 v, double rad, float cx, float cy)
		{
			Microsoft.Xna.Framework.Vector2 v2 = v;
			v2.X -= cx;
			v2.Y -= cy;
			v2 = vectRotate(v2, rad);
			v2.X += cx;
			v2.Y += cy;
			return v2;
		}

		public static Vector vectRotateAround(Vector v, double rad, float cx, float cy)
		{
			Vector v2 = v;
			v2.x -= cx;
			v2.y -= cy;
			v2 = vectRotate(v2, rad);
			v2.x += cx;
			v2.y += cy;
			return v2;
		}

		private static int vcode(float x_min, float y_min, float x_max, float y_max, Vector p)
		{
			return ((p.x < x_min) ? 1 : 0) + ((p.x > x_max) ? 2 : 0) + ((p.y < y_min) ? 4 : 0) + ((p.y > y_max) ? 8 : 0);
		}

		public static bool lineInRect(float x1, float y1, float x2, float y2, float rx, float ry, float w, float h)
		{
			Vector vector = new Vector(x1, y1);
			Vector vector2 = new Vector(x2, y2);
			float num = rx + w;
			float num2 = ry + h;
			int num3 = vcode(rx, ry, num, num2, vector);
			int num4 = vcode(rx, ry, num, num2, vector2);
			bool updateVector1 = true;
			while (num3 != 0 || num4 != 0)
			{
				if ((num3 & num4) != 0)
				{
					return false;
				}
				int num5;
				Vector vector3;
				if (num3 != 0)
				{
					num5 = num3;
					vector3 = vector;
					updateVector1 = true;
				}
				else
				{
					num5 = num4;
					vector3 = vector2;
					updateVector1 = false;
				}
				if (((uint)num5 & 1u) != 0)
				{
					vector3.y += (y1 - y2) * (rx - vector3.x) / (x1 - x2);
					vector3.x = rx;
				}
				else if (((uint)num5 & 2u) != 0)
				{
					vector3.y += (y1 - y2) * (num - vector3.x) / (x1 - x2);
					vector3.x = num;
				}
				if (((uint)num5 & 4u) != 0)
				{
					vector3.x += (x1 - x2) * (ry - vector3.y) / (y1 - y2);
					vector3.y = ry;
				}
				else if (((uint)num5 & 8u) != 0)
				{
					vector3.x += (x1 - x2) * (num2 - vector3.y) / (y1 - y2);
					vector3.y = num2;
				}
				if (num5 == num3)
				{
					num3 = vcode(rx, ry, num, num2, vector);
				}
				else
				{
					num4 = vcode(rx, ry, num, num2, vector2);
				}
				if (updateVector1)
				{
					vector = vector3;
				}
				else
				{
					vector2 = vector3;
				}
			}
			return true;
		}

		public bool lineInLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
		{
			Vector vector = default(Vector);
			vector.x = x3 - x1 + x4 - x2;
			vector.y = y3 - y1 + y4 - y2;
			Vector vector2 = default(Vector);
			vector2.x = x2 - x1;
			vector2.y = y2 - y1;
			Vector vector3 = default(Vector);
			vector3.x = x4 - x3;
			vector3.y = y4 - y3;
			float value = vector2.y * vector3.x - vector3.y * vector2.x;
			float value2 = vector3.x * vector.y - vector3.y * vector.x;
			float value3 = vector2.x * vector.y - vector2.y * vector.x;
			if (Math.Abs(value2) <= Math.Abs(value))
			{
				return Math.Abs(value3) <= Math.Abs(value);
			}
			return false;
		}

		public static float FLOAT_RND_RANGE(int S, int F)
		{
			return (float)RND_RANGE(S * 1000, F * 1000) / 1000f;
		}
	}
}
