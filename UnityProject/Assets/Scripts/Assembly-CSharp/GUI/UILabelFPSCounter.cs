using UnityEngine;

[RequireComponent(typeof(UILabel))]
[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Label FPS Counter")]
public class UILabelFPSCounter : MonoBehaviour
{
	public float updateInterval = 1f;

	private UILabel label;

	private float accum;

	private float frames;

	private float timeleft;

	private float fps = 15f;

	private double lastSample;

	private float gotIntervals;

	private void Start()
	{
		label = GetComponent<UILabel>();
		timeleft = updateInterval;
		lastSample = Time.realtimeSinceStartup;
	}

	public float GetFPS()
	{
		return fps;
	}

	public bool HasFPS()
	{
		return gotIntervals > 2f;
	}

	private void Update()
	{
		frames += 1f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = (float)((double)realtimeSinceStartup - lastSample);
		lastSample = realtimeSinceStartup;
		timeleft -= num;
		accum += 1f / num;
		if (!(timeleft <= 0f))
		{
			return;
		}
		fps = accum / frames;
		if ((bool)label)
		{
			label.text = fps.ToString("f2");
		}
		timeleft = updateInterval;
		accum = 0f;
		frames = 0f;
		gotIntervals += 1f;
		if ((bool)label)
		{
			if (fps < 30f)
			{
				label.color = Color.yellow;
			}
			else if (fps < 10f)
			{
				label.color = Color.red;
			}
			else
			{
				label.color = Color.green;
			}
		}
	}
}
