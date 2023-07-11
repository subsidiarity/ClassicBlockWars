using System.Collections;
using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

namespace NJG
{
	public class UIMapIconBase : MonoBehaviour
	{
		private static List<UIMapIconBase> selected = new List<UIMapIconBase>();

		public NJGMapItem item;

		public bool isValid;

		public bool isMapIcon = true;

		public bool isVisible;

		public new BoxCollider collider;

		private Transform mTrans;

		protected bool isLooping;

		protected bool isScaling;

		protected Vector3 onHoverScale = new Vector3(1.3f, 1.3f, 1.3f);

		protected TweenParms tweenParms = new TweenParms();

		private Tweener mLoop;

		private float mAlpha = 1f;

		protected bool mFadingOut;

		protected bool mSelected;

		public Transform cachedTransform
		{
			get
			{
				if (mTrans == null)
				{
					mTrans = base.transform;
				}
				return mTrans;
			}
		}

		public float alpha
		{
			get
			{
				return mAlpha;
			}
			set
			{
				mAlpha = value;
			}
		}

		protected virtual void Start()
		{
			if (Application.isPlaying)
			{
				CheckAnimations();
			}
		}

		protected virtual void Update()
		{
			if (mSelected != item.isSelected)
			{
				mSelected = item.isSelected;
				if (mSelected)
				{
					Select();
				}
				else
				{
					UnSelect();
				}
			}
			if (item.showIcon && item.showOnAction)
			{
				OnVisible();
				item.showIcon = false;
			}
		}

		public virtual void Select()
		{
			if (!Input.GetKey(KeyCode.LeftShift) && !item.forceSelection)
			{
				UnSelectAll();
			}
			item.isSelected = true;
			if (!selected.Contains(this))
			{
				selected.Add(this);
			}
		}

		public virtual void UnSelect()
		{
		}

		protected void UnSelectAll()
		{
			int i = 0;
			for (int count = selected.Count; i < count; i++)
			{
				UIMapIconBase uIMapIconBase = selected[i];
				uIMapIconBase.UnSelect();
			}
			selected.Clear();
		}

		protected void CheckAnimations()
		{
			alpha = 1f;
			if (!(item != null))
			{
				return;
			}
			if (item.showOnAction)
			{
				cachedTransform.localScale = Vector3.zero;
			}
			else if (item.fadeOutAfterDelay > 0f)
			{
				if (!mFadingOut)
				{
					mFadingOut = true;
					StartCoroutine(DelayedFadeOut());
				}
			}
			else if (item.loopAnimation)
			{
				OnLoop();
			}
			else if (item.animateOnVisible && !isMapIcon && item.fadeOutAfterDelay == 0f)
			{
				OnVisible();
			}
		}

		private void OnEnable()
		{
			if (Application.isPlaying)
			{
				if (mLoop != null && !item.loopAnimation)
				{
					mLoop.Kill();
				}
				cachedTransform.localScale = Vector3.one;
				CheckAnimations();
			}
		}

		private void OnDisable()
		{
			if (mFadingOut)
			{
				mFadingOut = false;
				StopAllCoroutines();
			}
			if (Application.isPlaying && item != null)
			{
				if (item.loopAnimation)
				{
					if (mLoop != null && !mLoop.isPaused)
					{
						mLoop.Pause();
					}
				}
				else if (mLoop != null)
				{
					mLoop.Kill();
				}
			}
			if (isVisible)
			{
				isVisible = false;
			}
		}

		protected virtual void OnVisible()
		{
			if (!isVisible)
			{
				if (item.fadeOutAfterDelay > 0f && !mFadingOut)
				{
					mFadingOut = true;
					StartCoroutine(DelayedFadeOut());
				}
				if (!item.loopAnimation)
				{
					cachedTransform.localScale = Vector3.one * 0.01f;
					tweenParms.Prop("localScale", Vector3.one).Ease(EaseType.EaseInOutElastic);
					HOTween.To(cachedTransform, 1f, tweenParms);
				}
				isVisible = true;
			}
		}

		protected virtual void OnLoop()
		{
			if (item.loopAnimation)
			{
				isLooping = true;
				if (mLoop == null)
				{
					cachedTransform.localScale = Vector3.one * 1.5f;
					mLoop = HOTween.To(cachedTransform, 0.5f, new TweenParms().Prop("localScale", Vector3.one).Ease(EaseType.Linear).Loops(-1, LoopType.Yoyo)
						.IntId(999));
				}
				else if (mLoop.isPaused)
				{
					mLoop.Play();
				}
			}
		}

		protected IEnumerator DelayedFadeOut()
		{
			yield return new WaitForSeconds(item.fadeOutAfterDelay);
			OnFadeOut();
		}

		protected virtual void OnFadeOut()
		{
			Tweener tweener = HOTween.To(this, 0.9f, "alpha", 0);
			tweener.easeType = EaseType.Linear;
			mFadingOut = false;
		}
	}
}
