using UnityEngine;

public class butZoomInWorldMap : MonoBehaviour
{
	private void OnClick()
	{
		GameController.thisScript.zoomWorldMapIn();
	}
}
