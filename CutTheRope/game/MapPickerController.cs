using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class MapPickerController : ViewController, ButtonDelegate
	{
		private const int BUTTON_START = 0;

		private const int VIEW_MAIN = 0;

		private const int VIEW_MAPLIST_LOADING = 1;

        private string selectedMap;

		private bool autoLoad;

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				selectedMap = null;
				createPickerView();
				View view = (View)new View().initFullscreen();
				RectangleElement rectangleElement = (RectangleElement)new RectangleElement().init();
				rectangleElement.color = RGBAColor.whiteRGBA;
				rectangleElement.width = (int)FrameworkTypes.SCREEN_WIDTH;
				rectangleElement.height = (int)FrameworkTypes.SCREEN_HEIGHT;
				view.addChild(rectangleElement);
				FontGeneric font = Application.getFont(FNT_SMALL_FONT);
				Text text = new Text().initWithFont(font);
				text.setString(NSObject.NSS("Loading..."));
				text.anchor = (text.parentAnchor = 18);
				view.addChild(text);
				addViewwithID(view, VIEW_MAPLIST_LOADING);
				setNormalMode();
			}
			return this;
		}

		public virtual void createPickerView()
		{
			View view = (View)new View().initFullscreen();
			RectangleElement rectangleElement = (RectangleElement)new RectangleElement().init();
			rectangleElement.color = RGBAColor.whiteRGBA;
			rectangleElement.width = (int)FrameworkTypes.SCREEN_WIDTH;
			rectangleElement.height = (int)FrameworkTypes.SCREEN_HEIGHT;
			view.addChild(rectangleElement);
			FontGeneric font = Application.getFont(FNT_SMALL_FONT);
			Text text = new Text().initWithFont(font);
			text.setString(NSObject.NSS("START"));
			Text text2 = new Text().initWithFont(font);
			text2.setString(NSObject.NSS("START"));
			text2.scaleX = (text2.scaleY = 1.2f);
			Button button = new Button().initWithUpElementDownElementandID(text, text2, 0);
			button.anchor = (button.parentAnchor = 34);
			button.delegateButtonDelegate = this;
			view.addChild(button);
			addViewwithID(view, VIEW_MAIN);
		}

		public override void activate()
		{
			base.activate();
			if (autoLoad)
			{
				string str = "maps/" + selectedMap;
				XMLNode xMLNode = XMLNode.parseXML(str);
				xmlLoaderFinishedWithfromwithSuccess(xMLNode, str, xMLNode != null);
			}
			else
			{
				showView(VIEW_MAIN);
			}
		}

		public override void deactivate()
		{
			base.deactivate();
		}

		public virtual void xmlLoaderFinishedWithfromwithSuccess(XMLNode rootNode, string url, bool success)
		{
			if (rootNode != null)
			{
				CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
				cTRRootController.setMap(rootNode);
				cTRRootController.setMapName(selectedMap);
				deactivate();
			}
		}

		public virtual void setNormalMode()
		{
			autoLoad = false;
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			cTRRootController.setPicker(true);
		}

		public virtual void setAutoLoadMap(string map)
		{
			autoLoad = true;
			CTRRootController cTRRootController = (CTRRootController)Application.sharedRootController();
			cTRRootController.setPicker(false);
			selectedMap = map;
		}

		public virtual void onButtonPressed(int n)
		{
		}

		public override void dealloc()
		{
			base.dealloc();
		}
	}
}
