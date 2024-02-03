using System;
using System.Collections.Generic;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.game
{
	internal class CTRMover : Mover
	{
		public override void setPathFromStringandStart(NSString p, Vector s)
		{
			if (p.characterAtIndex(0) == 'R')
			{
				bool flag = p.characterAtIndex(1) == 'C';
				NSString nSString = p.substringFromIndex(2);
				int num = (int)FrameworkTypes.RTD(nSString.intValue());
				num *= 3;
				int num2 = num / 2;
				float num3 = (float)(Math.PI * 2.0 / (double)num2);
				if (!flag)
				{
					num3 = 0f - num3;
				}
				float num4 = 0f;
				for (int i = 0; i < num2; i++)
				{
					float x = s.x + (float)num * MathHelper.cosf(num4);
					float y = s.y + (float)num * MathHelper.sinf(num4);
					addPathPoint(MathHelper.vect(x, y));
					num4 += num3;
				}
			}
			else
			{
				addPathPoint(s);
				if (p.characterAtIndex(p.length() - 1) == ',')
				{
					p = p.substringToIndex(p.length() - 1);
				}
				List<NSString> list = p.componentsSeparatedByString(',');
				for (int j = 0; j < list.Count; j += 2)
				{
					NSString nSString2 = list[j];
					NSString nSString3 = list[j + 1];
					addPathPoint(MathHelper.vect(s.x + nSString2.floatValue() * 3f, s.y + nSString3.floatValue() * 3f));
				}
			}
		}
	}
}
