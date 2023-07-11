using System;
using System.Collections.Generic;
using NJG;
using UnityEngine;

[AddComponentMenu("NJG MiniMap/Map Item")]
public class NJGMapItem : MonoBehaviour
{
	public static List<NJGMapItem> list = new List<NJGMapItem>();

	public bool isRevealed;

	public bool revealFOW;

	public bool drawDirection;

	public string content = string.Empty;

	public int type;

	public int revealDistance;

	public UIMapArrowBase arrow;

	public Action<bool> onSelect;

	public bool showIcon;

	public bool isActive = true;

	private Color mColor = Color.clear;

	private bool mInteraction;

	private bool mRotate;

	private bool mArrowRotate;

	private bool mUpdatePos;

	private bool mAnimOnVisible;

	private bool mAnimOnAction;

	private bool mLoop;

	private bool mArrow;

	private bool mFOW;

	private float mFadeOut = -1f;

	private Vector2 mIconScale;

	private Vector2 mBorderScale;

	private int mIconSize = int.MaxValue;

	private int mSize = int.MaxValue;

	private int mBSize = int.MaxValue;

	private int mBorderSize = int.MaxValue;

	private int mDepth = int.MaxValue;

	private int mArrowDepth = int.MaxValue;

	private int mArrowOffset = int.MaxValue;

	private bool mInteractionSet;

	private bool mColorSet;

	private bool mRotateSet;

	private bool mArrowRotateSet;

	private bool mUpdatePosSet;

	private bool mAnimOnVisibleSet;

	private bool mAnimOnActionSet;

	private bool mLoopSet;

	private bool mArrowSet;

	private bool mFOWSet;

	private bool mForceSelect;

	private bool mSelected;

	private Transform mTrans;

	private NJGFOW.Revealer mRevealer;

	public Color color
	{
		get
		{
			if (!mColorSet)
			{
				mColor = NJGMapBase.instance.GetColor(type);
			}
			mColorSet = true;
			return mColor;
		}
	}

	public bool rotate
	{
		get
		{
			if (!mRotateSet)
			{
				mRotateSet = true;
				mRotate = NJGMapBase.instance.GetRotate(type);
			}
			return mRotate;
		}
	}

	public bool interaction
	{
		get
		{
			if (!mInteractionSet)
			{
				mInteractionSet = true;
				mInteraction = NJGMapBase.instance.GetInteraction(type);
			}
			return mInteraction;
		}
	}

	public bool arrowRotate
	{
		get
		{
			if (!mArrowRotateSet)
			{
				mArrowRotateSet = true;
				mArrowRotate = NJGMapBase.instance.GetArrowRotate(type);
			}
			return mArrowRotate;
		}
	}

	public bool updatePosition
	{
		get
		{
			if (!mUpdatePosSet)
			{
				mUpdatePosSet = true;
				mUpdatePos = NJGMapBase.instance.GetUpdatePosition(type);
			}
			return mUpdatePos;
		}
	}

	public bool animateOnVisible
	{
		get
		{
			if (!mAnimOnVisibleSet)
			{
				mAnimOnVisibleSet = true;
				mAnimOnVisible = NJGMapBase.instance.GetAnimateOnVisible(type);
			}
			return mAnimOnVisible;
		}
	}

	public bool showOnAction
	{
		get
		{
			if (!mAnimOnActionSet)
			{
				mAnimOnActionSet = true;
				mAnimOnAction = NJGMapBase.instance.GetAnimateOnAction(type);
			}
			return mAnimOnAction;
		}
	}

	public bool loopAnimation
	{
		get
		{
			if (!mLoopSet)
			{
				mLoopSet = true;
				mLoop = NJGMapBase.instance.GetLoopAnimation(type);
			}
			return mLoop;
		}
	}

	public bool haveArrow
	{
		get
		{
			if (!mArrowSet)
			{
				mArrowSet = true;
				mArrow = NJGMapBase.instance.GetHaveArrow(type);
			}
			return mArrow;
		}
	}

	public float fadeOutAfterDelay
	{
		get
		{
			if (mFadeOut == -1f)
			{
				mFadeOut = NJGMapBase.instance.GetFadeOutAfter(type);
			}
			return mFadeOut;
		}
	}

	public int size
	{
		get
		{
			if (NJGMapBase.instance.GetCustom(type))
			{
				mSize = NJGMapBase.instance.GetSize(type);
			}
			else
			{
				mSize = NJGMapBase.instance.iconSize;
			}
			return mSize;
		}
	}

	public int borderSize
	{
		get
		{
			if (NJGMapBase.instance.GetCustomBorder(type))
			{
				mBSize = NJGMapBase.instance.GetBorderSize(type);
			}
			else
			{
				mBSize = NJGMapBase.instance.borderSize;
			}
			return mBSize;
		}
	}

	public virtual Vector3 iconScale
	{
		get
		{
			if (mIconSize != size)
			{
				mIconSize = size;
				mIconScale.x = (mIconScale.y = size);
			}
			return mIconScale;
		}
	}

	public virtual Vector3 borderScale
	{
		get
		{
			if (mBorderSize != borderSize)
			{
				mBorderSize = borderSize;
				mBorderScale.x = (mBorderScale.y = borderSize);
			}
			return mBorderScale;
		}
	}

	public int depth
	{
		get
		{
			if (mDepth == int.MaxValue)
			{
				mDepth = NJGMapBase.instance.GetDepth(type);
			}
			return mDepth;
		}
	}

	public int arrowDepth
	{
		get
		{
			if (mArrowDepth == int.MaxValue)
			{
				mArrowDepth = NJGMapBase.instance.GetArrowDepth(type);
			}
			return mArrowDepth;
		}
	}

	public int arrowOffset
	{
		get
		{
			if (mArrowOffset == int.MaxValue)
			{
				mArrowOffset = NJGMapBase.instance.GetArrowOffset(type);
			}
			return mArrowOffset;
		}
	}

	public bool isSelected
	{
		get
		{
			return mSelected;
		}
		set
		{
			mSelected = value;
			if (onSelect != null)
			{
				onSelect(mSelected);
			}
		}
	}

	public bool forceSelection
	{
		get
		{
			return mForceSelect;
		}
		set
		{
			mForceSelect = value;
		}
	}

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

	private void Start()
	{
		if (NJGMapBase.instance != null && revealFOW && NJGMapBase.instance.fow.enabled)
		{
			mRevealer = NJGFOW.CreateRevealer();
		}
	}

	private void OnEnable()
	{
		list.Add(this);
	}

	private void OnDestroy()
	{
		if (Application.isPlaying)
		{
			if (NJGMapBase.instance != null && NJGMapBase.instance.fow.enabled)
			{
				NJGFOW.DeleteRevealer(mRevealer);
			}
			mRevealer = null;
			if (arrow != null)
			{
				UIMiniMapBase.inst.DeleteArrow(arrow);
			}
			arrow = null;
		}
		list.Remove(this);
	}

	private void OnDisable()
	{
		if (Application.isPlaying)
		{
			if (mRevealer != null)
			{
				mRevealer.isActive = false;
			}
			if (arrow != null)
			{
				UIMiniMapBase.inst.DeleteArrow(arrow);
			}
			arrow = null;
		}
		list.Remove(this);
	}

	private void Update()
	{
		if (revealFOW && !(NJGMapBase.instance == null) && !(UIMiniMapBase.inst == null))
		{
			if (mRevealer == null)
			{
				mRevealer = NJGFOW.CreateRevealer();
			}
			if (isActive)
			{
				mRevealer.pos = UIMiniMapBase.inst.WorldToMap(cachedTransform.position, false);
				mRevealer.revealDistance = ((revealDistance <= 0) ? NJGMapBase.instance.fow.revealDistance : revealDistance);
				mRevealer.isActive = true;
			}
			else
			{
				mRevealer.isActive = false;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (revealFOW)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.transform.position, revealDistance);
		}
	}

	public void Select()
	{
		mSelected = true;
	}

	public void Select(bool forceSelect)
	{
		mSelected = true;
		mForceSelect = forceSelect;
	}

	public void UnSelect()
	{
		mSelected = false;
	}

	public void Show()
	{
		showIcon = true;
	}
}
