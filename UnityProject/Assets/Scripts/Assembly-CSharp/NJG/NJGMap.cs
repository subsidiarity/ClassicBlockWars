using NJG;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NJG MiniMap/NGUI/Map")]
public class NJGMap : NJGMapBase
{
	private static NJGMap mInst;

	public UIAtlas atlas;

	public UISpriteData defaultSprite;

	private UICamera uiCam;

	public new static NJGMap instance
	{
		get
		{
			if (mInst == null)
			{
				mInst = Object.FindObjectOfType(typeof(NJGMap)) as NJGMap;
			}
			return mInst;
		}
	}

	public override bool isMouseOver
	{
		get
		{
			return UICamera.hoveredObject != null || UICamera.inputHasFocus || base.isMouseOver;
		}
	}

	public UISpriteData GetSprite(int type)
	{
		if (atlas == null)
		{
			Debug.LogWarning("You need to assign an atlas", this);
			return null;
		}
		return (Get(type) != null) ? atlas.GetSprite(Get(type).sprite) : defaultSprite;
	}

	public UISpriteData GetSpriteBorder(int type)
	{
		if (atlas == null)
		{
			Debug.LogWarning("You need to assign an atlas", this);
			return null;
		}
		return (Get(type) != null) ? atlas.GetSprite(Get(type).selectedSprite) : null;
	}

	public UISpriteData GetArrowSprite(int type)
	{
		if (atlas == null)
		{
			Debug.LogWarning("You need to assign an atlas", this);
			return null;
		}
		return (Get(type) != null) ? atlas.GetSprite(Get(type).arrowSprite) : defaultSprite;
	}

	private void OnDestroy()
	{
		if (UIMiniMap.instance != null)
		{
			UIMiniMap.instance.material.mainTexture = null;
		}
		if (UIWorldMap.instance != null)
		{
			UIWorldMap.instance.material.mainTexture = null;
		}
		if (mapTexture != null)
		{
			NJGTools.Destroy(mapTexture);
		}
		mapTexture = null;
	}
}
