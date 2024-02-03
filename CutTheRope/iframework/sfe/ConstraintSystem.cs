using System;
using System.Collections.Generic;
using CutTheRope.ios;

namespace CutTheRope.iframework.sfe
{
	internal class ConstraintSystem : NSObject
	{
		public List<ConstraintedPoint> parts;

		public int relaxationTimes;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				relaxationTimes = 1;
				parts = new List<ConstraintedPoint>();
			}
			return this;
		}

		public virtual void addPart(ConstraintedPoint cp)
		{
			parts.Add(cp);
		}

		public virtual void addPartAt(ConstraintedPoint cp, int p)
		{
			parts.Insert(p, cp);
		}

		public virtual void update(float delta)
		{
			int count = parts.Count;
			for (int i = 0; i < count; i++)
			{
				ConstraintedPoint constraintedPoint = parts[i];
				if (constraintedPoint != null)
				{
					constraintedPoint.update(delta);
				}
			}
			int count2 = parts.Count;
			for (int j = 0; j < relaxationTimes; j++)
			{
				for (int k = 0; k < count2; k++)
				{
					ConstraintedPoint p = parts[k];
					ConstraintedPoint.satisfyConstraints(p);
				}
			}
		}

		public virtual void draw()
		{
			throw new NotImplementedException();
		}

		public override void dealloc()
		{
			parts = null;
			base.dealloc();
		}
	}
}
