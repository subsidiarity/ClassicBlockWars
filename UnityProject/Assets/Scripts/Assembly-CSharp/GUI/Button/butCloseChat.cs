using UnityEngine;

public class butCloseChat : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		ChatController.thisScript.closeChat();
	}
}
