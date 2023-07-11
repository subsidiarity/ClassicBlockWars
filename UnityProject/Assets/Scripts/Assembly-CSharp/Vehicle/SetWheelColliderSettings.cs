using UnityEngine;

[ExecuteInEditMode]
public class SetWheelColliderSettings : MonoBehaviour
{
	public WheelCollider flWheelCollider;

	public WheelCollider frWheelCollider;

	public WheelCollider rlWheelCollider;

	public WheelCollider rrWheelCollider;

	public Transform flWheel;

	public Transform frWheel;

	public Transform rlWheel;

	public Transform rrWheel;

	public float radiusFront = 0.3f;

	public float radiusBack = 0.3f;

	public bool mirrorWheels = true;

	private Vector3 frontLeftPosition;

	private Vector3 rearLeftPosition;

	private void Awake()
	{
		flWheelCollider.transform.position = flWheel.position;
		rlWheelCollider.transform.position = rlWheel.position;
		frWheelCollider.transform.position = frWheel.position;
		rrWheelCollider.transform.position = rrWheel.position;
		flWheelCollider.radius = radiusFront;
		frWheelCollider.radius = radiusFront;
		rlWheelCollider.radius = radiusBack;
		rrWheelCollider.radius = radiusBack;
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			if (mirrorWheels)
			{
				Vector3 localPosition = frWheel.localPosition;
				localPosition.x *= -1f;
				flWheel.localPosition = localPosition;
				Vector3 localPosition2 = rrWheel.localPosition;
				localPosition2.x *= -1f;
				rlWheel.localPosition = localPosition2;
			}
			flWheelCollider.transform.position = flWheel.position;
			rlWheelCollider.transform.position = rlWheel.position;
			frWheelCollider.transform.position = frWheel.position;
			rrWheelCollider.transform.position = rrWheel.position;
			flWheelCollider.radius = radiusFront;
			frWheelCollider.radius = radiusFront;
			rlWheelCollider.radius = radiusBack;
			rrWheelCollider.radius = radiusBack;
		}
	}

	public void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			float num = radiusFront * 2f;
			float num2 = radiusBack * 2f;
			Vector3 size = new Vector3(num, num, num);
			Vector3 size2 = new Vector3(num2, num2, num2);
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(frWheel.position, size);
			Gizmos.DrawWireCube(rrWheel.position, size2);
			if (mirrorWheels)
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawWireCube(flWheel.position, size);
			Gizmos.DrawWireCube(rlWheel.position, size2);
		}
	}
}
