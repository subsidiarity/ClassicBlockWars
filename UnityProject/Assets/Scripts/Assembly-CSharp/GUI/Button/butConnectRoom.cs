using UnityEngine;

public class butConnectRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.connectRoomWithPassword();
	}
}
