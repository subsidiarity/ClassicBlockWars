using UnityEngine;

[ExecuteInEditMode]
public class UIScale : MonoBehaviour
{
	public UIWidget target;

	private Vector2 mScale = Vector3.zero;

	private Transform mTrans;

	private void Awake()
	{
		mTrans = base.transform;
	}

	private void Update()
	{
		if (!(target == null) && (mScale.x != (float)target.width || mScale.y != (float)target.height))
		{
			mScale.x = target.width;
			mScale.y = target.height;
			mTrans.localScale = mScale;
		}
	}
}
