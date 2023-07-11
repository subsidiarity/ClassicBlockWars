using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	public new GameObject camera;

	public GameObject target;

	public float damping = 1f;

	public float rotateSpeed = 5f;

	private Vector2 deltaPosition;

	private Vector2 initialTPosition;

	public static Vector3 initialPosition;

	private bool draggingByHand;

	private ThirdPersonController controller;

	public static Vector3 offset;

	private Quaternion initialRotation;

	private void Start()
	{
		offset = target.transform.position - camera.transform.position;
		controller = target.GetComponent<ThirdPersonController>();
	}

	private void LateUpdate()
	{
		if (!draggingByHand && controller.isMoving)
		{
			RotateNormally();
		}
		else
		{
			RotateByHand();
		}
		Debug.Log(controller.isMoving);
	}

	private void RotateNormally()
	{
		float y = camera.transform.eulerAngles.y;
		float y2 = target.transform.eulerAngles.y;
		float y3 = Mathf.LerpAngle(y, y2, Time.deltaTime * damping);
		Quaternion quaternion = Quaternion.Euler(0f, y3, 0f);
		camera.transform.position = target.transform.position - quaternion * offset;
		camera.transform.LookAt(target.transform);
	}

	private void RotateByHand()
	{
		float y = camera.transform.eulerAngles.y;
		float x = target.transform.localEulerAngles.x;
		Debug.Log(x);
		float y2 = target.transform.eulerAngles.y;
		Quaternion angle = Quaternion.Euler(0f, deltaPosition.x, 0f);
		camera.transform.position = RotatePointAroundPivot(camera.transform.position, target.transform.position, angle);
		Quaternion angle2 = Quaternion.Euler(deltaPosition.y, 0f, 0f);
		camera.transform.position = RotatePointAroundPivot(camera.transform.position, target.transform.position, angle2);
		deltaPosition = Vector2.zero;
		camera.transform.LookAt(target.transform);
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
	{
		return angle * (point - pivot) + pivot;
	}

	private float AngleDistance(float a, float b)
	{
		a = Mathf.Repeat(a, 360f);
		b = Mathf.Repeat(b, 360f);
		return Mathf.Abs(b - a);
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			initialTPosition = UICamera.lastTouchPosition;
			draggingByHand = true;
		}
		else
		{
			deltaPosition = Vector2.zero;
			draggingByHand = false;
		}
	}

	private void OnDrag(Vector2 delta)
	{
		deltaPosition = Vector2.zero;
		deltaPosition = delta;
	}

	private void Update()
	{
	}

	private static void getTouchCount()
	{
		Debug.Log(UICamera.GetTouch(0));
	}
}
