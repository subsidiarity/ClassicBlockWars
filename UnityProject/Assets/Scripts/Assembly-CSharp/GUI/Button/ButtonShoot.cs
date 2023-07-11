using UnityEngine;

public class ButtonShoot : MonoBehaviour
{
	public CameraZone camZone;

	private float k;

	private void Start()
	{
		k = (float)Screen.height / 768f;
	}

	private void OnPress(bool isDown)
	{
		GameController.thisScript.playerScript.isShooting = isDown;
		Camera.main.gameObject.GetComponent<RPG_Camera>().isDragging = isDown;
	}

	public void OnDrag(Vector2 delta)
	{
		camZone.OnDrag(delta);
	}
}
