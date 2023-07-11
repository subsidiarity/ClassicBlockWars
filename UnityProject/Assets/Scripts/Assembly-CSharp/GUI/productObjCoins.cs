using UnityEngine;

public class productObjCoins : MonoBehaviour
{
	public UILabel lbMoney;

	public UILabel lbGiveCoins;

	public GameObject but;

	public GameObject butNoActiv;

	public string IdItemsIOS = string.Empty;

	public string IdItemsAndroid = string.Empty;

	public string idItems;

	public whatDoAfterBuy deistvAfterBuy;

	public int kolAddMoney;

	[HideInInspector]
	public int nomProduct;

	[HideInInspector]
	public bool productEnabled;

	public bool showByKey;

	public string keyForShow;

	private void Start()
	{
		setRuleIdPlatform();
		if (lbGiveCoins != null)
		{
			lbGiveCoins.text = string.Empty + kolAddMoney;
		}
	}

	public void setRuleIdPlatform()
	{
		if (Application.platform == RuntimePlatform.OSXEditor)
		{
			idItems = IdItemsIOS;
		}
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			idItems = IdItemsIOS;
		}
		if (Application.platform == RuntimePlatform.Android)
		{
			idItems = IdItemsAndroid;
		}
	}

	public void reset()
	{
		nomProduct = -1;
		productEnabled = false;
		if (but != null)
		{
			but.SetActive(false);
		}
		if (butNoActiv != null)
		{
			butNoActiv.SetActive(true);
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
}
