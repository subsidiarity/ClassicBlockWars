using UnityEngine;

public class butBackAndSaveProfile : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.saveProfile();
		controllerMenu.thisScript.showMainMenu();
	}
}
