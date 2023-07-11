using UnityEngine;

public class butResume : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.resume();
	}
}
