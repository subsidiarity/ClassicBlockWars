using UnityEngine;

public class butPlayOfflineMode : MonoBehaviour
{
	private void OnClick()
	{
		controllerMenu.thisScript.playOfflineMode();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_FREEPLAY);
	}
}
