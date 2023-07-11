using UnityEngine;

public class productObj : MonoBehaviour
{
	public UILabel lbCoins;

	public string titleName = string.Empty;

	public GameObject but;

	public GameObject butNoActiv;

	public idGuns idItems;

	public bool addToShop = true;

	public whatDoAfterBuy deistvAfterBuy;

	public int price;

	public bool showByKey;

	public string keyForShow;

	public bool showToMenu;

	public bool showDialogBeforeBuy;

	public bool showButEquip;

	public bool showGunInfo;

	public int percentDamage;

	public int percentRange;

	public int percentAmmo;

	public int percentMobility;

	public bool showInfoText;

	public string infoText = string.Empty;

	private float scaleToCenter = 1.5f;

	private float halhWidthClipPanel;

	private float centerOffsetXPanel;

	private float xScale;

	private float alpha;

	private UIPanel curPanel;

	private void Start()
	{
		if ((bool)lbCoins)
		{
			lbCoins.text = string.Empty + price;
		}

		if (shopController.thisScript != null)
		{
			if (addToShop)
			{
				shopController.thisScript.AddProdItemToList(this);
			}
		}
		else
		{
			Debug.Log("Didn't find shopController");
		}
		curPanel = base.transform.parent.parent.GetComponent<UIPanel>();
		if (curPanel != null)
		{
			centerOffsetXPanel = curPanel.gameObject.transform.localPosition.x;
			halhWidthClipPanel = curPanel.baseClipRegion.z / 4f;
		}
	}

	public void vklBut()
	{
		if (but != null)
		{
			but.SetActive(true);
		}
		if (butNoActiv != null)
		{
			butNoActiv.SetActive(false);
		}
	}

	public void otklBut()
	{
		if (but != null)
		{
			but.SetActive(false);
		}
		if (butNoActiv != null)
		{
			butNoActiv.SetActive(true);
		}
	}

	private void OnPress(bool isDown)
	{
		if (isDown)
		{
			shopController.thisScript.productIsDrag();
		}
	}

	private void Update()
	{
		float num = Mathf.Abs(base.transform.localPosition.x + curPanel.gameObject.transform.localPosition.x - centerOffsetXPanel);
		if (num > halhWidthClipPanel)
		{
			xScale = 0.6f;
		}
		else
		{
			xScale = Mathf.Abs(halhWidthClipPanel - num);
			xScale = xScale / halhWidthClipPanel * (scaleToCenter - 1f) + 1f;
			xScale *= 0.8f;
		}
		TweenScale.Begin(base.gameObject, 0.05f, new Vector3(xScale, xScale, xScale));
		if (num > 100f)
		{
			alpha = 0.2f;
			return;
		}
		alpha = Mathf.Abs(100f - num);
		alpha /= 100f;
		TweenAlpha.Begin(shopController.thisScript.sprFrameShopItem.gameObject, 0.05f, alpha);
	}
}
