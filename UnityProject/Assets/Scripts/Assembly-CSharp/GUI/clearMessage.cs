using Holoville.HOTween;
using UnityEngine;

public class clearMessage : MonoBehaviour
{
	public float timeForClear;

	public UILabel tekLabel;

	private void Start()
	{
		HOTween.Init(false, false, false);
		if (tekLabel == null)
		{
			tekLabel = GetComponent<UILabel>();
		}
	}

	public void start(float vrem)
	{
		HOTween.Kill(this);
		timeForClear = vrem;
		HOTween.To(this, timeForClear, new TweenParms().Prop("timeForClear", 0f).OnComplete(Clear));
	}

	public void Clear()
	{
		tekLabel.text = string.Empty;
	}
}
