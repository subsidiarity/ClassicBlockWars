using UnityEngine;

public class butShowPlayersList : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.showListPlayers();
	}
}
