using UnityEngine;

namespace NJG
{
	public abstract class UIWorldMapBase : UIMapBase
	{
		private static UIWorldMapBase mInst;

		public static UIWorldMapBase inst
		{
			get
			{
				if (mInst == null)
				{
					mInst = Object.FindObjectOfType(typeof(UIWorldMapBase)) as UIWorldMapBase;
				}
				return mInst;
			}
		}

		protected override void Awake()
		{
			inst.enabled = true;
			limitBounds = true;
			base.Awake();
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (Application.isPlaying)
			{
				NJGTools.SetActive(mChild.gameObject, false);
			}
			if (calculateBorder)
			{
				mapBorderRadius = rendererTransform.localScale.x / 2f / 4f;
			}
		}

		protected override void UpdateIcon(NJGMapItem item, float x, float y)
		{
			bool flag = x - mapBorderRadius >= 0f - mapHalfScale.x && x + mapBorderRadius <= mapHalfScale.x && y - mapBorderRadius >= 0f - mapHalfScale.y && y + mapBorderRadius <= mapHalfScale.y;
			Vector3 vector = new Vector3(x, y, 0f);
			if (!flag)
			{
				return;
			}
			UIMapIconBase entry = GetEntry(item);
			entry.isMapIcon = true;
			if (entry != null && !entry.isValid)
			{
				entry.isValid = true;
				Transform transform = entry.cachedTransform;
				if (item.updatePosition && transform.localPosition != vector)
				{
					transform.localPosition = vector;
				}
				if (item.rotate)
				{
					float z = ((!(Vector3.Dot(item.cachedTransform.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f)) ? (-1f) : 1f) * Vector3.Angle(item.cachedTransform.forward, Vector3.forward);
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
				}
				else if (transform.localEulerAngles != Vector3.zero)
				{
					transform.localEulerAngles = Vector3.zero;
				}
			}
		}
	}
}
