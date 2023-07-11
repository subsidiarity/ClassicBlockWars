using System;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine;

public class shopController : MonoBehaviour
{
	public enum exitTo
	{
		menu = 0,
		shopGuns = 1,
		heroRoom = 2,
		heroRoomFromGuns = 3,
		gameWin = 4,
		gameShopGun = 5,
		gamePause = 6,
		gamePauseShopGun = 7,
		gameEndGame = 8
	}

	public static shopController thisScript;

	public AudioClip soundBuyCoins;

	public UIPanel panelShopCoins;

	public UIPanel panelShopGuns;

	public UIPanel panelTekColCoins;

	public UIPanel panelMessagToShop;

	public UISprite sprSelCoins;

	public UISprite sprFrameShopItem;

	public UILabel lbKolCoins;

	public UILabel lbConnection;

	public UILabel lbTitleName;

	public UILabel lbInfoTextItem;

	public UILabel lbCountMoney;

	public InfoGun infoGunScript;

	public GameObject messagUspeshPokupka;

	public GameObject colliderFonMagaz;

	public GameObject butBuyProduct;

	public GameObject butEquipGun;

	public GameObject butUnEquipGun;

	[HideInInspector]
	public exitTo tekExitTo;

	private bool internetEnabled;

	private string metodForExit = string.Empty;

	private int kolMig = 5;

	private int tekKolMig;

	// TODO: This seems to be some kind of delay between getting coins.
	private float vremMig = 0.3f;

	[HideInInspector]
	public bool openToMenu;

	public List<productObj> listItemProducts = new List<productObj>();

	public List<productObjCoins> listCoinsProducts = new List<productObjCoins>();

	public List<ShopItemRazdel> listShopItemRazdel = new List<ShopItemRazdel>();

	public ShopItemRazdel currentRazdel;

	public UICenterOnChild centerOnChildScript;

	public productObj chooseProdObj;

	private UIWrapContent curWrap;

	private UIGrid curGrid;

	private void Awake()
	{
		thisScript = this;
		listItemProducts.Clear();
		listShopItemRazdel.Clear();
		foreach (productObjCoins listCoinsProduct in listCoinsProducts)
		{
			listCoinsProduct.setRuleIdPlatform();
		}
	}

	private void Start()
	{
		HOTween.Init(false, false, false);
		openToMenu = (controllerMenu.thisScript != null);
		lbKolCoins.text = string.Empty + settings.tekKolCoins;
		InitShop();
	}

	private void InitShop()
	{
		UpdateCoinShopUnlocks();
	}

	private void OnEnable()
	{
		UpdateCoinShopUnlocks();
	}

	private void OnDisable()
	{
	}

	private void Update()
	{
		if (centerOnChildScript == null)
		{
			return;
		}

		GameObject centeredObject = centerOnChildScript.centeredObject;
		chooseProdObj = centeredObject.GetComponent<productObj>();
		if (chooseProdObj != null)
		{
			lbTitleName.text = chooseProdObj.titleName;
			lbCountMoney.text = string.Empty + chooseProdObj.price;
			openGunShopWindowFor(chooseProdObj);
			if (chooseProdObj.showGunInfo)
			{
				infoGunScript.gameObject.SetActive(true);
				infoGunScript.setInfo(
					chooseProdObj.percentDamage,
					chooseProdObj.percentRange,
					chooseProdObj.percentAmmo,
					chooseProdObj.percentMobility
				);
			}
			else
			{
				infoGunScript.gameObject.SetActive(false);
			}
		}
	}

	public void openGunShopWindowFor(productObj curItem)
	{
		butBuyProduct.SetActive(
			!((curItem.showByKey && Load.LoadBool(curItem.keyForShow))
			|| (openToMenu && !curItem.showToMenu))
		);

		if (curItem.showInfoText)
		{
			lbInfoTextItem.text = curItem.infoText;
			lbInfoTextItem.gameObject.SetActive(true);
		}
		else
		{
			lbInfoTextItem.gameObject.SetActive(false);
		}

		if (curItem.showButEquip && curItem.showByKey && Load.LoadBool(curItem.keyForShow))
		{
			if (Load.LoadBool(settings.keyEquipGun + curItem.keyForShow))
			{
				butEquipGun.SetActive(false);
				butUnEquipGun.SetActive(true);
			}
			else
			{
				butEquipGun.SetActive(true);
				butUnEquipGun.SetActive(false);
			}
		}
		else
		{
			butEquipGun.SetActive(false);
			butUnEquipGun.SetActive(false);
		}
	}

	public void UpdateCoinShopUnlocks()
	{
		foreach (productObjCoins listCoinsProduct in listCoinsProducts)
		{
			listCoinsProduct.reset();
		}

		if (ActivityIndicator.activEnabled)
		{
			return;
		}

		internetEnabled = false;

		/* Unlocking all of the products by default. */
		foreach (productObjCoins listCoinsProduct2 in listCoinsProducts)
		{
			listCoinsProduct2.productEnabled = true;
			internetEnabled = true; // TODO: Should this be here still?
			listCoinsProduct2.nomProduct = 0; // TODO: Is this needed?
			listCoinsProduct2.vklBut();
			break;
		}

		if (internetEnabled)
		{
			if (lbConnection != null && lbConnection.gameObject != null)
			{
				lbConnection.gameObject.SetActive(false);
			}
		}
		else if (lbConnection != null && lbConnection.gameObject != null)
		{
			lbConnection.gameObject.SetActive(true);
		}
	}

	private void showUspeshPokupka()
	{
		if (messagUspeshPokupka != null)
		{
			messagUspeshPokupka.SetActive(true);
		}
		Utils.Instance.SafeInvoke(hideUspeshPokupka, 1.5f, false);
	}

	private void hideUspeshPokupka()
	{
		if (messagUspeshPokupka != null)
		{
			messagUspeshPokupka.SetActive(false);
		}
	}

	public void buyProductFromShopItemWithId(idGuns curIdItem)
	{
		foreach (productObj listItemProduct in listItemProducts)
		{
			if (listItemProduct.idItems != curIdItem)
			{
				continue;
			}

			/* If they have enough coins to buy the product. */
			if (settings.tekKolCoins >= listItemProduct.price)
			{
				Debug.Log("buy product " + curIdItem);
				settings.updateKolCoins(settings.tekKolCoins - listItemProduct.price);
				switch (listItemProduct.deistvAfterBuy)
				{
				case whatDoAfterBuy.updateKey:
					Save.SaveBool(listItemProduct.keyForShow, true);
					break;

				case whatDoAfterBuy.fullArmor:
					Save.SaveInt(settings.keyArmor, 100);
					settings.updateArmorEnabled();
					if (GameController.thisScript != null)
					{
						GameController.thisScript.playerScript.Armor = 100;
					}
					break;

				case whatDoAfterBuy.fullHealth:
					if (GameController.thisScript != null)
					{
						GameController.thisScript.playerScript.Health = 100;
					}
					break;

				case whatDoAfterBuy.activJetpack:
					JetpackBehavior.ActivateFromShop();
					break;

				case whatDoAfterBuy.fullAmmo:
					GameController.thisScript.playerScript.weaponManager.fullWeapon();
					break;

				case whatDoAfterBuy.addGrenade:
					settings.addGrenade(10);
					if (GameController.thisScript != null)
					{
						GameController.thisScript.playerScript.weaponManager.updateCountBulletsCurWeapon();
					}
					break;

				case whatDoAfterBuy.addMolotov:
					settings.addMolotov(10);
					if (GameController.thisScript != null)
					{
						GameController.thisScript.playerScript.weaponManager.updateCountBulletsCurWeapon();
					}
					break;

				case whatDoAfterBuy.addC4:
					settings.addC4(10);
					if (GameController.thisScript != null)
					{
						GameController.thisScript.playerScript.weaponManager.updateCountBulletsCurWeapon();
					}
					break;
				}

				EquipGun();
				showUspeshPokupka();
				lbKolCoins.text = string.Empty + settings.tekKolCoins;
				settings.updateAllKeychain();
			}
			else
			{
				showMessageGoToShop();
			}
			break;
		}
	}

	public void buyProductCoinsWithID(string idProduct)
	{
	}

	public static void showCoinShop(exitTo kuda)
	{
		thisScript.tekExitTo = kuda;
		thisScript.UpdateCoinShopUnlocks();
		thisScript.panelShopCoins.gameObject.SetActive(true);
		thisScript.panelShopGuns.gameObject.SetActive(false);
		thisScript.showPanelCoins();
		thisScript.hideMessageGoToShop();
		if (GameController.thisScript != null && settings.offlineMode)
		{
			Time.timeScale = 0f;
		}
		thisScript.UpdateCoinShopUnlocks();
	}

	public static void showShopGuns(exitTo kuda)
	{
		thisScript.tekExitTo = kuda;
		thisScript.panelShopCoins.gameObject.SetActive(false);
		thisScript.panelShopGuns.gameObject.SetActive(true);
		if (controllerMenu.thisScript != null)
		{
			controllerMenu.thisScript.hideAllPanelMenu();
		}
		if (GameController.thisScript != null)
		{
			if (settings.offlineMode)
			{
				Time.timeScale = 0f;
			}
			GameController.thisScript.hideAllPanels();
		}
		thisScript.showPanelCoins();
	}

	public static void hideShop()
	{
		switch (thisScript.tekExitTo)
		{
		case exitTo.menu:
			thisScript.hidePanelCoins();
			thisScript.panelShopCoins.gameObject.SetActive(false);
			thisScript.panelShopGuns.gameObject.SetActive(false);
			controllerMenu.thisScript.showMainMenu();
			break;

		case exitTo.shopGuns:
			thisScript.showPanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(true);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			if (controllerMenu.thisScript != null)
			{
				thisScript.tekExitTo = exitTo.menu;
			}
			break;

		case exitTo.heroRoom:
			thisScript.hidePanelCoins();
			thisScript.panelShopCoins.gameObject.SetActive(false);
			thisScript.panelShopGuns.gameObject.SetActive(false);
			controllerMenu.thisScript.showHeroRoom();
			break;

		case exitTo.heroRoomFromGuns:
			thisScript.showPanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(true);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			if (controllerMenu.thisScript != null)
			{
				thisScript.tekExitTo = exitTo.heroRoom;
			}
			break;

		case exitTo.gameWin:
			thisScript.hidePanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(false);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			if (settings.offlineMode)
			{
				Time.timeScale = 1f;
			}
			GameController.thisScript.showWindowGame();
			break;

		case exitTo.gameShopGun:
			thisScript.showPanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(true);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			thisScript.tekExitTo = exitTo.gameWin;
			break;

		case exitTo.gamePause:
			thisScript.hidePanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(false);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			GameController.thisScript.showWindowPause();
			break;

		case exitTo.gamePauseShopGun:
			thisScript.showPanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(true);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			thisScript.tekExitTo = exitTo.gamePause;
			break;

		case exitTo.gameEndGame:
			thisScript.hidePanelCoins();
			thisScript.panelShopGuns.gameObject.SetActive(false);
			thisScript.panelShopCoins.gameObject.SetActive(false);
			GameController.thisScript.showPanelEndGame();
			break;
		}
	}

	/* Brings up a menu to tell the user to buy more  coins. */
	public void showMessageGoToShop()
	{
		panelMessagToShop.gameObject.SetActive(true);
	}

	public void hideMessageGoToShop()
	{
		panelMessagToShop.gameObject.SetActive(false);
	}

	public void vostanovleniePokupok()
	{
		otklIndictator();
	}

	public void otklIndictator()
	{
		ActivityIndicator.activEnabled = false;
		if (colliderFonMagaz != null)
		{
			colliderFonMagaz.SetActive(ActivityIndicator.activEnabled);
		}
		UpdateCoinShopUnlocks();
	}

	public void showCoinShop()
	{
		if (panelShopCoins.gameObject.activeSelf)
		{
			return;
		}
		if (controllerMenu.thisScript != null)
		{
			if (panelShopGuns.gameObject.activeSelf)
			{
				tekExitTo = (tekExitTo == exitTo.heroRoom)
					? exitTo.heroRoomFromGuns
					: exitTo.shopGuns;

				panelShopGuns.gameObject.SetActive(false);
			}
			else if (controllerMenu.thisScript.panelHeroRoom.gameObject.activeSelf)
			{
				tekExitTo = exitTo.heroRoom;
			}
			else
			{
				tekExitTo = exitTo.menu;
			}
			panelShopCoins.gameObject.SetActive(true);
			showPanelCoins();
			hideMessageGoToShop();
			controllerMenu.thisScript.hideAllPanelMenu();
		}
		if (GameController.thisScript != null)
		{
			if (settings.offlineMode)
			{
				Time.timeScale = 0f;
			}

			if (GameController.thisScript.panelGameTop.gameObject.activeSelf)
			{
				tekExitTo = exitTo.gameWin;
			}

			if (panelShopGuns.gameObject.activeSelf)
			{
				tekExitTo = (tekExitTo == exitTo.gamePause)
					? exitTo.gamePauseShopGun
					: exitTo.gameShopGun;
			}

			if (GameController.thisScript.panelEndGame.gameObject.activeSelf)
			{
				tekExitTo = exitTo.gameEndGame;
			}

			hideMessageGoToShop();
			panelShopGuns.gameObject.SetActive(false);
			panelShopCoins.gameObject.SetActive(true);
			GameController.thisScript.hideAllPanels();
			showPanelCoins();
		}
		UpdateCoinShopUnlocks();
	}

	public void showPanelCoins()
	{
		lbKolCoins.text = string.Empty + settings.tekKolCoins;
		panelTekColCoins.gameObject.SetActive(true);
	}

	public void hidePanelCoins()
	{
		panelTekColCoins.gameObject.SetActive(false);
	}

	public void playSoundBuyCoins()
	{
		if (settings.soundEnabled)
		{
			NGUITools.PlaySound(soundBuyCoins);
		}
	}

	public void startMiganieCoins()
	{
		tekKolMig = 0;
		showHideSprCoins();
	}

	// TODO: I think this is some kind of delayed recursive function that
	// slowely incraments the coin counter.
	private void showHideSprCoins()
	{
		if (sprSelCoins != null)
		{
			if (sprSelCoins.enabled)
			{
				if (sprSelCoins != null)
				{
					sprSelCoins.enabled = false;
				}
				if (tekKolMig >= kolMig)
				{
					return;
				}
			}
			else
			{
				tekKolMig++;
				sprSelCoins.enabled = true;
			}
		}
		Utils.Instance.SafeInvoke(showHideSprCoins, vremMig, false);
	}

	public void cancelMiganieCoins()
	{
		CancelInvoke("showHideSprCoins");
		sprSelCoins.enabled = false;
	}

	private void productBuy()
	{
		otklIndictator();
		showUspeshPokupka();
		if (lbKolCoins != null)
		{
			lbKolCoins.text = string.Empty + settings.tekKolCoins;
		}
		startMiganieCoins();
	}

	private void productWithIdBuySuccessful(string idProduct)
	{
		foreach (productObjCoins listCoinsProduct in listCoinsProducts)
		{
			if (listCoinsProduct.idItems.Equals(idProduct))
			{
				Debug.Log("buy product shop coins with id = " + idProduct);
				if (listCoinsProduct.deistvAfterBuy == whatDoAfterBuy.addMoney)
				{
					settings.updateKolCoins(settings.tekKolCoins + listCoinsProduct.kolAddMoney);
				}
				playSoundBuyCoins();
				productBuy();
				break;
			}
		}
		settings.updateAllKeychain();
	}

	public void AddProdItemToList(productObj curProd)
	{
		if (!listItemProducts.Contains(curProd))
		{
			listItemProducts.Add(curProd);
		}
	}

	public void AddProdCoinsToList(productObjCoins curProd)
	{
		if (!listCoinsProducts.Contains(curProd))
		{
			listCoinsProducts.Add(curProd);
		}
	}

	public void AddShopItemRazdelToList(ShopItemRazdel curProd)
	{
		if (!listShopItemRazdel.Contains(curProd))
		{
			listShopItemRazdel.Add(curProd);
			if (curProd.ShowFirst)
			{
				SelectRadel(curProd);
			}
		}
	}

	public void SelectRadel(ShopItemRazdel chooseRazdel)
	{
		if (centerOnChildScript != null)
		{
			UICenterOnChild uICenterOnChild = centerOnChildScript;
			uICenterOnChild.onFinished = (SpringPanel.OnFinished)Delegate.Remove(uICenterOnChild.onFinished, new SpringPanel.OnFinished(centerProductFinished));
		}
		foreach (ShopItemRazdel item in listShopItemRazdel)
		{
			if (item.Equals(chooseRazdel))
			{
				currentRazdel = chooseRazdel;
				item.vklBut();
				curWrap = currentRazdel.scrollViewRazdel.GetComponentInChildren<UIWrapContent>();
				Invoke("AlignContent", 0.05f);
				centerOnChildScript = currentRazdel.scrollViewRazdel.GetComponentInChildren<UICenterOnChild>();
				UICenterOnChild uICenterOnChild2 = centerOnChildScript;
				uICenterOnChild2.onFinished = (SpringPanel.OnFinished)Delegate.Combine(uICenterOnChild2.onFinished, new SpringPanel.OnFinished(centerProductFinished));
				curGrid = chooseRazdel.scrollViewRazdel.GetComponentInChildren<UIGrid>();
			}
			else
			{
				item.otklBut();
			}
		}
	}

	private void AlignContent()
	{
		if (centerOnChildScript != null)
		{
			centerOnChildScript.CenterOn(currentRazdel.objForFirstAlign);
		}
	}

	public void BuyChooseItem()
	{
		if (chooseProdObj != null)
		{
			FlurryWrapper.LogEvent(FlurryWrapper.EV_BUY_ITEM + chooseProdObj.titleName);
			buyProductFromShopItemWithId(chooseProdObj.idItems);
		}
	}

	public void EquipGun()
	{
		if (chooseProdObj != null && chooseProdObj.showButEquip && chooseProdObj.showByKey && Load.LoadBool(chooseProdObj.keyForShow))
		{
			Debug.Log("euiped " + chooseProdObj.keyForShow);
			Save.SaveBool(settings.keyEquipGun + chooseProdObj.keyForShow, true);
			openGunShopWindowFor(chooseProdObj);
			if (!openToMenu)
			{
				GameController.thisScript.weaponManagerScripts.EquipWeapon(chooseProdObj.keyForShow);
			}
		}
	}

	public void UnEquipGun()
	{
		if (chooseProdObj != null)
		{
			Save.SaveBool(settings.keyEquipGun + chooseProdObj.keyForShow, false);
			openGunShopWindowFor(chooseProdObj);
			if (!openToMenu)
			{
				GameController.thisScript.weaponManagerScripts.UnequipWeapon(chooseProdObj.keyForShow);
			}
		}
	}

	public void centerProductFinished()
	{
	}

	public void productIsDrag()
	{
		productObj[] componentsInChildren = curGrid.GetComponentsInChildren<productObj>();
		productObj[] array = componentsInChildren;
		foreach (productObj productObj2 in array)
		{
			TweenScale.Begin(productObj2.gameObject, 0.2f, new Vector3(1f, 1f, 1f));
		}
	}
}
