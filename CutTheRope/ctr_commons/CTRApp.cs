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

		public virtual void applicationWillResignActive()
		{
			Application.sharedPreferences().savePreferences();
			if (Application.root != null && !Application.root.isSuspended())
			{
				Application.root.suspend();
			}
		}

		public virtual void applicationDidBecomeActive()
		{
			if (Application.root != null && Application.root.isSuspended())
			{
				Application.root.resume();
			}
		}
	}
}
