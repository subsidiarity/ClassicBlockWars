using UnityEngine;

public class butCustomGame : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.ClickOnButton = true;
		controllerMenu.thisScript.showCustomGame();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_ONLINE);
	}
}
