using UnityEngine;

public class butCreateRoomAndPlay : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.createNewRoom();
	}
}
