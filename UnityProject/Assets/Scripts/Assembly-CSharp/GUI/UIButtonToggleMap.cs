using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Toggle World Map")]
public class UIButtonToggleMap : MonoBehaviour
{
	private void OnClick()
	{
		UIMiniMap.instance.ToggleWorldMap();
	}
}
