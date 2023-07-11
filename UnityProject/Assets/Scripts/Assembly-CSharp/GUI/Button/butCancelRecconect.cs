using UnityEngine;

public class butCancelRecconect : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.stopRecconect();
	}
}
