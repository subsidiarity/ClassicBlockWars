using UnityEngine;

public class butSetSkin : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.setNewSkin();
	}
}
