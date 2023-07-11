using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
	private const float movementThreshold = 0.1f;

	private const float rotationThreshold = 0.1f;

	private const float groundedDistance = 0.5f;

	public Collider target;

	public new Camera camera;

	public LayerMask obstacleLayers = -1;

	public LayerMask groundLayers = -1;

	public float groundedCheckOffset = 0.7f;

	public float rotationUpdateSpeed = 60f;

	public float lookUpSpeed = 20f;

	public float distanceUpdateSpeed = 10f;

	public float followUpdateSpeed = 10f;

	public float maxForwardAngle = 80f;

	public float minDistance = 0.1f;

	public float maxDistance = 10f;

	public float zoomSpeed = 1f;

	public bool showGizmos = true;

	public bool requireLock = true;

	public bool controlLock = true;

	private Vector3 lastStationaryPosition;

	private float optimalDistance;

	private float targetDistance;

	private bool grounded;

	private float ViewRadius
	{
		get
		{
			float b = optimalDistance / Mathf.Sin(90f - camera.fieldOfView / 2f) * Mathf.Sin(camera.fieldOfView / 2f);
			float a = Mathf.Max(target.bounds.extents.x, target.bounds.extents.z) * 2f;
			return Mathf.Min(a, b);
		}
	}

	private Vector3 SnappedCameraForward
	{
		get
		{
			Vector2 vector = new Vector2(camera.transform.forward.x, camera.transform.forward.z);
			Vector2 vector2 = new Vector2(target.transform.forward.x, target.transform.forward.z);
			vector = vector2.normalized * vector.magnitude;
			return new Vector3(vector.x, camera.transform.forward.y, vector.y);
		}
	}

	private void Reset()
	{
		Setup();
	}

	private void Setup()
	{
		if (target == null)
		{
			target = GetComponent<Collider>();
		}
		if (camera == null && Camera.main != null)
		{
			camera = Camera.main;
		}
	}

	private void Start()
	{
		Setup();
		if (target == null)
		{
			Debug.LogError("No target assigned. Please correct and restart.");
			base.enabled = false;
		}
		else if (camera == null)
		{
			Debug.LogError("No camera assigned. Please correct and restart.");
			base.enabled = false;
		}
		else
		{
			lastStationaryPosition = target.transform.position;
			targetDistance = (optimalDistance = (camera.transform.position - target.transform.position).magnitude);
		}
	}

	private void FixedUpdate()
	{
		grounded = Physics.Raycast(camera.transform.position + target.transform.up * (0f - groundedCheckOffset), target.transform.up * -1f, 0.5f, groundLayers);
		Vector3 direction = camera.transform.position - target.transform.position;
		RaycastHit hitInfo;
		if (Physics.SphereCast(target.transform.position, ViewRadius, direction, out hitInfo, optimalDistance, obstacleLayers))
		{
			targetDistance = Mathf.Min((hitInfo.point - target.transform.position).magnitude, optimalDistance);
		}
		else
		{
			targetDistance = optimalDistance;
		}
	}

	private void Update()
	{
		optimalDistance = Mathf.Clamp(optimalDistance + Input.GetAxis("Mouse ScrollWheel") * (0f - zoomSpeed) * Time.deltaTime, minDistance, maxDistance);
	}

	private void LateUpdate()
	{
		if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && (!requireLock || controlLock || Screen.lockCursor))
		{
			FreeUpdate();
			lastStationaryPosition = target.transform.position;
		}
		else
		{
			Vector3 vector = target.transform.position - lastStationaryPosition;
			if (new Vector2(vector.x, vector.z).magnitude > 0.1f)
			{
				FollowUpdate();
			}
		}
		DistanceUpdate();
	}

	private void FollowUpdate()
	{
		Vector3 vector = target.transform.position - camera.transform.position;
		vector = new Vector3(vector.x, 0f, vector.z);
		float num = Vector3.Angle(vector, target.transform.forward);

		if (num < 0.1f)
		{
			lastStationaryPosition = target.transform.position;
		}

		num *= followUpdateSpeed * Time.deltaTime;

		if (Vector3.Angle(vector, target.transform.right) < Vector3.Angle(vector, target.transform.right * -1f))
		{
			num *= -1f;
		}

		camera.transform.RotateAround(target.transform.position, Vector3.up, num);
	}

	private void FreeUpdate()
	{
		float angle;

		if (Input.GetMouseButton(1))
		{
			FollowUpdate();
		}
		else
		{
			angle = Input.GetAxis("Mouse X") * rotationUpdateSpeed * Time.deltaTime;
			camera.transform.RotateAround(target.transform.position, Vector3.up, angle);
		}

		angle = Input.GetAxis("Mouse Y") * -1f * lookUpSpeed * Time.deltaTime;
		bool flag = Vector3.Angle(camera.transform.forward, target.transform.up * -1f) > Vector3.Angle(camera.transform.forward, target.transform.up);

		if (grounded && flag)
		{
			camera.transform.RotateAround(camera.transform.position, camera.transform.right, angle);
			return;
		}

		camera.transform.RotateAround(target.transform.position, camera.transform.right, angle);
		camera.transform.LookAt(target.transform.position);
		float num = Vector3.Angle(target.transform.forward, SnappedCameraForward);

		if (num > maxForwardAngle)
		{
			camera.transform.RotateAround(target.transform.position, camera.transform.right, (!flag) ? (maxForwardAngle - num) : (num - maxForwardAngle));
		}
	}

	private void DistanceUpdate()
	{
		Vector3 to = target.transform.position + (camera.transform.position - target.transform.position).normalized * targetDistance;
		camera.transform.position = Vector3.Lerp(camera.transform.position, to, Time.deltaTime * distanceUpdateSpeed);
	}

	private void OnDrawGizmosSelected()
	{
		if (showGizmos && !(target == null) && !(camera == null))
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(target.transform.position, target.transform.position + target.transform.forward);
			Gizmos.color = ((!grounded) ? Color.red : Color.blue);
			Gizmos.DrawLine(camera.transform.position + target.transform.up * (0f - groundedCheckOffset), camera.transform.position + target.transform.up * (0f - (groundedCheckOffset + 0.5f)));
			Gizmos.color = Color.green;
			Gizmos.DrawLine(camera.transform.position, camera.transform.position + camera.transform.forward);
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(camera.transform.position, camera.transform.position + SnappedCameraForward);
		}
	}
}
