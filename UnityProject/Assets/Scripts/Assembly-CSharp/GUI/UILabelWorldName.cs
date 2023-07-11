using System;
using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Label World Name")]
[RequireComponent(typeof(UILabel))]
public class UILabelWorldName : MonoBehaviour
{
	private UILabel label;

	private void Awake()
	{
		label = GetComponent<UILabel>();
		if (NJGMap.instance != null)
		{
			NJGMap instance = NJGMap.instance;
			instance.onWorldNameChanged = (Action<string>)Delegate.Combine(instance.onWorldNameChanged, new Action<string>(OnNameChanged));
		}
	}

	private void OnNameChanged(string worldName)
	{
		label.color = NJGMap.instance.zoneColor;
		label.text = worldName;
	}
}
