using UnityEngine;

public class butDialogToShop : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.thisScript.showCoinShop();
	}
}
