using NJG;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NJG MiniMap/NGUI/World Map")]
public class UIWorldMap : UIWorldMapBase
{
	private static UIWorldMap mInst;

	public static UIWorldMap instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(UIWorldMap)) as UIWorldMap;
			}
			return mInst;
		}
	}

	private void OnHover(bool isOver)
	{
		isMouseOver = isOver;
	}

	private void OnPress(bool isPress)
	{
		tapOnMap = isPress;
	}

	protected override void OnStart()
	{
		if (NJGMap.instance == null)
		{
			return;
		}
		if (mInst == null)
		{
			mInst = this;
		}
		base.OnStart();
		if (!(planeRenderer == null))
		{
			BoxCollider component = planeRenderer.GetComponent<BoxCollider>();
			if (component == null)
			{
				component = NGUITools.AddWidgetCollider(planeRenderer.gameObject);
			}
			UIForwardEvents uIForwardEvents = planeRenderer.GetComponent<UIForwardEvents>();
			if (uIForwardEvents == null)
			{
				uIForwardEvents = planeRenderer.gameObject.AddComponent<UIForwardEvents>();
			}
			uIForwardEvents.onClick = true;
			uIForwardEvents.onHover = true;
			uIForwardEvents.target = base.gameObject;
		}
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
			uIMapIcon2.sprite.depth = 1 + NGUITools.CalculateNextDepth(uIMapIcon2.sprite.gameObject) + item.depth;
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
				uIMapIcon2.border.depth = 1 + NGUITools.CalculateNextDepth(uIMapIcon2.border.gameObject) + item.depth + 1;
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
		uISprite.depth = 1 + NGUITools.CalculateNextDepth(uISprite.gameObject) + item.depth;
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
			if (uIMapIcon3.GetComponent<Collider>() == null)
			{
				uIMapIcon3.collider = NGUITools.AddWidgetCollider(gameObject);
				uIMapIcon3.collider.size = item.iconScale;
			}
			UISprite uISprite2 = null;
			uISpriteData = NJGMap.instance.GetSpriteBorder(item.type);
			if (uISpriteData != null)
			{
				uISprite2 = NGUITools.AddWidget<UISprite>(gameObject);
				uISprite2.name = "Selection";
				uISprite2.depth = 1 + NGUITools.CalculateNextDepth(uISprite2.gameObject) + item.depth + 1;
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
			Object.Destroy(gameObject);
		}
		else
		{
			mCount++;
			uIMapIcon3.item = item;
			mList.Add(uIMapIcon3);
		}
		return uIMapIcon3;
	}
}
