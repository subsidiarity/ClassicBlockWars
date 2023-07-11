using UnityEngine;

public class butExitToMenu : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.exitToMenu();
	}
}
