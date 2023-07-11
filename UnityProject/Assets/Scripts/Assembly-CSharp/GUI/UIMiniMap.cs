using System;
using NJG;
using UnityEngine;

[AddComponentMenu("NJG MiniMap/NGUI/Minimap")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIAnchor))]
public class UIMiniMap : UIMiniMapBase
{
	private static UIMiniMap mInst;

	public UISprite overlay;

	private UIAnchor mAnchor;

	public static Action<Vector3> onMapDoubleClick;

	private Vector3 mClickOffset;

	public static UIMiniMap instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = UnityEngine.Object.FindObjectOfType(typeof(UIMiniMap)) as UIMiniMap;
			}
			return mInst;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		if (!(map == null))
		{
			mAnchor = GetComponent<UIAnchor>();
			base.Start();
			UpdateAlignment();
		}
	}

	protected override void OnStart()
	{
		BoxCollider component = rendererTransform.GetComponent<BoxCollider>();
		if (component == null)
		{
			component = NGUITools.AddWidgetCollider(rendererTransform.gameObject);
		}
		UIForwardEvents uIForwardEvents = rendererTransform.GetComponent<UIForwardEvents>();
		if (uIForwardEvents == null)
		{
			uIForwardEvents = rendererTransform.gameObject.AddComponent<UIForwardEvents>();
		}
		uIForwardEvents.onDoubleClick = true;
		uIForwardEvents.onClick = true;
		uIForwardEvents.onHover = true;
		uIForwardEvents.target = base.gameObject;
		base.OnStart();
	}

	protected override void UpdateZoomKeys()
	{
		if (UIWorldMap.instance != null && !UIWorldMap.instance.isVisible)
		{
			base.UpdateZoomKeys();
		}
	}

	protected override void UpdateKeys()
	{
		if (!UICamera.inputHasFocus)
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
	}

	private void OnHover(bool isOver)
	{
		isMouseOver = isOver;
	}

	protected override UIMapIconBase GetEntry(NJGMapItem item)
	{
		UISpriteData uISpriteData = null;
		int i = 0;
		for (int count = mList.Count; i < count; i++)
		{
			UIMapIcon uIMapIcon = (UIMapIcon)mList[i];
			if (uIMapIcon.item.Equals(item))
			{
				return uIMapIcon;
			}
		}
		if (mUnused.Count > 0)
		{
			UIMapIcon uIMapIcon2 = (UIMapIcon)mUnused[mUnused.Count - 1];
			uIMapIcon2.item = item;
			uIMapIcon2.sprite.spriteName = NJGMap.instance.GetSprite(item.type).name;
			uIMapIcon2.sprite.depth = 1 + item.depth;
			uIMapIcon2.sprite.color = item.color;
			if (uIMapIcon2.sprite.localSize != (Vector2)item.iconScale)
			{
				if (uIMapIcon2.collider != null)
				{
					uIMapIcon2.collider.size = item.iconScale;
				}
				uIMapIcon2.sprite.width = (int)item.iconScale.x;
				uIMapIcon2.sprite.height = (int)item.iconScale.y;
			}
			uISpriteData = NJGMap.instance.GetSpriteBorder(item.type);
			if (uISpriteData != null && uIMapIcon2.border != null)
			{
				uIMapIcon2.border.spriteName = uISpriteData.name;
				uIMapIcon2.border.depth = 1 + item.depth;
				uIMapIcon2.border.color = item.color;
				if (uIMapIcon2.border.localSize != (Vector2)item.borderScale)
				{
					uIMapIcon2.border.width = (int)item.borderScale.x;
					uIMapIcon2.border.height = (int)item.borderScale.y;
				}
			}
			mUnused.RemoveAt(mUnused.Count - 1);
			NGUITools.SetActive(uIMapIcon2.gameObject, true);
			mList.Add(uIMapIcon2);
			return uIMapIcon2;
		}
		GameObject gameObject = NGUITools.AddChild(base.iconRoot.gameObject);
		gameObject.name = "Icon" + mCount;
		UISprite uISprite = NGUITools.AddWidget<UISprite>(gameObject);
		uISprite.name = "Icon";
		uISprite.depth = 1 + item.depth;
		uISprite.atlas = NJGMap.instance.atlas;
		uISprite.spriteName = NJGMap.instance.GetSprite(item.type).name;
		uISprite.color = item.color;
		uISprite.width = (int)item.iconScale.x;
		uISprite.height = (int)item.iconScale.y;
		UIMapIcon uIMapIcon3 = gameObject.AddComponent<UIMapIcon>();
		uIMapIcon3.item = item;
		uIMapIcon3.sprite = uISprite;
		if (item.interaction)
		{
			if (uIMapIcon3.collider == null)
			{
				uIMapIcon3.collider = NGUITools.AddWidgetCollider(gameObject);
				uIMapIcon3.collider.size = item.iconScale;
			}
			uISpriteData = NJGMap.instance.GetSpriteBorder(item.type);
			if (uISpriteData != null)
			{
				UISprite uISprite2 = NGUITools.AddWidget<UISprite>(gameObject);
				uISprite2.name = "Selection";
				uISprite2.depth = item.depth + 2;
				uISprite2.atlas = NJGMap.instance.atlas;
				uISprite2.spriteName = uISpriteData.name;
				uISprite2.color = item.color;
				uISprite2.width = (int)item.borderScale.x;
				uISprite2.height = (int)item.borderScale.y;
				uIMapIcon3.border = uISprite2;
			}
		}
		if (uIMapIcon3 == null)
		{
			Debug.LogError("Expected to find a Game Map Icon on the prefab to work with", this);
			UnityEngine.Object.Destroy(gameObject);
		}
		else
		{
			mCount++;
			uIMapIcon3.item = item;
			mList.Add(uIMapIcon3);
		}
		return uIMapIcon3;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (shaderType == NJGMapBase.ShaderType.TextureMask)
		{
			if (mMask != maskTexture)
			{
				mMask = maskTexture;
				if (material != null)
				{
					material.SetTexture("_Mask", mMask);
				}
			}
		}
		else if (material != null)
		{
			material.SetColor("_MaskColor", NJGMapBase.instance.cameraBackgroundColor);
		}
	}

	public override void UpdateAlignment()
	{
		base.UpdateAlignment();
		if (mAnchor == null)
		{
			mAnchor = GetComponent<UIAnchor>();
		}
		if (mAnchor != null)
		{
			mAnchor.side = (UIAnchor.Side)pivot;
		}
		if (base.iconRoot != null)
		{
			base.iconRoot.localPosition = new Vector3(rendererTransform.localPosition.x, rendererTransform.localPosition.y, 1f);
		}
		if (overlay != null && NJGMap.instance.atlas != null)
		{
			UISpriteData sprite = NJGMap.instance.atlas.GetSprite(overlay.spriteName);
			if (sprite != null)
			{
				Vector4 border = overlay.border;
				overlay.width = (int)mapScale.x + (int)(border.x + border.z + (float)overlayBorderOffset);
				overlay.height = (int)mapScale.y + (int)(border.y + border.w + (float)overlayBorderOffset);
				overlay.MakePixelPerfect();
			}
		}
	}

	public void OnDoubleClick()
	{
		mClickOffset = UICamera.currentTouch.pos - (Vector2)base.cachedTransform.position;
		mClickOffset.x = Mathf.Abs(mClickOffset.x);
		mClickOffset.y = 0f - Mathf.Abs(mClickOffset.y);
		if (onMapDoubleClick != null)
		{
			onMapDoubleClick(mClickPos);
		}
	}

	protected override UIMapArrowBase GetArrow(UnityEngine.Object o)
	{
		NJGMapItem nJGMapItem = (NJGMapItem)o;
		int i = 0;
		for (int count = mListArrow.Count; i < count; i++)
		{
			if (mListArrow[i].item == nJGMapItem)
			{
				return (UIMapArrow)mListArrow[i];
			}
		}
		if (mUnusedArrow.Count > 0)
		{
			UIMapArrow uIMapArrow = (UIMapArrow)mUnusedArrow[mUnusedArrow.Count - 1];
			uIMapArrow.item = nJGMapItem;
			uIMapArrow.child = uIMapArrow.sprite.cachedTransform;
			uIMapArrow.sprite.spriteName = NJGMap.instance.GetArrowSprite(nJGMapItem.type).name;
			uIMapArrow.sprite.depth = 1 + nJGMapItem.arrowDepth;
			uIMapArrow.sprite.color = nJGMapItem.color;
			uIMapArrow.sprite.width = (int)arrowScale.x;
			uIMapArrow.sprite.height = (int)arrowScale.y;
			uIMapArrow.sprite.cachedTransform.localPosition = new Vector3(0f, mapScale.y / 2f - (float)nJGMapItem.arrowOffset, 0f);
			mUnusedArrow.RemoveAt(mUnusedArrow.Count - 1);
			NGUITools.SetActive(uIMapArrow.gameObject, true);
			mListArrow.Add(uIMapArrow);
			return uIMapArrow;
		}
		GameObject gameObject = NGUITools.AddChild(rendererTransform.parent.gameObject);
		gameObject.name = "Arrow" + mArrowCount;
		gameObject.transform.parent = instance.arrowRoot.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		UISprite uISprite = NGUITools.AddWidget<UISprite>(gameObject);
		uISprite.depth = 1 + nJGMapItem.arrowDepth;
		uISprite.atlas = NJGMap.instance.atlas;
		uISprite.spriteName = NJGMap.instance.GetArrowSprite(nJGMapItem.type).name;
		uISprite.color = nJGMapItem.color;
		uISprite.width = (int)arrowScale.x;
		uISprite.height = (int)arrowScale.y;
		uISprite.cachedTransform.localPosition = new Vector3(0f, rendererTransform.localScale.y / 2f - (float)nJGMapItem.arrowOffset, 0f);
		UIMapArrow uIMapArrow2 = gameObject.AddComponent<UIMapArrow>();
		uIMapArrow2.child = uISprite.cachedTransform;
		uIMapArrow2.child.localEulerAngles = new Vector3(0f, 180f, 0f);
		uIMapArrow2.item = nJGMapItem;
		uIMapArrow2.sprite = uISprite;
		if (uIMapArrow2 == null)
		{
			Debug.LogError("Expected to find a UIMapArrow on the prefab to work with");
			UnityEngine.Object.Destroy(gameObject);
		}
		else
		{
			mArrowCount++;
			uIMapArrow2.item = nJGMapItem;
			mListArrow.Add(uIMapArrow2);
		}
		return uIMapArrow2;
	}
}
