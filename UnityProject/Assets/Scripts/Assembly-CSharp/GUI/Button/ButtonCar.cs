using UnityEngine;

public class ButtonCar : MonoBehaviour
{
	private void Start()
	{
	}

	public void OnPress(bool isDown)
	{
		if (isDown)
		{
			GameController.thisScript.playerScript.TryGetIntoOrOutVehicle();
		}
	}
}
