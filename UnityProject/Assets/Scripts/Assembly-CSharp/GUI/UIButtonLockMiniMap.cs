using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Interaction/Button Lock MiniMap")]
public class UIButtonLockMiniMap : MonoBehaviour
{
	public bool toggle;

	private void OnClick()
	{
		if ((bool)UIMiniMap.instance)
		{
			toggle = !toggle;
			UIMiniMap.instance.rotateWithPlayer = toggle;
		}
	}
}
