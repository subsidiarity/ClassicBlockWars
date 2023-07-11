using UnityEngine;

public class InfoGun : MonoBehaviour
{
	public UISprite sliderDamage;

	public UISprite sliderRage;

	public UISprite sliderAmmo;

	public UISprite sliderMobility;

	public GameObject indicatorAmmo;

	public void setInfo(int damage, int rage, int ammo, int mobility)
	{
		float fillAmount = (float)damage / 100f;
		float fillAmount2 = (float)rage / 100f;
		float fillAmount3 = (float)ammo / 100f;
		float fillAmount4 = (float)mobility / 100f;
		if (ammo <= 0)
		{
			indicatorAmmo.SetActive(false);
		}
		else
		{
			indicatorAmmo.SetActive(true);
		}
		sliderDamage.fillAmount = fillAmount;
		sliderAmmo.fillAmount = fillAmount3;
		sliderRage.fillAmount = fillAmount2;
		sliderMobility.fillAmount = fillAmount4;
	}
}
