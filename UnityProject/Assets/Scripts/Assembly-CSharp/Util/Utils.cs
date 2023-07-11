using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
	private static Utils instance;

	private int lastId;

	private List<int> invokeIDs;

	public static Utils Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(Utils)) as Utils;
			}
			return instance;
		}
	}

	public void SafeInvoke(Action act, float t, bool timeScaleDependant)
	{
		lastId++;
		StartCoroutine(coInvoke(act, t, timeScaleDependant));
	}

	public void CancelSafeInvoke(Action act, float t, bool timeScaleDependant)
	{
		StartCoroutine(coInvoke(act, t, timeScaleDependant));
	}

	private IEnumerator coInvoke(Action act, float t, bool timeScaleDependant)
	{
		if (!timeScaleDependant)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + t)
			{
				yield return null;
			}
			act();
		}
		else
		{
			yield return new WaitForSeconds(t);
			act();
		}
	}
}
