using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;

namespace CutTheRope.game
{
	internal class Pump : GameObject
	{
		public double angle;

		public Vector t1;

		public Vector t2;

		public float pumpTouchTimer;

		public int pumpTouch;

		public float initial_rotation;

		public float initial_x;

		public float initial_y;

		public RotatedCircle initial_rotatedCircle;

		public static Pump Pump_create(Texture2D t)
		{
			return (Pump)new Pump().initWithTexture(t);
		}

		public static Pump Pump_createWithResID(int r)
		{
			return Pump_create(Application.getTexture(r));
		}

		public virtual void updateRotation()
		{
			t1.x = x - bb.w / 2f;
			t2.x = x + bb.w / 2f;
			t1.y = (t2.y = y);
			angle = MathHelper.DEGREES_TO_RADIANS(rotation);
			t1 = MathHelper.vectRotateAround(t1, angle, x, y);
			t2 = MathHelper.vectRotateAround(t2, angle, x, y);
		}
	}
}
