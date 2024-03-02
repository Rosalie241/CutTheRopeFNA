using System;
using System.Collections.Generic;
using System.Diagnostics;
using CutTheRope.game;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.platform;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.ctr_commons
{
	internal class CtrRenderer : NSObject
	{
		private static CTRApp gApp = null;

		private static bool gPaused = false;

		public static void onInit(Language language)
		{
			if (gApp != null)
			{
				FrameworkTypes._LOG("Application already created");
				return;
			}
			ResDataPhoneFull.LANGUAGE = language;
			MathHelper.fmInit();
			gApp = new CTRApp();
			gApp.init();
			gApp.applicationDidFinishLaunching();
		}

		public static void onSurfaceChanged(int width, int height)
		{
			FrameworkTypes.VIEW_SCREEN_WIDTH = width;
			FrameworkTypes.VIEW_SCREEN_HEIGHT = FrameworkTypes.SCREEN_HEIGHT * width / FrameworkTypes.SCREEN_WIDTH;
			if (FrameworkTypes.VIEW_SCREEN_HEIGHT > height)
			{
				FrameworkTypes.VIEW_SCREEN_HEIGHT = height;
				FrameworkTypes.VIEW_SCREEN_WIDTH = FrameworkTypes.SCREEN_WIDTH * height / FrameworkTypes.SCREEN_HEIGHT;
			}
			FrameworkTypes.SCREEN_HEIGHT_EXPANDED = FrameworkTypes.SCREEN_HEIGHT * height / FrameworkTypes.VIEW_SCREEN_HEIGHT;
			FrameworkTypes.SCREEN_WIDTH_EXPANDED = FrameworkTypes.SCREEN_WIDTH * width / FrameworkTypes.VIEW_SCREEN_WIDTH;
			FrameworkTypes.SCREEN_OFFSET_Y = (FrameworkTypes.SCREEN_HEIGHT_EXPANDED - FrameworkTypes.SCREEN_HEIGHT) / 2f;
			FrameworkTypes.SCREEN_OFFSET_X = (FrameworkTypes.SCREEN_WIDTH_EXPANDED - FrameworkTypes.SCREEN_WIDTH) / 2f;
		}

		public static void onPause()
		{
			if (!gPaused)
			{
				CTRSoundMgr._pause();
				Application.sharedMovieMgr().pause();
				gPaused = true;
				if (gApp != null)
				{
					gApp.applicationWillResignActive();
				}
			}
		}

		public static void onResume()
		{
			if (gPaused)
			{
				CTRSoundMgr._unpause();
				Application.sharedMovieMgr().resume();
				gPaused = false;
				if (gApp != null)
				{
					gApp.applicationDidBecomeActive();
				}
			}
		}

		public static void onDestroy()
		{
			if (gApp == null)
			{
				FrameworkTypes._LOG("Application already destroyed");
				return;
			}
			Application.sharedSoundMgr().stopAllSounds();
			Application.sharedPreferences().savePreferences();
			gApp = null;
			gPaused = false;
		}

		public static void update(float gameTime)
		{
			if (gApp != null && !gPaused)
			{
				NSTimer.fireTimers(gameTime);
				Application.sharedRootController().performTick(gameTime);
			}
		}

		public static void onDrawFrame()
		{
			OpenGL.glClearColor(Color.Black);
			OpenGL.glClear(0);
			if (gApp != null)
			{
				Application.sharedRootController().performDraw();
			}
		}

		public static float transformX(float x)
		{
			return Global.ScreenSizeManager.TransformViewToGameX(x);
		}

		public static float transformY(float y)
		{
			return Global.ScreenSizeManager.TransformViewToGameY(y);
		}

		public static void onTouch(IList<TouchLocation> touches)
		{
			if (touches.Count > 0)
			{
				Application.sharedCanvas().touchesEndedwithEvent(touches);
				Application.sharedCanvas().touchesBeganwithEvent(touches);
				Application.sharedCanvas().touchesMovedwithEvent(touches);
			}
		}

		public static bool onBackPressed()
		{
			GLCanvas gLCanvas = Application.sharedCanvas();
			if (gLCanvas != null)
			{
				return gLCanvas.backButtonPressed();
			}
			return false;
		}
	}
}
