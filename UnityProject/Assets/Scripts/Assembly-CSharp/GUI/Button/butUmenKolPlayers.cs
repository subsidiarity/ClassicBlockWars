using UnityEngine;

public class butUmenKolPlayers : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.umenKolPlayers();
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			InvokeRepeating("fastUmen", 1f, 0.1f);
		}
		else
		{
			CancelInvoke("fastUmen");
		}
	}

	private void fastUmen()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.umenKolPlayers();
	}
}
