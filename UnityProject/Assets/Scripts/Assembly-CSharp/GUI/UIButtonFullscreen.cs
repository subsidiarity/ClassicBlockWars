using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Full Screen")]
public class UIButtonFullscreen : MonoBehaviour
{
	public UIWidget normalState;

	public UIWidget exitState;

	public UIStretch stretch;

	public UIWidget widget;

	public UIDragObject drag;

	public UIDragResize resize;

	public float speed = 1f;

	public UITweener.Method ease = UITweener.Method.BounceOut;

	public int defaultWidth = 750;

	public int defaultHeight = 400;

	private bool mToggle;

	private void Awake()
	{
		NGUITools.SetActive(normalState.gameObject, true);
		NGUITools.SetActive(exitState.gameObject, false);
	}

	private void OnClick()
	{
		mToggle = !mToggle;
		if (mToggle)
		{
			widget.cachedTransform.localPosition = Vector3.zero;
			NGUITools.SetActive(normalState.gameObject, false);
			NGUITools.SetActive(exitState.gameObject, true);
			TweenWidth tweenWidth = TweenWidth.Begin(widget, speed, Screen.width);
			tweenWidth.method = ease;
			EventDelegate.Add(tweenWidth.onFinished, OnFullScreen, true);
			TweenHeight tweenHeight = TweenHeight.Begin(widget, speed, Screen.height);
			tweenHeight.method = ease;
			if (drag != null)
			{
				NGUITools.SetActive(drag.gameObject, false);
			}
			if (resize != null)
			{
				NGUITools.SetActive(resize.gameObject, false);
			}
		}
		else
		{
			NGUITools.SetActive(normalState.gameObject, true);
			NGUITools.SetActive(exitState.gameObject, false);
			stretch.style = UIStretch.Style.None;
			stretch.enabled = false;
			TweenWidth tweenWidth2 = TweenWidth.Begin(widget, speed, defaultWidth);
			tweenWidth2.method = ease;
			TweenHeight tweenHeight2 = TweenHeight.Begin(widget, speed, defaultHeight);
			tweenHeight2.method = ease;
			if (drag != null)
			{
				NGUITools.SetActive(drag.gameObject, true);
			}
			if (resize != null)
			{
				NGUITools.SetActive(resize.gameObject, true);
			}
		}
	}

	private void OnFullScreen()
	{
		stretch.enabled = true;
		stretch.style = UIStretch.Style.Both;
	}
}
