using System;
using UnityEngine;

[Serializable]
public class CreateSkidmarks : MonoBehaviour
{
	public Transform skidmark;

	private WheelCollider wheelCol;

	private Vector3 newPos;

	private Vector3 wheelDistance;

	public virtual void Awake()
	{
		wheelCol = (WheelCollider)GetComponent(typeof(WheelCollider));
	}

	public virtual void LateUpdate()
	{
		WheelHit hit = default(WheelHit);
		if (wheelCol.GetGroundHit(out hit) && (Mathf.Abs(hit.forwardSlip) > 5f || !(Mathf.Abs(hit.sidewaysSlip) <= 5f)))
		{
			newPos = hit.point;
			newPos.y += 0.1f;
			UnityEngine.Object.Instantiate(skidmark, newPos, transform.rotation);
		}
	}

	public virtual void Main()
	{
	}
}
