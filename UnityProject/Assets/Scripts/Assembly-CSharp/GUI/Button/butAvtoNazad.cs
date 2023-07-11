using UnityEngine;

public class butAvtoNazad : MonoBehaviour
{
	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			GameController.thisScript.moveAvtoNazad();
		}
		else
		{
			GameController.thisScript.stopMoveAvtoNazad();
		}
	}
}
