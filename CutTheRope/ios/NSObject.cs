using CutTheRope.iframework;

namespace CutTheRope.ios
{
	internal class NSObject : FrameworkTypes
	{
		public static NSString NSS(string s)
		{
			return new NSString(s);
		}

		public virtual NSObject init()
		{
			return this;
		}

		public virtual void dealloc()
		{
		}

		public virtual void release()
		{
		}
	}
}
