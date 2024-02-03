using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
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
		private Branding branding;

		private Process parentProcess;

		private int mouseState_X;

		private int mouseState_Y;

		private int mouseState_ScrollWheelValue;

		private Microsoft.Xna.Framework.Input.ButtonState mouseState_LeftButton;

		private Microsoft.Xna.Framework.Input.ButtonState mouseState_MiddleButton;

		private Microsoft.Xna.Framework.Input.ButtonState mouseState_RightButton;

		private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton1;

		private Microsoft.Xna.Framework.Input.ButtonState mouseState_XButton2;

		private bool UseWindowMode_TODO_ChangeFullScreenResolution = true; // some bug in startup fullscrenn (NulRefrenceExcepton)

		private Cursor _cursorLast;

		private Dictionary<Microsoft.Xna.Framework.Input.Keys, bool> keyState = new Dictionary<Microsoft.Xna.Framework.Input.Keys, bool>();

		private KeyboardState keyboardStateXna;

		private bool _DrawMovie;

		private int _ignoreMouseClick;

		private int frameRate;

		private int frameCounter;

		private TimeSpan elapsedTime = TimeSpan.Zero;

		private bool bFirstFrame = true;

		private bool IsMinimized
		{
			get
			{
				Form form = WindowAsForm();
				return form.WindowState == FormWindowState.Minimized;
			}
		}

		public Game1()
		{
			Global.XnaGame = this;
			base.Content.RootDirectory = "content";
			Global.GraphicsDeviceManager = new GraphicsDeviceManager(this);
			try
			{
				Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.HiDef;
				Global.GraphicsDeviceManager.ApplyChanges();
			}
			catch (Exception)
			{
				Global.GraphicsDeviceManager.GraphicsProfile = GraphicsProfile.Reach;
				Global.GraphicsDeviceManager.ApplyChanges();
			}
			Global.GraphicsDeviceManager.PreparingDeviceSettings += GraphicsDeviceManager_PreparingDeviceSettings;
			base.TargetElapsedTime = TimeSpan.FromTicks(166666L);
			base.IsFixedTimeStep = false;
			base.InactiveSleepTime = TimeSpan.FromTicks(500000L);
			base.IsMouseVisible = true;
			base.Activated += Game1_Activated;
			base.Deactivated += Game1_Deactivated;
			base.Exiting += Game1_Exiting;
			parentProcess = ParentProcessUtilities.GetParentProcess();
			Form form = WindowAsForm();
			form.MouseMove += form_MouseMove;
			form.MouseUp += form_MouseUp;
			form.MouseDown += form_MouseDown;
		}

		private void form_MouseDown(object sender, MouseEventArgs e)
		{
			mouseState_X = e.X;
			mouseState_Y = e.Y;
			switch (e.Button)
			{
			case MouseButtons.Left:
				mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				break;
			case MouseButtons.Middle:
				mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				break;
			case MouseButtons.Right:
				mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				break;
			case MouseButtons.XButton1:
				mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				break;
			case MouseButtons.XButton2:
				mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Pressed;
				break;
			}
			if (_DrawMovie && e.Button == MouseButtons.Left)
			{
				global::CutTheRope.iframework.core.Application.sharedMovieMgr().stop();
			}
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(Global.MouseCursor.GetTouchLocation());
		}

		private void form_MouseUp(object sender, MouseEventArgs e)
		{
			mouseState_X = e.X;
			mouseState_Y = e.Y;
			switch (e.Button)
			{
			case MouseButtons.Left:
				mouseState_LeftButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
				break;
			case MouseButtons.Middle:
				mouseState_MiddleButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
				break;
			case MouseButtons.Right:
				mouseState_RightButton = Microsoft.Xna.Framework.Input.ButtonState.Released;
				break;
			case MouseButtons.XButton1:
				mouseState_XButton1 = Microsoft.Xna.Framework.Input.ButtonState.Released;
				break;
			case MouseButtons.XButton2:
				mouseState_XButton2 = Microsoft.Xna.Framework.Input.ButtonState.Released;
				break;
			}
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(Global.MouseCursor.GetTouchLocation());
		}

		private void form_MouseMove(object sender, MouseEventArgs e)
		{
			mouseState_X = e.X;
			mouseState_Y = e.Y;
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeTouchProcess(Global.MouseCursor.GetTouchLocation());
		}

		public MouseState GetMouseState()
		{
			return new MouseState(mouseState_X, mouseState_Y, mouseState_ScrollWheelValue, mouseState_LeftButton, mouseState_MiddleButton, mouseState_RightButton, mouseState_XButton1, mouseState_XButton2);
		}

		private void GraphicsDeviceManager_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
		{
			e.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat = DepthFormat.None;
			if (e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Width > ScreenSizeManager.MAX_WINDOW_WIDTH || e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Height > ScreenSizeManager.MAX_WINDOW_WIDTH)
			{
				UseWindowMode_TODO_ChangeFullScreenResolution = true;
			}
		}

		private void form_Resize(object sender, EventArgs e)
		{
			if (Global.ScreenSizeManager.SkipSizeChanges)
			{
				return;
			}
			Form form = WindowAsForm();
			if (form.WindowState == FormWindowState.Maximized)
			{
				form.WindowState = FormWindowState.Normal;
				if (!Global.ScreenSizeManager.IsFullScreen)
				{
					//Global.ScreenSizeManager.ToggleFullScreen();
				}
			}
		}

		public void SetCursor(Cursor cursor, MouseState mouseState)
		{
			if (base.Window.ClientBounds.Contains(base.Window.ClientBounds.X + mouseState.X, base.Window.ClientBounds.Y + mouseState.Y) && _cursorLast != cursor)
			{
				WindowAsForm().Cursor = cursor;
				_cursorLast = cursor;
			}
		}

		private Form WindowAsForm()
		{
			return (Form)Control.FromHandle(base.Window.Handle);
		}

		private void Window_ClientSizeChanged(object sender, EventArgs e)
		{
			base.Window.ClientSizeChanged -= Window_ClientSizeChanged;
			Global.ScreenSizeManager.FixWindowSize(base.Window.ClientBounds);
			base.Window.ClientSizeChanged += Window_ClientSizeChanged;
		}

		private void Game1_Exiting(object sender, EventArgs e)
		{
			Preferences._savePreferences();
			Preferences.Update();
		}

		private void Game1_Deactivated(object sender, EventArgs e)
		{
			_ignoreMouseClick = 60;
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativePause();
		}

		private void Game1_Activated(object sender, EventArgs e)
		{
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeResume();
		}

		protected override void Initialize()
		{
			base.Initialize();
		}

		protected override void LoadContent()
		{
			Global.GraphicsDevice = base.GraphicsDevice;
			Global.SpriteBatch = new SpriteBatch(base.GraphicsDevice);
			SoundMgr.SetContentManager(base.Content);
			OpenGL.Init();
			Global.MouseCursor.Load(base.Content);
			Form form = WindowAsForm();
			if (UseWindowMode_TODO_ChangeFullScreenResolution)
			{
				base.Window.AllowUserResizing = true;
				if (form != null)
				{
					form.MaximizeBox = false;
				}
			}
			else
			{
				base.Window.AllowUserResizing = true;
			}
			Preferences._loadPreferences();
			int num = Preferences._getIntForKey("PREFS_WINDOW_WIDTH");
			bool isFullScreen = !UseWindowMode_TODO_ChangeFullScreenResolution && (num <= 0 || Preferences._getBooleanForKey("PREFS_WINDOW_FULLSCREEN"));
			Global.ScreenSizeManager.Init(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode, num, isFullScreen);
			base.Window.ClientSizeChanged += Window_ClientSizeChanged;
			if (form != null)
			{
				Global.ScreenSizeManager.SetWindowMinimumSize(form);
				form.BackColor = System.Drawing.Color.Black;
				form.Resize += form_Resize;
			}
			CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeInit(GetSystemLanguage());
			CtrRenderer.onSurfaceCreated();
			CtrRenderer.onSurfaceChanged(Global.ScreenSizeManager.WindowWidth, Global.ScreenSizeManager.WindowHeight);
			branding = new Branding();
			branding.LoadSplashScreens();
		}

		protected override void UnloadContent()
		{
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
			/*
			if (parentProcess != null)
			{
				if (parentProcess.HasExited)
				{
					Exit();
				}
				bool createdNew = false;
				try
				{
					new Mutex(false, "Local\\{4A17D560-ABF3-4020-A3B0-1087993D49E1}", out createdNew);
					if (createdNew)
					{
						Exit();
					}
				}
				catch (Exception)
				{
					Exit();
				}
			}
			else
			{
				Exit();
			}
			*/
			elapsedTime += gameTime.ElapsedGameTime;
			if (elapsedTime > TimeSpan.FromSeconds(1.0))
			{
				elapsedTime -= TimeSpan.FromSeconds(1.0);
				frameRate = frameCounter;
				frameCounter = 0;
				Preferences.Update();
			}
			if (IsMinimized)
			{
				return;
			}
			if (frameRate > 0 && frameRate < 50)
			{
				base.IsFixedTimeStep = true;
			}
			else
			{
				base.IsFixedTimeStep = false;
			}
			keyboardStateXna = Keyboard.GetState();
			if ((IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.F11) || ((IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt)) && IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))) && !UseWindowMode_TODO_ChangeFullScreenResolution)
			{
				Global.ScreenSizeManager.ToggleFullScreen();
				Thread.Sleep(500);
				return;
			}
			if (branding != null)
			{
				if (base.IsActive && branding.IsLoaded)
				{
					if (branding.IsFinished)
					{
						branding = null;
					}
					else
					{
						branding.Update(gameTime);
					}
				}
				return;
			}
			if (IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
			{
				global::CutTheRope.iframework.core.Application.sharedMovieMgr().stop();
				CtrRenderer.Java_com_zeptolab_ctr_CtrRenderer_nativeBackPressed();
			}
			MouseState mouseState = windows.MouseCursor.GetMouseState();
			global::CutTheRope.iframework.core.Application.sharedRootController().mouseMoved(CtrRenderer.transformX(mouseState.X), CtrRenderer.transformY(mouseState.Y));
			CtrRenderer.update((float)gameTime.ElapsedGameTime.Milliseconds / 1000f);
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
			if (_ignoreMouseClick > 0)
			{
				_ignoreMouseClick--;
			}
			else
			{
				MouseState mouseState = Global.XnaGame.GetMouseState();
				if (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed && Global.ScreenSizeManager.CurrentSize.Contains(mouseState.X, mouseState.Y))
				{
					global::CutTheRope.iframework.core.Application.sharedMovieMgr().stop();
				}
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
			frameCounter++;
			base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
			if (branding != null)
			{
				if (branding.IsLoaded)
				{
					branding.Draw(gameTime);
					Global.GraphicsDevice.SetRenderTarget(null);
				}
				return;
			}
			Global.ScreenSizeManager.FullScreenCropWidth = true;
			Global.ScreenSizeManager.ApplyViewportToDevice();
			_DrawMovie = false;
			CtrRenderer.onDrawFrame();
			Global.MouseCursor.Draw();
			Global.GraphicsDevice.SetRenderTarget(null);
			if (bFirstFrame)
			{
				base.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Black);
			}
			else if (!_DrawMovie)
			{
				OpenGL.CopyFromRenderTargetToScreen();
			}
			base.Draw(gameTime);
			bFirstFrame = false;
		}
	}
}
