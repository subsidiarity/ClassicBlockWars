using UnityEngine;

public class butShopFromGame : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.showShopGuns(shopController.exitTo.gameWin);
	}
}
