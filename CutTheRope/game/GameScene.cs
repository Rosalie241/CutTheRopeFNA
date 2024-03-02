using System;
using System.Collections.Generic;
using System.Linq;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.sfe;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using static CutTheRope.iframework.core.ApplicationSettings;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.game
{
	internal class GameScene : BaseElement, TimelineDelegate, ButtonDelegate
	{
		private class FingerCut : NSObject
		{
			public Vector start;

			public Vector end;

			public float startSize;

			public float endSize;

			public RGBAColor c;
		}

		private class TutorialText : Text
		{
			public int special;
		}

		private class GameObjectSpecial : CTRGameObject
		{
			public int special;

			private static GameObjectSpecial GameObjectSpecial_create(Texture2D t)
			{
				GameObjectSpecial gameObjectSpecial = new GameObjectSpecial();
				gameObjectSpecial.initWithTexture(t);
				return gameObjectSpecial;
			}

			public static GameObjectSpecial GameObjectSpecial_createWithResIDQuad(int r, int q)
			{
				GameObjectSpecial gameObjectSpecial = GameObjectSpecial_create(Application.getTexture(r));
				gameObjectSpecial.setDrawQuad(q);
				return gameObjectSpecial;
			}
		}

		public const int MAX_TOUCHES = 5;

		public const float DIM_TIMEOUT = 0.15f;

		public const int RESTART_STATE_FADE_IN = 0;

		public const int RESTART_STATE_FADE_OUT = 1;

		public const int S_MOVE_DOWN = 0;

		public const int S_WAIT = 1;

		public const int S_MOVE_UP = 2;

		public const int CAMERA_MOVE_TO_CANDY_PART = 0;

		public const int CAMERA_MOVE_TO_CANDY = 1;

		public const int BUTTON_GRAVITY = 0;

		public const int PARTS_SEPARATE = 0;

		public const int PARTS_DIST = 1;

		public const int PARTS_NONE = 2;

		public const float SCOMBO_TIMEOUT = 0.2f;

		public const int SCUT_SCORE = 10;

		public const int MAX_LOST_CANDIES = 3;

		public const float ROPE_CUT_AT_ONCE_TIMEOUT = 0.1f;

		public const int STAR_RADIUS = 42;

		public const float MOUTH_OPEN_RADIUS = 200f;

		public const int BLINK_SKIP = 3;

		public const float MOUTH_OPEN_TIME = 1f;

		public const float PUMP_TIMEOUT = 0.05f;

		public const int CAMERA_SPEED = 14;

		public const float SOCK_SPEED_K = 0.9f;

		public const int SOCK_COLLISION_Y_OFFSET = 85;

		public const int BUBBLE_RADIUS = 60;

		public const int WHEEL_RADIUS = 110;

		public const int GRAB_MOVE_RADIUS = 65;

		public const int RC_CONTROLLER_RADIUS = 90;

		public const int CANDY_BLINK_INITIAL = 0;

		public const int CANDY_BLINK_STAR = 1;

		public const int TUTORIAL_SHOW_ANIM = 0;

		public const int TUTORIAL_HIDE_ANIM = 1;

		public const int EARTH_NORMAL_ANIM = 0;

		public const int EARTH_UPSIDEDOWN_ANIM = 1;

		private const int CHAR_ANIMATION_IDLE = 0;

		private const int CHAR_ANIMATION_IDLE2 = 1;

		private const int CHAR_ANIMATION_IDLE3 = 2;

		private const int CHAR_ANIMATION_EXCITED = 3;

		private const int CHAR_ANIMATION_PUZZLED = 4;

		private const int CHAR_ANIMATION_FAIL = 5;

		private const int CHAR_ANIMATION_WIN = 6;

		private const int CHAR_ANIMATION_MOUTH_OPEN = 7;

		private const int CHAR_ANIMATION_MOUTH_CLOSE = 8;

		private const int CHAR_ANIMATION_CHEW = 9;

		private const int CHAR_ANIMATION_GREETING = 10;

		private DelayedDispatcher dd;

		public GameSceneDelegate gameSceneDelegate;

		private AnimationsPool aniPool;

		private AnimationsPool staticAniPool;

		private PollenDrawer pollenDrawer;

		private TileMap back;

		private CharAnimations target;

		private Image support;

		private GameObject candy;

		private Image candyMain;

		private Image candyTop;

		private Animation candyBlink;

		private Animation candyBubbleAnimation;

		private Animation candyBubbleAnimationL;

		private Animation candyBubbleAnimationR;

		private ConstraintedPoint star;

		private DynamicArray bungees;

		private DynamicArray razors;

		private DynamicArray spikes;

		private DynamicArray stars;

		private DynamicArray bubbles;

		private DynamicArray pumps;

		private DynamicArray socks;

		private DynamicArray bouncers;

		private DynamicArray rotatedCircles;

		private DynamicArray tutorialImages;

		private DynamicArray tutorials;

		private GameObject candyL;

		private GameObject candyR;

		private ConstraintedPoint starL;

		private ConstraintedPoint starR;

		private Animation blink;

		private bool[] dragging = new bool[5];

		private Vector[] startPos = new Vector[5];

		private Vector[] prevStartPos = new Vector[5];

		private float ropePhysicsSpeed;

		private GameObject candyBubble;

		private GameObject candyBubbleL;

		private GameObject candyBubbleR;

		private Animation[] hudStar = new Animation[3];

		private Camera2D camera;

		private float mapWidth;

		private float mapHeight;

		private bool mouthOpen;

		private bool noCandy;

		private int blinkTimer;

		private int idlesTimer;

		private float mouthCloseTimer;

		private float lastCandyRotateDelta;

		private float lastCandyRotateDeltaL;

		private float lastCandyRotateDeltaR;

		private int special;

		private bool fastenCamera;

		private float savedSockSpeed;

		private Sock targetSock;

		private int ropesCutAtOnce;

		private float ropeAtOnceTimer;

		private bool clickToCut;

		public int starsCollected;

		public int starBonus;

		public int timeBonus;

		public int score;

		public float time;

		public float initialCameraToStarDistance;

		public float dimTime;

		public int restartState;

		public bool animateRestartDim;

		public bool freezeCamera;

		public int cameraMoveMode;

		public bool ignoreTouches;

		public bool nightLevel;

		public bool gravityNormal;

		public ToggleButton gravityButton;

		public int gravityTouchDown;

		public int twoParts;

		public bool noCandyL;

		public bool noCandyR;

		public float partsDist;

		public DynamicArray earthAnims;

		public int tummyTeasers;

		public Vector slastTouch;

		public DynamicArray[] fingerCuts = new DynamicArray[5];

		private static void drawCut(Vector fls, Vector frs, Vector start, Vector end, float startSize, float endSize, RGBAColor c, ref Vector le, ref Vector re)
		{
			Vector v = MathHelper.vectSub(end, start);
			Vector v2 = MathHelper.vectNormalize(v);
			Vector v3 = MathHelper.vectRperp(v2);
			Vector v4 = MathHelper.vectPerp(v2);
			Vector vector = (MathHelper.vectEqual(frs, MathHelper.vectUndefined) ? MathHelper.vectAdd(start, MathHelper.vectMult(v3, startSize)) : frs);
			Vector vector2 = (MathHelper.vectEqual(fls, MathHelper.vectUndefined) ? MathHelper.vectAdd(start, MathHelper.vectMult(v4, startSize)) : fls);
			Vector vector3 = MathHelper.vectAdd(end, MathHelper.vectMult(v3, endSize));
			Vector vector4 = MathHelper.vectAdd(end, MathHelper.vectMult(v4, endSize));
			float[] vertices = new float[8] { vector2.x, vector2.y, vector.x, vector.y, vector3.x, vector3.y, vector4.x, vector4.y };
			GLDrawer.drawSolidPolygonWOBorder(vertices, 4, c);
			le = vector4;
			re = vector3;
		}

		private static float maxOf4(float v1, float v2, float v3, float v4)
		{
			if (v1 >= v2 && v1 >= v3 && v1 >= v4)
			{
				return v1;
			}
			if (v2 >= v1 && v2 >= v3 && v2 >= v4)
			{
				return v2;
			}
			if (v3 >= v2 && v3 >= v1 && v3 >= v4)
			{
				return v3;
			}
			if (v4 >= v2 && v4 >= v3 && v4 >= v1)
			{
				return v4;
			}
			return -1f;
		}

		private static float minOf4(float v1, float v2, float v3, float v4)
		{
			if (v1 <= v2 && v1 <= v3 && v1 <= v4)
			{
				return v1;
			}
			if (v2 <= v1 && v2 <= v3 && v2 <= v4)
			{
				return v2;
			}
			if (v3 <= v2 && v3 <= v1 && v3 <= v4)
			{
				return v3;
			}
			if (v4 <= v2 && v4 <= v3 && v4 <= v1)
			{
				return v4;
			}
			return -1f;
		}

		public static ToggleButton createGravityButtonWithDelegate(ButtonDelegate d)
		{
			Image u = Image.Image_createWithResIDQuad(IMG_OBJ_STAR_IDLE, 56);
			Image d2 = Image.Image_createWithResIDQuad(IMG_OBJ_STAR_IDLE, 56);
			Image u2 = Image.Image_createWithResIDQuad(IMG_OBJ_STAR_IDLE, 57);
			Image d3 = Image.Image_createWithResIDQuad(IMG_OBJ_STAR_IDLE, 57);
			ToggleButton toggleButton = new ToggleButton().initWithUpElement1DownElement1UpElement2DownElement2andID(u, d2, u2, d3, 0);
			toggleButton.delegateButtonDelegate = d;
			return toggleButton;
		}

		public virtual bool pointOutOfScreen(ConstraintedPoint p)
		{
			if (!(p.pos.y > mapHeight + 400f))
			{
				return p.pos.y < -400f;
			}
			return true;
		}

		public override NSObject init()
		{
			if (base.init() != null)
			{
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				dd = (DelayedDispatcher)new DelayedDispatcher().init();
				initialCameraToStarDistance = -1f;
				restartState = -1;
				aniPool = (AnimationsPool)new AnimationsPool().init();
				aniPool.visible = false;
				addChild(aniPool);
				staticAniPool = (AnimationsPool)new AnimationsPool().init();
				staticAniPool.visible = false;
				addChild(staticAniPool);
				camera = new Camera2D().initWithSpeedandType(14f, CAMERA_TYPE.CAMERA_SPEED_DELAY);
				int textureResID = 104 + cTRRootController.getPack() * 2;
				back = new TileMap().initWithRowsColumns(1, 1);
				back.setRepeatHorizontally(TileMap.Repeat.REPEAT_NONE);
				back.setRepeatVertically(TileMap.Repeat.REPEAT_ALL);
				back.addTileQuadwithID(Application.getTexture(textureResID), 0, 0);
				back.fillStartAtRowColumnRowsColumnswithTile(0, 0, 1, 1, 0);
				if (base.canvas.isFullscreen)
				{
					back.scaleX = (float)Global.ScreenSizeManager.ScreenWidth / (float)base.canvas.backingWidth;
				}
				back.scaleX *= 1.25f;
				back.scaleY *= 1.25f;
				for (int i = 0; i < 3; i++)
				{
					hudStar[i] = Animation.Animation_createWithResID(IMG_HUD_STAR);
					hudStar[i].doRestoreCutTransparency();
					hudStar[i].addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 10);
					hudStar[i].setPauseAtIndexforAnimation(10, 0);
					hudStar[i].x = hudStar[i].width * i + base.canvas.xOffsetScaled;
					hudStar[i].y = 0f;
					addChild(hudStar[i]);
				}
				for (int j = 0; j < MAX_TOUCHES; j++)
				{
					fingerCuts[j] = (DynamicArray)new DynamicArray().init();
				}
				clickToCut = Preferences._getBooleanForKey("PREFS_CLICK_TO_CUT");
			}
			return this;
		}

		public virtual void xmlLoaderFinishedWithfromwithSuccess(XMLNode rootNode, string url, bool success)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			cTRRootController.setMap(rootNode);
			if (animateRestartDim)
			{
				animateLevelRestart();
			}
			else
			{
				restart();
			}
		}

		public virtual void reload()
		{
			dd.cancelAllDispatches();
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			if (cTRRootController.isPicker())
			{
				xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("mappicker://reload"), "mappicker://reload", true);
				return;
			}
			int pack = cTRRootController.getPack();
			int level = cTRRootController.getLevel();
			xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("maps/" + LevelsList.LEVEL_NAMES[pack, level]), "maps/" + LevelsList.LEVEL_NAMES[pack, level], true);
		}

		public virtual void loadNextMap()
		{
			dd.cancelAllDispatches();
			initialCameraToStarDistance = -1f;
			animateRestartDim = false;
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			if (cTRRootController.isPicker())
			{
				xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("mappicker://next"), "mappicker://next", true);
				return;
			}
			int pack = cTRRootController.getPack();
			int level = cTRRootController.getLevel();
			if (level < CTRPreferences.getLevelsInPackCount() - 1)
			{
				cTRRootController.setLevel(++level);
				cTRRootController.setMapName(LevelsList.LEVEL_NAMES[pack, level]);
				xmlLoaderFinishedWithfromwithSuccess(XMLNode.parseXML("maps/" + LevelsList.LEVEL_NAMES[pack, level]), "maps/" + LevelsList.LEVEL_NAMES[pack, level], true);
			}
		}

		public virtual void restart()
		{
			hide();
			show();
		}

		public virtual void createEarthImageWithOffsetXY(float xs, float ys)
		{
			Image image = Image.Image_createWithResIDQuad(IMG_OBJ_STAR_IDLE, 58);
			image.anchor = 18;
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeRotation(0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(180.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.3));
			image.addTimelinewithID(timeline, 1);
			timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeRotation(180.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.3));
			image.addTimelinewithID(timeline, 0);
			Image.setElementPositionWithQuadOffset(image, IMG_BGR_08_P1, 1);
			image.scaleX = 0.8f;
			image.scaleY = 0.8f;
			image.x += xs;
			image.y += ys;
			earthAnims.addObject(image);
		}

		public virtual bool shouldSkipTutorialElement(XMLNode c)
		{
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			if (cTRRootController.getPack() == 0 && cTRRootController.getLevel() == 1)
			{
				return true;
			}
			NSString @string = Application.sharedAppSettings().getString(AppSettings.APP_SETTING_LOCALE);
			NSString nSString = c["locale"];
			if (@string.isEqualToString("en") ||
				@string.isEqualToString("ru") ||
				@string.isEqualToString("de") ||
				@string.isEqualToString("fr"))
			{
				if (!nSString.isEqualToString(@string))
				{
					return true;
				}
			}
			else if (!nSString.isEqualToString("en"))
			{
				return true;
			}
			return false;
		}

		public virtual void showGreeting()
		{
			target.playAnimationtimeline(101, 10);
		}

		public override void show()
		{
			CTRSoundMgr.EnableLoopedSounds(true);
			aniPool.removeAllChilds();
			staticAniPool.removeAllChilds();
			gravityButton = null;
			gravityTouchDown = -1;
			twoParts = 2;
			partsDist = 0f;
			targetSock = null;
			CTRSoundMgr._stopLoopedSounds();
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			XMLNode map = cTRRootController.getMap();
			bungees = (DynamicArray)new DynamicArray().init();
			razors = (DynamicArray)new DynamicArray().init();
			this.spikes = (DynamicArray)new DynamicArray().init();
			stars = (DynamicArray)new DynamicArray().init();
			bubbles = (DynamicArray)new DynamicArray().init();
			pumps = (DynamicArray)new DynamicArray().init();
			socks = (DynamicArray)new DynamicArray().init();
			tutorialImages = (DynamicArray)new DynamicArray().init();
			tutorials = (DynamicArray)new DynamicArray().init();
			bouncers = (DynamicArray)new DynamicArray().init();
			rotatedCircles = (DynamicArray)new DynamicArray().init();
			pollenDrawer = (PollenDrawer)new PollenDrawer().init();
			this.star = (ConstraintedPoint)new ConstraintedPoint().init();
			this.star.setWeight(1f);
			starL = (ConstraintedPoint)new ConstraintedPoint().init();
			starL.setWeight(1f);
			starR = (ConstraintedPoint)new ConstraintedPoint().init();
			starR.setWeight(1f);
			candy = GameObject.GameObject_createWithResIDQuad(IMG_OBJ_CANDY_01, 0);
			candy.doRestoreCutTransparency();
			candy.anchor = 18;
			candy.bb = FrameworkTypes.MakeRectangle(142f, 157f, 112f, 104f);
			candy.passTransformationsToChilds = false;
			candy.scaleX = (candy.scaleY = 0.71f);
			candyMain = GameObject.GameObject_createWithResIDQuad(IMG_OBJ_CANDY_01, 1);
			candyMain.doRestoreCutTransparency();
			candyMain.anchor = (candyMain.parentAnchor = 18);
			candy.addChild(candyMain);
			candyMain.scaleX = (candyMain.scaleY = 0.71f);
			candyTop = GameObject.GameObject_createWithResIDQuad(IMG_OBJ_CANDY_01, 2);
			candyTop.doRestoreCutTransparency();
			candyTop.anchor = (candyTop.parentAnchor = 18);
			candy.addChild(candyTop);
			candyTop.scaleX = (candyTop.scaleY = 0.71f);
			candyBlink = Animation.Animation_createWithResID(IMG_OBJ_CANDY_01);
			candyBlink.addAnimationWithIDDelayLoopFirstLast(0, 0.07f, Timeline.LoopType.TIMELINE_NO_LOOP, 8, 17);
			candyBlink.addAnimationWithIDDelayLoopCountSequence(1, 0.3f, Timeline.LoopType.TIMELINE_NO_LOOP, 2, 18, new List<int> { 18 });
			Timeline timeline = candyBlink.getTimeline(1);
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
			candyBlink.visible = false;
			candyBlink.anchor = (candyBlink.parentAnchor = 18);
			candyBlink.scaleX = (candyBlink.scaleY = 0.71f);
			candy.addChild(candyBlink);
			candyBubbleAnimation = Animation.Animation_createWithResID(IMG_OBJ_BUBBLE_FLIGHT);
			candyBubbleAnimation.x = candy.x;
			candyBubbleAnimation.y = candy.y;
			candyBubbleAnimation.parentAnchor = (candyBubbleAnimation.anchor = 18);
			candyBubbleAnimation.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_REPLAY, 0, 12);
			candyBubbleAnimation.playTimeline(0);
			candy.addChild(candyBubbleAnimation);
			candyBubbleAnimation.visible = false;
			float num = 3f;
			float num2 = 0f;
			float num3 = 0f;
			for (int i = 0; i < 3; i++)
			{
				Timeline timeline2 = hudStar[i].getCurrentTimeline();
				if (timeline2 != null)
				{
					timeline2.stopTimeline();
				}
				hudStar[i].setDrawQuad(0);
			}
			int num4 = 0;
			int num5 = 0;
			List<XMLNode> list = map.childs();
			foreach (XMLNode item in list)
			{
				List<XMLNode> list2 = item.childs();
				foreach (XMLNode item2 in list2)
				{
					if (item2.Name == "map")
					{
						mapWidth = item2["width"].floatValue();
						mapHeight = item2["height"].floatValue();
						num3 = (2560f - mapWidth * num) / 2f;
						mapWidth *= num;
						mapHeight *= num;
						if (cTRRootController.getPack() == 7)
						{
							earthAnims = (DynamicArray)new DynamicArray().init();
							if (mapWidth > FrameworkTypes.SCREEN_WIDTH)
							{
								createEarthImageWithOffsetXY(back.width, 0f);
							}
							if (mapHeight > FrameworkTypes.SCREEN_HEIGHT)
							{
								createEarthImageWithOffsetXY(0f, back.height);
							}
							createEarthImageWithOffsetXY(0f, 0f);
						}
					}
					else if (item2.Name == "gameDesign")
					{
						num4 = item2["mapOffsetX"].intValue();
						num5 = item2["mapOffsetY"].intValue();
						special = item2["special"].intValue();
						ropePhysicsSpeed = item2["ropePhysicsSpeed"].floatValue();
						nightLevel = item2["nightLevel"].isEqualToString("true");
						twoParts = ((!item2["twoParts"].isEqualToString("true")) ? 2 : 0);
						ropePhysicsSpeed *= 1.4f;
					}
					else if (item2.Name == "candyL")
					{
						starL.pos.x = (float)item2["x"].intValue() * num + num3 + (float)num4;
						starL.pos.y = (float)item2["y"].intValue() * num + num2 + (float)num5;
						candyL = GameObject.GameObject_createWithResIDQuad(IMG_OBJ_CANDY_01, 19);
						candyL.scaleX = (candyL.scaleY = 0.71f);
						candyL.passTransformationsToChilds = false;
						candyL.doRestoreCutTransparency();
						candyL.anchor = 18;
						candyL.x = starL.pos.x;
						candyL.y = starL.pos.y;
						candyL.bb = FrameworkTypes.MakeRectangle(155.0, 176.0, 88.0, 76.0);
					}
					else if (item2.Name == "candyR")
					{
						starR.pos.x = (float)item2["x"].intValue() * num + num3 + (float)num4;
						starR.pos.y = (float)item2["y"].intValue() * num + num2 + (float)num5;
						candyR = GameObject.GameObject_createWithResIDQuad(IMG_OBJ_CANDY_01, 20);
						candyR.scaleX = (candyR.scaleY = 0.71f);
						candyR.passTransformationsToChilds = false;
						candyR.doRestoreCutTransparency();
						candyR.anchor = 18;
						candyR.x = starR.pos.x;
						candyR.y = starR.pos.y;
						candyR.bb = FrameworkTypes.MakeRectangle(155.0, 176.0, 88.0, 76.0);
					}
					else if (item2.Name == "candy")
					{
						this.star.pos.x = (float)item2["x"].intValue() * num + num3 + (float)num4;
						this.star.pos.y = (float)item2["y"].intValue() * num + num2 + (float)num5;
					}
				}
			}
			foreach (XMLNode item3 in list)
			{
				List<XMLNode> list3 = item3.childs();
				foreach (XMLNode item4 in list3)
				{
					if (item4.Name == "gravitySwitch")
					{
						gravityButton = createGravityButtonWithDelegate(this);
						gravityButton.visible = false;
						gravityButton.touchable = false;
						addChild(gravityButton);
						gravityButton.x = (float)item4["x"].intValue() * num + num3 + (float)num4;
						gravityButton.y = (float)item4["y"].intValue() * num + num2 + (float)num5;
						gravityButton.anchor = 18;
					}
					else if (item4.Name == "star")
					{
						Star star = Star.Star_createWithResID(IMG_OBJ_STAR_IDLE);
						star.x = (float)item4["x"].intValue() * num + num3 + (float)num4;
						star.y = (float)item4["y"].intValue() * num + num2 + (float)num5;
						star.timeout = item4["timeout"].floatValue();
						star.createAnimations();
						star.bb = FrameworkTypes.MakeRectangle(70.0, 64.0, 82.0, 82.0);
						star.parseMover(item4);
						star.update(0f);
						stars.addObject(star);
					}
					else if (item4.Name == "tutorialText")
					{
						if (!shouldSkipTutorialElement(item4))
						{
							TutorialText tutorialText = (TutorialText)new TutorialText().initWithFont(Application.getFont(FNT_SMALL_FONT));
							tutorialText.color = RGBAColor.MakeRGBA(1.0, 1.0, 1.0, 0.9);
							tutorialText.x = (float)item4["x"].intValue() * num + num3 + (float)num4;
							tutorialText.y = (float)item4["y"].intValue() * num + num2 + (float)num5;
							tutorialText.special = item4["special"].intValue();
							tutorialText.setAlignment(2);
							NSString newString = item4["text"];
							tutorialText.setStringandWidth(newString, (float)item4["width"].intValue() * num);
							tutorialText.color = RGBAColor.transparentRGBA;
							float num6 = ((tutorialText.special == 3) ? 12f : 0f);
							Timeline timeline3 = new Timeline().initWithMaxKeyFramesOnTrack(4);
							timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num6));
							timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
							if (cTRRootController.getPack() == 0 && cTRRootController.getLevel() == 0)
							{
								timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 10.0));
							}
							else
							{
								timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 5.0));
							}
							timeline3.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
							tutorialText.addTimelinewithID(timeline3, 0);
							if (tutorialText.special == 0 || tutorialText.special == 3)
							{
								tutorialText.playTimeline(0);
							}
							tutorials.addObject(tutorialText);
						}
					}
					else if (item4.Name == "tutorial01" || item4.Name == "tutorial02" || item4.Name == "tutorial03" || item4.Name == "tutorial04" || item4.Name == "tutorial05" || item4.Name == "tutorial06" || item4.Name == "tutorial07" || item4.Name == "tutorial08" || item4.Name == "tutorial09" || item4.Name == "tutorial10" || item4.Name == "tutorial11")
					{
						if (shouldSkipTutorialElement(item4))
						{
							continue;
						}
						NSString nSString = new NSString(item4.Name.Substring(8));
						int q = nSString.intValue() - 1;
						GameObjectSpecial gameObjectSpecial = GameObjectSpecial.GameObjectSpecial_createWithResIDQuad(84, q);
						gameObjectSpecial.color = RGBAColor.transparentRGBA;
						gameObjectSpecial.x = (float)item4["x"].intValue() * num + num3 + (float)num4;
						gameObjectSpecial.y = (float)item4["y"].intValue() * num + num2 + (float)num5;
						gameObjectSpecial.rotation = item4["angle"].intValue();
						gameObjectSpecial.special = item4["special"].intValue();
						gameObjectSpecial.parseMover(item4);
						float num7 = ((gameObjectSpecial.special == 3 || gameObjectSpecial.special == 4) ? 12f : 0f);
						Timeline timeline4 = new Timeline().initWithMaxKeyFramesOnTrack(4);
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, num7));
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
						if (cTRRootController.getPack() == 0 && cTRRootController.getLevel() == 0)
						{
							timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 10.0));
						}
						else
						{
							timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 5.2));
						}
						timeline4.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
						gameObjectSpecial.addTimelinewithID(timeline4, 0);
						if (gameObjectSpecial.special == 0 || gameObjectSpecial.special == 3)
						{
							gameObjectSpecial.playTimeline(0);
						}
						if (gameObjectSpecial.special == 2 || gameObjectSpecial.special == 4)
						{
							Timeline timeline5 = new Timeline().initWithMaxKeyFramesOnTrack(12);
							for (int j = 0; j < 2; j++)
							{
								timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, (j == 1) ? 0f : num7));
								timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
								timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
								timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.1));
								timeline5.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
								timeline5.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_IMMEDIATE, (j == 1) ? 0f : num7));
								timeline5.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
								timeline5.addKeyFrame(KeyFrame.makePos(gameObjectSpecial.x, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
								timeline5.addKeyFrame(KeyFrame.makePos((double)gameObjectSpecial.x + 230.0, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 0.5));
								timeline5.addKeyFrame(KeyFrame.makePos((double)gameObjectSpecial.x + 440.0, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.5));
								timeline5.addKeyFrame(KeyFrame.makePos((double)gameObjectSpecial.x + 440.0, gameObjectSpecial.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.6));
							}
							timeline5.setTimelineLoopType(Timeline.LoopType.TIMELINE_NO_LOOP);
							gameObjectSpecial.addTimelinewithID(timeline5, 1);
							gameObjectSpecial.playTimeline(1);
							gameObjectSpecial.rotation = 10f;
						}
						tutorialImages.addObject(gameObjectSpecial);
					}
					else if (item4.Name == "bubble")
					{
						int q2 = MathHelper.RND_RANGE(1, 3);
						Bubble bubble = Bubble.Bubble_createWithResIDQuad(75, q2);
						bubble.doRestoreCutTransparency();
						bubble.bb = FrameworkTypes.MakeRectangle(48.0, 48.0, 152.0, 152.0);
						bubble.initial_x = (bubble.x = (float)item4["x"].intValue() * num + num3 + (float)num4);
						bubble.initial_y = (bubble.y = (float)item4["y"].intValue() * num + num2 + (float)num5);
						bubble.initial_rotation = 0f;
						bubble.initial_rotatedCircle = null;
						bubble.anchor = 18;
						bubble.popped = false;
						Image image = Image.Image_createWithResIDQuad(IMG_OBJ_BUBBLE_ATTACHED, 0);
						image.doRestoreCutTransparency();
						image.parentAnchor = (image.anchor = 18);
						bubble.addChild(image);
						bubbles.addObject(bubble);
					}
					else if (item4.Name == "pump")
					{
						Pump pump = Pump.Pump_createWithResID(IMG_OBJ_PUMP);
						pump.doRestoreCutTransparency();
						pump.addAnimationWithDelayLoopedCountSequence(0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 4, 1, new List<int> { 2, 3, 0 });
						pump.bb = FrameworkTypes.MakeRectangle(300f, 300f, 175f, 175f);
						pump.initial_x = (pump.x = (float)item4["x"].intValue() * num + num3 + (float)num4);
						pump.initial_y = (pump.y = (float)item4["y"].intValue() * num + num2 + (float)num5);
						pump.initial_rotation = 0f;
						pump.initial_rotatedCircle = null;
						pump.rotation = item4["angle"].floatValue() + 90f;
						pump.updateRotation();
						pump.anchor = 18;
						pumps.addObject(pump);
					}
					else if (item4.Name == "sock")
					{
						Sock sock = Sock.Sock_createWithResID(IMG_OBJ_SOCKS);
						sock.createAnimations();
						sock.scaleX = (sock.scaleY = 0.7f);
						sock.doRestoreCutTransparency();
						sock.x = (float)item4["x"].intValue() * num + num3 + (float)num4;
						sock.y = (float)item4["y"].intValue() * num + num2 + (float)num5;
						sock.group = item4["group"].intValue();
						sock.anchor = 10;
						sock.rotationCenterY -= (float)sock.height / 2f - 85f;
						if (sock.group == 0)
						{
							sock.setDrawQuad(0);
						}
						else
						{
							sock.setDrawQuad(1);
						}
						sock.state = Sock.SOCK_IDLE;
						sock.parseMover(item4);
						sock.rotation += 90f;
						if (sock.mover != null)
						{
							sock.mover.angle_ += 90.0;
							sock.mover.angle_initial = sock.mover.angle_;
							if (cTRRootController.getPack() == 3 && cTRRootController.getLevel() == 24)
							{
								sock.mover.use_angle_initial = true;
							}
						}
						sock.updateRotation();
						socks.addObject(sock);
					}
					else if (item4.Name == "spike1" || item4.Name == "spike2" || item4.Name == "spike3" || item4.Name == "spike4" || item4.Name == "electro")
					{
						float px = (float)item4["x"].intValue() * num + num3 + (float)num4;
						float py = (float)item4["y"].intValue() * num + num2 + (float)num5;
						int w = item4["size"].intValue();
						double an = item4["angle"].intValue();
						NSString nSString2 = item4["toggled"];
						int num8 = -1;
						if (nSString2.length() > 0)
						{
							num8 = (nSString2.isEqualToString("false") ? (-1) : nSString2.intValue());
						}
						Spikes spikes = (Spikes)new Spikes().initWithPosXYWidthAndAngleToggled(px, py, w, an, num8);
						spikes.parseMover(item4);
						if (num8 != 0)
						{
							spikes.delegateRotateAllSpikesWithID = rotateAllSpikesWithID;
						}
						if (item4.Name == "electro")
						{
							spikes.electro = true;
							spikes.initialDelay = item4["initialDelay"].floatValue();
							spikes.onTime = item4["onTime"].floatValue();
							spikes.offTime = item4["offTime"].floatValue();
							spikes.electroTimer = 0f;
							spikes.turnElectroOff();
							spikes.electroTimer += spikes.initialDelay;
							spikes.updateRotation();
						}
						else
						{
							spikes.electro = false;
						}
						this.spikes.addObject(spikes);
					}
					else if (item4.Name == "rotatedCircle")
					{
						float num9 = (float)item4["x"].intValue() * num + num3 + (float)num4;
						float num10 = (float)item4["y"].intValue() * num + num2 + (float)num5;
						float num11 = item4["size"].intValue();
						float d = item4["handleAngle"].intValue();
						bool hasOneHandle = item4["oneHandle"].boolValue();
						RotatedCircle rotatedCircle = (RotatedCircle)new RotatedCircle().init();
						rotatedCircle.anchor = 18;
						rotatedCircle.x = num9;
						rotatedCircle.y = num10;
						rotatedCircle.rotation = d;
						rotatedCircle.inithanlde1 = (rotatedCircle.handle1 = MathHelper.vect(rotatedCircle.x - num11 * num, rotatedCircle.y));
						rotatedCircle.inithanlde2 = (rotatedCircle.handle2 = MathHelper.vect(rotatedCircle.x + num11 * num, rotatedCircle.y));
						rotatedCircle.handle1 = MathHelper.vectRotateAround(rotatedCircle.handle1, MathHelper.DEGREES_TO_RADIANS(d), rotatedCircle.x, rotatedCircle.y);
						rotatedCircle.handle2 = MathHelper.vectRotateAround(rotatedCircle.handle2, MathHelper.DEGREES_TO_RADIANS(d), rotatedCircle.x, rotatedCircle.y);
						rotatedCircle.setSize(num11);
						rotatedCircle.setHasOneHandle(hasOneHandle);
						rotatedCircles.addObject(rotatedCircle);
					}
					else if (item4.Name == "bouncer1" || item4.Name == "bouncer2")
					{
						float px2 = (float)item4["x"].intValue() * num + num3 + (float)num4;
						float py2 = (float)item4["y"].intValue() * num + num2 + (float)num5;
						int w2 = item4["size"].intValue();
						double an2 = item4["angle"].intValue();
						Bouncer bouncer = (Bouncer)new Bouncer().initWithPosXYWidthAndAngle(px2, py2, w2, an2);
						bouncer.parseMover(item4);
						bouncers.addObject(bouncer);
					}
					else if (item4.Name == "grab")
					{
						float hx = (float)item4["x"].intValue() * num + num3 + (float)num4;
						float hy = (float)item4["y"].intValue() * num + num2 + (float)num5;
						float len = (float)item4["length"].intValue() * num;
						float num12 = item4["radius"].floatValue();
						bool wheel = item4["wheel"].isEqualToString("true");
						float l = item4["moveLength"].floatValue() * num;
						bool v = item4["moveVertical"].isEqualToString("true");
						float o = item4["moveOffset"].floatValue() * num;
						bool spider = item4["spider"].isEqualToString("true");
						bool flag = item4["part"].isEqualToString("L");
						bool flag2 = item4["hidePath"].isEqualToString("true");
						Grab grab = (Grab)new Grab().init();
						grab.initial_x = (grab.x = hx);
						grab.initial_y = (grab.y = hy);
						grab.initial_rotation = 0f;
						grab.wheel = wheel;
						grab.setSpider(spider);
						grab.parseMover(item4);
						if (grab.mover != null)
						{
							grab.setBee();
							if (!flag2)
							{
								int num13 = 3;
								bool flag3 = item4["path"].hasPrefix(NSObject.NSS("R"));
								for (int k = 0; k < grab.mover.pathLen - 1; k++)
								{
									if (!flag3 || k % num13 == 0)
									{
										pollenDrawer.fillWithPolenFromPathIndexToPathIndexGrab(k, k + 1, grab);
									}
								}
								if (grab.mover.pathLen > 2)
								{
									pollenDrawer.fillWithPolenFromPathIndexToPathIndexGrab(0, grab.mover.pathLen - 1, grab);
								}
							}
						}
						if (num12 != -1f)
						{
							num12 *= num;
						}
						if (num12 == -1f)
						{
							ConstraintedPoint constraintedPoint = this.star;
							if (twoParts != 2)
							{
								constraintedPoint = (flag ? starL : starR);
							}
							Bungee bungee = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, hx, hy, constraintedPoint, constraintedPoint.pos.x, constraintedPoint.pos.y, len);
							bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
							grab.setRope(bungee);
						}
						grab.setRadius(num12);
						grab.setMoveLengthVerticalOffset(l, v, o);
						bungees.addObject(grab);
					}
					else if (item4.Name == "target")
					{
						CTRRootController cTRRootController2 = (CTRRootController)Application.sharedRootController();
						int pack = cTRRootController2.getPack();
						support = Image.Image_createWithResIDQuad(IMG_CHAR_SUPPORTS, pack);
						support.doRestoreCutTransparency();
						support.anchor = 18;
						target = CharAnimations.CharAnimations_createWithResID(IMG_CHAR_ANIMATIONS);
						target.doRestoreCutTransparency();
						target.passColorToChilds = false;
						NSString nSString3 = item4["x"];
						target.x = (support.x = (float)nSString3.intValue() * num + num3 + (float)num4);
						NSString nSString4 = item4["y"];
						target.y = (support.y = (float)nSString4.intValue() * num + num2 + (float)num5);
						target.addImage(IMG_CHAR_ANIMATIONS2);
						target.addImage(IMG_CHAR_ANIMATIONS3);
						target.bb = FrameworkTypes.MakeRectangle(264.0, 350.0, 108.0, 2.0);
						target.addAnimationWithIDDelayLoopFirstLast(0, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 0, 18);
						target.addAnimationWithIDDelayLoopFirstLast(1, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 43, 67);
						int num14 = 68;
						target.addAnimationWithIDDelayLoopCountSequence(2, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 32, num14, new List<int>
						{
							num14 + 1,
							num14 + 2,
							num14 + 3,
							num14 + 4,
							num14 + 5,
							num14 + 6,
							num14 + 7,
							num14 + 8,
							num14 + 9,
							num14 + 10,
							num14 + 11,
							num14 + 12,
							num14 + 13,
							num14 + 14,
							num14 + 15,
							num14,
							num14 + 1,
							num14 + 2,
							num14 + 3,
							num14 + 4,
							num14 + 5,
							num14 + 6,
							num14 + 7,
							num14 + 8,
							num14 + 9,
							num14 + 10,
							num14 + 11,
							num14 + 12,
							num14 + 13,
							num14 + 14,
							num14 + 15
						});
						target.addAnimationWithIDDelayLoopFirstLast(7, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 19, 27);
						target.addAnimationWithIDDelayLoopFirstLast(8, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 28, 31);
						target.addAnimationWithIDDelayLoopFirstLast(9, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 32, 40);
						target.addAnimationWithIDDelayLoopFirstLast(6, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 28, 31);
						target.addAnimationWithIDDelayLoopFirstLast(101, 10, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 47, 76);
						target.addAnimationWithIDDelayLoopFirstLast(101, 3, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 19);
						target.addAnimationWithIDDelayLoopFirstLast(101, 4, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 20, 46);
						target.addAnimationWithIDDelayLoopFirstLast(102, 5, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 12);
						target.switchToAnimationatEndOfAnimationDelay(9, 6, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(101, 4, 80, 8, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(80, 0, 101, 10, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(80, 0, 80, 1, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(80, 0, 80, 2, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(80, 0, 101, 3, 0.05f);
						target.switchToAnimationatEndOfAnimationDelay(80, 0, 101, 4, 0.05f);
						if (CTRRootController.isShowGreeting())
						{
							dd.callObjectSelectorParamafterDelay(selector_showGreeting, null, 1.3f);
							CTRRootController.setShowGreeting(false);
						}
						target.playTimeline(0);
						Timeline timeline6 = target.getTimeline(0);
						timeline6.delegateTimelineDelegate = this;
						target.setPauseAtIndexforAnimation(8, 7);
						blink = Animation.Animation_createWithResID(IMG_CHAR_ANIMATIONS);
						blink.parentAnchor = 9;
						blink.visible = false;
						blink.addAnimationWithIDDelayLoopCountSequence(0, 0.05f, Timeline.LoopType.TIMELINE_NO_LOOP, 4, 41, new List<int> { 41, 42, 42, 42 });
						blink.setActionTargetParamSubParamAtIndexforAnimation("ACTION_SET_VISIBLE", blink, 0, 0, 2, 0);
						blinkTimer = 3;
						blink.doRestoreCutTransparency();
						target.addChild(blink);
						idlesTimer = MathHelper.RND_RANGE(5, 20);
					}
				}
			}
			if (twoParts != 2)
			{
				candyBubbleAnimationL = Animation.Animation_createWithResID(IMG_OBJ_BUBBLE_FLIGHT);
				candyBubbleAnimationL.parentAnchor = (candyBubbleAnimationL.anchor = 18);
				candyBubbleAnimationL.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_REPLAY, 0, 12);
				candyBubbleAnimationL.playTimeline(0);
				candyL.addChild(candyBubbleAnimationL);
				candyBubbleAnimationL.visible = false;
				candyBubbleAnimationR = Animation.Animation_createWithResID(IMG_OBJ_BUBBLE_FLIGHT);
				candyBubbleAnimationR.parentAnchor = (candyBubbleAnimationR.anchor = 18);
				candyBubbleAnimationR.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_REPLAY, 0, 12);
				candyBubbleAnimationR.playTimeline(0);
				candyR.addChild(candyBubbleAnimationR);
				candyBubbleAnimationR.visible = false;
			}
			foreach (RotatedCircle rotatedCircle2 in rotatedCircles)
			{
				rotatedCircle2.operating = -1;
				rotatedCircle2.circlesArray = rotatedCircles;
			}
			startCamera();
			tummyTeasers = 0;
			starsCollected = 0;
			candyBubble = null;
			candyBubbleL = null;
			candyBubbleR = null;
			mouthOpen = false;
			noCandy = twoParts != 2;
			noCandyL = false;
			noCandyR = false;
			blink.playTimeline(0);
			time = 0f;
			score = 0;
			gravityNormal = true;
			MaterialPoint.globalGravity = MathHelper.vect(0f, 784f);
			dimTime = 0f;
			ropesCutAtOnce = 0;
			ropeAtOnceTimer = 0f;
			dd.callObjectSelectorParamafterDelay(selector_doCandyBlink, null, 1.0);
			Text text = Text.createWithFontandString(FNT_BIG_FONT, cTRRootController.getPack() + 1 + " - " + (cTRRootController.getLevel() + 1));
			text.anchor = 33;
			Text text2 = Text.createWithFontandString(FNT_BIG_FONT, Application.getString(STR_MENU_LEVEL));
			text2.anchor = 33;
			text2.parentAnchor = 9;
			text.setName("levelLabel");
			text.x = 15f + (float)base.canvas.xOffsetScaled;
			text.y = FrameworkTypes.SCREEN_HEIGHT + 15f;
			text2.y = 60f;
			text2.rotationCenterX -= (float)text2.width / 2f;
			text2.scaleX = (text2.scaleY = 0.7f);
			text.addChild(text2);
			Timeline timeline7 = new Timeline().initWithMaxKeyFramesOnTrack(5);
			timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
			timeline7.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.5));
			text.addTimelinewithID(timeline7, 0);
			text.playTimeline(0);
			timeline7.delegateTimelineDelegate = staticAniPool;
			staticAniPool.addChild(text);
			for (int m = 0; m < 5; m++)
			{
				dragging[m] = false;
				startPos[m] = (prevStartPos[m] = MathHelper.vectZero);
			}
			if (clickToCut)
			{
				resetBungeeHighlight();
			}
			CTRRootController.logEvent("IG_SHOWN");
		}

		public virtual void startCamera()
		{
			if (mapWidth > FrameworkTypes.SCREEN_WIDTH || mapHeight > FrameworkTypes.SCREEN_HEIGHT)
			{
				ignoreTouches = true;
				fastenCamera = false;
				camera.type = CAMERA_TYPE.CAMERA_SPEED_PIXELS;
				camera.speed = 20f;
				cameraMoveMode = 0;
				ConstraintedPoint constraintedPoint = ((twoParts != 2) ? starL : star);
				float num;
				float num2;
				if (mapWidth > FrameworkTypes.SCREEN_WIDTH)
				{
					if ((double)constraintedPoint.pos.x > (double)mapWidth / 2.0)
					{
						num = 0f;
						num2 = 0f;
					}
					else
					{
						num = mapWidth - FrameworkTypes.SCREEN_WIDTH;
						num2 = 0f;
					}
				}
				else if ((double)constraintedPoint.pos.y > (double)mapHeight / 2.0)
				{
					num = 0f;
					num2 = 0f;
				}
				else
				{
					num = 0f;
					num2 = mapHeight - FrameworkTypes.SCREEN_HEIGHT;
				}
				float num3 = constraintedPoint.pos.x - FrameworkTypes.SCREEN_WIDTH / 2f;
				float num4 = constraintedPoint.pos.y - FrameworkTypes.SCREEN_HEIGHT / 2f;
				float num5 = MathHelper.FIT_TO_BOUNDARIES(num3, 0.0, mapWidth - FrameworkTypes.SCREEN_WIDTH);
				float num6 = MathHelper.FIT_TO_BOUNDARIES(num4, 0.0, mapHeight - FrameworkTypes.SCREEN_HEIGHT);
				camera.moveToXYImmediate(num, num2, true);
				initialCameraToStarDistance = MathHelper.vectDistance(camera.pos, MathHelper.vect(num5, num6));
			}
			else
			{
				ignoreTouches = false;
				camera.moveToXYImmediate(0f, 0f, true);
			}
		}

		public virtual void doCandyBlink()
		{
			candyBlink.playTimeline(0);
		}

		public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
		{
			if (rotatedCircles.getObjectIndex(t.element) != -1 || i != 1)
			{
				return;
			}
			blinkTimer--;
			if (blinkTimer == 0)
			{
				blink.visible = true;
				blink.playTimeline(0);
				blinkTimer = 3;
			}
			idlesTimer--;
			if (idlesTimer == 0)
			{
				if (MathHelper.RND_RANGE(0, 1) == 1)
				{
					target.playTimeline(1);
				}
				else
				{
					target.playTimeline(2);
				}
				idlesTimer = MathHelper.RND_RANGE(5, 20);
			}
		}

		public virtual void timelineFinished(Timeline t)
		{
			if (rotatedCircles.getObjectIndex(t.element) != -1)
			{
				RotatedCircle rotatedCircle = (RotatedCircle)t.element;
				rotatedCircle.removeOnNextUpdate = true;
			}
			foreach (BaseElement tutorial in tutorials)
			{
				BaseElement baseElement = tutorial;
			}
		}

		public override void hide()
		{
			if (gravityButton != null)
			{
				removeChild(gravityButton);
			}
			pollenDrawer.release();
			if (earthAnims != null)
			{
				earthAnims.release();
			}
			candy.release();
			star.release();
			if (candyL != null)
			{
				candyL.release();
			}
			if (candyR != null)
			{
				candyR.release();
			}
			starL.release();
			starR.release();
			razors.release();
			spikes.release();
			bungees.release();
			stars.release();
			bubbles.release();
			pumps.release();
			socks.release();
			bouncers.release();
			rotatedCircles.release();
			target.release();
			support.release();
			tutorialImages.release();
			tutorials.release();
			candyL = null;
			candyR = null;
			starL = null;
			starR = null;
		}

		public override void update(float delta)
		{
			base.update(delta);
			dd.update(delta);
			pollenDrawer.update(delta);
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < fingerCuts[i].count(); j++)
				{
					FingerCut fingerCut = (FingerCut)fingerCuts[i].objectAtIndex(j);
					if (Mover.moveVariableToTarget(ref fingerCut.c.a, 0.0, 10.0, delta))
					{
						fingerCuts[i].removeObject(fingerCut);
						j--;
					}
				}
			}
			if (earthAnims != null)
			{
				foreach (Image earthAnim in earthAnims)
				{
					earthAnim.update(delta);
				}
			}
			Mover.moveVariableToTarget(ref ropeAtOnceTimer, 0.0, 1.0, delta);
			ConstraintedPoint constraintedPoint = ((twoParts != 2) ? starL : this.star);
			float num = constraintedPoint.pos.x - FrameworkTypes.SCREEN_WIDTH / 2f;
			float num2 = constraintedPoint.pos.y - FrameworkTypes.SCREEN_HEIGHT / 2f;
			float num3 = MathHelper.FIT_TO_BOUNDARIES(num, 0.0, mapWidth - FrameworkTypes.SCREEN_WIDTH);
			float num4 = MathHelper.FIT_TO_BOUNDARIES(num2, 0.0, mapHeight - FrameworkTypes.SCREEN_HEIGHT);
			camera.moveToXYImmediate(num3, num4, false);
			if (!freezeCamera || camera.type != CAMERA_TYPE.CAMERA_SPEED_DELAY)
			{
				camera.update(delta);
			}
			if (camera.type == CAMERA_TYPE.CAMERA_SPEED_PIXELS)
			{
				float num5 = 100f;
				float num6 = 800f;
				float num7 = 400f;
				float a = 1000f;
				float a2 = 300f;
				float num8 = MathHelper.vectDistance(camera.pos, MathHelper.vect(num3, num4));
				if (num8 < num5)
				{
					ignoreTouches = false;
				}
				if (fastenCamera)
				{
					if (camera.speed < 5500f)
					{
						camera.speed *= 1.5f;
					}
				}
				else if ((double)num8 > (double)initialCameraToStarDistance / 2.0)
				{
					camera.speed += delta * num6;
					camera.speed = MathHelper.MIN(a, camera.speed);
				}
				else
				{
					camera.speed -= delta * num7;
					camera.speed = MathHelper.MAX(a2, camera.speed);
				}
				if ((double)Math.Abs(camera.pos.x - num3) < 1.0 && (double)Math.Abs(camera.pos.y - num4) < 1.0)
				{
					camera.type = CAMERA_TYPE.CAMERA_SPEED_DELAY;
					camera.speed = 14f;
				}
			}
			else
			{
				time += delta;
			}
			if (bungees.count() > 0)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				int num9 = bungees.count();
				for (int k = 0; k < num9; k++)
				{
					Grab grab = (Grab)bungees.objectAtIndex(k);
					grab.update(delta);
					Bungee rope = grab.rope;
					if (grab.mover != null)
					{
						if (grab.rope != null)
						{
							grab.rope.bungeeAnchor.pos = MathHelper.vect(grab.x, grab.y);
							grab.rope.bungeeAnchor.pin = grab.rope.bungeeAnchor.pos;
						}
						if (grab.radius != -1f)
						{
							grab.reCalcCircle();
						}
					}
					if (rope != null)
					{
						if (rope.cut != -1 && (double)rope.cutTime == 0.0)
						{
							continue;
						}
						if (rope != null)
						{
							rope.update(delta * ropePhysicsSpeed);
						}
						if (grab.hasSpider)
						{
							if (camera.type != 0 || !ignoreTouches)
							{
								grab.updateSpider(delta);
							}
							if (grab.spiderPos == -1f)
							{
								spiderWon(grab);
								break;
							}
						}
					}
					if (grab.radius != -1f && grab.rope == null)
					{
						if (twoParts != 2)
						{
							if (!noCandyL)
							{
								float num10 = MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), starL.pos);
								if (num10 <= grab.radius + 42f)
								{
									Bungee bungee = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, grab.x, grab.y, starL, starL.pos.x, starL.pos.y, grab.radius + 42f);
									bungee.bungeeAnchor.pin = bungee.bungeeAnchor.pos;
									grab.hideRadius = true;
									grab.setRope(bungee);
									CTRSoundMgr._playSound(SND_ROPE_GET);
									if (grab.mover != null)
									{
										CTRSoundMgr._playSound(SND_BUZZ);
									}
								}
							}
							if (!noCandyR && grab.rope == null)
							{
								float num11 = MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), starR.pos);
								if (num11 <= grab.radius + 42f)
								{
									Bungee bungee2 = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, grab.x, grab.y, starR, starR.pos.x, starR.pos.y, grab.radius + 42f);
									bungee2.bungeeAnchor.pin = bungee2.bungeeAnchor.pos;
									grab.hideRadius = true;
									grab.setRope(bungee2);
									CTRSoundMgr._playSound(SND_ROPE_GET);
									if (grab.mover != null)
									{
										CTRSoundMgr._playSound(SND_BUZZ);
									}
								}
							}
						}
						else
						{
							float num12 = MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), this.star.pos);
							if (num12 <= grab.radius + 42f)
							{
								Bungee bungee3 = (Bungee)new Bungee().initWithHeadAtXYTailAtTXTYandLength(null, grab.x, grab.y, this.star, this.star.pos.x, this.star.pos.y, grab.radius + 42f);
								bungee3.bungeeAnchor.pin = bungee3.bungeeAnchor.pos;
								grab.hideRadius = true;
								grab.setRope(bungee3);
								CTRSoundMgr._playSound(SND_ROPE_GET);
								if (grab.mover != null)
								{
									CTRSoundMgr._playSound(SND_BUZZ);
								}
							}
						}
					}
					if (rope == null)
					{
						continue;
					}
					ConstraintedPoint bungeeAnchor = rope.bungeeAnchor;
					ConstraintedPoint constraintedPoint2 = rope.parts[rope.parts.Count - 1];
					Vector v = MathHelper.vectSub(bungeeAnchor.pos, constraintedPoint2.pos);
					bool flag4 = false;
					if (twoParts != 2)
					{
						if (constraintedPoint2 == starL && !noCandyL && !flag2)
						{
							flag4 = true;
						}
						if (constraintedPoint2 == starR && !noCandyR && !flag3)
						{
							flag4 = true;
						}
					}
					else if (!noCandy && !flag)
					{
						flag4 = true;
					}
					if (rope.relaxed != 0 && rope.cut == -1 && flag4)
					{
						float num13 = MathHelper.RADIANS_TO_DEGREES(MathHelper.vectAngleNormalized(v));
						if (twoParts != 2)
						{
							GameObject gameObject = ((constraintedPoint2 == starL) ? candyL : candyR);
							if (!rope.chosenOne)
							{
								rope.initialCandleAngle = gameObject.rotation - num13;
							}
							if (constraintedPoint2 == starL)
							{
								lastCandyRotateDeltaL = num13 + rope.initialCandleAngle - gameObject.rotation;
								flag2 = true;
							}
							else
							{
								lastCandyRotateDeltaR = num13 + rope.initialCandleAngle - gameObject.rotation;
								flag3 = true;
							}
							gameObject.rotation = num13 + rope.initialCandleAngle;
						}
						else
						{
							if (!rope.chosenOne)
							{
								rope.initialCandleAngle = candyMain.rotation - num13;
							}
							lastCandyRotateDelta = num13 + rope.initialCandleAngle - candyMain.rotation;
							candyMain.rotation = num13 + rope.initialCandleAngle;
							flag = true;
						}
						rope.chosenOne = true;
					}
					else
					{
						rope.chosenOne = false;
					}
				}
				if (twoParts != 2)
				{
					if (!flag2 && !noCandyL)
					{
						candyL.rotation += MathHelper.MIN(5.0, lastCandyRotateDeltaL);
						lastCandyRotateDeltaL *= 0.98f;
					}
					if (!flag3 && !noCandyR)
					{
						candyR.rotation += MathHelper.MIN(5.0, lastCandyRotateDeltaR);
						lastCandyRotateDeltaR *= 0.98f;
					}
				}
				else if (!flag && !noCandy)
				{
					candyMain.rotation += MathHelper.MIN(5.0, lastCandyRotateDelta);
					lastCandyRotateDelta *= 0.98f;
				}
			}
			if (!noCandy)
			{
				this.star.update(delta * ropePhysicsSpeed);
				candy.x = this.star.pos.x;
				candy.y = this.star.pos.y;
				candy.update(delta);
				BaseElement.calculateTopLeft(candy);
			}
			if (twoParts != 2)
			{
				candyL.update(delta);
				starL.update(delta * ropePhysicsSpeed);
				candyR.update(delta);
				starR.update(delta * ropePhysicsSpeed);
				if (twoParts == 1)
				{
					for (int l = 0; l < 30; l++)
					{
						ConstraintedPoint.satisfyConstraints(starL);
						ConstraintedPoint.satisfyConstraints(starR);
					}
				}
				if ((double)partsDist > 0.0)
				{
					if (Mover.moveVariableToTarget(ref partsDist, 0.0, 200.0, delta))
					{
						CTRSoundMgr._playSound(SND_CANDY_LINK);
						twoParts = 2;
						noCandy = false;
						noCandyL = true;
						noCandyR = true;
						int num14 = Preferences._getIntForKey("PREFS_CANDIES_UNITED") + 1;
						Preferences._setIntforKey(num14, "PREFS_CANDIES_UNITED", false);
						if (num14 == 100)
						{
							CTRRootController.postAchievementName(CTRPreferences.acRomanticSoul, FrameworkTypes.ACHIEVEMENT_STRING("\"Romantic Soul\""));
						}
						if (candyBubbleL != null || candyBubbleR != null)
						{
							candyBubble = ((candyBubbleL != null) ? candyBubbleL : candyBubbleR);
							candyBubbleAnimation.visible = true;
						}
						lastCandyRotateDelta = 0f;
						lastCandyRotateDeltaL = 0f;
						lastCandyRotateDeltaR = 0f;
						this.star.pos.x = starL.pos.x;
						this.star.pos.y = starL.pos.y;
						candy.x = this.star.pos.x;
						candy.y = this.star.pos.y;
						BaseElement.calculateTopLeft(candy);
						Vector vector = MathHelper.vectSub(starL.pos, starL.prevPos);
						Vector vector2 = MathHelper.vectSub(starR.pos, starR.prevPos);
						Vector v2 = MathHelper.vect((vector.x + vector2.x) / 2f, (vector.y + vector2.y) / 2f);
						this.star.prevPos = MathHelper.vectSub(this.star.pos, v2);
						int num15 = bungees.count();
						for (int m = 0; m < num15; m++)
						{
							Grab grab2 = (Grab)bungees.objectAtIndex(m);
							Bungee rope2 = grab2.rope;
							if (rope2 != null && rope2.cut != rope2.parts.Count - 3 && (rope2.tail == starL || rope2.tail == starR))
							{
								ConstraintedPoint constraintedPoint3 = rope2.parts[rope2.parts.Count - 2];
								int num16 = (int)rope2.tail.restLengthFor(constraintedPoint3);
								this.star.addConstraintwithRestLengthofType(constraintedPoint3, num16, Constraint.CONSTRAINT.CONSTRAINT_DISTANCE);
								rope2.tail = this.star;
								rope2.parts[rope2.parts.Count - 1] = this.star;
								rope2.initialCandleAngle = 0f;
								rope2.chosenOne = false;
							}
						}
						Animation animation = Animation.Animation_createWithResID(IMG_OBJ_CANDY_01);
						animation.doRestoreCutTransparency();
						animation.x = candy.x;
						animation.y = candy.y;
						animation.anchor = 18;
						int n = animation.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_NO_LOOP, 21, 25);
						animation.getTimeline(n).delegateTimelineDelegate = aniPool;
						animation.playTimeline(0);
						aniPool.addChild(animation);
					}
					else
					{
						starL.changeRestLengthToFor(partsDist, starR);
						starR.changeRestLengthToFor(partsDist, starL);
					}
				}
				if (!noCandyL && !noCandyR && GameObject.objectsIntersect(candyL, candyR) && twoParts == 0)
				{
					twoParts = 1;
					partsDist = MathHelper.vectDistance(starL.pos, starR.pos);
					starL.addConstraintwithRestLengthofType(starR, partsDist, Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN);
					starR.addConstraintwithRestLengthofType(starL, partsDist, Constraint.CONSTRAINT.CONSTRAINT_NOT_MORE_THAN);
				}
			}
			target.update(delta);
			if (camera.type != 0 || !ignoreTouches)
			{
				foreach (Star star in stars)
				{
					star.update(delta);
					if ((double)star.timeout > 0.0 && (double)star.time == 0.0)
					{
						star.getTimeline(1).delegateTimelineDelegate = aniPool;
						aniPool.addChild(star);
						stars.removeObject(star);
						star.timedAnim.playTimeline(1);
						star.playTimeline(1);
						break;
					}
					if ((twoParts == 2) ? (GameObject.objectsIntersect(candy, star) && !noCandy) : ((GameObject.objectsIntersect(candyL, star) && !noCandyL) || (GameObject.objectsIntersect(candyR, star) && !noCandyR)))
					{
						candyBlink.playTimeline(1);
						starsCollected++;
						hudStar[starsCollected - 1].playTimeline(0);
						Animation animation2 = Animation.Animation_createWithResID(IMG_OBJ_STAR_DISAPPEAR);
						animation2.doRestoreCutTransparency();
						animation2.x = star.x;
						animation2.y = star.y;
						animation2.anchor = 18;
						int n2 = animation2.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 12);
						animation2.getTimeline(n2).delegateTimelineDelegate = aniPool;
						animation2.playTimeline(0);
						aniPool.addChild(animation2);
						stars.removeObject(star);
						CTRSoundMgr._playSound(SND_STAR_1 + starsCollected - 1);
						if (target.getCurrentTimelineIndex() == 0)
						{
							target.playAnimationtimeline(101, 3);
						}
						break;
					}
				}
			}
			foreach (Bubble bubble3 in bubbles)
			{
				bubble3.update(delta);
				float num17 = 85f;
				if (twoParts != 2)
				{
					if (!noCandyL && !bubble3.popped && MathHelper.pointInRect(candyL.x, candyL.y, bubble3.x - num17, bubble3.y - num17, num17 * 2f, num17 * 2f))
					{
						if (candyBubbleL != null)
						{
							popBubbleAtXY(bubble3.x, bubble3.y);
						}
						candyBubbleL = bubble3;
						candyBubbleAnimationL.visible = true;
						CTRSoundMgr._playSound(SND_BUBBLE);
						bubble3.popped = true;
						bubble3.removeChildWithID(0);
						break;
					}
					if (!noCandyR && !bubble3.popped && MathHelper.pointInRect(candyR.x, candyR.y, bubble3.x - num17, bubble3.y - num17, num17 * 2f, num17 * 2f))
					{
						if (candyBubbleR != null)
						{
							popBubbleAtXY(bubble3.x, bubble3.y);
						}
						candyBubbleR = bubble3;
						candyBubbleAnimationR.visible = true;
						CTRSoundMgr._playSound(SND_BUBBLE);
						bubble3.popped = true;
						bubble3.removeChildWithID(0);
						break;
					}
				}
				else if (!noCandy && !bubble3.popped && MathHelper.pointInRect(candy.x, candy.y, bubble3.x - num17, bubble3.y - num17, num17 * 2f, num17 * 2f))
				{
					if (candyBubble != null)
					{
						popBubbleAtXY(bubble3.x, bubble3.y);
					}
					candyBubble = bubble3;
					candyBubbleAnimation.visible = true;
					CTRSoundMgr._playSound(SND_BUBBLE);
					bubble3.popped = true;
					bubble3.removeChildWithID(0);
					break;
				}
				if (bubble3.withoutShadow)
				{
					continue;
				}
				foreach (RotatedCircle rotatedCircle5 in rotatedCircles)
				{
					float num18 = MathHelper.vectDistance(MathHelper.vect(bubble3.x, bubble3.y), MathHelper.vect(rotatedCircle5.x, rotatedCircle5.y));
					if (num18 < rotatedCircle5.sizeInPixels)
					{
						bubble3.withoutShadow = true;
					}
				}
			}
			foreach (Text tutorial in tutorials)
			{
				tutorial.update(delta);
			}
			foreach (GameObject tutorialImage in tutorialImages)
			{
				tutorialImage.update(delta);
			}
			foreach (Pump pump in pumps)
			{
				pump.update(delta);
				if (Mover.moveVariableToTarget(ref pump.pumpTouchTimer, 0.0, 1.0, delta))
				{
					operatePump(pump, delta);
				}
			}
			RotatedCircle rotatedCircle2 = null;
			foreach (RotatedCircle rotatedCircle6 in rotatedCircles)
			{
				foreach (Grab bungee4 in bungees)
				{
					if (MathHelper.vectDistance(MathHelper.vect(bungee4.x, bungee4.y), MathHelper.vect(rotatedCircle6.x, rotatedCircle6.y)) <= rotatedCircle6.sizeInPixels + FrameworkTypes.RTPD(5.0) * 3f)
					{
						if (rotatedCircle6.containedObjects.getObjectIndex(bungee4) == -1)
						{
							rotatedCircle6.containedObjects.addObject(bungee4);
						}
					}
					else if (rotatedCircle6.containedObjects.getObjectIndex(bungee4) != -1)
					{
						rotatedCircle6.containedObjects.removeObject(bungee4);
					}
				}
				foreach (Bubble bubble4 in bubbles)
				{
					if (MathHelper.vectDistance(MathHelper.vect(bubble4.x, bubble4.y), MathHelper.vect(rotatedCircle6.x, rotatedCircle6.y)) <= rotatedCircle6.sizeInPixels + FrameworkTypes.RTPD(10.0) * 3f)
					{
						if (rotatedCircle6.containedObjects.getObjectIndex(bubble4) == -1)
						{
							rotatedCircle6.containedObjects.addObject(bubble4);
						}
					}
					else if (rotatedCircle6.containedObjects.getObjectIndex(bubble4) != -1)
					{
						rotatedCircle6.containedObjects.removeObject(bubble4);
					}
				}
				if (rotatedCircle6.removeOnNextUpdate)
				{
					rotatedCircle2 = rotatedCircle6;
				}
				rotatedCircle6.update(delta);
			}
			if (rotatedCircle2 != null)
			{
				rotatedCircles.removeObject(rotatedCircle2);
			}
			float num19 = FrameworkTypes.RTPD(20.0);
			foreach (Sock sock3 in socks)
			{
				sock3.update(delta);
				if (Mover.moveVariableToTarget(ref sock3.idleTimeout, 0.0, 1.0, delta))
				{
					sock3.state = Sock.SOCK_IDLE;
				}
				float num20 = sock3.rotation;
				sock3.rotation = 0f;
				sock3.updateRotation();
				Vector vector3 = MathHelper.vectRotate(this.star.posDelta, MathHelper.DEGREES_TO_RADIANS(0f - num20));
				sock3.rotation = num20;
				sock3.updateRotation();
				if ((double)vector3.y >= 0.0 && (MathHelper.lineInRect(sock3.t1.x, sock3.t1.y, sock3.t2.x, sock3.t2.y, this.star.pos.x - num19, this.star.pos.y - num19, num19 * 2f, num19 * 2f) || MathHelper.lineInRect(sock3.b1.x, sock3.b1.y, sock3.b2.x, sock3.b2.y, this.star.pos.x - num19, this.star.pos.y - num19, num19 * 2f, num19 * 2f)))
				{
					if (sock3.state != Sock.SOCK_IDLE)
					{
						continue;
					}
					foreach (Sock sock4 in socks)
					{
						if (sock4 != sock3 && sock4.group == sock3.group)
						{
							sock3.state = Sock.SOCK_RECEIVING;
							sock4.state = Sock.SOCK_THROWING;
							releaseAllRopes(false);
							savedSockSpeed = 0.9f * MathHelper.vectLength(this.star.v);
							savedSockSpeed *= 1.4f;
							targetSock = sock4;
							sock3.light.playTimeline(0);
							sock3.light.visible = true;
							CTRSoundMgr._playSound(SND_TELEPORT);
							dd.callObjectSelectorParamafterDelay(selector_teleport, null, 0.1);
							break;
						}
					}
					break;
				}
				if (sock3.state != Sock.SOCK_IDLE && sock3.idleTimeout == 0f)
				{
					sock3.idleTimeout = 0.8f;
				}
			}
			foreach (Razor razor in razors)
			{
				razor.update(delta);
				cutWithRazorOrLine1Line2Immediate(razor, MathHelper.vectZero, MathHelper.vectZero, false);
			}
			foreach (Spikes spike in this.spikes)
			{
				spike.update(delta);
				float num21 = 15f;
				if (spike.electro && (!spike.electro || !spike.electroOn))
				{
					continue;
				}
				bool flag6 = false;
				bool flag7 = false;
				if (twoParts != 2)
				{
					flag6 = (MathHelper.lineInRect(spike.t1.x, spike.t1.y, spike.t2.x, spike.t2.y, starL.pos.x - num21, starL.pos.y - num21, num21 * 2f, num21 * 2f) || MathHelper.lineInRect(spike.b1.x, spike.b1.y, spike.b2.x, spike.b2.y, starL.pos.x - num21, starL.pos.y - num21, num21 * 2f, num21 * 2f)) && !noCandyL;
					if (flag6)
					{
						flag7 = true;
					}
					else
					{
						flag6 = (MathHelper.lineInRect(spike.t1.x, spike.t1.y, spike.t2.x, spike.t2.y, starR.pos.x - num21, starR.pos.y - num21, num21 * 2f, num21 * 2f) || MathHelper.lineInRect(spike.b1.x, spike.b1.y, spike.b2.x, spike.b2.y, starR.pos.x - num21, starR.pos.y - num21, num21 * 2f, num21 * 2f)) && !noCandyR;
					}
				}
				else
				{
					flag6 = (MathHelper.lineInRect(spike.t1.x, spike.t1.y, spike.t2.x, spike.t2.y, this.star.pos.x - num21, this.star.pos.y - num21, num21 * 2f, num21 * 2f) || MathHelper.lineInRect(spike.b1.x, spike.b1.y, spike.b2.x, spike.b2.y, this.star.pos.x - num21, this.star.pos.y - num21, num21 * 2f, num21 * 2f)) && !noCandy;
				}
				if (!flag6)
				{
					continue;
				}
				if (twoParts != 2)
				{
					if (flag7)
					{
						if (candyBubbleL != null)
						{
							popCandyBubble(true);
						}
					}
					else if (candyBubbleR != null)
					{
						popCandyBubble(false);
					}
				}
				else if (candyBubble != null)
				{
					popCandyBubble(false);
				}
				Image image2 = Image.Image_createWithResID(IMG_OBJ_CANDY_01);
				image2.doRestoreCutTransparency();
				CandyBreak candyBreak = (CandyBreak)new CandyBreak().initWithTotalParticlesandImageGrid(5, image2);
				if (gravityButton != null && !gravityNormal)
				{
					candyBreak.gravity.y = -500f;
					candyBreak.angle = 90f;
				}
				candyBreak.particlesDelegate = aniPool.particlesFinished;
				if (twoParts != 2)
				{
					if (flag7)
					{
						candyBreak.x = candyL.x;
						candyBreak.y = candyL.y;
						noCandyL = true;
					}
					else
					{
						candyBreak.x = candyR.x;
						candyBreak.y = candyR.y;
						noCandyR = true;
					}
				}
				else
				{
					candyBreak.x = candy.x;
					candyBreak.y = candy.y;
					noCandy = true;
				}
				candyBreak.startSystem(5);
				aniPool.addChild(candyBreak);
				CTRSoundMgr._playSound(SND_CANDY_BREAK);
				releaseAllRopes(flag7);
				if (restartState != 0 && (twoParts == 2 || !noCandyL || !noCandyR))
				{
					dd.callObjectSelectorParamafterDelay(selector_gameLost, null, 0.3);
				}
				return;
			}
			foreach (Bouncer bouncer in bouncers)
			{
				bouncer.update(delta);
				float num22 = 40f;
				bool flag8 = false;
				bool flag9 = false;
				if (twoParts != 2)
				{
					flag8 = (MathHelper.lineInRect(bouncer.t1.X, bouncer.t1.Y, bouncer.t2.X, bouncer.t2.Y, starL.pos.x - num22, starL.pos.y - num22, num22 * 2f, num22 * 2f) || MathHelper.lineInRect(bouncer.b1.X, bouncer.b1.Y, bouncer.b2.X, bouncer.b2.Y, starL.pos.x - num22, starL.pos.y - num22, num22 * 2f, num22 * 2f)) && !noCandyL;
					if (flag8)
					{
						flag9 = true;
					}
					else
					{
						flag8 = (MathHelper.lineInRect(bouncer.t1.X, bouncer.t1.Y, bouncer.t2.X, bouncer.t2.Y, starR.pos.x - num22, starR.pos.y - num22, num22 * 2f, num22 * 2f) || MathHelper.lineInRect(bouncer.b1.X, bouncer.b1.Y, bouncer.b2.X, bouncer.b2.Y, starR.pos.x - num22, starR.pos.y - num22, num22 * 2f, num22 * 2f)) && !noCandyR;
					}
				}
				else
				{
					flag8 = (MathHelper.lineInRect(bouncer.t1.X, bouncer.t1.Y, bouncer.t2.X, bouncer.t2.Y, this.star.pos.x - num22, this.star.pos.y - num22, num22 * 2f, num22 * 2f) || MathHelper.lineInRect(bouncer.b1.X, bouncer.b1.Y, bouncer.b2.X, bouncer.b2.Y, this.star.pos.x - num22, this.star.pos.y - num22, num22 * 2f, num22 * 2f)) && !noCandy;
				}
				if (flag8)
				{
					if (twoParts != 2)
					{
						if (flag9)
						{
							handleBouncePtDelta(bouncer, starL, delta);
						}
						else
						{
							handleBouncePtDelta(bouncer, starR, delta);
						}
					}
					else
					{
						handleBouncePtDelta(bouncer, this.star, delta);
					}
				}
				else
				{
					bouncer.skip = false;
				}
			}
			float num23 = -40f;
			float num24 = 14f;
			float divImpulseVect = 0.0166667f / delta;
			if (twoParts == 0)
			{
				if (candyBubbleL != null)
				{
					if (gravityButton != null && !gravityNormal)
					{
						starL.applyImpulseDelta(MathHelper.vect((0f - starL.v.x) / num24 / divImpulseVect, ((0f - starL.v.y) / num24 - num23) / divImpulseVect), delta);
					}
					else
					{
						starL.applyImpulseDelta(MathHelper.vect((0f - starL.v.x) / num24 / divImpulseVect, ((0f - starL.v.y) / num24 + num23) / divImpulseVect), delta);
					}
				}
				if (candyBubbleR != null)
				{
					if (gravityButton != null && !gravityNormal)
					{
						starR.applyImpulseDelta(MathHelper.vect((0f - starR.v.x) / num24 / divImpulseVect, ((0f - starR.v.y) / num24 - num23) / divImpulseVect), delta);
					}
					else
					{
						starR.applyImpulseDelta(MathHelper.vect((0f - starR.v.x) / num24 / divImpulseVect, ((0f - starR.v.y) / num24 + num23) / divImpulseVect), delta);
					}
				}
			}
			if (twoParts == 1)
			{
				if (candyBubbleR != null || candyBubbleL != null)
				{
					if (gravityButton != null && !gravityNormal)
					{
						starL.applyImpulseDelta(MathHelper.vect((0f - starL.v.x) / num24 / divImpulseVect, ((0f - starL.v.y) / num24 - num23) / divImpulseVect), delta);
						starR.applyImpulseDelta(MathHelper.vect((0f - starR.v.x) / num24 / divImpulseVect, ((0f - starR.v.y) / num24 - num23) / divImpulseVect), delta);
					}
					else
					{
						starL.applyImpulseDelta(MathHelper.vect((0f - starL.v.x) / num24 / divImpulseVect, ((0f - starL.v.y) / num24 + num23) / divImpulseVect), delta);
						starR.applyImpulseDelta(MathHelper.vect((0f - starR.v.x) / num24 / divImpulseVect, ((0f - starR.v.y) / num24 + num23) / divImpulseVect), delta);
					}
				}
			}
			else if (candyBubble != null)
			{
				if (gravityButton != null && !gravityNormal)
				{
					this.star.applyImpulseDelta(MathHelper.vect((0f - this.star.v.x) / num24 / divImpulseVect, ((0f - this.star.v.y) / num24 - num23) / divImpulseVect), delta);
				}
				else
				{
					this.star.applyImpulseDelta(MathHelper.vect((0f - this.star.v.x) / num24 / divImpulseVect, ((0f - this.star.v.y) / num24 + num23) / divImpulseVect), delta);
				}
			}
			if (!noCandy)
			{
				if (!mouthOpen)
				{
					if (MathHelper.vectDistance(this.star.pos, MathHelper.vect(target.x, target.y)) < 200f)
					{
						mouthOpen = true;
						target.playTimeline(7);
						CTRSoundMgr._playSound(SND_MONSTER_OPEN);
						mouthCloseTimer = 1f;
					}
				}
				else if ((double)mouthCloseTimer > 0.0)
				{
					Mover.moveVariableToTarget(ref mouthCloseTimer, 0.0, 1.0, delta);
					if ((double)mouthCloseTimer <= 0.0)
					{
						if (MathHelper.vectDistance(this.star.pos, MathHelper.vect(target.x, target.y)) > 200f)
						{
							mouthOpen = false;
							target.playTimeline(8);
							CTRSoundMgr._playSound(SND_MONSTER_CLOSE);
							tummyTeasers++;
							if (tummyTeasers >= 10)
							{
								CTRRootController.postAchievementName(CTRPreferences.acTummyTeaser, FrameworkTypes.ACHIEVEMENT_STRING("\"Tummy Teaser\""));
							}
						}
						else
						{
							mouthCloseTimer = 1f;
						}
					}
				}
				if (restartState != 0 && GameObject.objectsIntersect(candy, target))
				{
					gameWon();
					return;
				}
			}
			bool flag10 = twoParts == 2 && pointOutOfScreen(this.star) && !noCandy;
			bool flag11 = twoParts != 2 && pointOutOfScreen(starL) && !noCandyL;
			bool flag12 = twoParts != 2 && pointOutOfScreen(starR) && !noCandyR;
			if (flag11 || flag12 || flag10)
			{
				if (flag10)
				{
					noCandy = true;
				}
				if (flag11)
				{
					noCandyL = true;
				}
				if (flag12)
				{
					noCandyR = true;
				}
				if (restartState != 0)
				{
					int num25 = Preferences._getIntForKey("PREFS_CANDIES_LOST") + 1;
					Preferences._setIntforKey(num25, "PREFS_CANDIES_LOST", false);
					if (num25 == 50)
					{
						CTRRootController.postAchievementName(CTRPreferences.acWeightLoser, FrameworkTypes.ACHIEVEMENT_STRING("\"Weight Loser\""));
					}
					if (num25 == 200)
					{
						CTRRootController.postAchievementName(CTRPreferences.acCalorieMinimizer, FrameworkTypes.ACHIEVEMENT_STRING("\"Calorie Minimizer\""));
					}
					if (twoParts == 2 || !noCandyL || !noCandyR)
					{
						gameLost();
					}
					return;
				}
			}
			if (special != 0 && special == 1 && !noCandy && candyBubble != null && candy.y < 400f && candy.x > 1200f)
			{
				special = 0;
				foreach (TutorialText tutorial2 in tutorials)
				{
					if (tutorial2.special == 1)
					{
						tutorial2.playTimeline(0);
					}
				}
				foreach (GameObjectSpecial tutorialImage2 in tutorialImages)
				{
					if (tutorialImage2.special == 1)
					{
						tutorialImage2.playTimeline(0);
					}
				}
			}
			if (clickToCut && !ignoreTouches)
			{
				resetBungeeHighlight();
				bool flag13 = false;
				Vector p = MathHelper.vectAdd(slastTouch, camera.pos);
				if (gravityButton != null)
				{
					Button button = (Button)gravityButton.getChild(gravityButton.on() ? 1 : 0);
					if (button.isInTouchZoneXYforTouchDown(p.x, p.y, true))
					{
						flag13 = true;
					}
				}
				if (candyBubble != null || (twoParts != 2 && (candyBubbleL != null || candyBubbleR != null)))
				{
					foreach (Bubble bubble5 in bubbles)
					{
						Bubble bubble6 = bubble5;
						if (candyBubble != null && MathHelper.pointInRect(p.x, p.y, this.star.pos.x - 60f, this.star.pos.y - 60f, 120f, 120f))
						{
							flag13 = true;
							break;
						}
						if (candyBubbleL != null && MathHelper.pointInRect(p.x, p.y, starL.pos.x - 60f, starL.pos.y - 60f, 120f, 120f))
						{
							flag13 = true;
							break;
						}
						if (candyBubbleR != null && MathHelper.pointInRect(p.x, p.y, starR.pos.x - 60f, starR.pos.y - 60f, 120f, 120f))
						{
							flag13 = true;
							break;
						}
					}
				}
				foreach (Spikes spike2 in spikes)
				{
					if (spike2.rotateButton != null && spike2.rotateButton.isInTouchZoneXYforTouchDown(p.x, p.y, true))
					{
						flag13 = true;
					}
				}
				foreach (Pump pump2 in pumps)
				{
					if (GameObject.pointInObject(p, pump2))
					{
						flag13 = true;
						break;
					}
				}
				foreach (RotatedCircle rotatedCircle7 in rotatedCircles)
				{
					if (rotatedCircle7.isLeftControllerActive() || rotatedCircle7.isRightControllerActive())
					{
						flag13 = true;
						break;
					}
					if (MathHelper.vectDistance(MathHelper.vect(p.x, p.y), MathHelper.vect(rotatedCircle7.handle1.x, rotatedCircle7.handle1.y)) <= 90f || MathHelper.vectDistance(MathHelper.vect(p.x, p.y), MathHelper.vect(rotatedCircle7.handle2.x, rotatedCircle7.handle2.y)) <= 90f)
					{
						flag13 = true;
						break;
					}
				}
				foreach (Grab bungee5 in bungees)
				{
					if (bungee5.wheel && MathHelper.pointInRect(p.x, p.y, bungee5.x - 110f, bungee5.y - 110f, 220f, 220f))
					{
						flag13 = true;
						break;
					}
					if ((double)bungee5.moveLength > 0.0 && (MathHelper.pointInRect(p.x, p.y, bungee5.x - 65f, bungee5.y - 65f, 130f, 130f) || bungee5.moverDragging != -1))
					{
						flag13 = true;
						break;
					}
				}
				if (!flag13)
				{
					Vector s = default(Vector);
					Grab grab5 = null;
					Bungee nearestBungeeSegmentByBeziersPointsatXYgrab = getNearestBungeeSegmentByBeziersPointsatXYgrab(ref s, slastTouch.x + camera.pos.x, slastTouch.y + camera.pos.y, ref grab5);
					if (nearestBungeeSegmentByBeziersPointsatXYgrab != null)
					{
						nearestBungeeSegmentByBeziersPointsatXYgrab.highlighted = true;
					}
				}
			}
			if (Mover.moveVariableToTarget(ref dimTime, 0.0, 1.0, delta))
			{
				if (restartState == 0)
				{
					restartState = 1;
					hide();
					show();
					dimTime = DIM_TIMEOUT;
				}
				else
				{
					restartState = -1;
				}
			}
		}

		public virtual void teleport()
		{
			if (targetSock != null)
			{
				targetSock.light.playTimeline(0);
				targetSock.light.visible = true;
				Vector v = MathHelper.vect(0f, -16f);
				v = MathHelper.vectRotate(v, MathHelper.DEGREES_TO_RADIANS(targetSock.rotation));
				star.pos.x = targetSock.x;
				star.pos.y = targetSock.y;
				star.pos = MathHelper.vectAdd(star.pos, v);
				star.prevPos.x = star.pos.x;
				star.prevPos.y = star.pos.y;
				star.v = MathHelper.vectMult(MathHelper.vectRotate(MathHelper.vect(0f, -1f), MathHelper.DEGREES_TO_RADIANS(targetSock.rotation)), savedSockSpeed);
				star.posDelta = MathHelper.vectDiv(star.v, 60f);
				star.prevPos = MathHelper.vectSub(star.pos, star.posDelta);
				targetSock = null;
			}
		}

		public virtual void animateLevelRestart()
		{
			restartState = 0;
			dimTime = DIM_TIMEOUT;
		}

		public virtual void releaseAllRopes(bool left)
		{
			int num = bungees.count();
			for (int i = 0; i < num; i++)
			{
				Grab grab = (Grab)bungees.objectAtIndex(i);
				Bungee rope = grab.rope;
				if (rope != null && (rope.tail == star || (rope.tail == starL && left) || (rope.tail == starR && !left)))
				{
					if (rope.cut == -1)
					{
						rope.setCut(rope.parts.Count - 2);
					}
					else
					{
						rope.hideTailParts = true;
					}
					if (grab.hasSpider && grab.spiderActive)
					{
						spiderBusted(grab);
					}
				}
			}
		}

		public virtual void calculateScore()
		{
			timeBonus = (int)MathHelper.MAX(0f, 30f - time) * 100;
			timeBonus /= 10;
			timeBonus *= 10;
			starBonus = 1000 * starsCollected;
			score = (int)MathHelper.ceil(timeBonus + starBonus);
		}

		public virtual void gameWon()
		{
			dd.cancelAllDispatches();
			target.playTimeline(6);
			CTRSoundMgr._playSound(SND_MONSTER_CHEWING);
			if (candyBubble != null)
			{
				popCandyBubble(false);
			}
			noCandy = true;
			candy.passTransformationsToChilds = true;
			candyMain.scaleX = (candyMain.scaleY = 1f);
			candyTop.scaleX = (candyTop.scaleY = 1f);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makePos(candy.x, candy.y, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makePos(target.x, (double)target.y + 10.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			timeline.addKeyFrame(KeyFrame.makeScale(0.71, 0.71, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeScale(0.0, 0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.1));
			candy.addTimelinewithID(timeline, 0);
			candy.playTimeline(0);
			timeline.delegateTimelineDelegate = aniPool;
			aniPool.addChild(candy);
			dd.callObjectSelectorParamafterDelay(selector_gameWon, null, 2.0);
			calculateScore();
			releaseAllRopes(false);
		}

		public virtual void gameLost()
		{
			dd.cancelAllDispatches();
			target.playAnimationtimeline(102, 5);
			CTRSoundMgr._playSound(SND_MONSTER_SAD);
			dd.callObjectSelectorParamafterDelay(selector_animateLevelRestart, null, 1.0);
			gameSceneDelegate.gameLost();
		}

		public override void draw()
		{
			OpenGL.glClear(0);
			base.preDraw();
			camera.applyCameraTransformation();
			OpenGL.glEnable(0);
			OpenGL.glDisable(1);
			Vector pos = MathHelper.vectDiv(camera.pos, 1.25f);
			back.updateWithCameraPos(pos);
			float num = base.canvas.xOffsetScaled;
			float num2 = 0f;
			OpenGL.glPushMatrix();
			OpenGL.glTranslatef(num, num2, 0.0);
			OpenGL.glScalef(back.scaleX, back.scaleY, 1.0);
			OpenGL.glTranslatef(0f - num, 0f - num2, 0.0);
			OpenGL.glTranslatef(base.canvas.xOffsetScaled, 0.0, 0.0);
			back.draw();
			if (mapHeight > FrameworkTypes.SCREEN_HEIGHT)
			{
				float num3 = FrameworkTypes.RTD(2.0);
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				int pack = cTRRootController.getPack();
				Texture2D texture = Application.getTexture(IMG_BGR_01_P2 + pack * 2);
				int num4 = 0;
				float num5 = texture.quadOffsets[num4].y;
				iframework.Rectangle r = texture.quadRects[num4];
				r.y += num3;
				r.h -= num3 * 2f;
				GLDrawer.drawImagePart(texture, r, 0.0, num5 + num3);
			}
			OpenGL.glEnable(1);
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			if (earthAnims != null)
			{
				foreach (Image earthAnim in earthAnims)
				{
					earthAnim.draw();
				}
			}
			OpenGL.glTranslatef(-base.canvas.xOffsetScaled, 0.0, 0.0);
			OpenGL.glPopMatrix();
			OpenGL.glEnable(1);
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			pollenDrawer.draw();
			if (gravityButton != null)
			{
				gravityButton.draw();
			}
			OpenGL.glColor4f(Color.White);
			OpenGL.glEnable(0);
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			support.draw();
			target.draw();
			foreach (Text tutorial in tutorials)
			{
				tutorial.draw();
			}
			foreach (GameObject tutorialImage in tutorialImages)
			{
				tutorialImage.draw();
			}
			foreach (Razor razor in razors)
			{
				razor.draw();
			}
			foreach (RotatedCircle rotatedCircle in rotatedCircles)
			{
				rotatedCircle.draw();
			}
			foreach (GameObject bubble in bubbles)
			{
				bubble.draw();
			}
			foreach (GameObject pump in pumps)
			{
				pump.draw();
			}
			foreach (Spikes spike in this.spikes)
			{
				spike.draw();
			}
			foreach (Bouncer bouncer in bouncers)
			{
				bouncer.draw();
			}
			foreach (Sock sock in socks)
			{
				sock.y -= 85f;
				sock.draw();
				sock.y += 85f;
			}
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			foreach (Grab bungee in bungees)
			{
				bungee.drawBack();
			}
			foreach (Grab bungee2 in bungees)
			{
				bungee2.draw();
			}
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			foreach (GameObject star in stars)
			{
				star.draw();
			}
			if (!noCandy && targetSock == null)
			{
				candy.x = this.star.pos.x;
				candy.y = this.star.pos.y;
				candy.draw();
				if (candyBlink.getCurrentTimeline() != null)
				{
					OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE);
					candyBlink.draw();
					OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
				}
			}
			if (twoParts != 2)
			{
				if (!noCandyL)
				{
					candyL.x = starL.pos.x;
					candyL.y = starL.pos.y;
					candyL.draw();
				}
				if (!noCandyR)
				{
					candyR.x = starR.pos.x;
					candyR.y = starR.pos.y;
					candyR.draw();
				}
			}
			foreach (Grab bungee3 in bungees)
			{
				if (bungee3.hasSpider)
				{
					bungee3.drawSpider();
				}
			}
			aniPool.draw();
			bool nightLevel2 = nightLevel;
			OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			OpenGL.glDisable(0);
			OpenGL.glColor4f(Color.White);
			drawCuts();
			OpenGL.glEnable(0);
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			camera.cancelCameraTransformation();
			staticAniPool.draw();
			if (nightLevel)
			{
				OpenGL.glDisable(4);
			}
			base.postDraw();
		}

		public virtual void drawCuts()
		{
			for (int i = 0; i < 5; i++)
			{
				int num = fingerCuts[i].count();
				if (num <= 0)
				{
					continue;
				}
				float num2 = FrameworkTypes.RTD(6.0);
				float num3 = 1f;
				int num4 = 0;
				int j = 0;
				FingerCut fingerCut = null;
				Vector[] array = new Vector[num + 1];
				int num5 = 0;
				for (; j < num; j++)
				{
					fingerCut = (FingerCut)fingerCuts[i].objectAtIndex(j);
					if (j == 0)
					{
						array[num5++] = fingerCut.start;
					}
					array[num5++] = fingerCut.end;
				}
				List<Vector> list = new List<Vector>();
				Vector vector = default(Vector);
				bool flag = true;
				for (int k = 0; k < array.Count(); k++)
				{
					if (k == 0)
					{
						list.Add(array[k]);
					}
					else if (array[k].x != vector.x || array[k].y != vector.y)
					{
						list.Add(array[k]);
						flag = false;
					}
					vector = array[k];
				}
				if (flag)
				{
					continue;
				}
				array = list.ToArray();
				num = array.Count() - 1;
				int num6 = num * 2;
				float[] array2 = new float[num6 * 2];
				float num7 = 1f / (float)num6;
				float num8 = 0f;
				int num9 = 0;
				while (true)
				{
					if ((double)num8 > 1.0)
					{
						num8 = 1f;
					}
					Vector vector2 = GLDrawer.calcPathBezier(array, num + 1, num8);
					if (num9 > array2.Count() - 2)
					{
						break;
					}
					array2[num9++] = vector2.x;
					array2[num9++] = vector2.y;
					if ((double)num8 == 1.0)
					{
						break;
					}
					num8 += num7;
				}
				float num10 = num2 / (float)num6;
				float[] array3 = new float[num6 * 4];
				for (int l = 0; l < num6 - 1; l++)
				{
					float s = num3;
					float s2 = ((l == num6 - 2) ? 1f : (num3 + num10));
					Vector vector3 = MathHelper.vect(array2[l * 2], array2[l * 2 + 1]);
					Vector v = MathHelper.vect(array2[(l + 1) * 2], array2[(l + 1) * 2 + 1]);
					Vector v2 = MathHelper.vectSub(v, vector3);
					Vector v3 = MathHelper.vectNormalize(v2);
					Vector v4 = MathHelper.vectRperp(v3);
					Vector v5 = MathHelper.vectPerp(v3);
					if (num4 == 0)
					{
						Vector vector4 = MathHelper.vectAdd(vector3, MathHelper.vectMult(v4, s));
						Vector vector5 = MathHelper.vectAdd(vector3, MathHelper.vectMult(v5, s));
						array3[num4++] = vector5.x;
						array3[num4++] = vector5.y;
						array3[num4++] = vector4.x;
						array3[num4++] = vector4.y;
					}
					Vector vector6 = MathHelper.vectAdd(v, MathHelper.vectMult(v4, s2));
					Vector vector7 = MathHelper.vectAdd(v, MathHelper.vectMult(v5, s2));
					array3[num4++] = vector7.x;
					array3[num4++] = vector7.y;
					array3[num4++] = vector6.x;
					array3[num4++] = vector6.y;
					num3 += num10;
				}
				OpenGL.glColor4f(Color.White);
				OpenGL.glVertexPointer(2, 5, 0, array3);
				OpenGL.glDrawArrays(8, 0, num4 / 2);
			}
		}

		public virtual void handlePumpFlowPtSkin(Pump p, ConstraintedPoint s, GameObject c, float delta)
		{
			float num = 624f;
			if (GameObject.rectInObject(p.x - num, p.y - num, p.x + num, p.y + num, c))
			{
				Vector v = MathHelper.vect(c.x, c.y);
				Vector vector = default(Vector);
				vector.x = p.x - p.bb.w / 2f;
				Vector vector2 = default(Vector);
				vector2.x = p.x + p.bb.w / 2f;
				vector.y = (vector2.y = p.y);
				if (p.angle != 0.0)
				{
					v = MathHelper.vectRotateAround(v, 0.0 - p.angle, p.x, p.y);
				}
				if (v.y < vector.y && MathHelper.rectInRect((float)((double)v.x - (double)c.bb.w / 2.0), (float)((double)v.y - (double)c.bb.h / 2.0), (float)((double)v.x + (double)c.bb.w / 2.0), (float)((double)v.y + (double)c.bb.h / 2.0), vector.x, vector.y - num, vector2.x, vector2.y))
				{
					float num2 = num * 2f;
					float num3 = num2 * (num - (vector.y - v.y)) / num;
					Vector v2 = MathHelper.vect(0f, 0f - num3);
					v2 = MathHelper.vectRotate(v2, p.angle);
					s.applyImpulseDelta(v2, delta);
				}
			}
		}

		public virtual void handleBouncePtDelta(Bouncer b, ConstraintedPoint s, float delta)
		{
			if (!b.skip)
			{
				b.skip = true;
				Vector v = MathHelper.vectSub(s.prevPos, s.pos);
				int num = ((!(MathHelper.vectRotateAround(s.prevPos, 0f - b.angle, b.x, b.y).y < b.y)) ? 1 : (-1));
				float s2 = MathHelper.MAX(MathHelper.vectLength(v) * 40f, 840.0) * (float)num;
				Vector v2 = MathHelper.vectForAngle(b.angle);
				Vector impulse = MathHelper.vectMult(MathHelper.vectPerp(v2), s2);
				s.pos = MathHelper.vectRotateAround(s.pos, 0f - b.angle, b.x, b.y);
				s.prevPos = MathHelper.vectRotateAround(s.prevPos, 0f - b.angle, b.x, b.y);
				s.prevPos.y = s.pos.y;
				s.pos = MathHelper.vectRotateAround(s.pos, b.angle, b.x, b.y);
				s.prevPos = MathHelper.vectRotateAround(s.prevPos, b.angle, b.x, b.y);
				s.applyImpulseDelta(impulse, delta);
				b.playTimeline(0);
				CTRSoundMgr._playSound(SND_BOUNCER);
			}
		}

		public virtual void operatePump(Pump p, float delta)
		{
			p.playTimeline(0);
			CTRSoundMgr._playSound(MathHelper.RND_RANGE(SND_PUMP_1, SND_PUMP_4));
			Image grid = Image.Image_createWithResID(IMG_OBJ_PUMP);
			PumpDirt pumpDirt = new PumpDirt().initWithTotalParticlesAngleandImageGrid(5, MathHelper.RADIANS_TO_DEGREES((float)p.angle) - 90f, grid);
			pumpDirt.particlesDelegate = aniPool.particlesFinished;
			Vector v = MathHelper.vect(p.x + 80f, p.y);
			v = MathHelper.vectRotateAround(v, p.angle - Math.PI / 2.0, p.x, p.y);
			pumpDirt.x = v.x;
			pumpDirt.y = v.y;
			pumpDirt.startSystem(5);
			aniPool.addChild(pumpDirt);
			if (!noCandy)
			{
				handlePumpFlowPtSkin(p, star, candy, delta);
			}
			if (twoParts != 2)
			{
				if (!noCandyL)
				{
					handlePumpFlowPtSkin(p, starL, candyL, delta);
				}
				if (!noCandyR)
				{
					handlePumpFlowPtSkin(p, starR, candyR, delta);
				}
			}
		}

		public virtual int cutWithRazorOrLine1Line2Immediate(Razor r, Vector v1, Vector v2, bool im)
		{
			int num = 0;
			for (int i = 0; i < bungees.count(); i++)
			{
				Grab grab = (Grab)bungees.objectAtIndex(i);
				Bungee rope = grab.rope;
				if (rope == null || rope.cut != -1)
				{
					continue;
				}
				for (int j = 0; j < rope.parts.Count - 1; j++)
				{
					ConstraintedPoint constraintedPoint = rope.parts[j];
					ConstraintedPoint constraintedPoint2 = rope.parts[j + 1];
					bool flag = false;
					if (r == null)
					{
						flag = (!grab.wheel || !MathHelper.lineInRect(v1.x, v1.y, v2.x, v2.y, grab.x - 110f, grab.y - 110f, 220f, 220f)) && lineInLine(v1.x, v1.y, v2.x, v2.y, constraintedPoint.pos.x, constraintedPoint.pos.y, constraintedPoint2.pos.x, constraintedPoint2.pos.y);
					}
					else if (constraintedPoint.prevPos.x != vectUndefinedValue)
					{
						float x1l = minOf4(constraintedPoint.pos.x, constraintedPoint.prevPos.x, constraintedPoint2.pos.x, constraintedPoint2.prevPos.x);
						float y1t = minOf4(constraintedPoint.pos.y, constraintedPoint.prevPos.y, constraintedPoint2.pos.y, constraintedPoint2.prevPos.y);
						float x1r = maxOf4(constraintedPoint.pos.x, constraintedPoint.prevPos.x, constraintedPoint2.pos.x, constraintedPoint2.prevPos.x);
						float y1b = maxOf4(constraintedPoint.pos.y, constraintedPoint.prevPos.y, constraintedPoint2.pos.y, constraintedPoint2.prevPos.y);
						flag = MathHelper.rectInRect(x1l, y1t, x1r, y1b, r.drawX, r.drawY, r.drawX + (float)r.width, r.drawY + (float)r.height);
					}
					if (flag)
					{
						num++;
						if (grab.hasSpider && grab.spiderActive)
						{
							spiderBusted(grab);
						}
						CTRSoundMgr._playSound(SND_ROPE_BLEAK_1 + rope.relaxed);
						rope.setCut(j);
						if (im)
						{
							rope.cutTime = 0f;
							rope.removePart(j);
						}
						return num;
					}
				}
			}
			return num;
		}

		public virtual void spiderBusted(Grab g)
		{
			int num = Preferences._getIntForKey("PREFS_SPIDERS_BUSTED") + 1;
			Preferences._setIntforKey(num, "PREFS_SPIDERS_BUSTED", false);
			if (num == 40)
			{
				CTRRootController.postAchievementName(CTRPreferences.acSpiderBuster, FrameworkTypes.ACHIEVEMENT_STRING("\"Spider Busted\""));
			}
			if (num == 200)
			{
				CTRRootController.postAchievementName(CTRPreferences.acSpiderTamer, FrameworkTypes.ACHIEVEMENT_STRING("\"Spider Tammer\""));
			}
			CTRSoundMgr._playSound(SND_SPIDER_FALL);
			g.hasSpider = false;
			Image image = Image.Image_createWithResIDQuad(IMG_OBJ_SPIDER, 11);
			image.doRestoreCutTransparency();
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
			if (gravityButton != null && !gravityNormal)
			{
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, (double)g.spider.y + 50.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y - FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			}
			else
			{
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, (double)g.spider.y - 50.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
				timeline.addKeyFrame(KeyFrame.makePos(g.spider.x, g.spider.y + FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			}
			timeline.addKeyFrame(KeyFrame.makeRotation(0.0, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
			timeline.addKeyFrame(KeyFrame.makeRotation(MathHelper.RND_RANGE(-120, 120), KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 1.0));
			image.addTimelinewithID(timeline, 0);
			image.playTimeline(0);
			image.x = g.spider.x;
			image.y = g.spider.y;
			image.anchor = 18;
			timeline.delegateTimelineDelegate = aniPool;
			aniPool.addChild(image);
		}

		public virtual void spiderWon(Grab sg)
		{
			CTRSoundMgr._playSound(SND_SPIDER_WIN);
			int num = bungees.count();
			for (int i = 0; i < num; i++)
			{
				Grab grab = (Grab)bungees.objectAtIndex(i);
				Bungee rope = grab.rope;
				if (rope != null && rope.tail == star)
				{
					if (rope.cut == -1)
					{
						rope.setCut(rope.parts.Count - 2);
						rope.forceWhite = false;
					}
					if (grab.hasSpider && grab.spiderActive && sg != grab)
					{
						spiderBusted(grab);
					}
				}
			}
			sg.hasSpider = false;
			noCandy = true;
			Image image = Image.Image_createWithResIDQuad(IMG_OBJ_SPIDER, 12);
			image.doRestoreCutTransparency();
			candy.anchor = (candy.parentAnchor = 18);
			candy.x = 0f;
			candy.y = -5f;
			image.addChild(candy);
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(3);
			if (gravityButton != null && !gravityNormal)
			{
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y - 10.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y + 70.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, sg.spider.y - FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			}
			else
			{
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y - 10.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.0));
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, (double)sg.spider.y - 70.0, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_OUT, 0.3));
				timeline.addKeyFrame(KeyFrame.makePos(sg.spider.x, sg.spider.y + FrameworkTypes.SCREEN_HEIGHT, KeyFrame.TransitionType.FRAME_TRANSITION_EASE_IN, 1.0));
			}
			image.addTimelinewithID(timeline, 0);
			image.playTimeline(0);
			image.x = sg.spider.x;
			image.y = sg.spider.y - 10f;
			image.anchor = 18;
			timeline.delegateTimelineDelegate = aniPool;
			aniPool.addChild(image);
			if (restartState != 0)
			{
				dd.callObjectSelectorParamafterDelay(selector_gameLost, null, 2.0);
			}
		}

		public virtual void popCandyBubble(bool left)
		{
			if (twoParts != 2)
			{
				if (left)
				{
					candyBubbleL = null;
					candyBubbleAnimationL.visible = false;
					popBubbleAtXY(candyL.x, candyL.y);
				}
				else
				{
					candyBubbleR = null;
					candyBubbleAnimationR.visible = false;
					popBubbleAtXY(candyR.x, candyR.y);
				}
			}
			else
			{
				candyBubble = null;
				candyBubbleAnimation.visible = false;
				popBubbleAtXY(candy.x, candy.y);
			}
		}

		public virtual void popBubbleAtXY(float bx, float by)
		{
			CTRSoundMgr._playSound(SND_BUBBLE_BREAK);
			Animation animation = Animation.Animation_createWithResID(IMG_OBJ_BUBBLE_POP);
			animation.doRestoreCutTransparency();
			animation.x = bx;
			animation.y = by;
			animation.anchor = 18;
			int n = animation.addAnimationDelayLoopFirstLast(0.05, Timeline.LoopType.TIMELINE_NO_LOOP, 0, 11);
			animation.getTimeline(n).delegateTimelineDelegate = aniPool;
			animation.playTimeline(0);
			aniPool.addChild(animation);
		}

		public virtual bool handleBubbleTouchXY(ConstraintedPoint s, float tx, float ty)
		{
			if (MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, s.pos.x - 60f, s.pos.y - 60f, 120f, 120f))
			{
				popCandyBubble(s == starL);
				int num = Preferences._getIntForKey("PREFS_BUBBLES_POPPED") + 1;
				Preferences._setIntforKey(num, "PREFS_BUBBLES_POPPED", false);
				if (num == 50)
				{
					CTRRootController.postAchievementName(CTRPreferences.acBubblePopper, FrameworkTypes.ACHIEVEMENT_STRING("\"Bubble Popper\""));
				}
				if (num == 300)
				{
					CTRRootController.postAchievementName(CTRPreferences.acBubbleMaster, FrameworkTypes.ACHIEVEMENT_STRING("\"Bubble Master\""));
				}
				return true;
			}
			return false;
		}

		public virtual void resetBungeeHighlight()
		{
			for (int i = 0; i < bungees.count(); i++)
			{
				Grab grab = (Grab)bungees.objectAtIndex(i);
				Bungee rope = grab.rope;
				if (rope != null && rope.cut == -1)
				{
					rope.highlighted = false;
				}
			}
		}

		public virtual Bungee getNearestBungeeSegmentByBeziersPointsatXYgrab(ref Vector s, float tx, float ty, ref Grab grab)
		{
			float num = 60f;
			Bungee result = null;
			float num2 = num;
			Vector v = MathHelper.vect(tx, ty);
			for (int i = 0; i < bungees.count(); i++)
			{
				Grab grab2 = (Grab)bungees.objectAtIndex(i);
				Bungee rope = grab2.rope;
				if (rope == null)
				{
					continue;
				}
				for (int j = 0; j < rope.drawPtsCount; j += 2)
				{
					Vector vector = MathHelper.vect(rope.drawPts[j], rope.drawPts[j + 1]);
					float num3 = MathHelper.vectDistance(vector, v);
					if (num3 < num && num3 < num2)
					{
						num2 = num3;
						result = rope;
						s = vector;
						grab = grab2;
					}
				}
			}
			return result;
		}

		public virtual Bungee getNearestBungeeSegmentByConstraintsforGrab(ref Vector s, Grab g)
		{
			float num = vectUndefinedValue;
			Bungee result = null;
			float num2 = num;
			Vector v = s;
			Bungee rope = g.rope;
			if (rope == null || rope.cut != -1)
			{
				return null;
			}
			for (int i = 0; i < rope.parts.Count - 1; i++)
			{
				ConstraintedPoint constraintedPoint = rope.parts[i];
				float num3 = MathHelper.vectDistance(constraintedPoint.pos, v);
				if (num3 < num2 && (!g.wheel || !MathHelper.pointInRect(constraintedPoint.pos.x, constraintedPoint.pos.y, g.x - 110f, g.y - 110f, 220f, 220f)))
				{
					num2 = num3;
					result = rope;
					s = constraintedPoint.pos;
				}
			}
			return result;
		}

		public virtual bool touchDownXYIndex(float tx, float ty, int ti)
		{
			if (ignoreTouches)
			{
				if (camera.type == CAMERA_TYPE.CAMERA_SPEED_PIXELS)
				{
					fastenCamera = true;
				}
				return true;
			}
			if (ti >= 5)
			{
				return true;
			}
			if (gravityButton != null)
			{
				Button button = (Button)gravityButton.getChild(gravityButton.on() ? 1 : 0);
				if (button.isInTouchZoneXYforTouchDown(tx + camera.pos.x, ty + camera.pos.y, true))
				{
					gravityTouchDown = ti;
				}
			}
			Vector vector = MathHelper.vect(tx, ty);
			if (candyBubble != null && handleBubbleTouchXY(star, tx, ty))
			{
				return true;
			}
			if (twoParts != 2)
			{
				if (candyBubbleL != null && handleBubbleTouchXY(starL, tx, ty))
				{
					return true;
				}
				if (candyBubbleR != null && handleBubbleTouchXY(starR, tx, ty))
				{
					return true;
				}
			}
			if (!dragging[ti])
			{
				dragging[ti] = true;
				prevStartPos[ti] = (startPos[ti] = vector);
			}
			foreach (Spikes spike in this.spikes)
			{
				if (spike.rotateButton != null && spike.touchIndex == -1 && spike.rotateButton.onTouchDownXY(tx + camera.pos.x, ty + camera.pos.y))
				{
					spike.touchIndex = ti;
					return true;
				}
			}
			int num = pumps.count();
			for (int i = 0; i < num; i++)
			{
				Pump pump = (Pump)pumps.objectAtIndex(i);
				if (GameObject.pointInObject(MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y), pump))
				{
					pump.pumpTouchTimer = 0.05f;
					pump.pumpTouch = ti;
					return true;
				}
			}
			RotatedCircle rotatedCircle = null;
			bool flag = false;
			bool flag2 = false;
			foreach (RotatedCircle rotatedCircle5 in rotatedCircles)
			{
				float num2 = MathHelper.vectDistance(MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y), rotatedCircle5.handle1);
				float num3 = MathHelper.vectDistance(MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y), rotatedCircle5.handle2);
				if ((!(num2 < 90f) || rotatedCircle5.hasOneHandle()) && !(num3 < 90f))
				{
					continue;
				}
				foreach (RotatedCircle rotatedCircle6 in rotatedCircles)
				{
					if (rotatedCircles.getObjectIndex(rotatedCircle6) > rotatedCircles.getObjectIndex(rotatedCircle5))
					{
						float num4 = MathHelper.vectDistance(MathHelper.vect(rotatedCircle6.x, rotatedCircle6.y), MathHelper.vect(rotatedCircle5.x, rotatedCircle5.y));
						if (num4 + rotatedCircle6.sizeInPixels <= rotatedCircle5.sizeInPixels)
						{
							flag = true;
						}
						if (num4 <= rotatedCircle5.sizeInPixels + rotatedCircle6.sizeInPixels)
						{
							flag2 = true;
						}
					}
				}
				rotatedCircle5.lastTouch = MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y);
				rotatedCircle5.operating = ti;
				if (num2 < 90f)
				{
					rotatedCircle5.setIsLeftControllerActive(true);
				}
				if (num3 < 90f)
				{
					rotatedCircle5.setIsRightControllerActive(true);
				}
				rotatedCircle = rotatedCircle5;
				break;
			}
			if (rotatedCircles.getObjectIndex(rotatedCircle) != rotatedCircles.count() - 1 && flag2 && !flag)
			{
				Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
				timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.transparentRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.0));
				timeline.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
				Timeline timeline2 = new Timeline().initWithMaxKeyFramesOnTrack(1);
				timeline2.addKeyFrame(KeyFrame.makeColor(RGBAColor.solidOpaqueRGBA, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0.2));
				timeline2.delegateTimelineDelegate = this;
				RotatedCircle rotatedCircle4 = (RotatedCircle)rotatedCircle.copy();
				rotatedCircle4.addTimeline(timeline2);
				rotatedCircle4.playTimeline(0);
				rotatedCircle.addTimeline(timeline);
				rotatedCircle.playTimeline(0);
				rotatedCircles.setObjectAt(rotatedCircle4, rotatedCircles.getObjectIndex(rotatedCircle));
				rotatedCircles.addObject(rotatedCircle);
				rotatedCircle.release();
			}
			foreach (Grab bungee in bungees)
			{
				if (bungee.wheel && MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, bungee.x - 110f, bungee.y - 110f, 220f, 220f))
				{
					bungee.handleWheelTouch(MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y));
					bungee.wheelOperating = ti;
				}
				if ((double)bungee.moveLength > 0.0 && MathHelper.pointInRect(tx + camera.pos.x, ty + camera.pos.y, bungee.x - 65f, bungee.y - 65f, 130f, 130f))
				{
					bungee.moverDragging = ti;
					return true;
				}
			}
			if (clickToCut && !ignoreTouches)
			{
				Vector s = default(Vector);
				Grab grab2 = null;
				Bungee nearestBungeeSegmentByBeziersPointsatXYgrab = getNearestBungeeSegmentByBeziersPointsatXYgrab(ref s, tx + camera.pos.x, ty + camera.pos.y, ref grab2);
				if (nearestBungeeSegmentByBeziersPointsatXYgrab != null && nearestBungeeSegmentByBeziersPointsatXYgrab.highlighted && getNearestBungeeSegmentByConstraintsforGrab(ref s, grab2) != null)
				{
					cutWithRazorOrLine1Line2Immediate(null, s, s, false);
				}
			}
			return true;
		}

		public virtual bool touchUpXYIndex(float tx, float ty, int ti)
		{
			if (ignoreTouches)
			{
				return true;
			}
			dragging[ti] = false;
			if (ti >= 5)
			{
				return true;
			}
			if (gravityButton != null && gravityTouchDown == ti)
			{
				Button button = (Button)gravityButton.getChild(gravityButton.on() ? 1 : 0);
				if (button.isInTouchZoneXYforTouchDown(tx + camera.pos.x, ty + camera.pos.y, true))
				{
					gravityButton.toggle();
					onButtonPressed(BUTTON_GRAVITY);
				}
				gravityTouchDown = -1;
			}
			foreach (Spikes spike in this.spikes)
			{
				if (spike.rotateButton != null && spike.touchIndex == ti)
				{
					spike.touchIndex = -1;
					if (spike.rotateButton.onTouchUpXY(tx + camera.pos.x, ty + camera.pos.y))
					{
						return true;
					}
				}
			}
			foreach (RotatedCircle rotatedCircle in rotatedCircles)
			{
				if (rotatedCircle.operating == ti)
				{
					rotatedCircle.operating = -1;
					rotatedCircle.soundPlaying = -1;
					rotatedCircle.setIsLeftControllerActive(false);
					rotatedCircle.setIsRightControllerActive(false);
				}
			}
			foreach (Grab bungee in bungees)
			{
				if (bungee.wheel && bungee.wheelOperating == ti)
				{
					bungee.wheelOperating = -1;
				}
				if ((double)bungee.moveLength > 0.0 && bungee.moverDragging == ti)
				{
					bungee.moverDragging = -1;
				}
			}
			return true;
		}

		public virtual bool touchMoveXYIndex(float tx, float ty, int ti)
		{
			if (ignoreTouches)
			{
				return true;
			}
			Vector vector = MathHelper.vect(tx, ty);
			if (ti >= 5)
			{
				return true;
			}
			foreach (Pump pump3 in pumps)
			{
				if (pump3.pumpTouch == ti && (double)pump3.pumpTouchTimer != 0.0 && (double)MathHelper.vectDistance(startPos[ti], vector) > 10.0)
				{
					pump3.pumpTouchTimer = 0f;
				}
			}
			if (rotatedCircles != null)
			{
				for (int i = 0; i < rotatedCircles.count(); i++)
				{
					RotatedCircle rotatedCircle = (RotatedCircle)rotatedCircles[i];
					if (rotatedCircle == null || rotatedCircle.operating != ti)
					{
						continue;
					}
					Vector v = MathHelper.vect(rotatedCircle.x, rotatedCircle.y);
					Vector vector2 = MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y);
					Vector v2 = MathHelper.vectSub(rotatedCircle.lastTouch, v);
					Vector v3 = MathHelper.vectSub(vector2, v);
					float num = MathHelper.vectAngleNormalized(v3) - MathHelper.vectAngleNormalized(v2);
					float initial_rotation = MathHelper.DEGREES_TO_RADIANS(rotatedCircle.rotation);
					rotatedCircle.rotation += MathHelper.RADIANS_TO_DEGREES(num);
					float a = MathHelper.DEGREES_TO_RADIANS(rotatedCircle.rotation);
					a = FBOUND_PI(a);
					rotatedCircle.handle1 = MathHelper.vectRotateAround(rotatedCircle.inithanlde1, a, rotatedCircle.x, rotatedCircle.y);
					rotatedCircle.handle2 = MathHelper.vectRotateAround(rotatedCircle.inithanlde2, a, rotatedCircle.x, rotatedCircle.y);
					int num2 = ((num > 0f) ? SND_SCRATCH_IN : SND_SCRATCH_OUT);
					if ((double)Math.Abs(num) < 0.07)
					{
						num2 = -1;
					}
					if (rotatedCircle.soundPlaying != num2 && num2 != -1)
					{
						CTRSoundMgr._playSound(num2);
						rotatedCircle.soundPlaying = num2;
					}
					for (int j = 0; j < bungees.count(); j++)
					{
						Grab grab = (Grab)bungees[j];
						if (MathHelper.vectDistance(MathHelper.vect(grab.x, grab.y), MathHelper.vect(rotatedCircle.x, rotatedCircle.y)) <= rotatedCircle.sizeInPixels + 5f)
						{
							if (grab.initial_rotatedCircle != rotatedCircle)
							{
								grab.initial_x = grab.x;
								grab.initial_y = grab.y;
								grab.initial_rotatedCircle = rotatedCircle;
								grab.initial_rotation = initial_rotation;
							}
							float a2 = MathHelper.DEGREES_TO_RADIANS(rotatedCircle.rotation) - grab.initial_rotation;
							a2 = FBOUND_PI(a2);
							Vector vector3 = MathHelper.vectRotateAround(MathHelper.vect(grab.initial_x, grab.initial_y), a2, rotatedCircle.x, rotatedCircle.y);
							grab.x = vector3.x;
							grab.y = vector3.y;
							if (grab.rope != null)
							{
								grab.rope.bungeeAnchor.pos = MathHelper.vect(grab.x, grab.y);
								grab.rope.bungeeAnchor.pin = grab.rope.bungeeAnchor.pos;
							}
							if (grab.radius != -1f)
							{
								grab.reCalcCircle();
							}
						}
					}
					for (int k = 0; k < pumps.count(); k++)
					{
						Pump pump2 = (Pump)pumps[k];
						if (MathHelper.vectDistance(MathHelper.vect(pump2.x, pump2.y), MathHelper.vect(rotatedCircle.x, rotatedCircle.y)) <= rotatedCircle.sizeInPixels + 5f)
						{
							if (pump2.initial_rotatedCircle != rotatedCircle)
							{
								pump2.initial_x = pump2.x;
								pump2.initial_y = pump2.y;
								pump2.initial_rotatedCircle = rotatedCircle;
								pump2.initial_rotation = initial_rotation;
							}
							float a3 = MathHelper.DEGREES_TO_RADIANS(rotatedCircle.rotation) - pump2.initial_rotation;
							a3 = FBOUND_PI(a3);
							Vector vector4 = MathHelper.vectRotateAround(MathHelper.vect(pump2.initial_x, pump2.initial_y), a3, rotatedCircle.x, rotatedCircle.y);
							pump2.x = vector4.x;
							pump2.y = vector4.y;
							pump2.rotation += MathHelper.RADIANS_TO_DEGREES(num);
							pump2.updateRotation();
						}
					}
					for (int l = 0; l < bubbles.count(); l++)
					{
						Bubble bubble = (Bubble)bubbles[l];
						if (MathHelper.vectDistance(MathHelper.vect(bubble.x, bubble.y), MathHelper.vect(rotatedCircle.x, rotatedCircle.y)) <= rotatedCircle.sizeInPixels + 10f && bubble != candyBubble && bubble != candyBubbleR && bubble != candyBubbleL)
						{
							if (bubble.initial_rotatedCircle != rotatedCircle)
							{
								bubble.initial_x = bubble.x;
								bubble.initial_y = bubble.y;
								bubble.initial_rotatedCircle = rotatedCircle;
								bubble.initial_rotation = initial_rotation;
							}
							float a4 = MathHelper.DEGREES_TO_RADIANS(rotatedCircle.rotation) - bubble.initial_rotation;
							a4 = FBOUND_PI(a4);
							Vector vector5 = MathHelper.vectRotateAround(MathHelper.vect(bubble.initial_x, bubble.initial_y), a4, rotatedCircle.x, rotatedCircle.y);
							bubble.x = vector5.x;
							bubble.y = vector5.y;
						}
					}
					if (MathHelper.pointInRect(target.x, target.y, rotatedCircle.x - rotatedCircle.size, rotatedCircle.y - rotatedCircle.size, 2f * rotatedCircle.size, 2f * rotatedCircle.size))
					{
						Vector vector6 = MathHelper.vectRotateAround(MathHelper.vect(target.x, target.y), num, rotatedCircle.x, rotatedCircle.y);
						target.x = vector6.x;
						target.y = vector6.y;
					}
					rotatedCircle.lastTouch = vector2;
					return true;
				}
			}
			int num3 = bungees.count();
			for (int m = 0; m < num3; m++)
			{
				Grab grab2 = (Grab)bungees.objectAtIndex(m);
				if (grab2 == null)
				{
					continue;
				}
				if (grab2.wheel && grab2.wheelOperating == ti)
				{
					grab2.handleWheelRotate(MathHelper.vect(tx + camera.pos.x, ty + camera.pos.y));
					return true;
				}
				if ((double)grab2.moveLength > 0.0 && grab2.moverDragging == ti)
				{
					if (grab2.moveVertical)
					{
						grab2.y = MathHelper.FIT_TO_BOUNDARIES(ty + camera.pos.y, grab2.minMoveValue, grab2.maxMoveValue);
					}
					else
					{
						grab2.x = MathHelper.FIT_TO_BOUNDARIES(tx + camera.pos.x, grab2.minMoveValue, grab2.maxMoveValue);
					}
					if (grab2.rope != null)
					{
						grab2.rope.bungeeAnchor.pos = MathHelper.vect(grab2.x, grab2.y);
						grab2.rope.bungeeAnchor.pin = grab2.rope.bungeeAnchor.pos;
					}
					if (grab2.radius != -1f)
					{
						grab2.reCalcCircle();
					}
					return true;
				}
			}
			if (dragging[ti])
			{
				Vector start = MathHelper.vectAdd(startPos[ti], camera.pos);
				Vector end = MathHelper.vectAdd(MathHelper.vect(tx, ty), camera.pos);
				FingerCut fingerCut = (FingerCut)new FingerCut().init();
				fingerCut.start = start;
				fingerCut.end = end;
				fingerCut.startSize = 5f;
				fingerCut.endSize = 5f;
				fingerCut.c = RGBAColor.whiteRGBA;
				fingerCuts[ti].addObject(fingerCut);
				int num4 = 0;
				foreach (FingerCut item in fingerCuts[ti])
				{
					num4 += cutWithRazorOrLine1Line2Immediate(null, item.start, item.end, false);
				}
				if (num4 > 0)
				{
					freezeCamera = false;
					if (ropesCutAtOnce > 0 && (double)ropeAtOnceTimer > 0.0)
					{
						ropesCutAtOnce += num4;
					}
					else
					{
						ropesCutAtOnce = num4;
					}
					ropeAtOnceTimer = 0.1f;
					int num5 = Preferences._getIntForKey("PREFS_ROPES_CUT") + 1;
					Preferences._setIntforKey(num5, "PREFS_ROPES_CUT", false);
					if (num5 == 100)
					{
						CTRRootController.postAchievementName(CTRPreferences.acRopeCutter, FrameworkTypes.ACHIEVEMENT_STRING("\"Rope Cutter\""));
					}
					if (ropesCutAtOnce >= 3 && ropesCutAtOnce < 5)
					{
						CTRRootController.postAchievementName(CTRPreferences.acQuickFinger, FrameworkTypes.ACHIEVEMENT_STRING("\"Quick Finger\""));
					}
					if (ropesCutAtOnce >= 5)
					{
						CTRRootController.postAchievementName(CTRPreferences.acMasterFinger, FrameworkTypes.ACHIEVEMENT_STRING("\"Master Finger\""));
					}
					if (num5 == 800)
					{
						CTRRootController.postAchievementName(CTRPreferences.acRopeCutterManiac, FrameworkTypes.ACHIEVEMENT_STRING("\"Rope Cutter Maniac\""));
					}
					if (num5 == 2000)
					{
						CTRRootController.postAchievementName(CTRPreferences.acUltimateRopeCutter, FrameworkTypes.ACHIEVEMENT_STRING("\"Ultimate Rope Cutter\""));
					}
				}
				prevStartPos[ti] = startPos[ti];
				startPos[ti] = vector;
			}
			return true;
		}

		public virtual bool touchDraggedXYIndex(float tx, float ty, int index)
		{
			if (index > 5)
			{
				return false;
			}
			slastTouch = MathHelper.vect(tx, ty);
			return true;
		}

		public virtual void onButtonPressed(int n)
		{
			if ((double)MaterialPoint.globalGravity.y == 784.0)
			{
				MaterialPoint.globalGravity.y = -784f;
				gravityNormal = false;
				CTRSoundMgr._playSound(SND_GRAVITY_ON);
			}
			else
			{
				MaterialPoint.globalGravity.y = 784f;
				gravityNormal = true;
				CTRSoundMgr._playSound(SND_GRAVITY_OFF);
			}
			if (earthAnims == null)
			{
				return;
			}
			foreach (Image earthAnim in earthAnims)
			{
				if (gravityNormal)
				{
					earthAnim.playTimeline(0);
				}
				else
				{
					earthAnim.playTimeline(1);
				}
			}
		}

		public virtual void rotateAllSpikesWithID(int sid)
		{
			foreach (Spikes spike in this.spikes)
			{
				if (spike.getToggled() == sid)
				{
					spike.rotateSpikes();
				}
			}
		}

		public override void dealloc()
		{
			for (int i = 0; i < 5; i++)
			{
				fingerCuts[i].release();
			}
			dd.release();
			camera.release();
			back.release();
			base.dealloc();
		}

		public virtual void fullscreenToggled(bool isFullscreen)
		{
			BaseElement childWithName = staticAniPool.getChildWithName("levelLabel");
			if (childWithName != null)
			{
				childWithName.x = 15f + (float)base.canvas.xOffsetScaled;
			}
			for (int i = 0; i < 3; i++)
			{
				hudStar[i].x = hudStar[i].width * i + base.canvas.xOffsetScaled;
			}
			if (isFullscreen)
			{
				float num = Global.ScreenSizeManager.ScreenWidth;
				back.scaleX = num / (float)base.canvas.backingWidth * 1.25f;
			}
			else
			{
				back.scaleX = 1.25f;
			}
		}

		private void selector_gameLost(NSObject param)
		{
			gameLost();
		}

		private void selector_gameWon(NSObject param)
		{
			CTRSoundMgr.EnableLoopedSounds(false);
			if (gameSceneDelegate != null)
			{
				gameSceneDelegate.gameWon();
			}
		}

		private void selector_animateLevelRestart(NSObject param)
		{
			animateLevelRestart();
		}

		private void selector_showGreeting(NSObject param)
		{
			showGreeting();
		}

		private void selector_doCandyBlink(NSObject param)
		{
			doCandyBlink();
		}

		private void selector_teleport(NSObject param)
		{
			teleport();
		}

		public static float FBOUND_PI(float a)
		{
			return (float)(((double)a > Math.PI) ? ((double)a - Math.PI * 2.0) : (((double)a < -Math.PI) ? ((double)a + Math.PI * 2.0) : ((double)a)));
		}
	}
}
