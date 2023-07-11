using UnityEngine;

public class ButBackToHeroRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showHeroRoom();
	}
}
