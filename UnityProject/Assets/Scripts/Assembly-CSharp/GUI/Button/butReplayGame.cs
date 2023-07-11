using UnityEngine;

public class butReplayGame : MonoBehaviour
{
	private void OnClick()
	{
		GameController.thisScript.replay();
	}
}
