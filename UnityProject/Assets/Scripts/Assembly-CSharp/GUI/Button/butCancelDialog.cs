using UnityEngine;

public class butCancelDialog : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		shopController.thisScript.hideMessageGoToShop();
	}
}
