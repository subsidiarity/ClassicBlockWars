using UnityEngine;

public class butUvelKolPlayers : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.uvelKolPlayers();
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			InvokeRepeating("fastUvel", 1f, 0.1f);
		}
		else
		{
			CancelInvoke("fastUvel");
		}
	}

	private void fastUvel()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.uvelKolPlayers();
	}
}
