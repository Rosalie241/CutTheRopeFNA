using System;
using CutTheRope.iframework;
using CutTheRope.iframework.core;
using CutTheRope.iframework.helpers;
using CutTheRope.iframework.visual;
using CutTheRope.ios;
using Microsoft.Xna.Framework.Audio;

namespace CutTheRope.game
{
	internal class Spikes : CTRGameObject, TimelineDelegate, ButtonDelegate
	{
		private enum SPIKES_ANIM
		{
			SPIKES_ANIM_ELECTRODES_BASE,
			SPIKES_ANIM_ELECTRODES_ELECTRIC,
			SPIKES_ANIM_ROTATION_ADJUSTED
		}

		private enum SPIKES_ROTATION
		{
			SPIKES_ROTATION_BUTTON
		}

		public delegate void rotateAllSpikesWithID(int sid);

		private const float SPIKES_HEIGHT = 10f;

		private int toggled;

		public double angle;

		public Vector t1;

		public Vector t2;

		public Vector b1;

		public Vector b2;

		public bool electro;

		public float initialDelay;

		public float onTime;

		public float offTime;

		public bool electroOn;

		public float electroTimer;

		private bool updateRotationFlag;

		private bool spikesNormal;

		private float origRotation;

		public Button rotateButton;

		public int touchIndex;

		public rotateAllSpikesWithID delegateRotateAllSpikesWithID;

		private SoundEffectInstance sndElectric;

		public virtual NSObject initWithPosXYWidthAndAngleToggled(float px, float py, int w, double an, int t)
		{
			int textureResID = -1;
			if (t != -1)
			{
				textureResID = 93 + w - 1;
			}
			else
			{
				switch (w)
				{
				case 1:
					textureResID = 88;
					break;
				case 2:
					textureResID = 89;
					break;
				case 3:
					textureResID = 90;
					break;
				case 4:
					textureResID = 91;
					break;
				case 5:
					textureResID = 92;
					break;
				}
			}
			if (initWithTexture(Application.getTexture(textureResID)) == null)
			{
				return null;
			}
			if (t > 0)
			{
				doRestoreCutTransparency();
				int num = (t - 1) * 2;
				int q = 1 + (t - 1) * 2;
				Image image = Image.Image_createWithResIDQuad(IMG_OBJ_ROTATABLE_SPIKES_BUTTON, num);
				Image image2 = Image.Image_createWithResIDQuad(IMG_OBJ_ROTATABLE_SPIKES_BUTTON, q);
				image.doRestoreCutTransparency();
				image2.doRestoreCutTransparency();
				rotateButton = new Button().initWithUpElementDownElementandID(image, image2, 0);
				rotateButton.delegateButtonDelegate = this;
				rotateButton.anchor = (rotateButton.parentAnchor = 18);
				addChild(rotateButton);
				Vector quadOffset = Image.getQuadOffset(IMG_OBJ_ROTATABLE_SPIKES_BUTTON, num);
				Vector quadSize = Image.getQuadSize(IMG_OBJ_ROTATABLE_SPIKES_BUTTON, num);
				Vector v = MathHelper.vect(image.texture.preCutSize.x, image.texture.preCutSize.y);
				Vector vector = MathHelper.vectSub(v, MathHelper.vectAdd(quadSize, quadOffset));
				rotateButton.setTouchIncreaseLeftRightTopBottom(0f - quadOffset.x + quadSize.x / 2f, 0f - vector.x + quadSize.x / 2f, 0f - quadOffset.y + quadSize.y / 2f, 0f - vector.y + quadSize.y / 2f);
			}
			passColorToChilds = false;
			spikesNormal = false;
			origRotation = (rotation = (float)an);
			x = px;
			y = py;
			setToggled(t);
			updateRotation();
			if (w == 5)
			{
				addAnimationWithIDDelayLoopFirstLast(0, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 0, 0);
				addAnimationWithIDDelayLoopFirstLast(1, 0.05f, Timeline.LoopType.TIMELINE_REPLAY, 1, 4);
				doRestoreCutTransparency();
			}
			touchIndex = -1;
			return this;
		}

		public virtual void updateRotation()
		{
			float num = ((!electro) ? texture.quadRects[quadToDraw].w : ((float)width - FrameworkTypes.RTPD(400.0)));
			num /= 2f;
			t1.x = x - num;
			t2.x = x + num;
			t1.y = (t2.y = y - 5f);
			b1.x = t1.x;
			b2.x = t2.x;
			b1.y = (b2.y = y + 5f);
			angle = MathHelper.DEGREES_TO_RADIANS(rotation);
			t1 = MathHelper.vectRotateAround(t1, angle, x, y);
			t2 = MathHelper.vectRotateAround(t2, angle, x, y);
			b1 = MathHelper.vectRotateAround(b1, angle, x, y);
			b2 = MathHelper.vectRotateAround(b2, angle, x, y);
		}

		public virtual void turnElectroOff()
		{
			electroOn = false;
			playTimeline(0);
			electroTimer = offTime;
			if (sndElectric != null)
			{
				sndElectric.Stop();
				sndElectric = null;
			}
		}

		public virtual void turnElectroOn()
		{
			electroOn = true;
			playTimeline(1);
			electroTimer = onTime;
			sndElectric = CTRSoundMgr._playSoundLooped(SND_ELECTRIC);
		}

		public virtual void rotateSpikes()
		{
			spikesNormal = !spikesNormal;
			removeTimeline(2);
			float num = (spikesNormal ? 90 : 0);
			float num2 = origRotation + num;
			Timeline timeline = new Timeline().initWithMaxKeyFramesOnTrack(2);
			timeline.addKeyFrame(KeyFrame.makeRotation((int)rotation, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, 0f));
			timeline.addKeyFrame(KeyFrame.makeRotation((int)num2, KeyFrame.TransitionType.FRAME_TRANSITION_LINEAR, Math.Abs(num2 - rotation) / 90f * 0.3f));
			timeline.delegateTimelineDelegate = this;
			addTimelinewithID(timeline, 2);
			playTimeline(2);
			updateRotationFlag = true;
			rotateButton.scaleX = 0f - rotateButton.scaleX;
		}

		public virtual void setToggled(int t)
		{
			toggled = t;
		}

		public virtual int getToggled()
		{
			return toggled;
		}

		public override void update(float delta)
		{
			base.update(delta);
			if (mover != null || updateRotationFlag)
			{
				updateRotation();
			}
			if (!electro)
			{
				return;
			}
			if (electroOn)
			{
				Mover.moveVariableToTarget(ref electroTimer, 0f, 1f, delta);
				if ((double)electroTimer == 0.0)
				{
					turnElectroOff();
				}
			}
			else
			{
				Mover.moveVariableToTarget(ref electroTimer, 0f, 1f, delta);
				if ((double)electroTimer == 0.0)
				{
					turnElectroOn();
				}
			}
		}

		public virtual void timelineReachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
		{
		}

		public virtual void timelineFinished(Timeline t)
		{
			updateRotationFlag = false;
		}

		public virtual void onButtonPressed(int n)
		{
			if (n == 0)
			{
				delegateRotateAllSpikesWithID(toggled);
				if (spikesNormal)
				{
					CTRSoundMgr._playSound(SND_SPIKE_ROTATE_IN);
				}
				else
				{
					CTRSoundMgr._playSound(SND_SPIKE_ROTATE_OUT);
				}
			}
		}

		public virtual void timelinereachedKeyFramewithIndex(Timeline t, KeyFrame k, int i)
		{
		}
	}
}
