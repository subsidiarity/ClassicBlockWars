using System;
using UnityEngine;

public class HHHScipt : MonoBehaviour
{
	public string myPath;

	public string startPath;

	public float myFirstTime;

	public float myDelay;

	private bool first = true;

	private Vector3 myOldPosition;

	private void Start()
	{
		myOldPosition = base.transform.position;
	}

	private void newPath()
	{
		float num = 40f;
		int @int = PlayerPrefs.GetInt("trackNumber");
		if (@int == 1)
		{
			num = 45f;
		}
		if (@int == 2)
		{
			num = 45f;
		}
		iTween.MoveTo(base.gameObject, iTween.Hash("path", iTweenPath.GetPath(string.Empty + myPath), "time", num, "easeType", "linear", "loopType", "loop"));
	}

	private void Update()
	{
		float num = base.transform.position.z - myOldPosition.z;
		float num2 = base.transform.position.x - myOldPosition.x;
		if (num != 0f && num2 != 0f)
		{
			float num3 = Mathf.Sqrt(Mathf.Pow(num, 2f) + Mathf.Pow(num2, 2f));
			float num4 = 180f / (float)Math.PI * Mathf.Asin(num / num3);
			if (myOldPosition.x > base.transform.position.x)
			{
				num4 = 180f - num4;
			}
			if (num4 < 0f)
			{
				num4 += 360f;
			}
			num4 = 90f - num4;
			base.transform.rotation = Quaternion.Euler(0f, num4, 0f);
		}
		myOldPosition = base.transform.position;
	}

	private void startMove()
	{
		iTween.MoveTo(base.gameObject, iTween.Hash("path", iTweenPath.GetPath(string.Empty + startPath), "time", myFirstTime, "easeType", "linear", "loopType", "none", "oncomplete", "newPath"));
	}
}
