using UnityEngine;

public class butOpenShopCoins : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.thisScript.showCoinShop();
	}
}
