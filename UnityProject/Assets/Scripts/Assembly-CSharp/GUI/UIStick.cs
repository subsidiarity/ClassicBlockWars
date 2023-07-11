using UnityEngine;

[ExecuteInEditMode]
public class UIStick : MonoBehaviour
{
	public Transform target;

	private Vector3 mLastPosition;

	private Transform mTrans;

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void Update()
	{
		if (!(target == null) && mLastPosition != target.localPosition)
		{
			mLastPosition = target.localPosition;
			mTrans.localPosition = mLastPosition;
		}
	}
}
