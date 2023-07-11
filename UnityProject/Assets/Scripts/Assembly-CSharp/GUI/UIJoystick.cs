using UnityEngine;

public class UIJoystick : MonoBehaviour
{
	public Transform target;

	public float radius = 50f;

	public Vector2 position;

	public float speed;

	private Vector3 initPos;

	private Vector3 initRot;

	public float xRotSpeed = 200f;

	public float yRotSpeed = 200f;

	public float rotDamp = 5f;

	public float yMinRotLimit = -10f;

	public float yMaxRotLimit = 60f;

	private float xDeg;

	private float yDeg;

	private Quaternion desiredRotation;

	private Quaternion currentRotation;

	private Quaternion rotation;

	private Vector2 HELL = new Vector3(-1000f, -1000f, -1000f);

	private GameObject parentPanel;

	private void Start()
	{
		parentPanel = base.gameObject.transform.parent.gameObject;
	}

	private void LateUpdate()
	{
	}

	private void OnDrag(Vector2 delta)
	{
		Vector3 origin = UICamera.currentCamera.ScreenPointToRay(UICamera.lastTouchPosition).origin;
		origin.z = 0f;
		target.position = origin;
		float magnitude = target.localPosition.magnitude;
		if (magnitude > radius)
		{
			target.localPosition = Vector3.ClampMagnitude(target.localPosition, radius);
		}
		position = target.localPosition;
		position = position / radius * Mathf.InverseLerp(radius, 2f, 1f);
	}

	public void OnPress(bool pressed)
	{
		if (!pressed)
		{
			TweenAlpha.Begin(parentPanel, 0.5f, 0f);
			position = Vector2.zero;
			target.position = base.transform.position;
		}
		else
		{
			TweenAlpha.Begin(parentPanel, 0.1f, 1f);
		}
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
	}
}
