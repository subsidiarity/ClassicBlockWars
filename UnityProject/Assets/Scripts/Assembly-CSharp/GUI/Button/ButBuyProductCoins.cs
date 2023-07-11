using UnityEngine;

public class ButBuyProductCoins : MonoBehaviour
{
	public productObjCoins curProductItem;

	private void Start()
	{
		if (curProductItem == null)
		{
			FindInParent(base.transform);
		}
	}

	private void OnClick()
	{
		settings.playSoundButton();
		shopController.thisScript.buyProductCoinsWithID(curProductItem.idItems);
	}

	private void FindInParent(Transform curObj)
	{
		if (curObj != null)
		{
			productObjCoins component = curObj.gameObject.GetComponent<productObjCoins>();
			if (component != null)
			{
				curProductItem = component;
			}
			else
			{
				FindInParent(curObj.parent);
			}
		}
	}
}
