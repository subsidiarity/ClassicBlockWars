using UnityEngine;

public class RotationHelper : MonoBehaviour
{
	public float multiplier = 1f;

	private void Update()
	{
		base.transform.Rotate(Vector3.right * (multiplier * 10f) * Time.deltaTime);
	}
}
