using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Generate Map")]
public class UIButtonGenerate : MonoBehaviour
{
	private void OnClick()
	{
		NJGMap.instance.GenerateMap();
	}
}
