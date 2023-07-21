using System;
using UnityEngine;

public class TouchCamera : MonoBehaviour
{
	public Transform target;

	public Transform cam;

	private Camera NGUICamera;

	private BoxCollider collideZone;

	public float maxFai = 0.1f;

	public float minFai = -0.8f;

	public float maxZoomDistance = 4.5f;

	public float minZoomDistance = 2f;

	public float damping = 1f;

	private Vector3 f0Dir = Vector3.zero;

	private float zoomDistance = 5f;

	private float theta;

	private float fai;

	private float dx;

	private float dy;

	private float loc_x;

	private float loc_y;

	private float delta;

	private float deltaWeight = 0.05f;

	private Vector2 curDist = Vector2.zero;

	private Vector2 prevDist = Vector2.zero;

	private Transform dm;

	private Vector3 upVal = Vector3.zero;

	private Vector3 pos = new Vector3(0f, 0f, 0f);

	private Vector3 rot = new Vector3(0f, 0f, 0f);

	private Vector3 offset;

	private bool activeTouch;

	private void Start()
	{
		if (cam == null)
		{
			cam = GameController.thisScript.cameraGame.transform;
		}
		if (target != null)
		{
			offset = target.transform.position - cam.transform.position;
		}
		NGUICamera = UICamera.FindCameraForLayer(8).GetComponent<Camera>();
		collideZone = base.gameObject.GetComponent<BoxCollider>();
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			activeTouch = true;
		}
		else
		{
			activeTouch = false;
		}
	}

	private void OnHover(bool isOver)
	{
		if (!isOver)
		{
			activeTouch = false;
		}
	}

	private bool FingerGesturesGlobalFilter(int fingerIndex, Vector2 position)
	{
		if (!activeTouch)
		{
			return false;
		}
		Ray ray = NGUICamera.ScreenPointToRay(new Vector3(position.x, position.y, 0f));
		RaycastHit hitInfo;
		return collideZone.Raycast(ray, out hitInfo, 200f);
	}

	public void translateToBack()
	{
		float y = cam.transform.eulerAngles.y;
		float y2 = target.transform.eulerAngles.y;
		float y3 = Mathf.LerpAngle(y, y2, Time.maximumDeltaTime * damping);
		Quaternion quaternion = Quaternion.Euler(0f, y3, 0f);
		cam.transform.position = target.transform.position - quaternion * offset;
		cam.transform.LookAt(target.transform);
	}

	private Touch whichTouchIsOurs(Touch t1, Touch t2)
	{
		if (FingerGesturesGlobalFilter(0, t1.position))
		{
			return t1;
		}
		if (FingerGesturesGlobalFilter(0, t2.position))
		{
			return t2;
		}
		return default(Touch);
	}

	private void Update()
	{
		f0Dir = Vector2.zero;
		if (Input.touchCount == 1)
		{
			Touch touch = Input.GetTouch(0);
			if (FingerGesturesGlobalFilter(0, touch.position) && touch.phase == TouchPhase.Moved && !MoveJoystick.isJoystickMoving())
			{
				Vector3 vector = new Vector3(touch.deltaPosition.x, 0f - touch.deltaPosition.y, 0f);
				f0Dir = vector;
				loc_x = (float)Math.PI / 180f * f0Dir.x * 1f;
				loc_y = -(float)Math.PI / 180f * f0Dir.y * 1f;
			}
		}
		else if (Input.touchCount == 2)
		{
			loc_x = 0f;
			loc_y = 0f;
			Touch touch2 = Input.GetTouch(1);
			Touch touch3 = Input.GetTouch(0);
			if (FingerGesturesGlobalFilter(0, touch2.position) && FingerGesturesGlobalFilter(0, touch3.position))
			{
				Vector3 lhs = new Vector3(0f - touch2.deltaPosition.x, 0f - touch2.deltaPosition.y, 0f);
				Vector3 rhs = new Vector3(0f - touch3.deltaPosition.x, 0f - touch3.deltaPosition.y, 0f);
				float num = Vector3.Dot(lhs, rhs);
				Debug.Log(num);
				if (touch3.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved)
				{
					if (num > 0f)
					{
						f0Dir = Vector3.zero;
						dy += lhs.y * 0.01f;
						dx += lhs.x * 0.01f;
						loc_x = lhs.x * 0.01f;
						loc_y = lhs.y * 0.01f;
					}
					else
					{
						curDist = touch2.position - touch3.position;
						prevDist = touch2.position - touch2.deltaPosition - (touch3.position - touch3.deltaPosition);
						float num2 = curDist.magnitude - prevDist.magnitude;
						zoomDistance += (0f - num2) * deltaWeight;
						if (zoomDistance > maxZoomDistance)
						{
							zoomDistance = maxZoomDistance;
						}
						if (zoomDistance < minZoomDistance)
						{
							zoomDistance = minZoomDistance;
						}
					}
				}
			}
			else if ((FingerGesturesGlobalFilter(0, touch2.position) || FingerGesturesGlobalFilter(0, touch3.position)) && MoveJoystick.isJoystickMoving())
			{
				Touch touch4 = whichTouchIsOurs(touch2, touch3);
				Debug.Log("T1: " + touch2);
				Debug.Log("T2: " + touch3);
				Vector3 vector2 = new Vector3(touch4.deltaPosition.x, 0f - touch4.deltaPosition.y, 0f);
				f0Dir = vector2;
				loc_x = (float)Math.PI / 180f * f0Dir.x * 1f;
				loc_y = -(float)Math.PI / 180f * f0Dir.y * 1f;
			}
		}
		else
		{
			f0Dir = Vector3.zero;
			loc_x = 0f;
			loc_y = 0f;
		}
		theta += (float)Math.PI / 180f * f0Dir.x * 1f;
		fai += -(float)Math.PI / 180f * f0Dir.y * 1f;
		if (fai > maxFai)
		{
			fai = maxFai;
		}
		if (fai < minFai)
		{
			fai = minFai;
		}
		upVal.z = zoomDistance * Mathf.Cos(theta) * Mathf.Sin(fai + (float)Math.PI / 2f);
		upVal.x = zoomDistance * Mathf.Sin(theta) * Mathf.Sin(fai + (float)Math.PI / 2f);
		upVal.y = zoomDistance * Mathf.Cos(fai + (float)Math.PI / 2f);
		cam.transform.position = upVal;
		if (target != null)
		{
			cam.transform.position += target.position;
			cam.transform.LookAt(target.position);
		}
	}
}
