using UnityEngine;

public class ButtonJump : MonoBehaviour
{
	private ThirdPersonController cacheController;

	private float k;

	public CameraZone camZone;

	private void Start()
	{
		k = (float)Screen.height / 768f;
	}

	private void OnPress(bool isDown)
	{
		// Camera.main.gameObject.GetComponent<RPG_Camera>().isDragging = isDown;
		CheckCache();
		cacheController.SetJumping(isDown);
	}

	public void OnDrag(Vector2 delta)
	{
		camZone.OnDrag(delta / k);
	}

	private void OnDoubleClick()
	{
		CheckCache();
		cacheController.ActivateJetpack();
	}

	private void CheckCache()
	{
		if (cacheController == null)
		{
			cacheController = GameController.thisScript.myPlayer.GetComponent<ThirdPersonController>();
		}
	}
}
