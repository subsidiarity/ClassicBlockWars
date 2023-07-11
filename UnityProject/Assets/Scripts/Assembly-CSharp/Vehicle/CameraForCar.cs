using UnityEngine;

public class CameraForCar : MonoBehaviour
{
	public Transform target;

	public float distance = 10f;

	public float height = 5f;

	public float heightDamping = 2f;

	public float rotationDamping = 3f;

	public float wantedRotationAngle;

	public float wantedHeight;

	public float currentRotationAngle;

	public float currentHeight;

	public Quaternion currentRotation;

	private void Start()
	{
		if (target == null)
		{
			target = GameController.thisScript.myCar.transform;
		}
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			wantedRotationAngle = target.eulerAngles.y;
			wantedHeight = target.position.y + height;
			currentRotationAngle = base.transform.eulerAngles.y;
			currentHeight = base.transform.position.y;
			currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);
			currentRotation = Quaternion.Euler(0f, currentRotationAngle, 0f);
			base.transform.position = target.position;
			base.transform.position -= currentRotation * Vector3.forward * distance;
			base.transform.position.Set(base.transform.position.x, currentHeight, base.transform.position.z);
			base.transform.LookAt(target);
		}
	}
}
