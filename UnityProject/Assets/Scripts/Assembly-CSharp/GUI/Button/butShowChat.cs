using UnityEngine;

public class butShowChat : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.showPanelChat();
	}
}
