using System.Collections.Generic;
using UnityEngine;

namespace NJG
{
	public abstract class UIMiniMapBase : UIMapBase
	{
		public enum ZoomType
		{
			ZoomIn = 0,
			ZoomOut = 1
		}

		private static UIMiniMapBase mInst;

		public KeyCode lockKey = KeyCode.L;

		public GameObject northIcon;

		public KeyCode mapKey = KeyCode.M;

		public int overlayBorderOffset;

		private bool worldMapVisible;

		private GameObject northRoot;

		protected Vector2 mArrowScale;

		protected Transform mArrowRoot;

		protected List<NJGMapItem> mPingList = new List<NJGMapItem>();

		protected List<NJGMapItem> mPingUnused = new List<NJGMapItem>();

		protected List<UIMapArrowBase> mListArrow = new List<UIMapArrowBase>();

		protected List<UIMapArrowBase> mUnusedArrow = new List<UIMapArrowBase>();

		private NJGMapItem pingMarker;

		protected int mArrowCount;

		private Pivot mPivot;

		private Vector2 mMargin;

		public static bool initialized
		{
			get
			{
				return mInst != null;
			}
		}

		public static UIMiniMapBase inst
		{
			get
			{
				if (mInst == null)
				{
					mInst = Object.FindObjectOfType(typeof(UIMiniMapBase)) as UIMiniMapBase;
				}
				return mInst;
			}
		}

		public Transform arrowRoot
		{
			get
			{
				if (mArrowRoot == null && Application.isPlaying)
				{
					mArrowRoot = NJGTools.AddChild(base.gameObject).transform;
					mArrowRoot.parent = base.iconRoot;
					mArrowRoot.name = "_MapArrows";
					mArrowRoot.localEulerAngles = Vector3.zero;
					mArrowRoot.localScale = Vector3.one;
					mArrowRoot.localPosition = Vector3.zero;
				}
				return mArrowRoot;
			}
		}

		protected override void OnStart()
		{
			base.OnStart();
			if (Application.isPlaying)
			{
				northRoot = NJGTools.AddChild(base.iconRoot.gameObject);
				northRoot.name = "North";
				northRoot.transform.localPosition = Vector3.zero;
				if (northIcon != null)
				{
					northIcon.transform.parent = northRoot.transform;
					northIcon.transform.localRotation = Quaternion.identity;
				}
				if (calculateBorder)
				{
					mapBorderRadius = rendererTransform.localScale.x / 2f / 4f;
				}
			}
			UpdateAlignment();
		}

		private void OnClick()
		{
		}

		public virtual void UpdateAlignment()
		{
			Vector3 zero = Vector3.zero;
			zero.z = rendererTransform.localPosition.z;
			if (pivot != Pivot.Center)
			{
				switch (pivot)
				{
				case Pivot.TopRight:
					zero.x = Mathf.Round(-0.5f * mapScale.x) - margin.x;
					zero.y = Mathf.Round(-0.5f * mapScale.y) - margin.y;
					break;
				case Pivot.Right:
					zero.x = Mathf.Round(-0.5f * mapScale.x) - margin.x;
					break;
				case Pivot.BottomRight:
					zero.x = Mathf.Round(-0.5f * mapScale.x) - margin.x;
					zero.y = Mathf.Round(0.5f * mapScale.y) + margin.y;
					break;
				case Pivot.Bottom:
					zero.y = Mathf.Round(0.5f * mapScale.y) + margin.y;
					break;
				case Pivot.Top:
					zero.y = Mathf.Round(-0.5f * mapScale.y) - margin.y;
					break;
				case Pivot.TopLeft:
					zero.x = Mathf.Round(0.5f * mapScale.x) + margin.x;
					zero.y = Mathf.Round(-0.5f * mapScale.y) - margin.y;
					break;
				case Pivot.Left:
					zero.x = Mathf.Round(0.5f * mapScale.x) + margin.x;
					break;
				case Pivot.BottomLeft:
					zero.x = Mathf.Round(0.5f * mapScale.x) + margin.x;
					zero.y = Mathf.Round(0.5f * mapScale.y) + margin.y;
					break;
				}
			}
			rendererTransform.localPosition = zero;
			if (base.iconRoot != null)
			{
				base.iconRoot.localPosition = new Vector3(rendererTransform.localPosition.x, rendererTransform.localPosition.y, 1f);
			}
		}

		protected override void UpdateIcon(NJGMapItem item, float x, float y)
		{
			bool flag = x - mapBorderRadius >= 0f - mapHalfScale.x && x + mapBorderRadius <= mapHalfScale.x && y - mapBorderRadius >= 0f - mapHalfScale.y && y + mapBorderRadius <= mapHalfScale.y;
			if (!base.isPanning)
			{
				if (!flag && item.haveArrow)
				{
					if (item.arrow == null)
					{
						item.arrow = GetArrow(item);
					}
					if (item.arrow != null)
					{
						if (!NJGTools.GetActive(item.arrow.gameObject))
						{
							NJGTools.SetActive(item.arrow.gameObject, true);
						}
						item.arrow.UpdateRotation(target.position);
					}
				}
				else if (flag && item.haveArrow && item.arrow != null && NJGTools.GetActive(item.arrow.gameObject))
				{
					NJGTools.SetActive(item.arrow.gameObject, false);
				}
			}
			if (!flag)
			{
				return;
			}
			UIMapIconBase entry = GetEntry(item);
			if (!(entry != null) || entry.isValid)
			{
				return;
			}
			entry.isMapIcon = false;
			entry.isValid = true;
			Transform transform = entry.cachedTransform;
			Vector3 vector = new Vector3(x, y, 0f);
			if (item.updatePosition && transform.localPosition != vector)
			{
				transform.localPosition = vector;
			}
			if (item.rotate)
			{
				float z = ((!(Vector3.Dot(item.cachedTransform.forward, Vector3.Cross(Vector3.up, Vector3.forward)) <= 0f)) ? (-1f) : 1f) * Vector3.Angle(item.cachedTransform.forward, Vector3.forward);
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
			}
			else if (!item.rotate && rotateWithPlayer)
			{
				Vector3 vector2 = new Vector3(0f, 0f, 0f - base.iconRoot.localEulerAngles.z);
				if (!transform.localEulerAngles.Equals(vector2))
				{
					transform.localEulerAngles = vector2;
				}
			}
			else if (!transform.localEulerAngles.Equals(Vector3.zero))
			{
				transform.localEulerAngles = Vector3.zero;
			}
		}

		protected virtual UIMapArrowBase GetArrow(Object o)
		{
			return (UIMapArrowBase)o;
		}

		protected override void Update()
		{
			if (mPivot != pivot || mMargin != margin || mMapScale != mapScale)
			{
				mMapScale = mapScale;
				mPivot = pivot;
				mMargin = margin;
				UpdateAlignment();
			}
			if (arrowRoot != null)
			{
				if (base.isPanning && arrowRoot.localScale != new Vector3(0.001f, 0.001f, 0.001f))
				{
					arrowRoot.localScale = new Vector3(0.001f, 0.001f, 0.001f);
				}
				else if (!base.isPanning && arrowRoot.localScale != Vector3.one)
				{
					arrowRoot.localScale = Vector3.one;
				}
			}
			UpdateKeys();
			base.Update();
		}

		protected virtual void UpdateKeys()
		{
			if (Input.GetKeyDown(mapKey))
			{
				ToggleWorldMap();
			}
			if (Input.GetKeyDown(lockKey))
			{
				rotateWithPlayer = !rotateWithPlayer;
			}
		}

		public virtual void DeleteArrow(UIMapArrowBase ent)
		{
			if (ent != null)
			{
				mListArrow.Remove(ent);
				mUnusedArrow.Add(ent);
				NJGTools.SetActive(ent.gameObject, false);
			}
		}

		public void ToggleWorldMap()
		{
			if (UIWorldMapBase.inst != null)
			{
				UIWorldMapBase.inst.Toggle();
			}
		}
	}
}
