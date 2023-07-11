using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Show World Map")]
public class UIButtonShowMap : MonoBehaviour
{
	private void OnClick()
	{
		if ((bool)UIWorldMap.instance)
		{
			UIWorldMap.instance.Show();
		}
	}
}
