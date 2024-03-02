using System.Globalization;
using CutTheRope.game;
using CutTheRope.iframework.media;
using CutTheRope.iframework.platform;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using static CutTheRope.iframework.core.ApplicationSettings;

namespace CutTheRope.iframework.core
{
	internal class Application : NSObject
	{
		private static CTRPreferences prefs;

		private static CTRResourceMgr resourceMgr = (CTRResourceMgr)new CTRResourceMgr().init();

		protected static RootController root;

		private static ApplicationSettings appSettings;

		private static GLCanvas _canvas = (GLCanvas)new GLCanvas().init();

		private static SoundMgr soundMgr;

		private static MovieMgr movieMgr;

		public static CTRPreferences sharedPreferences()
		{
			return prefs;
		}

		public static CTRResourceMgr sharedResourceMgr()
		{
			return resourceMgr;
		}

		public static RootController sharedRootController()
		{
			if (root == null)
			{
				root = (CTRRootController)new CTRRootController().initWithParent(null);
			}
			return root;
		}

		public static ApplicationSettings sharedAppSettings()
		{
			return appSettings;
		}

		public static GLCanvas sharedCanvas()
		{
			return _canvas;
		}

		public static SoundMgr sharedSoundMgr()
		{
			if (soundMgr == null)
			{
				soundMgr = new SoundMgr().init();
			}
			return soundMgr;
		}

		public static MovieMgr sharedMovieMgr()
		{
			if (movieMgr == null)
			{
				movieMgr = new MovieMgr();
			}
			return movieMgr;
		}

		public virtual ApplicationSettings createAppSettings()
		{
			return (ApplicationSettings)new ApplicationSettings().init();
		}

		public virtual GLCanvas createCanvas()
		{
			return (GLCanvas)new GLCanvas().init();
		}

		public virtual CTRResourceMgr createResourceMgr()
		{
			return (CTRResourceMgr)new CTRResourceMgr().init();
		}

		public virtual SoundMgr createSoundMgr()
		{
			return new SoundMgr().init();
		}

		public virtual CTRPreferences createPreferences()
		{
			return (CTRPreferences)new CTRPreferences().init();
		}

		public virtual RootController createRootController()
		{
			return (CTRRootController)new CTRRootController().initWithParent(null);
		}

		public virtual void applicationDidFinishLaunching()
		{
			appSettings = createAppSettings();
			prefs = createPreferences();
			string text = sharedPreferences().getStringForKey("PREFS_LOCALE");
			if (text == null || text.Length == 0)
			{
				text = ((CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "ru") ? "ru" :
						((CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "de") ? "de" :
						((!(CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "fr")) ? "en" : "fr")));
			}
			appSettings.setString(AppSettings.APP_SETTING_LOCALE, NSObject.NSS(text));
			updateOrientation();
			root = createRootController();
			soundMgr = createSoundMgr();
			movieMgr = createMovieMgr();
			_canvas.touchDelegate = root;
			root.activate();
		}

		public virtual MovieMgr createMovieMgr()
		{
			return new MovieMgr();
		}

		internal static FontGeneric getFont(int fontResID)
		{
			return (FontGeneric)sharedResourceMgr().loadResource(fontResID, ResourceMgr.ResourceType.FONT);
		}

		internal static Texture2D getTexture(int textureResID)
		{
			return (Texture2D)sharedResourceMgr().loadResource(textureResID, ResourceMgr.ResourceType.IMAGE);
		}

		internal static NSString getString(int strResID)
		{
			return (NSString)sharedResourceMgr().loadResource(strResID, ResourceMgr.ResourceType.STRINGS);
		}

		public virtual void updateOrientation()
		{
			FrameworkTypes.PORTRAIT_SCREEN_WIDTH = 2560f;
			FrameworkTypes.PORTRAIT_SCREEN_HEIGHT = 1440f;
			FrameworkTypes.SCREEN_WIDTH = FrameworkTypes.PORTRAIT_SCREEN_WIDTH;
			FrameworkTypes.SCREEN_HEIGHT = FrameworkTypes.PORTRAIT_SCREEN_HEIGHT;
		}
	}
}
