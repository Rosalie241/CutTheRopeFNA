using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;

namespace CutTheRope.game
{
	internal class GameView : View
	{
		public const int VIEW_ELEMENT_GAME_SCENE = 0;

		public const int VIEW_ELEMENT_PAUSE_BUTTON = 1;

		public const int VIEW_ELEMENT_RESTART_BUTTON = 2;

		public const int VIEW_ELEMENT_PAUSE_MENU = 3;

		public const int VIEW_ELEMENT_RESULTS = 4;

		public override NSObject initFullscreen()
		{
			if (base.initFullscreen() == null)
			{
				return null;
			}
			return this;
		}

		public override void show()
		{
			base.show();
		}

		public override void hide()
		{
			base.hide();
		}

		public override void draw()
		{
			Global.MouseCursor.Enable(true);
			int num = childsCount();
			for (int i = 0; i < num; i++)
			{
				BaseElement child = getChild(i);
				if (child != null && child.visible)
				{
					int num2 = i;
					if (num2 == 3)
					{
						OpenGL.glDisable(0);
						OpenGL.glEnable(1);
						OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
						GLDrawer.drawSolidRectWOBorder(0f, 0f, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT, RGBAColor.MakeRGBA(0.1, 0.1, 0.1, 0.5));
						OpenGL.glColor4f(Color.White);
						OpenGL.glEnable(0);
					}
					child.draw();
				}
			}
			GameScene gameScene = (GameScene)getChild(0);
			if ((double)gameScene.dimTime > 0.0)
			{
				float num3 = gameScene.dimTime / 0.15f;
				if (gameScene.restartState == 0)
				{
					num3 = 1f - num3;
				}
				OpenGL.glDisable(0);
				OpenGL.glEnable(1);
				OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
				GLDrawer.drawSolidRectWOBorder(0f, 0f, FrameworkTypes.SCREEN_WIDTH, FrameworkTypes.SCREEN_HEIGHT, RGBAColor.MakeRGBA(1.0, 1.0, 1.0, num3));
				OpenGL.glColor4f(Color.White);
				OpenGL.glEnable(0);
			}
		}
	}
}
