using UnityEngine;

public class butFly : MonoBehaviour
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
		CheckCache();
		if (isDown)
		{
			Debug.Log("cacheController=" + cacheController);
			cacheController.lastJumpButtonTime = Time.time;
			cacheController.jetpack.activated = true;
			cacheController.jetpack.isFlying = true;
			if (!cacheController.jumping)
			{
				cacheController.jumping = true;
			}
		}
		else
		{
			cacheController.jetpack.activated = false;
			cacheController.jetpack.isFlying = false;
		}
	}

	public void OnDrag(Vector2 delta)
	{
		camZone.OnDrag(delta / k);
	}

	private void CheckCache()
	{
		if (cacheController == null)
		{
			cacheController = GameController.thisScript.myPlayer.GetComponent<ThirdPersonController>();
		}
	}
}
