using UnityEngine;

public class carWithWeapon : MonoBehaviour
{
	private CarBehavior curCar;

	private void OnEnabled()
	{
		curCar = GameController.thisScript.carScript;
	}
}
