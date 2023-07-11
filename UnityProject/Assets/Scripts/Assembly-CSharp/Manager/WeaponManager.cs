using System.Collections.Generic;
using Photon;
using UnityEngine;

public class WeaponManager : Photon.MonoBehaviour
{
	[HideInInspector]
	public Weapon currentWeaponScript;

	public int currentWeaponIndex;

	public List<GameObject> myWeapons;

	public List<Bullets> myWeaponsBullets;

	public Transform weaponHolder;

	private GameObject currentWeapon;

	public GameObject CurrentWeapon
	{
		get
		{
			return currentWeapon;
		}
		set
		{
		}
	}

	private bool PlayerMine
	{
		get
		{
			if (settings.offlineMode)
			{
				return true;
			}
			if (!settings.offlineMode && base.photonView.isMine)
			{
				return true;
			}
			return false;
		}
	}

	private void Start()
	{
		initMyWeapons();
		if (PlayerMine)
		{
			initialWeaponChange();
		}
	}

	protected void initMyWeapons()
	{
		myWeapons = new List<GameObject>();
		myWeaponsBullets = new List<Bullets>();
		Object[] array = Resources.LoadAll("Weapons");
		for (int i = 0; i < settings.arrAllWeapons.Length; i++)
		{
			Object @object = Resources.Load("Weapons/" + settings.arrAllWeapons[i]);
			GameObject gameObject = @object as GameObject;
			gameObject.GetComponent<Weapon>().equipped = Load.LoadBool(settings.keyEquipGun + gameObject.name);
			myWeapons.Add(@object as GameObject);
			Weapon component = myWeapons[i].GetComponent<Weapon>();
			if (component.saveCountBullets)
			{
				component.initSavedBullets();
				myWeaponsBullets.Add(new Bullets(component.bulletPackCount, Load.LoadInt(settings.keyCountBullets + component.gameObject.name)));
			}
			else
			{
				myWeaponsBullets.Add(new Bullets(component.bulletPackCount, component.bulletAllCount));
			}
		}
	}

	public void SaveEquipValues()
	{
		foreach (GameObject myWeapon in myWeapons)
		{
			Save.SaveBool(settings.keyEquipGun + myWeapon.name, myWeapon.GetComponent<Weapon>().equipped);
		}
	}

	public void LoadEquipValues()
	{
		foreach (GameObject myWeapon in myWeapons)
		{
			myWeapon.GetComponent<Weapon>().equipped = Load.LoadBool(settings.keyEquipGun + myWeapon.name);
		}
	}

	public void UnequipWeapon(string weaponname)
	{
		GameObject gameObject = myWeapons.Find((GameObject wep) => wep.name.Equals(weaponname));
		Debug.Log(gameObject);
		if (gameObject != null)
		{
			gameObject.GetComponent<Weapon>().equipped = false;
			if (gameObject.GetComponent<Weapon>().name.Equals(currentWeapon.GetComponent<Weapon>().name))
			{
				switchToNext();
			}
		}
	}

	public void EquipWeapon(string weaponname)
	{
		GameObject gameObject = myWeapons.Find((GameObject wep) => wep.name.Equals(weaponname));
		if (gameObject != null)
		{
			gameObject.GetComponent<Weapon>().equipped = true;
		}
		switchWeaponByName(weaponname);
	}

	protected void weaponDebug()
	{
	}

	public void getBulletBonus()
	{
		if (PlayerMine)
		{
			myWeaponsBullets[currentWeaponIndex].bulletAllCount += currentWeaponScript.addCountBulletBonuse;
			showCountBulletCurrentWeapon();
		}
	}

	protected void initialWeaponChange()
	{
		if (!settings.isLearned)
		{
			currentWeaponIndex = 0;
		}
		switchWeaponByName(myWeapons[currentWeaponIndex].name);
	}

	public void switchWeaponByName(string name)
	{
		for (int i = 0; i < myWeapons.Count; i++)
		{
			GameObject gameObject = myWeapons[i];
			if (gameObject != null && gameObject.name.Equals(name))
			{
				currentWeaponIndex = i;
				if (settings.offlineMode)
				{
					switchWeapon(currentWeaponIndex, settings.tekNomSkin);
					break;
				}
				base.photonView.RPC("switchWeapon", PhotonTargets.AllBuffered, currentWeaponIndex, settings.tekNomSkin);
				break;
			}
		}
	}

	public GameObject getWeaponByPrefabName(string name)
	{
		if (myWeapons.Count == 0 || myWeapons == null)
		{
			initMyWeapons();
		}
		foreach (GameObject myWeapon in myWeapons)
		{
			if (myWeapon.name.Equals(name))
			{
				return myWeapon;
			}
		}
		return myWeapons[0];
	}

	[RPC]
	protected void switchWeapon(int weaponNum, int nomSkin)
	{
		if (currentWeaponScript != null)
		{
			currentWeaponScript.RemoveWeapon();
		}
		if (myWeapons == null || myWeapons.Count == 0)
		{
			initMyWeapons();
		}
		GameObject gameObject = (GameObject)Object.Instantiate(myWeapons[weaponNum], weaponHolder.position, weaponHolder.rotation);
		gameObject.transform.parent = weaponHolder;
		currentWeapon = gameObject;
		currentWeaponScript = currentWeapon.GetComponent<Weapon>();
		currentWeaponIndex = weaponNum;
		switchWeaponTexture(nomSkin);
		if (PlayerMine)
		{
			GameController.thisScript.setSprButPereklWeapon(currentWeaponScript.nameSpritePreview);
			showCountBulletCurrentWeapon();
		}
	}

	public void showCountBulletCurrentWeapon()
	{
		if (currentWeaponScript.isMelee)
		{
			GameController.thisScript.lbAmmo.text = string.Empty;
		}
		else if (currentWeaponScript.showFullInfoAmmo)
		{
			GameController.thisScript.lbAmmo.text = myWeaponsBullets[currentWeaponIndex].getCountBullets();
		}
		else
		{
			GameController.thisScript.lbAmmo.text = myWeaponsBullets[currentWeaponIndex].getCountBulletsOnly();
		}
	}

	public void switchWeaponTexture(int tekNomSkin)
	{
		Texture2D tekSkin = settings.getTekSkin(tekNomSkin);
		Renderer[] componentsInChildren = currentWeapon.GetComponentsInChildren<Renderer>(true);
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (!(renderer.gameObject.tag == "Weapon"))
			{
				renderer.material.mainTexture = tekSkin;
			}
		}
	}

	protected void SaveWeaponBulletsCount(int index)
	{
		myWeaponsBullets[index] = new Bullets(currentWeaponScript.bulletPackCount, currentWeaponScript.bulletAllCount);
	}

	public void fullWeapon()
	{
		for (int i = 0; i < myWeaponsBullets.Count; i++)
		{
			myWeaponsBullets[i].bulletAllCount += myWeapons[i].GetComponent<Weapon>().addCountBulletShop;
			showCountBulletCurrentWeapon();
		}
	}

	public void updateCountBulletsCurWeapon()
	{
		GameObject gameObject = myWeapons[currentWeaponIndex];
		Weapon component = gameObject.GetComponent<Weapon>();
		if (component.saveCountBullets)
		{
			myWeaponsBullets[currentWeaponIndex].bulletAllCount = Load.LoadInt(settings.keyCountBullets + component.name);
			showCountBulletCurrentWeapon();
		}
	}

	// TODO: Reimplement this.
	// public void switchToLast()
	// {
	// 	if (currentWeaponIndex == 0)
	// 	{
	// 		currentWeaponIndex = myWeapons.Count - 1;
	// 	}
	// 	else
	// 	{
	// 		currentWeaponIndex--;
	// 	}

	// 	ProcessWeaponSwitch();
	// }

	public void switchToNext()
	{
		if (!PlayerMine)
		{
			return;
		}

		currentWeaponIndex = (currentWeaponIndex + 1) % myWeapons.Count;

		ProcessWeaponSwitch();
	}

	private void ProcessWeaponSwitch()
	{
		GameObject gameObject = myWeapons[currentWeaponIndex];
		Weapon component = gameObject.GetComponent<Weapon>();
		if (component.saveCountBullets)
		{
			myWeaponsBullets[currentWeaponIndex].bulletAllCount = Load.LoadInt(settings.keyCountBullets + component.name);
		}

		// TODO: This flag should just be in that if statment.
		bool flag = (component.scriptGrenade != null && GameController.thisScript.playerScript.GetActiveButDetonator(component.scriptGrenade));

		if (!component.showWeaponThanNullBullets
		&& myWeaponsBullets[currentWeaponIndex].bulletAllCount <= 0
		&& !flag)
		{
			Debug.Log("null ammo");
			switchToNext();
		}
		else if (settings.isWeaponBought(gameObject.name) && component.equipped)
		{
			Bullets bullets = myWeaponsBullets[currentWeaponIndex];
			showCountBulletCurrentWeapon();
			if (settings.offlineMode)
			{
				switchWeapon(currentWeaponIndex, settings.tekNomSkin);
				return;
			}
			base.photonView.RPC("switchWeapon", PhotonTargets.AllBuffered, currentWeaponIndex, settings.tekNomSkin);
		}
		else
		{
			switchToNext();
		}
	}
}
