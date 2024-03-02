using CutTheRope.game;
using CutTheRope.ios;

namespace CutTheRope.iframework.core
{
	internal class ApplicationSettings : NSObject
	{
		public enum AppSettings
		{
			APP_SETTING_LOCALE = 8,
		}

		public virtual NSString getString(AppSettings s)
		{
			if (s == AppSettings.APP_SETTING_LOCALE)
			{
				switch (ResDataPhoneFull.LANGUAGE)
				{
				case Language.LANG_EN:
					return NSObject.NSS("en");
				case Language.LANG_RU:
					return NSObject.NSS("ru");
				case Language.LANG_DE:
					return NSObject.NSS("de");
				case Language.LANG_FR:
					return NSObject.NSS("fr");
				case Language.LANG_ZH:
					return NSObject.NSS("zh");
				case Language.LANG_JA:
					return NSObject.NSS("ja");
				default:
					return NSObject.NSS("en");
				}
			}
			return NSObject.NSS("");
		}

		public virtual void setString(AppSettings s, NSString str)
		{
			if (s == AppSettings.APP_SETTING_LOCALE)
			{
				string locale = str.ToString();
				ResDataPhoneFull.LANGUAGE = Language.LANG_EN;
				if (locale == "ru")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_RU;
				}
				else if (locale == "de")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_DE;
				}
				else if (locale == "fr")
				{
					ResDataPhoneFull.LANGUAGE = Language.LANG_FR;
				}
			}
		}
	}
}
