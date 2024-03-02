using System;
using System.Linq;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using CutTheRope.windows;

namespace CutTheRope.game
{
	internal class PollenDrawer : BaseElement
	{
		private ImageMultiDrawer drawer;

		private int pollenCount;

		private int totalCapacity;

		private Pollen[] pollens;

		private float qw;

		private float qh;

		private RGBAColor[] colors;

		private uint colorsID;

		private PointSprite[] vertices;

		private uint verticesID;

		public override NSObject init()
		{
			if (base.init() != null)
			{
				Image image = Image.Image_createWithResID(IMG_OBJ_POLLEN_HD);
				qw = (float)image.width * 1.5f;
				qh = (float)image.height * 1.5f;
				totalCapacity = 200;
				drawer = new ImageMultiDrawer().initWithImageandCapacity(image, totalCapacity);
				pollens = new Pollen[totalCapacity];
				colors = new RGBAColor[4 * totalCapacity];
				OpenGL.glGenBuffers(1, ref colorsID);
			}
			return this;
		}

		public override void dealloc()
		{
			if (pollens != null)
			{
				pollens = null;
			}
			if (colors != null)
			{
				colors = null;
				OpenGL.glDeleteBuffers(1, ref colorsID);
			}
			if (vertices != null)
			{
				vertices = null;
				OpenGL.glDeleteBuffers(1, ref verticesID);
			}
			drawer = null;
			base.dealloc();
		}

		public virtual void addPollenAtparentIndex(Vector v, int pi)
		{
			float num = 1f;
			float num2 = 1f;
			float[] array = new float[5] { 0.3f, 0.3f, 0.5f, 0.5f, 0.6f };
			int num3 = array.Count();
			float num4 = array[MathHelper.RND_RANGE(0, num3 - 1)];
			float num5 = num4;
			if (MathHelper.RND(1) == 1)
			{
				num4 *= 1f + (float)MathHelper.RND(1) / 10f;
			}
			else
			{
				num5 *= 1f + (float)MathHelper.RND(1) / 10f;
			}
			num *= num4;
			num2 *= num5;
			int num6 = (int)qw;
			int num7 = (int)qh;
			num6 *= (int)num;
			num7 *= (int)num2;
			Pollen pollen = default(Pollen);
			pollen.parentIndex = pi;
			pollen.x = v.x;
			pollen.y = v.y;
			float num8 = 1f;
			float num9 = Math.Min(num8 - num, num8 - num2);
			float rND_0_ = MathHelper.RND_0_1;
			pollen.startScaleX = num9 + num;
			pollen.startScaleY = num9 + num2;
			pollen.scaleX = pollen.startScaleX * rND_0_;
			pollen.scaleY = pollen.startScaleY * rND_0_;
			pollen.endScaleX = num;
			pollen.endScaleY = num2;
			pollen.endAlpha = 0.3f;
			pollen.startAlpha = 1f;
			pollen.alpha = 0.7f * rND_0_ + 0.3f;
			Quad2D qt = drawer.image.texture.quads[0];
			Quad3D qv = Quad3D.MakeQuad3D(v.x - (float)(num6 / 2), v.y - (float)(num7 / 2), 0.0, num6, num7);
			drawer.setTextureQuadatVertexQuadatIndex(qt, qv, pollenCount);
			if (pollenCount >= totalCapacity)
			{
				totalCapacity = pollenCount;
				pollens = new Pollen[totalCapacity + 1];
				colors = new RGBAColor[4 * (totalCapacity + 1)];
			}
			for (int i = 0; i < 4; i++)
			{
				colors[pollenCount * 4 + i] = RGBAColor.whiteRGBA;
			}
			pollens[pollenCount] = pollen;
			pollenCount++;
		}

		public virtual void fillWithPolenFromPathIndexToPathIndexGrab(int p1, int p2, Grab g)
		{
			int num = 44;
			Vector vector = g.mover.path[p1];
			Vector v = g.mover.path[p2];
			Vector v2 = MathHelper.vectSub(v, vector);
			float num2 = MathHelper.vectLength(v2);
			int num3 = (int)(num2 / (float)num);
			Vector v3 = MathHelper.vectNormalize(v2);
			for (int i = 0; i <= num3; i++)
			{
				Vector v4 = MathHelper.vectAdd(vector, MathHelper.vectMult(v3, i * num));
				v4.x += MathHelper.RND_RANGE((int)FrameworkTypes.RTPD(-2.0), (int)FrameworkTypes.RTPD(2.0));
				v4.y += MathHelper.RND_RANGE((int)FrameworkTypes.RTPD(-2.0), (int)FrameworkTypes.RTPD(2.0));
				addPollenAtparentIndex(v4, p1);
			}
		}

		public override void update(float delta)
		{
			base.update(delta);
			drawer.update(delta);
			for (int i = 0; i < pollenCount; i++)
			{
				if (Mover.moveVariableToTarget(ref pollens[i].scaleX, pollens[i].endScaleX, 1f, delta))
				{
					float startScaleX = pollens[i].startScaleX;
					pollens[i].startScaleX = pollens[i].endScaleX;
					pollens[i].endScaleX = startScaleX;
				}
				if (Mover.moveVariableToTarget(ref pollens[i].scaleY, pollens[i].endScaleY, 1f, delta))
				{
					float startScaleY = pollens[i].startScaleY;
					pollens[i].startScaleY = pollens[i].endScaleY;
					pollens[i].endScaleY = startScaleY;
				}
				float num = qw * pollens[i].scaleX;
				float num2 = qh * pollens[i].scaleY;
				drawer.vertices[i] = Quad3D.MakeQuad3D(pollens[i].x - num / 2f, pollens[i].y - num2 / 2f, 0.0, num, num2);
				if (Mover.moveVariableToTarget(ref pollens[i].alpha, pollens[i].endAlpha, 1f, delta))
				{
					float startAlpha = pollens[i].startAlpha;
					pollens[i].startAlpha = pollens[i].endAlpha;
					pollens[i].endAlpha = startAlpha;
				}
				float alpha = pollens[i].alpha;
				for (int j = 0; j < 4; j++)
				{
					colors[i * 4 + j] = RGBAColor.MakeRGBA(alpha, alpha, alpha, alpha);
				}
			}
			OpenGL.glBindBuffer(2, colorsID);
			OpenGL.glBufferData(2, colors, 3);
			OpenGL.glBindBuffer(2, 0u);
		}

		public override void draw()
		{
			if (pollenCount >= 2)
			{
				preDraw();
				OpenGL.glBlendFunc(BlendingFactor.GL_SRC_ALPHA, BlendingFactor.GL_ONE);
				OpenGL.glEnable(0);
				OpenGL.glBindTexture(drawer.image.texture.name());
				OpenGL.glVertexPointer(3, 5, 0, FrameworkTypes.toFloatArray(drawer.vertices));
				OpenGL.glTexCoordPointer(2, 5, 0, FrameworkTypes.toFloatArray(drawer.texCoordinates));
				OpenGL.glEnableClientState(13);
				OpenGL.glBindBuffer(2, colorsID);
				OpenGL.glBufferData(2, colors, 3);
				OpenGL.glColorPointer(4, 5, 0, colors);
				OpenGL.glDrawElements(7, (pollenCount - 1) * 6, drawer.indices);
				OpenGL.glBlendFunc(BlendingFactor.GL_ONE, BlendingFactor.GL_ONE_MINUS_SRC_ALPHA);
				OpenGL.glBindBuffer(2, 0u);
				OpenGL.glDisableClientState(13);
				postDraw();
			}
		}
	}
}
