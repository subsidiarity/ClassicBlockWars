using UnityEngine;

public class ButEquipGun : MonoBehaviour
{
	private void OnClick()
	{
		shopController.thisScript.EquipGun();
	}
}
