using UnityEngine;

public class butShowShopGunsFromHero : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.showShopGuns(shopController.exitTo.heroRoom);
	}
}
