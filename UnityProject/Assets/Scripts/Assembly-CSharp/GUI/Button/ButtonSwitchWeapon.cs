using UnityEngine;

public class ButtonSwitchWeapon : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			GameController.thisScript.weaponManagerScripts.switchToNext();
		}
	}
}
