using UnityEngine;

public class butPredSkin : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.predSkins();
	}
}
