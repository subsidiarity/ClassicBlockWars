using UnityEngine;

public class ButBuyProductItem : MonoBehaviour
{
	public productObj curProductItem;

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
		shopController.thisScript.buyProductFromShopItemWithId(curProductItem.idItems);
	}

	private void FindInParent(Transform curObj)
	{
		if (curObj != null)
		{
			productObj component = curObj.gameObject.GetComponent<productObj>();
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
