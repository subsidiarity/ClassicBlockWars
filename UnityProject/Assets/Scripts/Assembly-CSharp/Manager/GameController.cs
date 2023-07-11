using System.Collections.Generic;
using Holoville.HOTween;
using Photon;
using UnityEngine;

public class GameController : Photon.MonoBehaviour
{
	public static GameController thisScript;

	public NewDriving newDrivingScript;

	[HideInInspector]
	public CarBehavior carScript;

	public HelicopterBehavior helicopterScript;

	[HideInInspector]
	public Rigidbody curCarRigidBody;

	[HideInInspector]
	public bonuseManager bonuseManagerScript;

	public bool useLestncu;

	public GameObject myCar;

	public GameObject myPlayer;

	public GameObject myHelic;

	public PlayerBehavior playerScript;

	public Transform allPlayerRespawnPoints;

	public GameObject bonuseRespawn;

	public GameObject spisokBonuse;

	public GameObject starsRespawn;

	public GameObject spisokStars;

	public GameObject spisokCars;

	private MeshRenderer[] arrPointStar;

	private bool[] arrPointStarNoEmpty;

	[HideInInspector]
	public WeaponManager weaponManagerScripts;

	public GameObject cameraGame;

	[HideInInspector]
	public RPG_Camera camSriptWalk;

	[HideInInspector]
	public SmoothFollow2 camScriptCar;

	private List<string> listMessages = new List<string>();

	[HideInInspector]
	public List<PlayerBehavior> listPlayers = new List<PlayerBehavior>();

	[HideInInspector]
	public List<butScrollPlayers> butPlayersList = new List<butScrollPlayers>();

	private List<butScrollPlayers> delButPlayerList = new List<butScrollPlayers>();

	private List<PlayerBehavior> delPlayerFromList = new List<PlayerBehavior>();

	[HideInInspector]
	public List<CarBehavior> arrAllCar = new List<CarBehavior>();

	[HideInInspector]
	public List<HelicopterBehavior> listAllHelicopter = new List<HelicopterBehavior>();

	public UITexture textDamage;

	public UIButton butPereklWeapon;

	public UILabel lbAmmo;

	public UILabel lbHealth;

	public UILabel lbArmor;

	public UILabel lbCurKolStars;

	public UILabel lbTimeGame;

	public UILabel lbPoints;

	public UILabel lbMessage1;

	public UILabel lbMessage2;

	public UILabel lbMessage3;

	public UILabel lbOfflineStars;

	public UILabel lbOfflineKilled;

	public UILabel lbOfflineDied;

	public UILabel lbEndGameMessage;

	public UILabel lbMoney;

	public UILabel lbCountCarBullets;

	public UIJoystick joystickWalk;

	public TouchCamera cameraRotateScript;

	public UIPanel walkInterface;

	public UIPanel carInteface;

	public UIPanel panelJoystick;

	public UIPanel panelPause;

	public UIPanel panelGameTop;

	public UIPanel panelListPlayers;

	public UIPanel panelScrollPlayers;

	public UIPanel panelLoading;

	public UIPanel panelReconnect;

	public UIPanel panelEndGame;

	public UIPanel panelChat;

	public UIPanel panelFly;

	public UIMiniMap panelMiniMap;

	public UIWorldMap panelWorldMap;

	public NJGMap mapScript;

	public GameObject exampleButPlayers;

	public GameObject objGrid;

	public GameObject butPlayerList;

	public GameObject butInCar;

	public GameObject butOutCar;

	public GameObject butOutHelicopter;

	public GameObject butEndGameExitMenu;

	public GameObject butDetonator;

	public GameObject indicatorStars;

	public GameObject indicatorTimeGame;

	public GameObject indicatorPoints;

	public GameObject indicatorArmor;

	public GameObject lbInfo;

	public GameObject butChat;

	public GameObject interfaceForCarWithWeapon;

	public GameObject rightInterfaceWalk;

	public UISlider sliderZoomWorldMap;

	public UIGrid gridScript;

	public AudioClip soundGameFon1;

	public AudioClip soundGameFon2;

	public AudioClip soundGameFon3;

	public AudioClip soundResaultStars;

	private bool spisokUpdate;

	private bool needUpdateSpisok;

	private clearMessage lbClaer1;

	private clearMessage lbClaer2;

	private clearMessage lbClaer3;

	public showResaultStars starsAnimScript;

	public int curKolStar;

	public int offlineKolKill;

	public int offlineKolDied;

	public int resaultKolStars;

	public float timeGame;

	public float stepZoom = 0.5f;

	public PathManager[] arrRoads = new PathManager[2];

	public Ladder curScriptLadder;

	public int countCarBullets
	{
		set
		{
			if (lbCountCarBullets != null)
			{
				lbCountCarBullets.text = string.Empty + value;
			}
		}
	}

	public int healthValue
	{
		set
		{
			lbHealth.text = string.Empty + value;
		}
	}

	public int armorValue
	{
		set
		{
			if (value <= 0)
			{
				lbArmor.text = string.Empty + 0;
				indicatorArmor.SetActive(false);
			}
			else
			{
				lbArmor.text = string.Empty + value;
				indicatorArmor.SetActive(true);
			}
		}
	}

	private void Awake()
	{
		thisScript = this;
		gridScript = objGrid.GetComponent<UIGrid>();
		HOTween.Init(false, false, false);
		if (cameraGame == null)
		{
			cameraGame = GameObject.FindGameObjectWithTag("MainCamera");
		}
		camSriptWalk = cameraGame.GetComponent<RPG_Camera>();
		camScriptCar = cameraGame.GetComponent<SmoothFollow2>();
		if (settings.includePreloadingSectors)
		{
			hideAllPanels();
			panelLoading.gameObject.SetActive(true);
		}
		if (settings.offlineMode)
		{
			butPlayerList.SetActive(false);
			createOfflinePlayer();
			arrPointStar = starsRespawn.GetComponentsInChildren<MeshRenderer>();
			arrPointStarNoEmpty = new bool[arrPointStar.Length];
			butChat.SetActive(false);
		}
		else
		{
			indicatorPoints.SetActive(true);
			indicatorStars.SetActive(false);
			indicatorTimeGame.SetActive(false);
			butChat.SetActive(true);
		}
	}

	private void Start()
	{
		if (bonuseRespawn == null)
		{
			bonuseRespawn = GameObject.FindGameObjectWithTag("Bonuse_Respawn");
		}
		if (starsRespawn == null)
		{
			starsRespawn = GameObject.FindGameObjectWithTag("Stars_Respawn");
		}
		if (spisokCars == null)
		{
			spisokCars = GameObject.FindGameObjectWithTag("Spisok_Cars");
		}
		if (spisokBonuse == null)
		{
			spisokBonuse = GameObject.FindGameObjectWithTag("Spisok_Bonuse");
		}
		if (spisokStars == null)
		{
			spisokStars = GameObject.FindGameObjectWithTag("Spisok_Starts");
		}
		lbMessage1.text = string.Empty;
		lbMessage2.text = string.Empty;
		lbMessage3.text = string.Empty;
		lbPoints.text = "0";
		lbClaer1 = lbMessage1.GetComponent<clearMessage>();
		lbClaer2 = lbMessage2.GetComponent<clearMessage>();
		lbClaer3 = lbMessage3.GetComponent<clearMessage>();
		playNextFonSound();
		textDamage.alpha = 0f;
		updateLbMoney();
		InvokeRepeating("updateLbMoney", 1f, 1f);
		Invoke("resetBoundsMap", 0.2f);
		if (!settings.includePreloadingSectors)
		{
			playerScript.tController.gravity = 20f;
		}
	}

	private void resetBoundsMap()
	{
		mapScript.UpdateBounds();
	}

	public void updateLbMoney()
	{
		lbMoney.text = string.Empty + settings.tekKolCoins;
	}

	public void updateLbTimeGame()
	{
		int num = (int)(timeGame / 60f);
		int num2 = (int)(timeGame - (float)(num * 60));
		string text = ((num2 >= 10) ? string.Empty : "0");
		lbTimeGame.text = string.Empty + num + ":" + text + num2;
	}

	public void updateKolStars(int kolStar)
	{
		curKolStar = kolStar;
		lbCurKolStars.text = string.Empty + curKolStar + "/" + settings.kolStarOnLevel;
		if (curKolStar == settings.kolStarOnLevel)
		{
			Invoke("endOfflineGame", 2f);
		}
	}

	public void endOfflineGame()
	{
		updateLbOfflineEndGame();
		starsAnimScript.updateShowKolStars(0);
		Time.timeScale = 0f;
		if (timeGame < 1f)
		{
			lbEndGameMessage.text = "Time is Over!";
		}
		else
		{
			lbEndGameMessage.text = "Level Complete!";
		}
		butEndGameExitMenu.SetActive(false);
		showPanelEndGame();
		int num = curKolStar * 100 + offlineKolKill * 10 - offlineKolDied * 5;
		if (num >= 1100)
		{
			resaultKolStars = 3;
			settings.updateKolCoins(settings.tekKolCoins + 15);
			settings.updateKeychainCoins();
		}
		else if (num >= 800)
		{
			resaultKolStars = 2;
			settings.updateKolCoins(settings.tekKolCoins + 10);
			settings.updateKeychainCoins();
		}
		else if (num >= 500)
		{
			resaultKolStars = 1;
			settings.updateKolCoins(settings.tekKolCoins + 5);
			settings.updateKeychainCoins();
		}
		else
		{
			resaultKolStars = 0;
		}
		animOfflineEndGame();
	}

	private void animOfflineEndGame()
	{
		float num = 0f;
		float num2 = ((curKolStar > 20) ? 2f : ((float)curKolStar * 0.1f));
		num = num2;
		float num3 = ((offlineKolKill > 20) ? 2f : ((float)offlineKolKill * 0.1f));
		if (num3 > num)
		{
			num = num3;
		}
		float num4 = ((offlineKolDied > 20) ? 2f : ((float)offlineKolDied * 0.1f));
		if (num4 > num)
		{
			num = num4;
		}
		HOTween.To(this, num2, new TweenParms().Prop("curKolStar", 0).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.EaseInOutCubic)
			.OnUpdate(updateLbOfflineEndGame));
		HOTween.To(this, num3, new TweenParms().Prop("offlineKolKill", 0).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.EaseInOutCubic)
			.OnUpdate(updateLbOfflineEndGame));
		HOTween.To(this, num4, new TweenParms().Prop("offlineKolDied", 0).UpdateType(UpdateType.TimeScaleIndependentUpdate).Ease(EaseType.EaseInOutCubic)
			.OnUpdate(updateLbOfflineEndGame));
		HOTween.To(this, num, new TweenParms().Prop("resaultKolStars", resaultKolStars).UpdateType(UpdateType.TimeScaleIndependentUpdate).OnComplete(animResultStars));
	}

	public void updateLbOfflineEndGame()
	{
		lbOfflineDied.text = string.Empty + offlineKolDied;
		lbOfflineKilled.text = string.Empty + offlineKolKill;
		lbOfflineStars.text = string.Empty + curKolStar + "/" + settings.kolStarOnLevel;
	}

	public void animResultStars()
	{
		if (resaultKolStars > 0)
		{
			resaultKolStars--;
			starsAnimScript.updateShowKolStars(starsAnimScript.showKolStars + 1);
			if (settings.soundEnabled)
			{
				NGUITools.PlaySound(soundResaultStars);
			}
			HOTween.To(this, 0.5f, new TweenParms().Prop("resaultKolStars", resaultKolStars).UpdateType(UpdateType.TimeScaleIndependentUpdate).OnComplete(animResultStars));
		}
		else
		{
			showEndGameButExitToMenu();
			shopController.thisScript.showPanelCoins();
			shopController.thisScript.startMiganieCoins();
		}
	}

	private void showEndGameButExitToMenu()
	{
		butEndGameExitMenu.SetActive(true);
	}

	private void createStarsToPointsRespawn()
	{
		Object original = Resources.Load("Bonuse/Star");
		for (int i = 0; i < arrPointStarNoEmpty.Length; i++)
		{
			arrPointStarNoEmpty[i] = false;
		}
		curKolStar = 0;
		int num = Mathf.Min(settings.kolStarOnLevel, arrPointStar.Length);
		for (int j = 0; j < num; j++)
		{
			int num2 = Random.Range(0, arrPointStar.Length);
			if (!arrPointStarNoEmpty[num2])
			{
				arrPointStarNoEmpty[num2] = true;
				Vector3 position = arrPointStar[num2].transform.position;
				GameObject gameObject = (GameObject)Object.Instantiate(original, position, Quaternion.identity);
				gameObject.transform.parent = spisokStars.transform;
			}
			else
			{
				j--;
			}
		}
	}

	public void animDamage()
	{
		HOTween.Kill(textDamage);
		HOTween.To(textDamage, 0.15f, new TweenParms().Prop("alpha", 1f));
		HOTween.To(textDamage, 0.15f, new TweenParms().Delay(0.15f).Prop("alpha", 0f));
	}

	public void playNextFonSound()
	{
		if (settings.musicEnabled)
		{
			AudioClip audioClip;
			switch (Random.Range(0, 3))
			{
			case 0:
				audioClip = soundGameFon1;
				break;
			case 1:
				audioClip = soundGameFon2;
				break;
			default:
				audioClip = soundGameFon3;
				break;
			}
			NGUITools.PlaySound(audioClip);
			Invoke("playNextFonSound", audioClip.length);
		}
	}

	public void createOfflinePlayer()
	{
		myPlayer = (GameObject)Object.Instantiate(Resources.Load("Player"), controllerConnectGame.thisScript.getRandomPointNearPlayer().position, Quaternion.identity);
		playerScript = myPlayer.GetComponent<PlayerBehavior>();
		Object.Destroy(myPlayer.GetComponent<lerpTransformPhoton>());
		Object.Destroy(myPlayer.GetComponent<PhotonView>());
		weaponManagerScripts = myPlayer.GetComponent<WeaponManager>();
	}

	public void switchInterface(bool car)
	{
		if (car)
		{
			walkInterface.gameObject.SetActive(false);
			carInteface.gameObject.SetActive(true);
			panelJoystick.gameObject.SetActive(false);
			camSriptWalk.enabled = false;
			camScriptCar.enabled = true;
		}
		else
		{
			walkInterface.gameObject.SetActive(true);
			carInteface.gameObject.SetActive(false);
			panelJoystick.gameObject.SetActive(true);
			camScriptCar.enabled = false;
			camSriptWalk.enabled = true;
		}
	}

	public void CheckInterfaceHelicopter(bool inHelic)
	{
		if (inHelic)
		{
			panelFly.gameObject.SetActive(true);
			rightInterfaceWalk.SetActive(false);
			camSriptWalk.enabled = false;
			camScriptCar.enabled = true;
			playerScript.tController.gravity = 3f;
		}
		else
		{
			panelFly.gameObject.SetActive(false);
			rightInterfaceWalk.SetActive(true);
			camScriptCar.enabled = false;
			camScriptCar.target = null;
			camSriptWalk.enabled = true;
			panelJoystick.gameObject.SetActive(true);
			playerScript.tController.gravity = 20f;
		}
	}

	public void moveAvtoVpered()
	{
		if (newDrivingScript != null)
		{
			newDrivingScript.isGo = true;
		}
	}

	public void stopMoveAvtoVpered()
	{
		if (newDrivingScript != null)
		{
			newDrivingScript.isGo = false;
		}
	}

	public void moveAvtoNazad()
	{
		if (newDrivingScript != null)
		{
			newDrivingScript.isBack = true;
		}
	}

	public void stopMoveAvtoNazad()
	{
		if (newDrivingScript != null)
		{
			newDrivingScript.isBack = false;
		}
	}

	public void hideAllPanels()
	{
		if (panelJoystick != null)
		{
			panelJoystick.gameObject.SetActive(false);
		}
		if (walkInterface != null)
		{
			walkInterface.gameObject.SetActive(false);
		}
		if (carInteface != null)
		{
			carInteface.gameObject.SetActive(false);
		}
		if (panelGameTop != null)
		{
			panelGameTop.gameObject.SetActive(false);
		}
		if (panelPause != null)
		{
			panelPause.gameObject.SetActive(false);
		}
		if (panelEndGame != null)
		{
			panelEndGame.gameObject.SetActive(false);
		}
		if (panelChat != null)
		{
			panelChat.gameObject.SetActive(false);
		}
		if (panelMiniMap != null)
		{
			panelMiniMap.gameObject.SetActive(false);
		}
		if (panelLoading != null)
		{
			panelLoading.gameObject.SetActive(false);
		}
		hideWorldMap();
		if (panelJoystick != null)
		{
			panelJoystick.gameObject.SetActive(false);
		}
		if (joystickWalk != null)
		{
			joystickWalk.position = new Vector2(0f, 0f);
		}
		if (shopController.thisScript != null)
		{
			shopController.thisScript.hidePanelCoins();
		}
	}

	public void showWindowPause()
	{
		hideAllPanels();
		panelPause.gameObject.SetActive(true);
	}

	public void showWindowGame()
	{
		hideAllPanels();
		if (playerScript.inCar)
		{
			carInteface.gameObject.SetActive(true);
		}
		else
		{
			walkInterface.gameObject.SetActive(true);
		}
		if (panelGameTop != null)
		{
			panelGameTop.gameObject.SetActive(true);
		}
		if (panelJoystick != null)
		{
			panelJoystick.gameObject.SetActive(true);
		}
		if (panelMiniMap != null)
		{
			panelMiniMap.gameObject.SetActive(true);
		}
	}

	public void showListPlayers()
	{
		panelListPlayers.gameObject.SetActive(true);
		panelScrollPlayers.gameObject.SetActive(true);
	}

	public void hideListPlayers()
	{
		panelListPlayers.gameObject.SetActive(false);
		panelScrollPlayers.gameObject.SetActive(false);
	}

	public void showLoadingMenu()
	{
		hideAllPanels();
		panelLoading.gameObject.SetActive(true);
	}

	public void showPanelReconnect()
	{
		hideAllPanels();
		panelReconnect.gameObject.SetActive(true);
		ActivityIndicator.activEnabled = true;
	}

	public void hidePanelReconnect()
	{
		panelReconnect.gameObject.SetActive(false);
		ActivityIndicator.activEnabled = false;
		showWindowGame();
	}

	public void showPanelEndGame()
	{
		hideAllPanels();
		shopController.thisScript.showPanelCoins();
		panelEndGame.gameObject.SetActive(true);
	}

	public void showPanelChat()
	{
		hideAllPanels();
		panelChat.gameObject.SetActive(true);
		ChatController.thisScript.createChat();
	}

	public void pause()
	{
		showWindowPause();
		if (settings.offlineMode)
		{
			Time.timeScale = 0f;
		}
	}

	public void resume()
	{
		showWindowGame();
		if (settings.offlineMode)
		{
			Time.timeScale = 1f;
		}
	}

	public void replay()
	{
		Time.timeScale = 1f;
		Application.LoadLevel("LoadingGame");
	}

	public void exitToMenu()
	{
		if (settings.offlineMode)
		{
			Time.timeScale = 1f;
			loadSceneLoading();
		}
		else
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	public void exitAfterDisconect()
	{
		showLoadingMenu();
		loadSceneLoading();
	}

	private void loadSceneLoading()
	{
		Application.LoadLevel("LoadingMenu");
	}

	public void setSprButPereklWeapon(string name)
	{
		butPereklWeapon.normalSprite = name;
		butPereklWeapon.hoverSprite = name;
		butPereklWeapon.pressedSprite = name;
	}

	public void obnovitSpisokButForPlayersWithDelay()
	{
		if (thisScript != null)
		{
			Invoke("obnovitSpisokButForPlayers", 0.2f);
		}
	}

	public void obnovitSpisokButForPlayers()
	{
		if (settings.offlineMode)
		{
			return;
		}
		Object original = exampleButPlayers;
		removeAllButBezPlayers();
		int num = 0;
		sortListPlayers();
		foreach (PlayerBehavior listPlayer in listPlayers)
		{
			butScrollPlayers butPlayer = getButPlayer(listPlayer.indPlayer);
			if (butPlayer != null)
			{
				butPlayer.gameObject.name = string.Empty + num;
				butPlayer.labelNomPlayers.text = string.Empty + (num + 1);
				butPlayer.labelPointsPlayers.text = string.Empty + listPlayer.Points;
				butPlayer.labelNamePlayers.text = listPlayer.nick;
			}
			else
			{
				GameObject gameObject = (GameObject)Object.Instantiate(original);
				gameObject.SetActive(true);
				gameObject.name = string.Empty + num;
				butScrollPlayers component = gameObject.GetComponent<butScrollPlayers>();
				component.indentificator = listPlayer.indPlayer;
				component.labelNomPlayers.text = string.Empty + (num + 1);
				component.labelPointsPlayers.text = string.Empty + listPlayer.Points;
				component.labelNamePlayers.text = string.Empty + listPlayer.nick;
				gameObject.transform.parent = objGrid.transform;
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				butPlayersList.Add(component);
			}
			num++;
		}
		Invoke("sortGrid", 0.1f);
	}

	private void sortGrid()
	{
		if (gridScript != null)
		{
			gridScript.repositionNow = true;
		}
	}

	private void removeAllPlayersFromListBezObj()
	{
		foreach (PlayerBehavior listPlayer in listPlayers)
		{
			if (listPlayer == null)
			{
				delPlayerFromList.Add(listPlayer);
			}
		}
		foreach (PlayerBehavior delPlayerFrom in delPlayerFromList)
		{
			listPlayers.Remove(delPlayerFrom);
		}
		delPlayerFromList.Clear();
	}

	public void removeAllButBezPlayers()
	{
		if (settings.offlineMode)
		{
			return;
		}
		delButPlayerList.Clear();
		bool flag = false;
		removeAllPlayersFromListBezObj();
		foreach (butScrollPlayers butPlayers in butPlayersList)
		{
			flag = false;
			foreach (PlayerBehavior listPlayer in listPlayers)
			{
				if (butPlayers.indentificator == listPlayer.indPlayer)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				delButPlayerList.Add(butPlayers);
			}
		}
		foreach (butScrollPlayers delButPlayer in delButPlayerList)
		{
			Object.Destroy(delButPlayer.gameObject);
			butPlayersList.Remove(delButPlayer);
		}
		delButPlayerList.Clear();
	}

	private bool sozdanaLiButPlayer(int idPlayer)
	{
		if (settings.offlineMode)
		{
			return true;
		}
		foreach (butScrollPlayers butPlayers in butPlayersList)
		{
			if (butPlayers.indentificator == idPlayer)
			{
				return true;
			}
		}
		return false;
	}

	private butScrollPlayers getButPlayer(int idPlayer)
	{
		if (settings.offlineMode)
		{
			return null;
		}
		foreach (butScrollPlayers butPlayers in butPlayersList)
		{
			if (butPlayers.indentificator == idPlayer)
			{
				return butPlayers;
			}
		}
		return null;
	}

	public void addPlayerToList(PlayerBehavior newPlayer)
	{
		if (settings.offlineMode)
		{
			return;
		}
		foreach (PlayerBehavior listPlayer in listPlayers)
		{
			if (newPlayer == listPlayer)
			{
				obnovitSpisokButForPlayers();
				return;
			}
		}
		listPlayers.Add(newPlayer);
		obnovitSpisokButForPlayers();
		SendPositionAllCar();
		SendPositionAllHelic();
	}

	public void SendPositionAllCar()
	{
		if (settings.offlineMode)
		{
			return;
		}
		foreach (CarBehavior item in arrAllCar)
		{
			if (item.playerInCar)
			{
				item.SendSetIdPlayerInCar();
			}
			else
			{
				item.SendPositionCarOther();
			}
		}
	}

	public void SendPositionAllHelic()
	{
		if (settings.offlineMode)
		{
			return;
		}
		foreach (HelicopterBehavior item in listAllHelicopter)
		{
			if (item.playerInHelic)
			{
				item.SendSetIdPlayerInHelic();
			}
			else
			{
				item.SendPositionHelicOther();
			}
		}
	}

	public void removePlayerFromList(PlayerBehavior pb)
	{
		if (!settings.offlineMode)
		{
			listPlayers.Remove(pb);
			obnovitSpisokButForPlayers();
		}
	}

	public void sortListPlayers()
	{
		if (settings.offlineMode)
		{
			return;
		}
		bool flag = true;
		while (flag)
		{
			flag = false;
			for (int i = 0; i < listPlayers.Count - 1; i++)
			{
				if (listPlayers[i].points < listPlayers[i + 1].points)
				{
					PlayerBehavior value = listPlayers[i];
					listPlayers[i] = listPlayers[i + 1];
					listPlayers[i + 1] = value;
					flag = true;
				}
			}
		}
	}

	private static int ComparePlayers(PlayerBehavior x, PlayerBehavior y)
	{
		if (x == null)
		{
			if (y == null)
			{
				return 0;
			}
			return 1;
		}
		if (y == null)
		{
			return -1;
		}
		if (y.points == x.points)
		{
			return 0;
		}
		if (y.points > x.points)
		{
			return 1;
		}
		return -1;
	}

	public void stopRecconect()
	{
		controllerConnectGame.thisScript.stopRecconect();
		exitAfterDisconect();
	}

	public void addMessageToListOnline(string msg)
	{
		if (!settings.offlineMode)
		{
			base.photonView.RPC("addMessageToList", PhotonTargets.All, FilterBadWorld.FilterString(msg));
		}
	}

	[RPC]
	public void addMessageToList(string msg)
	{
		Debug.Log("addMessageToList=" + msg);
		listMessages.Add(msg);
		showNextMessageFromList();
	}

	public void showNextMessageFromList()
	{
		if (listMessages.Count > 0)
		{
			lbMessage3.text = lbMessage2.text;
			lbMessage2.text = lbMessage1.text;
			lbMessage1.text = listMessages[0];
			lbClaer3.start(lbClaer2.timeForClear);
			lbClaer2.start(lbClaer1.timeForClear);
			lbClaer1.start(5f);
			listMessages.RemoveAt(0);
		}
	}

	public void removePlayerWithID(int idPlayer)
	{
		foreach (PlayerBehavior listPlayer in listPlayers)
		{
			if (listPlayer.ownerIDPlayer == idPlayer)
			{
				if (!listPlayer.gameObject.activeSelf)
				{
					removePlayerFromList(listPlayer);
					Object.Destroy(listPlayer.gameObject);
				}
				break;
			}
		}
	}

	public void showWorldMap()
	{
		hideAllPanels();
		returnPositionWorldMap();
		sliderZoomWorldMap.value = 0.2f;
		setZoomWorldMap(sliderZoomWorldMap.value);
		setSizeIconsOnMap(60);
		panelMiniMap.ToggleWorldMap();
	}

	public void hideWorldMap()
	{
		if (panelWorldMap != null)
		{
			panelWorldMap.Hide();
		}
		setSizeIconsOnMap(30);
	}

	public void setZoomWorldMap(float val)
	{
		val = panelWorldMap.minZoom + val * (panelWorldMap.maxZoom - panelWorldMap.minZoom);
		if (val < panelWorldMap.minZoom)
		{
			val = panelWorldMap.minZoom;
		}
		if (val > panelWorldMap.maxZoom)
		{
			val = panelWorldMap.maxZoom;
		}
		panelWorldMap.zoom = val;
	}

	public void zoomWorldMapIn()
	{
		sliderZoomWorldMap.value = (panelWorldMap.zoom + stepZoom - panelWorldMap.minZoom) / (panelWorldMap.maxZoom - panelWorldMap.minZoom);
		setZoomWorldMap(sliderZoomWorldMap.value);
	}

	public void zoomWorldMapOut()
	{
		sliderZoomWorldMap.value = (panelWorldMap.zoom - stepZoom - panelWorldMap.minZoom) / (panelWorldMap.maxZoom - panelWorldMap.minZoom);
		setZoomWorldMap(sliderZoomWorldMap.value);
	}

	public void returnPositionWorldMap()
	{
		panelWorldMap.ResetPanning();
	}

	public void setSizeIconsOnMap(int curSize)
	{
		if (mapScript != null)
		{
			mapScript.iconSize = curSize;
		}
	}

	private void OnDestroy()
	{
		if (IsInvoking("sortGrid"))
		{
			CancelInvoke("sortGrid");
		}
		listPlayers.Clear();
		butPlayersList.Clear();
	}
}
