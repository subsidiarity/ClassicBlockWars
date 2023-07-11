using UnityEngine;

public class butPereklMusic : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.pereklMusicEnabled();
	}
}
