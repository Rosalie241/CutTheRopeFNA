using System;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.windows;
using Microsoft.Xna.Framework;
using MathHelper = CutTheRope.iframework.helpers.MathHelper;

namespace CutTheRope.iframework.visual
{
	internal class Scrollbar : BaseElement
	{
		public delegate void ProvideScrollPosMaxScrollPosScrollCoeff(ref Vector sp, ref Vector mp, ref Vector sc);

		public Vector sp;

		public Vector mp;

		public Vector sc;

		public bool vertical;

		public RGBAColor backColor;

		public RGBAColor scrollerColor;

		public override void update(float delta)
		{
			base.update(delta);
		}

		public override void draw()
		{
			base.preDraw();
			OpenGL.glDisable(0);
			bool flag = false;
			float num;
			float num2;
			float num3;
			float num5;
			if (vertical)
			{
				num = (float)width - 2f;
				num2 = 1f;
				num3 = (float)Math.Round(((double)height - 2.0) / (double)sc.y);
				float num4 = ((mp.y != 0f) ? (sp.y / mp.y) : 1f);
				num5 = (float)(1.0 + ((double)height - 2.0 - (double)num3) * (double)num4);
				if (num3 > (float)height)
				{
					flag = true;
				}
			}
			else
			{
				num3 = (float)height - 2f;
				num5 = 1f;
				num = (float)Math.Round(((double)width - 2.0) / (double)sc.x);
				float num6 = ((mp.x != 0f) ? (sp.x / mp.x) : 1f);
				num2 = (float)(1.0 + ((double)width - 2.0 - (double)num) * (double)num6);
				if (num > (float)width)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				GLDrawer.drawSolidRectWOBorder(drawX, drawY, width, height, backColor);
				GLDrawer.drawSolidRectWOBorder(drawX + num2, drawY + num5, num, num3, scrollerColor);
			}
			OpenGL.glEnable(0);
			OpenGL.glColor4f(Color.White);
			base.postDraw();
		}

		public virtual Scrollbar initWithWidthHeightVertical(float w, float h, bool v)
		{
			if (init() != null)
			{
				width = (int)w;
				height = (int)h;
				vertical = v;
				sp = MathHelper.vectUndefined;
				mp = MathHelper.vectUndefined;
				sc = MathHelper.vectUndefined;
				backColor = RGBAColor.MakeRGBA(1f, 1f, 1f, 0.5f);
				scrollerColor = RGBAColor.MakeRGBA(0f, 0f, 0f, 0.5f);
			}
			return this;
		}
	}
}
