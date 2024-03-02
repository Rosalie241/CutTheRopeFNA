using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.game
{
	internal class Bouncer : CTRGameObject
	{
		private const float BOUNCER_HEIGHT = 10f;

		public float angle;

		public Vector2 t1;

		public Vector2 t2;

		public Vector2 b1;

		public Vector2 b2;

		public bool skip;

		public virtual NSObject initWithPosXYWidthAndAngle(float px, float py, int w, double an)
		{
			int textureResID = -1;
			switch (w)
			{
			case 1:
				textureResID = IMG_OBJ_BOUNCER_01;
				break;
			case 2:
				textureResID = IMG_OBJ_BOUNCER_02;
				break;
			}
			if (initWithTexture(Application.getTexture(textureResID)) == null)
			{
				return null;
			}
			rotation = (float)an;
			x = px;
			y = py;
			updateRotation();
			int n = addAnimationDelayLoopFirstLast(0.04f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 4);
			Timeline timeline = getTimeline(n);
			timeline.addKeyFrame(KeyFrame.makeSingleAction(this, ACTION_SET_DRAWQUAD, 0, 0, 0.04f));
			return this;
		}

		public override void update(float delta)
		{
			base.update(delta);
			if (mover != null)
			{
				updateRotation();
			}
		}

		public virtual void updateRotation()
		{
			t1.X = x - (float)(width / 2);
			t2.X = x + (float)(width / 2);
			t1.Y = (t2.Y = (float)((double)y - 5.0));
			b1.X = t1.X;
			b2.X = t2.X;
			b1.Y = (b2.Y = (float)((double)y + 5.0));
			angle = MathHelper.DEGREES_TO_RADIANS(rotation);
			t1 = MathHelper.vectRotateAround(t1, angle, x, y);
			t2 = MathHelper.vectRotateAround(t2, angle, x, y);
			b1 = MathHelper.vectRotateAround(b1, angle, x, y);
			b2 = MathHelper.vectRotateAround(b2, angle, x, y);
		}
	}
}
