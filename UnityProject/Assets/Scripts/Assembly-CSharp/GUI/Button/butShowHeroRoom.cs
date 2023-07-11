using UnityEngine;

public class butShowHeroRoom : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showHeroRoom();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_HERO_ROOM);
	}
}
