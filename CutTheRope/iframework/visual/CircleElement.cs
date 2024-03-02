using CutTheRope.iframework.helpers;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.iframework.visual
{
	internal class CircleElement : BaseElement
	{
		public bool solid;

		public int vertextCount;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				vertextCount = 32;
				solid = true;
			}
			return this;
		}

		public override void draw()
		{
			base.preDraw();
			OpenGL.glDisable(0);
			MathHelper.MIN(width, height);
			bool solid2 = solid;
			OpenGL.glEnable(0);
			OpenGL.glColor4f(Color.White);
			base.postDraw();
		}
	}
}
