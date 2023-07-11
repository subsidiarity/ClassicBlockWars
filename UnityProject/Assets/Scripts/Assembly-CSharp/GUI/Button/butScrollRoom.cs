using UnityEngine;

public class butScrollRoom : MonoBehaviour
{
	public UILabel labelKolPlayers;

	public UILabel labelNameRoom;

	public GameObject sprLock;

	public GameObject sprUnlock;

	public string nameRoom = string.Empty;

	public string password = string.Empty;

	private void OnClick()
	{
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_ROOM + nameRoom);
		settings.playSoundButton();
		if (password.Equals(string.Empty))
		{
			controllerConnectPhoton.thisScript.joinRoomName(nameRoom);
			return;
		}
		Save.SaveString(settings.keyRoomName, nameRoom);
		Save.SaveString(settings.keyRoomPass, password);
		controllerMenu.thisScript.showEnterPassword();
	}
}
