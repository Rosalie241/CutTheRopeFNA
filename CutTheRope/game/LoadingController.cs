using System;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.visual;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class LoadingController : ViewController, ResourceMgrDelegate
	{
		private enum ViewID
		{
			VIEW_LOADING = 0
		}

		public int nextController;

		public override NSObject initWithParent(ViewController p)
		{
			if (base.initWithParent(p) != null)
			{
				LoadingView loadingView = (LoadingView)new LoadingView().initFullscreen();
				addViewwithID(loadingView, (int)ViewID.VIEW_LOADING);
				Text text = new Text().initWithFont(Application.getFont(FNT_BIG_FONT));
				text.setAlignment(2);
				text.setStringandWidth(Application.getString(STR_MENU_LOADING), 300f);
				text.anchor = (text.parentAnchor = 18);
				loadingView.addChild(text);
			}
			return this;
		}

		public override void activate()
		{
			base.activate();
			LoadingView loadingView = (LoadingView)getView((int)ViewID.VIEW_LOADING);
			loadingView.game = nextController == 0;
			showView((int)ViewID.VIEW_LOADING);
		}

		public virtual void allResourcesLoaded()
		{
			GC.Collect();
			Application.sharedRootController().setViewTransition(4);
			base.deactivate();
		}
	}
}
