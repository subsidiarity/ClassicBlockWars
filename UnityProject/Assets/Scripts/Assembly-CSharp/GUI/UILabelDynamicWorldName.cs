using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Label World Name (Dynamic)")]
public class UILabelDynamicWorldName : MonoBehaviour
{
	public UITweener.Method method;

	public bool showOnStart = true;

	public float fadeInDuration = 0.5f;

	public float fadeOutDuration = 1f;

	public float hideDelay = 3f;

	private UILabel label;

	private TweenColor tc;

	private Color oc;

	private void Awake()
	{
		label = GetComponent<UILabel>();
		oc = label.color;
	}

	private void Start()
	{
		if (NJGMap.instance != null)
		{
			NJGMap instance = NJGMap.instance;
			instance.onWorldNameChanged = (Action<string>)Delegate.Combine(instance.onWorldNameChanged, new Action<string>(OnNameChanged));
		}
		if (showOnStart)
		{
			StartCoroutine(FadeIn());
		}
	}

	private IEnumerator FadeIn()
	{
		Color ec = oc;
		ec.a = 1f;
		Color sc = oc;
		sc.a = 0f;
		tc = TweenColor.Begin(base.gameObject, fadeInDuration, ec);
		tc.from = sc;
		tc.method = method;
		yield return new WaitForSeconds(hideDelay);
		FadeOut();
	}

	private void FadeOut()
	{
		Color color = oc;
		color.a = 0f;
		Color from = oc;
		from.a = 1f;
		tc = TweenColor.Begin(base.gameObject, fadeOutDuration, color);
		tc.from = from;
		tc.method = method;
	}

	private void OnNameChanged(string worldName)
	{
		oc = NJGMap.instance.zoneColor;
		StopAllCoroutines();
		StartCoroutine(FadeIn());
		label.text = worldName;
	}
}
