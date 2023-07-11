using UnityEngine;

public class butShowShopToMenu : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.showShopGunsFromMenu();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_OPEN_SHOP_M);
	}
}
