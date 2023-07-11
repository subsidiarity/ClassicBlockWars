using UnityEngine;

public class butShopFromPause : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.showShopGuns(shopController.exitTo.gamePause);
	}
}
