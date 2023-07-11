using UnityEngine;

public class ButtonMissions : MonoBehaviour
{
	private void OnClick()
	{
		if (!GameController.thisScript.playerScript.isDead && !MissionManager.Instance.panelMissions.gameObject.activeSelf)
		{
			settings.playSoundButton();
			MissionManager.Instance.panelMissionsButton.SetActive(false);
			MissionManager.Instance.ShowMissionView();
		}
	}
}
