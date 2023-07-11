using UnityEngine;

namespace NJG
{
	public class UIMapArrowBase : MonoBehaviour
	{
		[SerializeField]
		public NJGMapItem item;

		public Transform child;

		public bool isValid;

		private Transform mTrans;

		protected float rotationOffset;

		private Vector3 mRot = Vector3.zero;

		private Vector3 mArrowRot = Vector3.zero;

		private Vector3 mFrom = Vector3.zero;

		public Transform cachedTransform
		{
			get
			{
				if (mTrans == null)
				{
					mTrans = base.transform;
				}
				return mTrans;
			}
		}

		public void UpdateRotation(Vector3 fromTarget)
		{
			mFrom = fromTarget - item.cachedTransform.position;
			float num = 0f;
			if (NJGMapBase.instance.orientation == NJGMapBase.Orientation.XZDefault)
			{
				mFrom.y = 0f;
				num = Vector3.Angle(Vector3.forward, mFrom);
			}
			else
			{
				mFrom.z = 0f;
				num = Vector3.Angle(Vector3.up, mFrom);
			}
			if (Vector3.Dot(Vector3.right, mFrom) < 0f)
			{
				num = 360f - num;
			}
			num += 180f;
			mRot = Vector3.zero;
			if (NJGMapBase.instance.orientation == NJGMapBase.Orientation.XZDefault)
			{
				mRot.z = num;
				mRot.y = 180f;
			}
			else
			{
				mRot.z = 0f - num;
				mRot.y = (mRot.x = 0f);
			}
			if (!cachedTransform.localEulerAngles.Equals(mRot))
			{
				cachedTransform.localEulerAngles = mRot;
			}
			if (!item.arrowRotate)
			{
				mArrowRot.x = 0f;
				mArrowRot.y = 180f;
				mArrowRot.z = ((!UIMiniMapBase.inst.rotateWithPlayer) ? (0f - cachedTransform.localEulerAngles.z) : (UIMiniMapBase.inst.iconRoot.localEulerAngles.z - cachedTransform.localEulerAngles.z));
				if (child.localEulerAngles != mArrowRot)
				{
					child.localEulerAngles = mArrowRot;
				}
			}
		}
	}
}
