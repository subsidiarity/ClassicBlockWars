using NJG;
using UnityEngine;

public class UIMapColorChanger : MonoBehaviour
{
	public KeyCode changeKey = KeyCode.C;

	public KeyCode resetKey = KeyCode.R;

	private Color mColor = Color.white;

	private Color mMinimapColor = Color.white;

	private Color mWorldmapColor = Color.white;

	private void Start()
	{
		if (UIMiniMapBase.inst != null && UIMiniMapBase.inst.material != null)
		{
			mMinimapColor = UIMiniMapBase.inst.material.color;
		}
		if (UIWorldMapBase.inst != null && UIWorldMapBase.inst.material != null)
		{
			mWorldmapColor = UIWorldMapBase.inst.material.color;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(changeKey))
		{
			mColor = ColorHSV.GetRandomColor(Random.Range(0f, 360f), 1f, 1f);
			if (UIMiniMapBase.inst != null && UIMiniMapBase.inst.material != null)
			{
				UIMiniMapBase.inst.material.color = mColor;
				UIMiniMapBase.inst.material.SetColor("_Color", mColor);
			}
			if (UIWorldMapBase.inst != null && UIWorldMapBase.inst.material != null)
			{
				UIWorldMapBase.inst.material.color = mColor;
				UIWorldMapBase.inst.material.SetColor("_Color", mColor);
			}
		}
		if (Input.GetKeyDown(resetKey))
		{
			if (UIMiniMapBase.inst != null && UIMiniMapBase.inst.material != null)
			{
				UIMiniMapBase.inst.material.color = mMinimapColor;
				UIMiniMapBase.inst.material.SetColor("_Color", mMinimapColor);
			}
			if (UIWorldMapBase.inst != null && UIWorldMapBase.inst.material != null)
			{
				UIWorldMapBase.inst.material.color = mWorldmapColor;
				UIWorldMapBase.inst.material.SetColor("_Color", mWorldmapColor);
			}
		}
	}
}
