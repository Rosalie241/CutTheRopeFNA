using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class CTRResourceMgr : ResourceMgr
	{
		private static Dictionary<int, string> resNames_;

		public override NSObject init()
		{
			base.init();
			return this;
		}

		public static int handleLocalizedResource(int r)
		{
			switch (r)
			{
			case IMG_MENU_EXTRA_BUTTONS_EN:
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_RU)
				{
					return IMG_MENU_EXTRA_BUTTONS_RU;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_DE)
				{
					return IMG_MENU_EXTRA_BUTTONS_GR;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_FR)
				{
					return IMG_MENU_EXTRA_BUTTONS_FR;
				}
				break;
			case IMG_HUD_BUTTONS_EN:
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_RU)
				{
					return IMG_HUD_BUTTONS_RU;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_DE)
				{
					return IMG_HUD_BUTTONS_GR;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_FR)
				{
					return IMG_HUD_BUTTONS_EN;
				}
				break;
			case IMG_MENU_RESULT_EN:
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_RU)
				{
					return IMG_MENU_RESULT_RU;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_DE)
				{
					return IMG_MENU_RESULT_GR;
				}
				if (ResDataPhoneFull.LANGUAGE == Language.LANG_FR)
				{
					return IMG_MENU_RESULT_FR;
				}
				break;
			}
			return r;
		}

		public static string XNA_ResName(int resId)
		{
			if (resNames_ == null)
			{
				Dictionary<int, string> dictionary = new Dictionary<int, string>();
				dictionary.Add(IMG_DEFAULT, "zeptolab_no_link");
				dictionary.Add(IMG_LOADERBAR_FULL, "loaderbar_full");
				dictionary.Add(IMG_MENU_BUTTON_DEFAULT, "menu_button_default");
				dictionary.Add(FNT_BIG_FONT, "big_font");
				dictionary.Add(FNT_SMALL_FONT, "small_font");
				dictionary.Add(IMG_MENU_LOADING, "menu_loading");
				dictionary.Add(IMG_MENU_NOTIFICATION, "menu_notification");
				dictionary.Add(IMG_MENU_ACHIEVEMENT, "menu_achievement");
				dictionary.Add(IMG_MENU_OPTIONS, "menu_options");
				dictionary.Add(SND_TAP, "tap");
				dictionary.Add(STR_MENU, "menu_strings");
				dictionary.Add(SND_BUTTON, "button");
				dictionary.Add(SND_BUBBLE_BREAK, "bubble_break");
				dictionary.Add(SND_BUBBLE, "bubble");
				dictionary.Add(SND_CANDY_BREAK, "candy_break");
				dictionary.Add(SND_MONSTER_CHEWING, "monster_chewing");
				dictionary.Add(SND_MONSTER_CLOSE, "monster_close");
				dictionary.Add(SND_MONSTER_OPEN, "monster_open");
				dictionary.Add(SND_MONSTER_SAD, "monster_sad");
				dictionary.Add(SND_RING, "ring");
				dictionary.Add(SND_ROPE_BLEAK_1, "rope_bleak_1");
				dictionary.Add(SND_ROPE_BLEAK_2, "rope_bleak_2");
				dictionary.Add(SND_ROPE_BLEAK_3, "rope_bleak_3");
				dictionary.Add(SND_ROPE_BLEAK_4, "rope_bleak_4");
				dictionary.Add(SND_ROPE_GET, "rope_get");
				dictionary.Add(SND_STAR_1, "star_1");
				dictionary.Add(SND_STAR_2, "star_2");
				dictionary.Add(SND_STAR_3, "star_3");
				dictionary.Add(SND_ELECTRIC, "electric");
				dictionary.Add(SND_PUMP_1, "pump_1");
				dictionary.Add(SND_PUMP_2, "pump_2");
				dictionary.Add(SND_PUMP_3, "pump_3");
				dictionary.Add(SND_PUMP_4, "pump_4");
				dictionary.Add(SND_SPIDER_ACTIVATE, "spider_activate");
				dictionary.Add(SND_SPIDER_FALL, "spider_fall");
				dictionary.Add(SND_SPIDER_WIN, "spider_win");
				dictionary.Add(SND_WHEEL, "wheel");
				dictionary.Add(SND_WIN, "win");
				dictionary.Add(SND_GRAVITY_OFF, "gravity_off");
				dictionary.Add(SND_GRAVITY_ON, "gravity_on");
				dictionary.Add(SND_CANDY_LINK, "candy_link");
				dictionary.Add(SND_BOUNCER, "bouncer");
				dictionary.Add(SND_SPIKE_ROTATE_IN, "spike_rotate_in");
				dictionary.Add(SND_SPIKE_ROTATE_OUT, "spike_rotate_out");
				dictionary.Add(SND_BUZZ, "buzz");
				dictionary.Add(SND_TELEPORT, "teleport");
				dictionary.Add(SND_SCRATCH_IN, "scratch_in");
				dictionary.Add(SND_SCRATCH_OUT, "scratch_out");
				dictionary.Add(IMG_MENU_BGR, "menu_bgr");
				dictionary.Add(IMG_MENU_POPUP, "menu_popup");
				dictionary.Add(IMG_MENU_LOGO, "menu_logo");
				dictionary.Add(IMG_MENU_LEVEL_SELECTION, "menu_level_selection");
				dictionary.Add(IMG_MENU_PACK_SELECTION, "menu_pack_selection");
				dictionary.Add(IMG_MENU_PACK_SELECTION2, "menu_pack_selection2");
				dictionary.Add(IMG_MENU_EXTRA_BUTTONS, "menu_extra_buttons");
				dictionary.Add(IMG_MENU_SCROLLBAR, "menu_scrollbar");
				dictionary.Add(IMG_MENU_LEADERBOARD, "menu_leaderboard");
				dictionary.Add(IMG_MENU_PROCESSING, "menu_processing_hd");
				dictionary.Add(IMG_MENU_SCROLLBAR_CHANGENAME, "menu_scrollbar_changename");
				dictionary.Add(IMG_MENU_BUTTON_ACHIV_CUP, "menu_button_achiv_cup");
				dictionary.Add(IMG_MENU_BGR_SHADOW, "menu_bgr_shadow");
				dictionary.Add(IMG_MENU_BUTTON_SHORT, "menu_button_short");
				dictionary.Add(IMG_HUD_BUTTONS, "hud_buttons");
				dictionary.Add(IMG_OBJ_CANDY_01, "obj_candy_01");
				dictionary.Add(IMG_OBJ_SPIDER, "obj_spider");
				dictionary.Add(IMG_CONFETTI_PARTICLES, "confetti_particles");
				dictionary.Add(IMG_MENU_PAUSE, "menu_pause");
				dictionary.Add(IMG_MENU_RESULT, "menu_result");
				dictionary.Add(FNT_FONT_NUMBERS_BIG, "font_numbers_big");
				dictionary.Add(IMG_HUD_BUTTONS_EN, "hud_buttons_en");
				dictionary.Add(IMG_MENU_RESULT_EN, "menu_result_en");
				dictionary.Add(IMG_OBJ_STAR_DISAPPEAR, "obj_star_disappear");
				dictionary.Add(IMG_OBJ_BUBBLE_FLIGHT, "obj_bubble_flight");
				dictionary.Add(IMG_OBJ_BUBBLE_POP, "obj_bubble_pop");
				dictionary.Add(IMG_OBJ_HOOK_AUTO, "obj_hook_auto");
				dictionary.Add(IMG_OBJ_BUBBLE_ATTACHED, "obj_bubble_attached");
				dictionary.Add(IMG_OBJ_HOOK_01, "obj_hook_01");
				dictionary.Add(IMG_OBJ_HOOK_02, "obj_hook_02");
				dictionary.Add(IMG_OBJ_STAR_IDLE, "obj_star_idle");
				dictionary.Add(IMG_HUD_STAR, "hud_star");
				dictionary.Add(IMG_CHAR_ANIMATIONS, "char_animations");
				dictionary.Add(IMG_OBJ_HOOK_REGULATED, "obj_hook_regulated");
				dictionary.Add(IMG_OBJ_HOOK_MOVABLE, "obj_hook_movable");
				dictionary.Add(IMG_OBJ_PUMP, "obj_pump");
				dictionary.Add(IMG_TUTORIAL_SIGNS, "tutorial_signs");
				dictionary.Add(IMG_OBJ_SOCKS, "obj_hat");
				dictionary.Add(IMG_OBJ_BOUNCER_01, "obj_bouncer_01");
				dictionary.Add(IMG_OBJ_BOUNCER_02, "obj_bouncer_02");
				dictionary.Add(IMG_OBJ_SPIKES_01, "obj_spikes_01");
				dictionary.Add(IMG_OBJ_SPIKES_02, "obj_spikes_02");
				dictionary.Add(IMG_OBJ_SPIKES_03, "obj_spikes_03");
				dictionary.Add(IMG_OBJ_SPIKES_04, "obj_spikes_04");
				dictionary.Add(IMG_OBJ_ELECTRODES, "obj_electrodes");
				dictionary.Add(IMG_OBJ_ROTATABLE_SPIKES_01, "obj_rotatable_spikes_01");
				dictionary.Add(IMG_OBJ_ROTATABLE_SPIKES_02, "obj_rotatable_spikes_02");
				dictionary.Add(IMG_OBJ_ROTATABLE_SPIKES_03, "obj_rotatable_spikes_03");
				dictionary.Add(IMG_OBJ_ROTATABLE_SPIKES_04, "obj_rotatable_spikes_04");
				dictionary.Add(IMG_OBJ_ROTATABLE_SPIKES_BUTTON, "obj_rotatable_spikes_button");
				dictionary.Add(IMG_OBJ_BEE_HD, "obj_bee_hd");
				dictionary.Add(IMG_OBJ_POLLEN_HD, "obj_pollen_hd");
				dictionary.Add(IMG_CHAR_SUPPORTS, "char_supports");
				dictionary.Add(IMG_CHAR_ANIMATIONS2, "char_animations2");
				dictionary.Add(IMG_CHAR_ANIMATIONS3, "char_animations3");
				dictionary.Add(IMG_OBJ_VINIL, "obj_vinil");
				dictionary.Add(IMG_BGR_01_P1, "bgr_01_p1");
				dictionary.Add(IMG_BGR_01_P2, "bgr_01_p2");
				dictionary.Add(IMG_BGR_02_P1, "bgr_02_p1");
				dictionary.Add(IMG_BGR_02_P2, "bgr_02_p2");
				dictionary.Add(IMG_BGR_03_P1, "bgr_03_p1");
				dictionary.Add(IMG_BGR_03_P2, "bgr_03_p2");
				dictionary.Add(IMG_BGR_04_P1, "bgr_04_p1");
				dictionary.Add(IMG_BGR_04_P2, "bgr_04_p2");
				dictionary.Add(IMG_BGR_05_P1, "bgr_05_p1");
				dictionary.Add(IMG_BGR_05_P2, "bgr_05_p2");
				dictionary.Add(IMG_BGR_06_P1, "bgr_06_p1");
				dictionary.Add(IMG_BGR_06_P2, "bgr_06_p2");
				dictionary.Add(IMG_BGR_07_P1, "bgr_07_p1");
				dictionary.Add(IMG_BGR_07_P2, "bgr_07_p2");
				dictionary.Add(IMG_BGR_08_P1, "bgr_08_p1");
				dictionary.Add(IMG_BGR_08_P2, "bgr_08_p2");
				dictionary.Add(IMG_BGR_09_P1, "bgr_09_p1");
				dictionary.Add(IMG_BGR_09_P2, "bgr_09_p2");
				dictionary.Add(IMG_BGR_10_P1, "bgr_10_p1");
				dictionary.Add(IMG_BGR_10_P2, "bgr_10_p2");
				dictionary.Add(IMG_BGR_11_P1, "bgr_11_p1");
				dictionary.Add(IMG_BGR_11_P2, "bgr_11_p2");
				dictionary.Add(IMG_BGR_COVER_01, "bgr_01_cover");
				dictionary.Add(IMG_BGR_COVER_02, "bgr_02_cover");
				dictionary.Add(IMG_BGR_COVER_03, "bgr_03_cover");
				dictionary.Add(IMG_BGR_COVER_04, "bgr_04_cover");
				dictionary.Add(IMG_BGR_COVER_05, "bgr_05_cover");
				dictionary.Add(IMG_BGR_COVER_06, "bgr_06_cover");
				dictionary.Add(IMG_BGR_COVER_07, "bgr_07_cover");
				dictionary.Add(IMG_BGR_COVER_08, "bgr_08_cover");
				dictionary.Add(IMG_BGR_COVER_09, "bgr_09_cover");
				dictionary.Add(IMG_BGR_COVER_10, "bgr_10_cover");
				dictionary.Add(IMG_BGR_COVER_11, "bgr_11_cover");
				dictionary.Add(IMG_MENU_EXTRA_BUTTONS_FR, "menu_extra_buttons_fr");
				dictionary.Add(IMG_MENU_EXTRA_BUTTONS_GR, "menu_extra_buttons_gr");
				dictionary.Add(IMG_MENU_EXTRA_BUTTONS_RU, "menu_extra_buttons_ru");
				dictionary.Add(IMG_HUD_BUTTONS_RU, "hud_buttons_ru");
				dictionary.Add(IMG_HUD_BUTTONS_GR, "hud_buttons_gr");
				dictionary.Add(IMG_MENU_RESULT_RU, "menu_result_ru");
				dictionary.Add(IMG_MENU_RESULT_FR, "menu_result_fr");
				dictionary.Add(IMG_MENU_RESULT_GR, "menu_result_gr");
				dictionary.Add(SND_MENU_MUSIC, "menu_music");
				dictionary.Add(SND_GAME_MUSIC, "game_music");
				dictionary.Add(SND_GAME_MUSIC2, "game_music2");
				dictionary.Add(SND_GAME_MUSIC3, "game_music3");
				dictionary.Add(IMG_MENU_EXTRA_BUTTONS_EN, "menu_extra_buttons_en");
				resNames_ = dictionary;
			}
			string value;
			resNames_.TryGetValue(handleLocalizedResource(resId), out value);
			return value;
		}

		public override NSObject loadResource(int resID, ResourceType resType)
		{
			return base.loadResource(handleLocalizedResource(resID), resType);
		}

		public override void freeResource(int resID)
		{
			base.freeResource(handleLocalizedResource(resID));
		}
	}
}
