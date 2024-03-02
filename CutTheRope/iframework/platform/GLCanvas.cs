using System.Collections.Generic;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.iframework.core;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.platform
{
	internal class GLCanvas : NSObject
	{
		public const int MASTER_WIDTH = 2560;

		public const int MASTER_HEIGHT = 1440;

		private int origWidth;

		private int origHeight;

		public TouchDelegate touchDelegate;

		public bool isFullscreen;

		public float aspect;

		public int touchesCount;

		public int xOffset;

		public int yOffset;

		public int xOffsetScaled;

		public int backingWidth;

		public int backingHeight;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				xOffset = 0;
				yOffset = 0;
				origWidth = (backingWidth = MASTER_WIDTH);
				origHeight = (backingHeight = MASTER_HEIGHT);
				aspect = (float)backingHeight / (float)backingWidth;
				touchesCount = 0;
			}
			return this;
		}

		public virtual void prepareOpenGL()
		{
			OpenGL.glEnableClientState(11);
			OpenGL.glEnableClientState(12);
		}

		public virtual void setDefaultRealProjection()
		{
			setDefaultProjection();
		}

		public virtual void setDefaultProjection()
		{
			if (Global.ScreenSizeManager.IsFullScreen)
			{
				xOffset = Global.ScreenSizeManager.ScaledViewRect.X;
				xOffsetScaled = (int)((double)((float)(-xOffset) * 1f) / Global.ScreenSizeManager.WidthAspectRatio);
				isFullscreen = true;
			}
			else
			{
				xOffset = 0;
				xOffsetScaled = 0;
				isFullscreen = false;
			}
			OpenGL.glViewport(xOffset, yOffset, backingWidth, backingHeight);
			OpenGL.glMatrixMode(15);
			OpenGL.glLoadIdentity();
			OpenGL.glOrthof(0.0, origWidth, origHeight, 0.0, -1.0, 1.0);
			OpenGL.glMatrixMode(14);
			OpenGL.glLoadIdentity();
		}

		public virtual void reshape()
		{
			Microsoft.Xna.Framework.Rectangle scaledViewRect = Global.ScreenSizeManager.ScaledViewRect;
			backingWidth = scaledViewRect.Width;
			backingHeight = scaledViewRect.Height;
			setDefaultProjection();
		}

		public virtual void touchesBeganwithEvent(IList<TouchLocation> touches)
		{
			if (touchDelegate != null)
			{
				touchDelegate.touchesBeganwithEvent(touches);
			}
		}

		public virtual void touchesMovedwithEvent(IList<TouchLocation> touches)
		{
			if (touchDelegate != null)
			{
				touchDelegate.touchesMovedwithEvent(touches);
			}
		}

		public virtual void touchesEndedwithEvent(IList<TouchLocation> touches)
		{
			if (touchDelegate != null)
			{
				touchDelegate.touchesEndedwithEvent(touches);
			}
		}

		public virtual bool backButtonPressed()
		{
			if (touchDelegate != null)
			{
				return touchDelegate.backButtonPressed();
			}
			return false;
		}

		public virtual void beforeRender()
		{
			setDefaultProjection();
			OpenGL.glDisable(1);
			OpenGL.glEnableClientState(11);
			OpenGL.glEnableClientState(12);
		}

		public virtual void afterRender()
		{
		}
	}
}
