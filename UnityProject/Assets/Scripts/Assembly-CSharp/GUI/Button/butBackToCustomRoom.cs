using UnityEngine;

public class butBackToCustomRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showCustomGame();
	}
}
