using System;
using CutTheRope.iframework.core;
using CutTheRope.ios;

namespace CutTheRope.ctr_commons
{
	internal class CTRApp : Application
	{
		public override void dealloc()
		{
			throw new NotImplementedException();
		}

		public virtual void applicationWillTerminate(UIApplication application)
		{
			Application.sharedPreferences().savePreferences();
		}

		public virtual void applicationDidReceiveMemoryWarning(UIApplication application)
		{
			throw new NotImplementedException();
		}

		public virtual void challengeStartedWithGameConfig(NSString gameConfig)
		{
			throw new NotImplementedException();
		}

		public virtual void applicationWillResignActive(UIApplication application)
		{
			Application.sharedPreferences().savePreferences();
			if (Application.root != null && !Application.root.isSuspended())
			{
				Application.root.suspend();
			}
		}

		public virtual void applicationDidBecomeActive(UIApplication application)
		{
			if (Application.root != null && Application.root.isSuspended())
			{
				Application.root.resume();
			}
		}
	}
}
