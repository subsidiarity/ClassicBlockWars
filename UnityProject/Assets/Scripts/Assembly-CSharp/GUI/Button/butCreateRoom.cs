using UnityEngine;

public class butCreateRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showCreateRoom();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_CREATEGAME);
	}
}
