using UnityEngine;

public class ButtonRun : MonoBehaviour
{
	public bool wasJumping;

	public UISprite staminaSprite;

	private ThirdPersonController tController;

	private UIButton button;

	private float k;

	public CameraZone camZone;

	private void Start()
	{
		button = GetComponent<UIButton>();
		k = (float)Screen.height / 768f;
	}

	private void OnPress(bool isDown)
	{
		if (tController == null)
		{
			tController = GameController.thisScript.playerScript.tController;
		}
		if (!tController.IsJumping())
		{
			tController.isRunning = isDown;
		}
		Camera.main.gameObject.GetComponent<RPG_Camera>().isDragging = isDown;
	}

	private void Update()
	{
		if (tController == null)
		{
			tController = GameController.thisScript.playerScript.tController;
		}
		if (tController != null)
		{
			if (tController.IsJumping() != wasJumping)
			{
				button.isEnabled = tController.IsGrounded();
			}
			staminaSprite.fillAmount = tController.stamina;
			wasJumping = tController.IsJumping();
		}
	}

	public void OnDrag(Vector2 delta)
	{
		camZone.OnDrag(delta);
	}
}
