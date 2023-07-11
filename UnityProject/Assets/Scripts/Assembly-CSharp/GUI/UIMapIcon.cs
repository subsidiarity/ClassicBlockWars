using NJG;
using UnityEngine;

[ExecuteInEditMode]
public class UIMapIcon : UIMapIconBase
{
	public UISprite sprite;

	public UISprite border;

	private Color mColor;

	private TweenColor mTweenColor;

	private TweenScale mLoop;

	protected override void Start()
	{
		UnSelect();
		if (item.fadeOutAfterDelay == 0f)
		{
			sprite.alpha = 1f;
		}
		base.Start();
	}

	protected virtual void OnTooltip(bool show)
	{
		if (!string.IsNullOrEmpty(item.content))
		{
			if (show)
			{
				UICustomTooltip.Show(item.content);
			}
			else
			{
				UICustomTooltip.Hide();
			}
		}
	}

	protected virtual void OnHover(bool isOver)
	{
		if (isOver)
		{
			if (!isLooping)
			{
				TweenScale.Begin(sprite.cachedGameObject, 0.1f, onHoverScale);
			}
		}
		else if (!isLooping)
		{
			TweenScale.Begin(sprite.cachedGameObject, 0.3f, Vector3.one);
		}
	}

	public override void Select()
	{
		base.Select();
		if (border != null)
		{
			border.enabled = true;
		}
	}

	public override void UnSelect()
	{
		base.UnSelect();
		if (border != null)
		{
			border.enabled = false;
		}
	}

	private void OnClick()
	{
		Select();
	}

	private void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
			Select();
		}
		else if (!Input.GetKey(KeyCode.LeftShift) && !item.forceSelection)
		{
			UnSelectAll();
		}
	}

	private void OnKey(KeyCode key)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && key == KeyCode.Escape)
		{
			OnSelect(false);
		}
	}

	protected override void OnVisible()
	{
		if (!isVisible)
		{
			if (item.fadeOutAfterDelay > 0f && !mFadingOut)
			{
				mFadingOut = true;
				StartCoroutine(DelayedFadeOut());
			}
			TweenAlpha tweenAlpha = TweenAlpha.Begin(sprite.cachedGameObject, 1f, 1f);
			tweenAlpha.from = 0f;
			tweenAlpha.method = UITweener.Method.Linear;
			if (!item.loopAnimation)
			{
				TweenScale tweenScale = TweenScale.Begin(sprite.cachedGameObject, 1f, Vector3.one);
				tweenScale.from = new Vector3(0.01f, 0.01f, 0.01f);
				tweenScale.method = UITweener.Method.BounceOut;
			}
			isVisible = true;
		}
	}

	protected override void OnLoop()
	{
		if (item.loopAnimation)
		{
			isLooping = true;
			if (mLoop == null)
			{
				mLoop = TweenScale.Begin(sprite.cachedGameObject, 1f, Vector3.one);
				mLoop.from = Vector3.one * 1.5f;
				mLoop.style = UITweener.Style.PingPong;
				mLoop.method = UITweener.Method.Linear;
			}
		}
	}

	protected override void OnFadeOut()
	{
		if (mTweenColor == null)
		{
			mColor.a = 0f;
			mTweenColor = TweenColor.Begin(sprite.cachedGameObject, 1f, mColor);
			mColor.a = 1f;
			mTweenColor.from = mColor;
			mTweenColor.method = UITweener.Method.Linear;
		}
		else
		{
			mTweenColor.Play(true);
		}
		mFadingOut = false;
	}

	protected override void Update()
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
}
