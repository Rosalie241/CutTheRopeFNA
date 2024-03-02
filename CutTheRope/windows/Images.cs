using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CutTheRope.windows
{
	internal class Images
	{
		private static Dictionary<string, ContentManager> _contentManagers = new Dictionary<string, ContentManager>();

		private static ContentManager getContentManager(string imgName)
		{
			ContentManager value = null;
			_contentManagers.TryGetValue(imgName, out value);
			if (value == null)
			{
				value = new ContentManager(Global.XnaGame.Services, Global.XnaGame.Content.RootDirectory);
				_contentManagers.Add(imgName, value);
			}
			return value;
		}

		public static Texture2D get(string imgName)
		{
			ContentManager contentManager = getContentManager(imgName);
			try
			{
				return contentManager.Load<Texture2D>(imgName);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static void free(string imgName)
		{
			ContentManager contentManager = getContentManager(imgName);
			contentManager.Unload();
		}
	}
}
