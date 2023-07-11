using UnityEngine;

public class butHidePlayersList : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.hideListPlayers();
	}
}
