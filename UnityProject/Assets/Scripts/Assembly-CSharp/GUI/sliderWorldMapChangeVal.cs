using UnityEngine;

public class sliderWorldMapChangeVal : MonoBehaviour
{
	private UISlider curSlider;

	private void Start()
	{
		curSlider = GetComponent<UISlider>();
	}

	private void changeValue()
	{
		if (curSlider != null)
		{
			GameController.thisScript.setZoomWorldMap(curSlider.value);
		}
	}
}
