using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using CutTheRope.ctr_commons;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.media;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CutTheRope
{
	public class Game1 : Game
	{
		private Dictionary<Keys, bool> keyState = new Dictionary<Keys, bool>();

		private KeyboardState keyboardStateXna;

		private bool _DrawMovie = false;

		private TimeSpan elapsedTime = TimeSpan.Zero;

		private TimeSpan fixWindowElapsedTime = TimeSpan.Zero;

		private bool fixWindowSize = false;

		public Game1()
		{
			Global.XnaGame = this;
			base.Content.RootDirectory = "content";

			base.Window.AllowUserResizing = true;
			base.Window.ClientSizeChanged += Window_ClientSizeChanged;

			Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
			Global.GraphicsDeviceManager.PreferredDepthStencilFormat = DepthFormat.None;
			Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
			
			// set fixed framerate
			// technically we don't need to set this
			// but do it anyways to be explicit
			base.TargetElapsedTime = TimeSpan.FromTicks(166667);
			base.IsFixedTimeStep   = true;

			// load preferences and initialize the window
			// mode here, to ensure we don't need to change
			// it afterwards, which causes issues for wayland
			Preferences._loadPreferences();
			int width = Preferences._getIntForKey("PREFS_WINDOW_WIDTH");
			bool isFullScreen = (width <= 0 || Preferences._getBooleanForKey("PREFS_WINDOW_FULLSCREEN"));
			Global.ScreenSizeManager.Init(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode, width, isFullScreen);

			base.Activated   += Game1_Activated;
			base.Deactivated += Game1_Deactivated;
			base.Exiting     += Game1_Exiting;
		}

		private void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			// signal that we should update
			// the window size and reset
			// the elapsed time
			fixWindowSize        = true;
			fixWindowElapsedTime = TimeSpan.Zero;
		}

		private void Game1_Exiting(object sender, EventArgs e)
		{
			Preferences._savePreferences();
			Preferences.Update();
		}

		private void Game1_Deactivated(object sender, EventArgs e)
		{
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativePause();
		}

		private void Game1_Activated(object sender, EventArgs e)
		{
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeResume();
		}

		protected override void LoadContent()
		{
			Global.GraphicsDevice = base.GraphicsDevice;
			Global.SpriteBatch = new SpriteBatch(base.GraphicsDevice);
			SoundMgr.SetContentManager(base.Content);
			OpenGL.Init();
			Global.MouseCursor.Load(base.Content);
			Global.ScreenSizeManager.InitCanvas();
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeInit(GetSystemLanguage());
			CtrRenderer.onSurfaceCreated();
			CtrRenderer.onSurfaceChanged(Global.ScreenSizeManager.WindowWidth, Global.ScreenSizeManager.WindowHeight);
		}

		private Language GetSystemLanguage()
		{
			Language result = Language.LANG_EN;
			if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru")
			{
				result = Language.LANG_RU;
			}
			if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de")
			{
				result = Language.LANG_DE;
			}
			if (CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")
			{
				result = Language.LANG_FR;
			}
			return result;
		}

		public bool IsKeyPressed(Microsoft.Xna.Framework.Input.Keys key)
		{
			bool value = false;
			keyState.TryGetValue(key, out value);
			bool flag = keyboardStateXna.IsKeyDown(key);
			keyState[key] = flag;
			if (flag)
			{
				return value != flag;
			}
			return false;
		}

		public bool IsKeyDown(Microsoft.Xna.Framework.Input.Keys key)
		{
			return keyboardStateXna.IsKeyDown(key);
		}

		protected override void Update(GameTime gameTime)
		{
			elapsedTime += gameTime.ElapsedGameTime;
			if (elapsedTime > TimeSpan.FromSeconds(1.0))
			{
				elapsedTime -= TimeSpan.FromSeconds(1.0);
				Preferences.Update();
			}

			// don't spam the changed window size,
			// but only do it after 250ms of no
			// resizing, this has a better UX
			if (fixWindowSize)
			{
				fixWindowElapsedTime += gameTime.ElapsedGameTime;
				if (fixWindowElapsedTime > TimeSpan.FromMilliseconds(250))
				{
					Global.ScreenSizeManager.FixWindowSize(base.Window.ClientBounds);
					fixWindowElapsedTime = TimeSpan.Zero;
					fixWindowSize        = false;
				}
			}

			keyboardStateXna = Keyboard.GetState();
			if ((IsKeyPressed(Keys.F11) || 
				((IsKeyDown(Keys.LeftAlt) || 
				  IsKeyDown(Keys.RightAlt)) && 
				  IsKeyPressed(Keys.Enter))))
			{
				Global.ScreenSizeManager.ToggleFullScreen();
				return;
			}

			if (IsKeyPressed(Keys.Escape))
			{
				global::CutTheRope.iframework.core.Application.sharedMovieMgr().stop();
				CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeBackPressed();
			}

			MouseState mouseState = Mouse.GetState();
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(Global.MouseCursor.GetTouchLocation(mouseState));
			global::CutTheRope.iframework.core.Application.sharedRootController().mouseMoved(CtrRenderer.transformX(mouseState.X), CtrRenderer.transformY(mouseState.Y));
			CtrRenderer.update((float)gameTime.ElapsedGameTime.TotalSeconds);
			base.Update(gameTime);
		}

		public void DrawMovie()
		{
			_DrawMovie = true;
			base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
			Texture2D texture = global::CutTheRope.iframework.core.Application.sharedMovieMgr().getTexture();
			if (texture == null)
			{
				return;
			}
			Global.GraphicsDevice.SetRenderTarget(null);
			base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
			Global.ScreenSizeManager.FullScreenCropWidth = false;
			Global.ScreenSizeManager.ApplyViewportToDevice();
			Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(0, 0, base.GraphicsDevice.Viewport.Width, base.GraphicsDevice.Viewport.Height);
			Global.SpriteBatch.Begin();
			Global.SpriteBatch.Draw(texture, destinationRectangle, Microsoft.Xna.Framework.Color.White);
			Global.SpriteBatch.End();
		}

		protected override void Draw(GameTime gameTime)
		{
			base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
			Global.ScreenSizeManager.FullScreenCropWidth = true;
			Global.ScreenSizeManager.ApplyViewportToDevice();
			_DrawMovie = false;
			CtrRenderer.onDrawFrame();
			Global.MouseCursor.Draw();
			Global.GraphicsDevice.SetRenderTarget(null);
			if (!_DrawMovie)
			{
				OpenGL.CopyFromRenderTargetToScreen();
			}
			base.Draw(gameTime);
		}
	}
}
