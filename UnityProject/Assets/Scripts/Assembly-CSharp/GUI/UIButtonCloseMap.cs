using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Close Map")]
public class UIButtonCloseMap : MonoBehaviour
{
	private void OnClick()
	{
		if ((bool)UIWorldMap.instance)
		{
			UIWorldMap.instance.Hide();
		}
	}
}
