using UnityEngine;

public class MoveJoystick : MonoBehaviour
{
	public GameObject joystickObject;

	private static UIJoystick jScript;

	private float k;

	private void Start()
	{
		jScript = joystickObject.GetComponent<UIJoystick>();
		k = (float)Screen.height / 768f;
	}

	private void Update()
	{
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			joystickObject.transform.localPosition = new Vector3(UICamera.lastTouchPosition.x / k, UICamera.lastTouchPosition.y / k, 0f);
		}
		joystickObject.SendMessage("OnPress", isDown);
	}

	private void OnDrag(Vector2 delta)
	{
		joystickObject.SendMessage("OnDrag", delta);
	}

	public static bool isJoystickMoving()
	{
		bool result = false;
		if (jScript.position != Vector2.zero)
		{
			result = true;
		}
		return result;
	}
}
