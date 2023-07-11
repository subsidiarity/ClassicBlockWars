using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Zoom Mini Map")]
public class UIButtonZoom : MonoBehaviour
{
	public bool zoomIn;

	public bool isMinimap = true;

	public float amount = 0.5f;

	private void OnClick()
	{
		if (isMinimap)
		{
			if ((bool)UIMiniMap.instance)
			{
				if (zoomIn)
				{
					UIMiniMap.instance.ZoomIn(amount);
				}
				else
				{
					UIMiniMap.instance.ZoomOut(amount);
				}
			}
		}
		else if ((bool)UIWorldMap.instance)
		{
			if (zoomIn)
			{
				UIWorldMap.instance.ZoomIn(amount);
			}
			else
			{
				UIWorldMap.instance.ZoomOut(amount);
			}
		}
	}
}
