using System;
using CutTheRope.ios;

namespace CutTheRope.iframework.visual
{
	internal class Font : FontGeneric
	{
		private NSString chars;

		private char[] sortedChars;

		private int quadsCount;

		private float height;

		private Image charmap;

		public virtual Font initWithVariableSizeCharscharMapFileKerning(NSString strParam, Texture2D charmapfile, object k)
		{
			if (base.init() != null)
			{
				charmap = new Image().initWithTexture(charmapfile);
				quadsCount = charmapfile.quadsCount;
				height = charmapfile.quadRects[0].h;
				chars = strParam;
				sortedChars = chars.getCharacters();
				Array.Sort(sortedChars);
				charOffset = 0f;
				lineOffset = 0f;
			}
			return this;
		}

		public override void dealloc()
		{
			chars = null;
			sortedChars = null;
			charmap = null;
			base.dealloc();
		}

		public override void setCharOffsetLineOffsetSpaceWidth(float co, float lo, float sw)
		{
			charOffset = co;
			lineOffset = lo;
			spaceWidth = sw;
		}

		public override float fontHeight()
		{
			return height;
		}

		public override bool canDraw(char c)
		{
			if (c == ' ')
			{
				return true;
			}
			return Array.BinarySearch(sortedChars, c) >= 0;
		}

		public override float getCharWidth(char c)
		{
			switch (c)
			{
			case '*':
				return 0f;
			case ' ':
				return spaceWidth;
			default:
				return charmap.texture.quadRects[getCharQuad(c)].w;
			}
		}

		public override int getCharmapIndex(char c)
		{
			return 0;
		}

		public override int getCharQuad(char c)
		{
			int num = chars.IndexOf(c);
			if (num >= 0)
			{
				return num;
			}
			return -1;
		}

		public override float getCharOffset(char[] s, int c, int len)
		{
			if (c == len - 1)
			{
				return 0f;
			}
			return charOffset;
		}

		public override int totalCharmaps()
		{
			return 1;
		}

		public override Image getCharmap(int i)
		{
			return charmap;
		}
	}
}
