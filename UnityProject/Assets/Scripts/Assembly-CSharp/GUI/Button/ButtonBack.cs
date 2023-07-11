using UnityEngine;

public class ButtonBack : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		MissionView mView = MissionManager.Instance.mView;
		if (mView != null)
		{
			mView.HideMissions();
			mView.ShowGameInterface();
			Object.Destroy(mView.missionEndGamePanel);
		}
	}
}
