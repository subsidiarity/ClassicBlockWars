using UnityEngine;

public class butChooseRazdel : MonoBehaviour
{
	public ShopItemRazdel razdelScript;

	private void OnClick()
	{
		if (shopController.thisScript != null)
		{
			shopController.thisScript.SelectRadel(razdelScript);
		}
	}
}
