using System;
using System.Collections.Generic;
using CutTheRope.ctr_commons;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class CTRRootController : RootController
	{
		public const int NEXT_GAME = 0;

		public const int NEXT_MENU = 1;

		public const int NEXT_PICKER = 2;

		public const int NEXT_PICKER_NEXT_PACK = 3;

		public const int NEXT_PICKER_SHOW_UNLOCK = 4;

		public const int CHILD_START = 0;

		public const int CHILD_MENU = 1;

		public const int CHILD_LOADING = 2;

		public const int CHILD_GAME = 3;

		public int pack;

		private string mapName;

		private XMLNode loadedMap;

		private int level;

		private bool picker;

		private bool survival;

		private bool showGreeting;

		public static void logEvent(string s)
		{
#if DEBUG
			Console.WriteLine($"[EVENT] {s}");
#endif // DEBUG
		}

		public virtual void setMap(XMLNode map)
		{
			loadedMap = map;
		}

		public virtual XMLNode getMap()
		{
			return loadedMap;
		}

		public virtual void setMapName(string map)
		{
			mapName = map;
		}

		public virtual int getPack()
		{
			return pack;
		}

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				loadedMap = null;
				ResourceMgr resourceMgr = Application.sharedResourceMgr();
				resourceMgr.initLoading();
				resourceMgr.loadPack(PACK_STARTUP);
				resourceMgr.loadImmediately();
				StartupController startupController = (StartupController)new StartupController().initWithParent(this);
				addChildwithID(startupController, 0);
				viewTransition = -1;
			}
			return this;
		}

		public override void activate()
		{
			CTRPreferences.isFirstLaunch();
			base.activate();
			activateChild(0);
			Application.sharedCanvas().beforeRender();
			activeChild().activeView().draw();
			Application.sharedCanvas().afterRender();
		}

		public virtual void deleteMenu()
		{
			ResourceMgr resourceMgr = Application.sharedResourceMgr();
			deleteChild(1);
			resourceMgr.freePack(PACK_MENU);
			GC.Collect();
		}

		public override void suspend()
		{
			suspended = true;
		}

		public override void resume()
		{
			suspended = false;
		}

		public override void onChildDeactivated(int n)
		{
			base.onChildDeactivated(n);
			ResourceMgr resourceMgr = Application.sharedResourceMgr();
			switch (n)
			{
			case 0:
			{
				setViewTransition(4);
				LoadingController c2 = (LoadingController)new LoadingController().initWithParent(this);
				addChildwithID(c2, 2);
				MenuController menuController2 = (MenuController)new MenuController().initWithParent(this);
				addChildwithID(menuController2, 1);
				deleteChild(0);
				resourceMgr.freePack(PACK_STARTUP);
				menuController2.viewToShow = 0;
				FrameworkTypes._LOG("activate child menu");
				activateChild(1);
				break;
			}
			case 1:
			{
				deleteMenu();
				resourceMgr.resourcesDelegate = (LoadingController)getChild(2);
				int[] array = null;
				switch (pack)
				{
				case 0:
					array = PACK_GAME_01;
					break;
				case 1:
					array = PACK_GAME_02;
					break;
				case 2:
					array = PACK_GAME_03;
					break;
				case 3:
					array = PACK_GAME_04;
					break;
				case 4:
					array = PACK_GAME_05;
					break;
				case 5:
					array = PACK_GAME_06;
					break;
				case 6:
					array = PACK_GAME_07;
					break;
				case 7:
					array = PACK_GAME_08;
					break;
				case 8:
					array = PACK_GAME_09;
					break;
				case 9:
					array = PACK_GAME_10;
					break;
				case 10:
					array = PACK_GAME_11;
					break;
				}
				resourceMgr.initLoading();
				resourceMgr.loadPack(PACK_GAME);
				resourceMgr.loadPack(PACK_GAME_NORMAL);
				resourceMgr.loadPack(array);
				resourceMgr.startLoading();
				LoadingController loadingController3 = (LoadingController)getChild(2);
				loadingController3.nextController = 0;
				activateChild(2);
				break;
			}
			case 2:
			{
				LoadingController loadingController2 = (LoadingController)getChild(2);
				int nextController = loadingController2.nextController;
				switch (nextController)
				{
				case 0:
				{
					setShowGreeting(true);
					GameController c = (GameController)new GameController().initWithParent(this);
					addChildwithID(c, 3);
					activateChild(3);
					break;
				}
				case 1:
				case 2:
				case 3:
				case 4:
				{
					MenuController menuController = (MenuController)new MenuController().initWithParent(this);
					addChildwithID(menuController, 1);
					resourceMgr.freePack(PACK_GAME_COVER_01);
					resourceMgr.freePack(PACK_GAME_COVER_02);
					resourceMgr.freePack(PACK_GAME_COVER_03);
					resourceMgr.freePack(PACK_GAME_COVER_04);
					resourceMgr.freePack(PACK_GAME_COVER_05);
					resourceMgr.freePack(PACK_GAME_COVER_06);
					resourceMgr.freePack(PACK_GAME_COVER_07);
					resourceMgr.freePack(PACK_GAME_COVER_08);
					resourceMgr.freePack(PACK_GAME_COVER_09);
					resourceMgr.freePack(PACK_GAME_COVER_10);
					resourceMgr.freePack(PACK_GAME_COVER_11);
					setViewTransition(4);
					if (nextController == 1)
					{
						menuController.viewToShow = 0;
					}
					if (nextController == 2 || nextController == 4)
					{
						menuController.viewToShow = 6;
					}
					if (nextController == 3)
					{
						menuController.viewToShow = ((pack < CTRPreferences.getPacksCount() - 1) ? 5 : 7);
					}
					activateChild(1);
					if (nextController == 3)
					{
						menuController.showNextPack();
					}
					GC.Collect();
					break;
				}
				}
				break;
			}
			case 3:
			{
				GameController gameController = (GameController)getChild(3);
				int exitCode = gameController.exitCode;
				switch (exitCode)
				{
				case 0:
				case 1:
				case 2:
				{
					deleteChild(3);
					resourceMgr.freePack(PACK_GAME);
					resourceMgr.freePack(PACK_GAME_NORMAL);
					resourceMgr.freePack(PACK_GAME_01);
					resourceMgr.freePack(PACK_GAME_02);
					resourceMgr.freePack(PACK_GAME_03);
					resourceMgr.freePack(PACK_GAME_04);
					resourceMgr.freePack(PACK_GAME_05);
					resourceMgr.freePack(PACK_GAME_06);
					resourceMgr.freePack(PACK_GAME_07);
					resourceMgr.freePack(PACK_GAME_08);
					resourceMgr.freePack(PACK_GAME_09);
					resourceMgr.freePack(PACK_GAME_10);
					resourceMgr.freePack(PACK_GAME_11);
					resourceMgr.resourcesDelegate = (LoadingController)getChild(2);
					resourceMgr.initLoading();
					resourceMgr.loadPack(PACK_MENU);
					resourceMgr.startLoading();
					LoadingController loadingController = (LoadingController)getChild(2);
					switch (exitCode)
					{
					case 0:
						loadingController.nextController = 1;
						break;
					case 1:
						loadingController.nextController = 2;
						break;
					default:
						loadingController.nextController = 3;
						break;
					}
					activateChild(2);
					GC.Collect();
					break;
				}
				}
				break;
			}
			}
		}

		public override void dealloc()
		{
			loadedMap = null;
			mapName = null;
			base.dealloc();
		}

		public virtual void setPack(int p)
		{
			pack = p;
		}

		public virtual void setLevel(int l)
		{
			level = l;
		}

		public virtual int getLevel()
		{
			return level;
		}

		public virtual void setPicker(bool p)
		{
			picker = p;
		}

		public virtual bool isPicker()
		{
			return picker;
		}

		public virtual void setSurvival(bool s)
		{
			survival = s;
		}

		public virtual bool isSurvival()
		{
			return survival;
		}

		public static bool isShowGreeting()
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			return cTRRootController.showGreeting;
		}

		public static void setShowGreeting(bool s)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			cTRRootController.showGreeting = s;
		}

		public static void postAchievementName(string name, string s)
		{
#if DEBUG
			Console.WriteLine($"[ACHIEVEMENT] {s}");
#endif // DEBUG
		}

		public static void postAchievementName(string name)
		{
			// TODO: add a achievement ID -> name function
			// and then call postAchievementName(string, string)
		}

		internal void recreateLoadingController()
		{
			deleteChild(2);
			LoadingController c = (LoadingController)new LoadingController().initWithParent(this);
			addChildwithID(c, 2);
		}
	}
}
