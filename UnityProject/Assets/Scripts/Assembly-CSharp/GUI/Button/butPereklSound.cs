using UnityEngine;

public class butPereklSound : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.pereklSoundEnabled();
	}
}
