using System;
using UnityEngine;

[Serializable]
[AddComponentMenu("Camera-Control/Mouse Orbit smoothed")]
public class SmoothRotateMove : MonoBehaviour
{
	public Transform target;

	public float distance;

	public float xSpeed;

	public float ySpeed;

	public int yMinLimit;

	public int yMaxLimit;

	private float x;

	private float y;

	public float smoothTime;

	private float xSmooth;

	private float ySmooth;

	private float xVelocity;

	private float yVelocity;

	private Vector3 posSmooth;

	private Vector3 posVelocity;

	public SmoothRotateMove()
	{
		distance = 10f;
		xSpeed = 250f;
		ySpeed = 120f;
		yMinLimit = -20;
		yMaxLimit = 80;
		smoothTime = 0.3f;
		posSmooth = Vector3.zero;
		posVelocity = Vector3.zero;
	}

	public virtual void Start()
	{
		Vector3 eulerAngles = transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	public virtual void LateUpdate()
	{
		if ((bool)target)
		{
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			xSmooth = Mathf.SmoothDamp(xSmooth, x, ref xVelocity, smoothTime);
			ySmooth = Mathf.SmoothDamp(ySmooth, y, ref yVelocity, smoothTime);
			ySmooth = ClampAngle(ySmooth, yMinLimit, yMaxLimit);
			Quaternion quaternion = Quaternion.Euler(ySmooth, xSmooth, 0f);
			posSmooth = target.position;
			transform.rotation = quaternion;
			transform.position = quaternion * new Vector3(0f, 0f, 0f - distance) + posSmooth;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (!(angle >= -360f))
		{
			angle += 360f;
		}
		if (!(angle <= 360f))
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public virtual void Main()
	{
	}
}
