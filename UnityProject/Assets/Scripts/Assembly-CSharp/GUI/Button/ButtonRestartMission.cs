using UnityEngine;

public class ButtonRestartMission : MonoBehaviour
{
	private bool missionManagerSet;

	private void OnEnable()
	{
		if (!missionManagerSet)
		{
			MissionManager.Instance.pauseRestartButton = base.gameObject;
			missionManagerSet = true;
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		if (!GameController.thisScript.playerScript.isDead)
		{
			settings.playSoundButton();
			MissionManager.Instance.RestartLastMission();
			GameController.thisScript.resume();
		}
	}
}
