using UnityEngine;

public class butBackToMainMenu : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showMainMenu();
	}
}
