using UnityEngine;

public class butConnectRandomRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.ClickOnButton = true;
		controllerMenu.thisScript.startRandomGame();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_QUICKGAME);
	}
}
