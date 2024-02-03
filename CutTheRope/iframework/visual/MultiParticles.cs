using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.windows;

namespace CutTheRope.iframework.visual
{
	internal class MultiParticles : Particles
	{
		public ImageMultiDrawer drawer;

		public Image imageGrid;

		public virtual Particles initWithTotalParticlesandImageGrid(int numberOfParticles, Image image)
		{
			if (init() == null)
			{
				return null;
			}
			imageGrid = image;
			drawer = new ImageMultiDrawer().initWithImageandCapacity(imageGrid, numberOfParticles);
			width = (int)FrameworkTypes.SCREEN_WIDTH;
			height = (int)FrameworkTypes.SCREEN_HEIGHT;
			totalParticles = numberOfParticles;
			particles = new Particle[totalParticles];
			colors = new RGBAColor[4 * totalParticles];
			if (particles == null || colors == null)
			{
				particles = null;
				colors = null;
				return null;
			}
			active = false;
			blendAdditive = false;
			OpenGL.glGenBuffers(1, ref colorsID);
			return this;
		}

		public override void initParticle(ref Particle particle)
		{
			Image image = imageGrid;
			int num = MathHelper.RND(image.texture.quadsCount - 1);
			Quad2D qt = image.texture.quads[num];
			Quad3D qv = Quad3D.MakeQuad3D(0f, 0f, 0f, 0f, 0f);
			Rectangle rectangle = image.texture.quadRects[num];
			drawer.setTextureQuadatVertexQuadatIndex(qt, qv, particleCount);
			base.initParticle(ref particle);
			particle.width = rectangle.w * particle.size;
			particle.height = rectangle.h * particle.size;
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
				drawer.vertices[particleIdx] = Quad3D.MakeQuad3D(p.pos.x - p.width / 2f, p.pos.y - p.height / 2f, 0.0, p.width, p.height);
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

		public override void draw()
		{
			preDraw();
			if (blendAdditive)
			{
				OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE);
			}
			else
			{
				OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			}
			OpenGL.glEnable(0);
			OpenGL.glBindTexture(drawer.image.texture.name());
			OpenGL.glVertexPointer(3, 5, 0, FrameworkTypes.toFloatArray(drawer.vertices));
			OpenGL.glTexCoordPointer(2, 5, 0, FrameworkTypes.toFloatArray(drawer.texCoordinates));
			OpenGL.glEnableClientState(13);
			OpenGL.glBindBuffer(2, colorsID);
			OpenGL.glColorPointer(4, 5, 0, colors);
			OpenGL.glDrawElements(7, particleIdx * 6, drawer.indices);
			OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
			OpenGL.glBindBuffer(2, 0u);
			OpenGL.glDisableClientState(13);
			postDraw();
		}

		public override void dealloc()
		{
			drawer = null;
			imageGrid = null;
			base.dealloc();
		}
	}
}
