using UnityEngine;

public class ButtonStartMission : MonoBehaviour
{
	private void OnClick()
	{
		MissionManager.Instance.mView.StartSelectedMission();
	}
}
