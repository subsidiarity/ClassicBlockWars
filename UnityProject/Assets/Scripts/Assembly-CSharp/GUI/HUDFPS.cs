using System.Collections;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class HUDFPS : MonoBehaviour
{
	public Rect startRect = new Rect(10f, 10f, 75f, 150f);

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 0.5f;

	public int nbDecimal = 1;

	private float accum;

	private int frames;

	private Color color = Color.white;

	private string sFPS = string.Empty;

	private GUIStyle style;

	private string maxFPS = "0.0";

	private string minFPS = "60.0";

	private string middleFPS = "0.0";

	private float updateTime = 5f;

	private ArrayList fpsArray;

	private void Start()
	{
		InvokeRepeating("updateMidFPS", 5f, updateTime);
		fpsArray = new ArrayList();
		StartCoroutine(FPS());
	}

	private void updateMidFPS()
	{
		float num = 0f;
		foreach (float item in fpsArray)
		{
			if (item > 0f)
			{
				num += item;
			}
		}
		middleFPS = (num / (float)fpsArray.Count).ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
	}

	private void Update()
	{
		accum += Time.timeScale / Time.deltaTime;
		frames++;
	}

	private IEnumerator FPS()
	{
		while (true)
		{
			float fps = accum / (float)frames;
			if (fps > 0f)
			{
				fpsArray.Add(fps);
				if (fpsArray.Count > 320)
				{
					fpsArray.RemoveAt(0);
				}
			}
			if (fps > float.Parse(maxFPS))
			{
				maxFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			}
			if (fps < float.Parse(minFPS))
			{
				minFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			}
			sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));
			color = ((fps >= 30f) ? Color.green : ((!(fps > 10f)) ? Color.yellow : Color.red));
			accum = 0f;
			frames = 0;
			yield return new WaitForSeconds(frequency);
		}
	}

	private void OnGUI()
	{
		if (style == null)
		{
			style = new GUIStyle(GUI.skin.label);
			style.normal.textColor = Color.white;
			style.alignment = TextAnchor.UpperCenter;
		}
		GUI.color = ((!updateColor) ? Color.white : color);
		startRect = GUI.Window(0, startRect, DoMyWindow, string.Empty);
	}

	private void DoMyWindow(int windowID)
	{
		GUI.Label(new Rect(0f, 20f, startRect.width, startRect.height), sFPS + " FPS", style);
		GUI.Label(new Rect(0f, 40f, startRect.width, startRect.height), maxFPS + " MAX", style);
		GUI.Label(new Rect(0f, 60f, startRect.width, startRect.height), minFPS + " MIN", style);
		GUI.Label(new Rect(0f, 80f, startRect.width, startRect.height), middleFPS + " MID", style);
		if (allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, Screen.width, Screen.height));
		}
	}
}
