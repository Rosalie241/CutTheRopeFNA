using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.ios;

namespace CutTheRope.iframework.sfe
{
	internal class MaterialPoint : NSObject
	{
		protected const double TIME_SCALE = 1.0;

		private const double PIXEL_TO_SI_METERS_K = 80.0;

		public const double GCONST = 784.0;

		private const int MAX_FORCES = 10;

		public static Vector globalGravity = default(Vector);

		public Vector pos;

		public Vector posDelta;

		public Vector v;

		public Vector a;

		public Vector totalForce;

		public float weight;

		public float invWeight;

		public Vector[] forces;

		public int highestForceIndex;

		public Vector gravity;

		public bool disableGravity;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				forces = new Vector[10];
				setWeight(1f);
				resetAll();
			}
			return this;
		}

		public virtual void setWeight(float w)
		{
			weight = w;
			invWeight = (float)(1.0 / (double)weight);
			gravity = MathHelper.vect(0f, 784f * weight);
		}

		public override void dealloc()
		{
			forces = null;
			base.dealloc();
		}

		public virtual void resetForces()
		{
			forces = new Vector[10];
			highestForceIndex = -1;
		}

		public virtual void resetAll()
		{
			resetForces();
			v = MathHelper.vectZero;
			a = MathHelper.vectZero;
			pos = MathHelper.vectZero;
			posDelta = MathHelper.vectZero;
			totalForce = MathHelper.vectZero;
		}

		public virtual void setForcewithID(Vector force, int n)
		{
			forces[n] = force;
			if (n > highestForceIndex)
			{
				highestForceIndex = n;
			}
		}

		public virtual void deleteForce(int n)
		{
			forces[n] = MathHelper.vectZero;
		}

		public virtual Vector getForce(int n)
		{
			return forces[n];
		}

		public virtual void applyImpulseDelta(Vector impulse, float delta)
		{
			if (!MathHelper.vectEqual(impulse, MathHelper.vectZero))
			{
				Vector v = MathHelper.vectMult(impulse, (float)((double)delta / 1.0));
				pos = MathHelper.vectAdd(pos, v);
			}
		}

		public virtual void updatewithPrecision(float delta, float p)
		{
			int num = (int)(delta / p) + 1;
			if (num != 0)
			{
				delta /= (float)num;
			}
			for (int i = 0; i < num; i++)
			{
				update(delta);
			}
		}

		public virtual void update(float delta)
		{
			totalForce = MathHelper.vectZero;
			if (!disableGravity)
			{
				if (!MathHelper.vectEqual(globalGravity, MathHelper.vectZero))
				{
					totalForce = MathHelper.vectAdd(totalForce, MathHelper.vectMult(globalGravity, weight));
				}
				else
				{
					totalForce = MathHelper.vectAdd(totalForce, gravity);
				}
			}
			if (highestForceIndex != -1)
			{
				for (int i = 0; i <= highestForceIndex; i++)
				{
					totalForce = MathHelper.vectAdd(totalForce, forces[i]);
				}
			}
			totalForce = MathHelper.vectMult(totalForce, invWeight);
			a = MathHelper.vectMult(totalForce, (float)((double)delta / 1.0));
			v = MathHelper.vectAdd(v, a);
			posDelta = MathHelper.vectMult(v, (float)((double)delta / 1.0));
			pos = MathHelper.vectAdd(pos, posDelta);
		}

		public virtual void drawForces()
		{
		}
	}
}
