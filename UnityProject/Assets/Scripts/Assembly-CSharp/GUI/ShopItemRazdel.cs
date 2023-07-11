using UnityEngine;

public class ShopItemRazdel : MonoBehaviour
{
	public GameObject butChoose;

	public GameObject butUnChoose;

	public ItemRazdel currentRazdel;

	public GameObject scrollViewRazdel;

	public Transform objForFirstAlign;

	public bool ShowFirst;

	private void Start()
	{
		otklBut();
		if (shopController.thisScript != null)
		{
			shopController.thisScript.AddShopItemRazdelToList(this);
		}
	}

	public void otklBut()
	{
		butChoose.SetActive(false);
		butUnChoose.SetActive(true);
		scrollViewRazdel.SetActive(false);
	}

	public void vklBut()
	{
		butChoose.SetActive(true);
		butUnChoose.SetActive(false);
		scrollViewRazdel.SetActive(true);
	}
}
