using UnityEngine;

[AddComponentMenu("Game/Face Target")]
public class AimRotation : MonoBehaviour
{
	public Transform target;

	public float speed = 1000f;

	private Transform mTrans;

	private void Start()
	{
		mTrans = base.transform;
		target = Camera.current.transform;
	}

	private void LateUpdate()
	{
		if (target != null)
		{
			Vector3 forward = target.position - mTrans.position;
			float magnitude = forward.magnitude;
			if (magnitude > 0.001f)
			{
				forward *= 1f / magnitude;
				Quaternion quaternion = Quaternion.LookRotation(forward);
				mTrans.rotation = ((!(speed > 0f)) ? quaternion : Quaternion.Slerp(mTrans.rotation, quaternion, Time.deltaTime * speed));
			}
		}
	}
}
