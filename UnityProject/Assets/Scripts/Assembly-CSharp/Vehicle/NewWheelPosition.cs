using UnityEngine;

public class NewWheelPosition : MonoBehaviour
{
	public WheelCollider WheelCol;

	private Vector3 newPos;

	private void Update()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(WheelCol.transform.position, -WheelCol.transform.up, out hitInfo, WheelCol.suspensionDistance + WheelCol.radius))
		{
			if (hitInfo.collider.isTrigger)
			{
				newPos = base.transform.position;
			}
			else
			{
				newPos = hitInfo.point + WheelCol.transform.up * WheelCol.radius;
			}
		}
		else
		{
			newPos = WheelCol.transform.position - WheelCol.transform.up * WheelCol.suspensionDistance;
		}
		base.transform.position = newPos;
	}
}
