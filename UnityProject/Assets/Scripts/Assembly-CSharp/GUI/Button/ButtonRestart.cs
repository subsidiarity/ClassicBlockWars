using UnityEngine;

public class ButtonRestart : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		Restart();
		MissionView mView = MissionManager.Instance.mView;
		if (mView != null)
		{
			mView.HideMissions();
			mView.ShowGameInterface();
			Object.Destroy(mView.missionEndGamePanel);
		}
	}

	private void Restart()
	{
		MissionManager.Instance.RestartLastMission();
	}
}
