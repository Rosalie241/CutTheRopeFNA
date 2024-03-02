using CutTheRope.ios;

namespace CutTheRope.iframework.visual
{
	internal class FormattedString : NSObject
	{
		public NSString string_;

		public float width;

		public virtual FormattedString initWithStringAndWidth(NSString str, float w)
		{
			if (base.init() != null)
			{
				string_ = str;
				width = w;
			}
			return this;
		}

		public override void dealloc()
		{
			string_ = null;
			base.dealloc();
		}
	}
}
