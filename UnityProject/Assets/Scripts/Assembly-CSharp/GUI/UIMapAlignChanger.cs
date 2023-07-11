using System;
using NJG;
using UnityEngine;

public class UIMapAlignChanger : MonoBehaviour
{
	public KeyCode changeKey = KeyCode.P;

	private UIMiniMapBase map;

	private string[] mPivots;

	private void Start()
	{
		map = UIMiniMapBase.inst;
		mPivots = Enum.GetNames(typeof(UIMapBase.Pivot));
	}

	private void Update()
	{
		if (!(map == null) && Input.GetKeyDown(changeKey))
		{
			map.pivot = (UIMapBase.Pivot)(int)Enum.Parse(typeof(UIMapBase.Pivot), mPivots[UnityEngine.Random.Range(0, mPivots.Length - 1)]);
		}
	}
}
