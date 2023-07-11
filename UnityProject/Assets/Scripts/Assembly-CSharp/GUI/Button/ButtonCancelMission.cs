using UnityEngine;

public class ButtonCancelMission : MonoBehaviour
{
	private bool missionManagerSet;

	private void OnEnable()
	{
		if (!missionManagerSet)
		{
			MissionManager.Instance.pauseCancelButton = base.gameObject;
			missionManagerSet = true;
			base.gameObject.SetActive(false);
		}
	}

	private void OnClick()
	{
		MissionManager.Instance.CancelCurrentMission();
		settings.playSoundButton();
		GameController.thisScript.resume();
	}
}
