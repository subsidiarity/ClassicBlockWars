using UnityEngine;

public class butUpload : MonoBehaviour
{
	private void OnClick()
	{
		settings.playSoundButton();
		controllerMenu.thisScript.upload();
	}
}
