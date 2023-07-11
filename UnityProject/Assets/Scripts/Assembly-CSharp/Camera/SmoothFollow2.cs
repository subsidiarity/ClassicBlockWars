using System;
using UnityEngine;

public class SmoothFollow2 : MonoBehaviour
{
	public struct ClipPlaneVertexes
	{
		public Vector3 UpperLeft;

		public Vector3 UpperRight;

		public Vector3 LowerLeft;

		public Vector3 LowerRight;
	}

	public static SmoothFollow2 instance;

	public float TimeRotationCam = 15f;

	public float Height = 1f;

	public Transform target;

	public float distance = 5f;

	public float distanceMax = 30f;

	public float mouseSpeed = 8f;

	public float mouseScroll = 15f;

	public float mouseSmoothingFactor = 0.08f;

	public float camDistanceSpeed = 0.7f;

	public float camBottomDistance = 1f;

	public float firstPersonThreshold = 0.8f;

	public float characterFadeThreshold = 1.8f;

	public bool isDragging;

	private Vector3 desiredPosition;

	public float desiredDistance;

	private float lastDistance;

	private float mouseX;

	private float mouseXSmooth;

	private float mouseXVel;

	private float mouseY;

	private float mouseYSmooth;

	private float mouseYVel;

	private float mouseYMin = -89.5f;

	private float mouseYMax = 89.5f;

	private float distanceVel;

	private bool camBottom;

	private bool constraint;

	private static float halfFieldOfView;

	private static float planeAspect;

	private static float halfPlaneHeight;

	private static float halfPlaneWidth;

	public Vector2 controlVector;

	private float startTimeRotationCam;

	private float startHeight;

	private float startDesiredDistance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		distance = Mathf.Clamp(distance, 0.05f, distanceMax);
		desiredDistance = distance;
		halfFieldOfView = Camera.mainCamera.fieldOfView / 2f * ((float)Math.PI / 180f);
		planeAspect = Camera.mainCamera.aspect;
		halfPlaneHeight = Camera.mainCamera.nearClipPlane * Mathf.Tan(halfFieldOfView);
		halfPlaneWidth = halfPlaneHeight * planeAspect;
		startTimeRotationCam = TimeRotationCam;
		startHeight = Height;
		startDesiredDistance = desiredDistance;
		base.enabled = false;
	}

	public void CheckSettingsHelicopter(bool helic)
	{
		if (helic)
		{
			TimeRotationCam = 3f;
			Height = 33f;
			desiredDistance = 19f;
			distance = desiredDistance;
		}
		else
		{
			TimeRotationCam = startTimeRotationCam;
			Height = startHeight;
			desiredDistance = startDesiredDistance;
			distance = desiredDistance;
		}
	}

	public static void CameraSetup()
	{
		GameObject gameObject;
		if (Camera.mainCamera != null)
		{
			gameObject = Camera.mainCamera.gameObject;
		}
		else
		{
			gameObject = new GameObject("Main Camera");
			gameObject.AddComponent("Camera");
			gameObject.tag = "MainCamera";
		}
		if (!gameObject.GetComponent("RPG_Camera"))
		{
			gameObject.AddComponent("RPG_Camera");
		}
		RPG_Camera rPG_Camera = gameObject.GetComponent("RPG_Camera") as RPG_Camera;
		GameObject gameObject2 = GameObject.Find("cameraPivot");
		rPG_Camera.cameraPivot = gameObject2.transform;
	}

	private void LateUpdate()
	{
		if (target == null)
		{
			Debug.Log("Error: No cameraPivot found! Please read the manual for further instructions.");
			return;
		}
		GetInput();
		GetDesiredPosition();
		PositionUpdate();
	}

	private void GetInput()
	{
		if ((double)distance > 0.1)
		{
			Debug.DrawLine(base.transform.position, base.transform.position - Vector3.up * camBottomDistance, Color.green);
			camBottom = Physics.Linecast(base.transform.position, base.transform.position - Vector3.up * camBottomDistance);
		}
		bool flag = camBottom && base.transform.position.y - target.transform.position.y <= 0f;
		bool flag2 = true;
		if (!GameController.thisScript.playerScript.inHelic)
		{
			if (GameController.thisScript.newDrivingScript.isBack)
			{
				flag2 = false;
			}
			if (GameController.thisScript.newDrivingScript.isGo)
			{
				flag2 = true;
			}
			if (GameController.thisScript.newDrivingScript.currentSpeed > -5f)
			{
				flag2 = true;
			}
		}
		if (target != null)
		{
			if (flag2)
			{
				mouseX = Mathf.LerpAngle(mouseX, target.eulerAngles.y, TimeRotationCam * Time.deltaTime);
			}
			else
			{
				mouseX = Mathf.LerpAngle(mouseX, target.eulerAngles.y + 180f, TimeRotationCam * Time.deltaTime);
			}
			mouseY = Mathf.LerpAngle(mouseY, target.eulerAngles.x + Height, TimeRotationCam * Time.deltaTime);
		}
		mouseY = ClampAngle(mouseY, -89.5f, 89.5f);
		mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, mouseSmoothingFactor);
		mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, mouseSmoothingFactor);
		if (flag)
		{
			mouseYMin = mouseY;
		}
		else
		{
			mouseYMin = -89.5f;
		}
		mouseYSmooth = ClampAngle(mouseYSmooth, mouseYMin, mouseYMax);
		if (desiredDistance > distanceMax)
		{
			desiredDistance = distanceMax;
		}
		if ((double)desiredDistance < 0.05)
		{
			desiredDistance = 0.05f;
		}
		controlVector = Vector2.zero;
	}

	private void GetDesiredPosition()
	{
		distance = desiredDistance;
		desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);
		constraint = false;
		float num = CheckCameraClipPlane(target.position, desiredPosition);
		if (num != -1f)
		{
			distance = num;
			desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, distance);
			constraint = true;
		}
	}

	private void PositionUpdate()
	{
		base.transform.position = desiredPosition;
		if ((double)distance > 0.05)
		{
			base.transform.LookAt(target);
		}
	}

	private void CharacterFade()
	{
		if (RPG_Animation.instance == null)
		{
			return;
		}
		if (distance < firstPersonThreshold)
		{
			RPG_Animation.instance.renderer.enabled = false;
		}
		else if (distance < characterFadeThreshold)
		{
			RPG_Animation.instance.renderer.enabled = true;
			float num = 1f - (characterFadeThreshold - distance) / (characterFadeThreshold - firstPersonThreshold);
			if (RPG_Animation.instance.renderer.material.color.a != num)
			{
				RPG_Animation.instance.renderer.material.color = new Color(RPG_Animation.instance.renderer.material.color.r, RPG_Animation.instance.renderer.material.color.g, RPG_Animation.instance.renderer.material.color.b, num);
			}
		}
		else
		{
			RPG_Animation.instance.renderer.enabled = true;
			if (RPG_Animation.instance.renderer.material.color.a != 1f)
			{
				RPG_Animation.instance.renderer.material.color = new Color(RPG_Animation.instance.renderer.material.color.r, RPG_Animation.instance.renderer.material.color.g, RPG_Animation.instance.renderer.material.color.b, 1f);
			}
		}
	}

	private Vector3 GetCameraPosition(float xAxis, float yAxis, float distance)
	{
		Vector3 vector = new Vector3(0f, 0f, 0f - distance);
		Quaternion quaternion = Quaternion.Euler(xAxis, yAxis, 0f);
		return target.position + quaternion * vector;
	}

	private float CheckCameraClipPlane(Vector3 from, Vector3 to)
	{
		float num = -1f;
		ClipPlaneVertexes clipPlaneAt = GetClipPlaneAt(to);
		Debug.DrawLine(clipPlaneAt.UpperLeft, clipPlaneAt.UpperRight);
		Debug.DrawLine(clipPlaneAt.UpperRight, clipPlaneAt.LowerRight);
		Debug.DrawLine(clipPlaneAt.LowerRight, clipPlaneAt.LowerLeft);
		Debug.DrawLine(clipPlaneAt.LowerLeft, clipPlaneAt.UpperLeft);
		Debug.DrawLine(from, to, Color.red);
		Debug.DrawLine(from - base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, clipPlaneAt.UpperLeft, Color.cyan);
		Debug.DrawLine(from + base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, clipPlaneAt.UpperRight, Color.cyan);
		Debug.DrawLine(from - base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, clipPlaneAt.LowerLeft, Color.cyan);
		Debug.DrawLine(from + base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, clipPlaneAt.LowerRight, Color.cyan);
		RaycastHit hitInfo;
		if (Physics.Linecast(from, to, out hitInfo) && OwnerCollider(hitInfo))
		{
			num = hitInfo.distance - Camera.mainCamera.nearClipPlane;
		}
		if (Physics.Linecast(from - base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, clipPlaneAt.UpperLeft, out hitInfo) && OwnerCollider(hitInfo) && (hitInfo.distance < num || num == -1f))
		{
			num = Vector3.Distance(hitInfo.point + base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, from);
		}
		if (Physics.Linecast(from + base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, clipPlaneAt.UpperRight, out hitInfo) && OwnerCollider(hitInfo) && (hitInfo.distance < num || num == -1f))
		{
			num = Vector3.Distance(hitInfo.point - base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, from);
		}
		if (Physics.Linecast(from - base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, clipPlaneAt.LowerLeft, out hitInfo) && OwnerCollider(hitInfo) && (hitInfo.distance < num || num == -1f))
		{
			num = Vector3.Distance(hitInfo.point + base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, from);
		}
		if (Physics.Linecast(from + base.transform.right * halfPlaneWidth - base.transform.up * halfPlaneHeight, clipPlaneAt.LowerRight, out hitInfo) && OwnerCollider(hitInfo) && (hitInfo.distance < num || num == -1f))
		{
			num = Vector3.Distance(hitInfo.point - base.transform.right * halfPlaneWidth + base.transform.up * halfPlaneHeight, from);
		}
		return num;
	}

	private bool OwnerCollider(RaycastHit curHitInfo)
	{
		if (curHitInfo.collider.tag != "Car" && curHitInfo.collider.tag != "Player" && curHitInfo.collider.tag != "Vision" && curHitInfo.collider.tag != "colliderPoint" && curHitInfo.collider.tag != "pointExitCar")
		{
			return true;
		}
		return false;
	}

	private float ClampAngle(float angle, float min, float max)
	{
		while (angle < -360f || angle > 360f)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
		}
		return Mathf.Clamp(angle, min, max);
	}

	public static ClipPlaneVertexes GetClipPlaneAt(Vector3 pos)
	{
		ClipPlaneVertexes result = default(ClipPlaneVertexes);
		if (Camera.mainCamera == null)
		{
			return result;
		}
		Transform transform = Camera.mainCamera.transform;
		float nearClipPlane = Camera.mainCamera.nearClipPlane;
		result.UpperLeft = pos - transform.right * halfPlaneWidth;
		result.UpperLeft += transform.up * halfPlaneHeight;
		result.UpperLeft += transform.forward * nearClipPlane;
		result.UpperRight = pos + transform.right * halfPlaneWidth;
		result.UpperRight += transform.up * halfPlaneHeight;
		result.UpperRight += transform.forward * nearClipPlane;
		result.LowerLeft = pos - transform.right * halfPlaneWidth;
		result.LowerLeft -= transform.up * halfPlaneHeight;
		result.LowerLeft += transform.forward * nearClipPlane;
		result.LowerRight = pos + transform.right * halfPlaneWidth;
		result.LowerRight -= transform.up * halfPlaneHeight;
		result.LowerRight += transform.forward * nearClipPlane;
		return result;
	}
}
