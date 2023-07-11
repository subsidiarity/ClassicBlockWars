using UnityEngine;

public class butPause : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		GameController.thisScript.pause();
		MissionManager.Instance.CheckPauseButtons();
	}
}
