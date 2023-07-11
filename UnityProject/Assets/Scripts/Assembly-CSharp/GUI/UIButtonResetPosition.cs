using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Reset Panning Position")]
public class UIButtonResetPosition : MonoBehaviour
{
	public bool isMinimap = true;

	private void OnClick()
	{
		if (isMinimap)
		{
			if ((bool)UIMiniMap.instance)
			{
				UIMiniMap.instance.ResetPanning();
			}
		}
		else if ((bool)UIWorldMap.instance)
		{
			UIWorldMap.instance.ResetPanning();
		}
	}
}
