using UnityEngine;

public class CameraZone : MonoBehaviour
{
	private RPG_Camera rpCam;

	[HideInInspector]
	public float k;

	[HideInInspector]
	public float kAdditional = 0.1f;

	private void Start()
	{
		k = (float)Screen.height / 768f;
		rpCam = Camera.main.gameObject.GetComponent<RPG_Camera>();
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			if (rpCam == null)
			{
				rpCam = Camera.main.gameObject.GetComponent<RPG_Camera>();
			}
			rpCam.isDragging = true;
		}
		else
		{
			rpCam.isDragging = false;
		}
	}

	public void OnDrag(Vector2 delta)
	{
		rpCam.controlVector = delta / k * kAdditional;
	}
}
