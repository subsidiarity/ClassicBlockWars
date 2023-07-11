using UnityEngine;

public class butExitFromShop : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.hideShop();
	}
}
