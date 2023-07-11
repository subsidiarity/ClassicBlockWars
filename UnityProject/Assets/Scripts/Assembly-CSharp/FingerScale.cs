using Holoville.HOTween;
using UnityEngine;

public class FingerScale : MonoBehaviour
{
	private void Start()
	{
		HOTween.Init();
		scaleDown();
	}

	private void scaleDown()
	{
		HOTween.To(base.transform, 0.3f, new TweenParms().Prop("localScale", new Vector3(0.8f, 0.8f, 0.8f)).Ease(EaseType.Linear).OnComplete(scaleUp));
	}

	private void scaleUp()
	{
		HOTween.To(base.transform, 0.3f, new TweenParms().Prop("localScale", new Vector3(1f, 1f, 1f)).Ease(EaseType.Linear).OnComplete(scaleDown));
	}
}
