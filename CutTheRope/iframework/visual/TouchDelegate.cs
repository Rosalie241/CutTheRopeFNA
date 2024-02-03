using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;

namespace CutTheRope.iframework.visual
{
	internal interface TouchDelegate
	{
		bool touchesBeganwithEvent(IList<TouchLocation> touches);

		bool touchesEndedwithEvent(IList<TouchLocation> touches);

		bool touchesMovedwithEvent(IList<TouchLocation> touches);

		bool touchesCancelledwithEvent(IList<TouchLocation> touches);

		bool backButtonPressed();

		bool menuButtonPressed();
	}
}
