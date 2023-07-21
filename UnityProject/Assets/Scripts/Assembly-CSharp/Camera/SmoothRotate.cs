using UnityEngine;

public class SmoothRotate : MonoBehaviour
{
	public Transform target;

	public float distance = 7f;

	public float damping = 1f;

	public float rotationDamping = 10f;

	public float xSpeed = 250f;

	public float ySpeed = 120f;

	public float yMinLimit;

	public float yMaxLimit = 60f;

	public float x;

	public float y;

	public float smoothTime = 0.3f;

	public float xSmooth;

	public float ySmooth;

	private float xVelocity;

	private float yVelocity;

	private Ray toPlayer;

	private float lastYPosition;

	private Vector3 posSmooth = Vector3.zero;

	private Vector3 posVelocity = Vector3.zero;

	private Vector3 wantedPosition;

	public ThirdPersonController playerController;

	public bool isDragging;

	public CameraZone camZone;

	public Vector2 smoothRotateVector;

	private void Start()
	{
		lastYPosition = distance / 2f;
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if ((bool)base.GetComponent<Rigidbody>())
		{
			base.GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private void AlignCamera(Vector3 relativeTo)
	{
		base.transform.position = relativeTo;
	}

	private void MoveBack()
	{
		Vector3 relativeTo = checkRay();
		if (relativeTo.Equals(Vector3.zero))
		{
			if (playerController.IsMovingBackwards())
			{
				wantedPosition = target.TransformPoint(0f, lastYPosition, distance * 2f);
			}
			else
			{
				wantedPosition = target.TransformPoint(0f, lastYPosition, 0f - distance);
			}
			base.transform.position = Vector3.Lerp(base.transform.position, wantedPosition, Time.deltaTime * damping);
			Quaternion to = Quaternion.LookRotation(target.position - base.transform.position, target.up);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, Time.deltaTime * rotationDamping);
			x = base.transform.position.x;
			y = base.transform.position.y;
			smoothRotateVector = Vector2.zero;
		}
		else
		{
			AlignCamera(relativeTo);
		}
	}

	private void RotateAround()
	{
		Vector3 relativeTo = checkRay();
		if (relativeTo.Equals(Vector3.zero))
		{
			if ((bool)target)
			{
				x += smoothRotateVector.x * xSpeed * 0.002f;
				y -= smoothRotateVector.y * ySpeed * 0.002f;
				xSmooth = Mathf.SmoothDamp(xSmooth, x, ref xVelocity, smoothTime);
				ySmooth = Mathf.SmoothDamp(ySmooth, y, ref yVelocity, smoothTime);
				ySmooth = ClampAngle(ySmooth, yMinLimit, yMaxLimit);
				Quaternion quaternion = Quaternion.Euler(ySmooth, xSmooth, 0f);
				base.transform.rotation = quaternion;
				posSmooth = target.position;
				Vector3 vector = new Vector3(0f, 0f, 0f - distance);
				base.transform.position = quaternion * vector + posSmooth;
				lastYPosition = base.transform.position.y;
				smoothRotateVector = Vector2.zero;
			}
		}
		else
		{
			AlignCamera(relativeTo);
		}
	}

	private Vector3 checkRay()
	{
		Debug.DrawRay(target.transform.position, base.transform.position - target.transform.position);
		RaycastHit hitInfo;
		if (Physics.Raycast(toPlayer, out hitInfo, 10000f) && !hitInfo.transform.gameObject.tag.Equals("MainCamera"))
		{
			return Vector3.zero;
		}
		return Vector3.zero;
	}

	private void LateUpdate()
	{
		toPlayer = new Ray(target.transform.position, base.transform.position - target.transform.position);
		if (playerController != null)
		{
			if (!isDragging)
			{
				MoveBack();
			}
			else
			{
				RotateAround();
			}
		}
	}

	public static float ClampAngle(float angle, float min, float max)
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
}
