using UnityEngine;

public class butAvtoVpered : MonoBehaviour
{
	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			GameController.thisScript.moveAvtoVpered();
		}
		else
		{
			GameController.thisScript.stopMoveAvtoVpered();
		}
	}
}
