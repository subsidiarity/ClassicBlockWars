using UnityEngine;

public class butZoomOutWorldMap : MonoBehaviour
{
	private void OnClick()
	{
		GameController.thisScript.zoomWorldMapOut();
	}
}
