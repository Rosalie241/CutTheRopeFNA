using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.windows
{
	internal class MouseCursor
	{
		private Texture2D _cursor;

		private Texture2D _cursorActive;

		private MouseState _mouseStateTranformed;

		private int _touchID;

		private bool _enabled;

		public void Enable(bool b)
		{
			_enabled = b;
		}

		public void Load(ContentManager contentManager)
		{
			_cursor       = contentManager.Load<Texture2D>("cursor.png");
			_cursorActive = contentManager.Load<Texture2D>("cursor_active.png");
		}

		public void Draw()
		{
			if (_enabled)
			{
				Texture2D texture2D = ((_mouseStateTranformed.LeftButton == ButtonState.Pressed) ? _cursorActive : _cursor);
				Microsoft.Xna.Framework.Rectangle scaledViewRect = Global.ScreenSizeManager.ScaledViewRect;
				float widthScale  = FrameworkTypes.SCREEN_WIDTH / (float)scaledViewRect.Width;
				float heightScale = FrameworkTypes.SCREEN_HEIGHT / (float)scaledViewRect.Height;
				Global.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
				Global.SpriteBatch.Draw(texture2D,
					new Microsoft.Xna.Framework.Rectangle(_mouseStateTranformed.X, _mouseStateTranformed.Y,
						// TODO: this looks fine but is it accurate to the game?
						(int)(((float)texture2D.Width  / widthScale)  * 1.2), 
						(int)(((float)texture2D.Height / heightScale) * 1.2)), 
					Color.White);
				Global.SpriteBatch.End();
			}
		}

		private static MouseState TransformMouseState(MouseState mouseState)
		{
			return new MouseState(Global.ScreenSizeManager.TransformWindowToViewX(mouseState.X), Global.ScreenSizeManager.TransformWindowToViewY(mouseState.Y), mouseState.ScrollWheelValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
		}

		public List<TouchLocation> GetTouchLocation(MouseState mouseState)
		{
			List<TouchLocation> list = new List<TouchLocation>();
			MouseState mouseStateTranformed = TransformMouseState(mouseState);
			TouchLocation item = default(TouchLocation);
			if (_touchID > 0)
			{
				if (mouseStateTranformed.LeftButton == ButtonState.Pressed)
				{
					item = ((_mouseStateTranformed.LeftButton != ButtonState.Pressed) ? new TouchLocation(++_touchID, TouchLocationState.Pressed, new Vector2(mouseStateTranformed.X, mouseStateTranformed.Y)) : new TouchLocation(_touchID, TouchLocationState.Moved, new Vector2(mouseStateTranformed.X, mouseStateTranformed.Y)));
				}
				else if (_mouseStateTranformed.LeftButton == ButtonState.Pressed)
				{
					item = new TouchLocation(_touchID, TouchLocationState.Released, new Vector2(_mouseStateTranformed.X, _mouseStateTranformed.Y));
				}
			}
			else if (mouseStateTranformed.LeftButton == ButtonState.Pressed)
			{
				item = new TouchLocation(++_touchID, TouchLocationState.Pressed, new Vector2(mouseStateTranformed.X, mouseStateTranformed.Y));
			}
			if (item.State != 0)
			{
				list.Add(item);
			}
			_mouseStateTranformed = mouseStateTranformed;
			return list;
		}
	}
}
