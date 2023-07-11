using UnityEngine;

public class butShowSettings : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showSettings();
	}
}
