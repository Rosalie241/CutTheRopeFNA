namespace CutTheRope.iframework.visual
{
	internal class VBox : BaseElement
	{
		public float offset;

		public int align;

		public float nextElementY;

		public override int addChildwithID(BaseElement c, int i)
		{
			int result = base.addChildwithID(c, i);
			if (align == 1)
			{
				c.anchor = (c.parentAnchor = 9);
			}
			else if (align == 4)
			{
				c.anchor = (c.parentAnchor = 12);
			}
			else if (align == 2)
			{
				c.anchor = (c.parentAnchor = 10);
			}
			c.y = nextElementY;
			nextElementY += (float)c.height + offset;
			height = (int)(nextElementY - offset);
			return result;
		}

		public virtual VBox initWithOffsetAlignWidth(double of, int a, double w)
		{
			return initWithOffsetAlignWidth((float)of, a, (float)w);
		}

		public virtual VBox initWithOffsetAlignWidth(float of, int a, float w)
		{
			if (init() != null)
			{
				offset = of;
				align = a;
				nextElementY = 0f;
				width = (int)w;
			}
			return this;
		}
	}
}
