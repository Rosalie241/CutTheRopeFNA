using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.windows;

namespace CutTheRope.iframework.visual
{
	internal class RotateableMultiParticles : MultiParticles
	{
		public float rotateSpeed;

		public float rotateSpeedVar;

		public override void initParticle(ref Particle particle)
		{
			base.initParticle(ref particle);
			particle.angle = 0f;
			particle.deltaAngle = MathHelper.DEGREES_TO_RADIANS(rotateSpeed + rotateSpeedVar * MathHelper.RND_MINUS1_1);
		}

		public override void updateParticle(ref Particle p, float delta)
		{
			if (p.life > 0f)
			{
				Vector vector = MathHelper.vectZero;
				if (p.pos.x != 0f || p.pos.y != 0f)
				{
					vector = MathHelper.vectNormalize(p.pos);
				}
				Vector v = vector;
				vector = MathHelper.vectMult(vector, p.radialAccel);
				float num = v.x;
				v.x = 0f - v.y;
				v.y = num;
				v = MathHelper.vectMult(v, p.tangentialAccel);
				Vector v2 = MathHelper.vectAdd(MathHelper.vectAdd(vector, v), gravity);
				v2 = MathHelper.vectMult(v2, delta);
				p.dir = MathHelper.vectAdd(p.dir, v2);
				v2 = MathHelper.vectMult(p.dir, delta);
				p.pos = MathHelper.vectAdd(p.pos, v2);
				p.color.r += p.deltaColor.r * delta;
				p.color.g += p.deltaColor.g * delta;
				p.color.b += p.deltaColor.b * delta;
				p.color.a += p.deltaColor.a * delta;
				p.life -= delta;
				float num2 = p.pos.x - p.width / 2f;
				float num3 = p.pos.y - p.height / 2f;
				float num4 = p.pos.x + p.width / 2f;
				float num5 = p.pos.y - p.height / 2f;
				float num6 = p.pos.x - p.width / 2f;
				float num7 = p.pos.y + p.height / 2f;
				float num8 = p.pos.x + p.width / 2f;
				float num9 = p.pos.y + p.height / 2f;
				float cx = p.pos.x;
				float cy = p.pos.y;
				Vector v3 = MathHelper.vect(num2, num3);
				Vector v4 = MathHelper.vect(num4, num5);
				Vector v5 = MathHelper.vect(num6, num7);
				Vector v6 = MathHelper.vect(num8, num9);
				p.angle += p.deltaAngle * delta;
				float cosA = MathHelper.cosf(p.angle);
				float sinA = MathHelper.sinf(p.angle);
				v3 = Particles.rotatePreCalc(v3, cosA, sinA, cx, cy);
				v4 = Particles.rotatePreCalc(v4, cosA, sinA, cx, cy);
				v5 = Particles.rotatePreCalc(v5, cosA, sinA, cx, cy);
				v6 = Particles.rotatePreCalc(v6, cosA, sinA, cx, cy);
				drawer.vertices[particleIdx] = Quad3D.MakeQuad3DEx(v3.x, v3.y, v4.x, v4.y, v5.x, v5.y, v6.x, v6.y);
				for (int i = 0; i < 4; i++)
				{
					colors[particleIdx * 4 + i] = p.color;
				}
				particleIdx++;
			}
			else
			{
				if (particleIdx != particleCount - 1)
				{
					particles[particleIdx] = particles[particleCount - 1];
					drawer.vertices[particleIdx] = drawer.vertices[particleCount - 1];
					drawer.texCoordinates[particleIdx] = drawer.texCoordinates[particleCount - 1];
				}
				particleCount--;
			}
		}

		public override void update(float delta)
		{
			base.update(delta);
			if (active && emissionRate != 0f)
			{
				float num = 1f / emissionRate;
				emitCounter += delta;
				while (particleCount < totalParticles && emitCounter > num)
				{
					addParticle();
					emitCounter -= num;
				}
				elapsed += delta;
				if (duration != -1f && duration < elapsed)
				{
					stopSystem();
				}
			}
			particleIdx = 0;
			while (particleIdx < particleCount)
			{
				updateParticle(ref particles[particleIdx], delta);
			}
			OpenGL.glBindBuffer(2, colorsID);
			OpenGL.glBufferData(2, colors, 3);
			OpenGL.glBindBuffer(2, 0u);
		}
	}
}
