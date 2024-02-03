using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;

namespace CutTheRope.iframework.core
{
	internal class View : BaseElement
	{
		public virtual NSObject initFullscreen()
		{
			if (base.init() != null)
			{
				width = (int)FrameworkTypes.SCREEN_WIDTH;
				height = (int)FrameworkTypes.SCREEN_HEIGHT;
			}
			return this;
		}

		public override NSObject init()
		{
			return initFullscreen();
		}

		public override void draw()
		{
			OpenGL.glColor4f(Color.White);
			OpenGL.glEnable(0);
			OpenGL.glEnable(1);
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			base.preDraw();
			base.postDraw();
			OpenGL.glDisable(0);
			OpenGL.glDisable(1);
		}
	}
}
