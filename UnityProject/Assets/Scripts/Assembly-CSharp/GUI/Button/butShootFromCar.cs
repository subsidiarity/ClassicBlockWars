using UnityEngine;

public class butShootFromCar : MonoBehaviour
{
	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			GameController.thisScript.carScript.startShoot();
		}
		else
		{
			GameController.thisScript.carScript.cancelShoot();
		}
	}
}
