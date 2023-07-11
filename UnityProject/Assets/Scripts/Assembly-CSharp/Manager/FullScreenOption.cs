using UnityEngine;

public class FullScreenOption : MonoBehaviour
{
	public Vector2 defaultResolution = new Vector2(1280f, 720f);

	public KeyCode key = KeyCode.F5;

	private void Update()
	{
		if (Input.GetKeyDown(key))
		{
			if (Screen.fullScreen)
			{
				Screen.SetResolution((int)defaultResolution.x, (int)defaultResolution.y, false);
			}
			else
			{
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
		}
	}
}
