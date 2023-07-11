using System;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class controllerMenu : MonoBehaviour
{
	public static controllerMenu thisScript;

	public UIPanel panelMainMenu;

	public UIPanel panelCustomGame;

	public UIPanel panelCreateRoom;

	public UIPanel panelScrollRoom;

	public UIPanel panelHeroRoom;

	public UIPanel panelSettings;

	public UIPanel panelEnterPassword;

	public UIPanel panelMessage;

	public UIPanel panelUpload;

	public GameObject exampleButRoom;

	public GameObject objGrid;

	public GameObject objPlayer;

	public GameObject butSetSkin;

	public GameObject butSetSkinOff;

	public GameObject butBuySkin;

	public GameObject plaskaPriceHeroRoom;

	public GameObject butSoundOn;

	public GameObject butSoundOff;

	public GameObject butMusicOn;

	public GameObject butMusicOff;

	public GameObject messageErrorPass;

	public GameObject colliderIndicator;

	public GameObject sprChooseSkinOn;

	public GameObject sprChooseSkinOff;

	public UILabel lbKolPlayers;

	public UILabel lbNameRoom;

	public UILabel lbPassword;

	public UILabel lbName;

	public UILabel lbKolDead;

	public UILabel lbKolKill;

	public UILabel lbKolPoints;

	public UILabel lbEnterPassword;

	public UILabel lbFilterRoom;

	public UILabel lbMessage;

	public UILabel lbPriceSkin;

	[HideInInspector]
	public UIGrid gridScript;

	private int nomTekSkin;

	private bool clickOnButton;

	private float BROWSER_BOTTOM_OFFSET = (float)(139 * Screen.height) / 768f;

	[HideInInspector]
	public Texture2D[] arrSkins;

	public infoSkin[] arrInfoSkin;

	// TMP private TouchScreenKeyboard mKeyboard;

	public bool ClickOnButton
	{
		get
		{
			return clickOnButton;
		}
		set
		{
			clickOnButton = value;
		}
	}

	private void Awake()
	{
		thisScript = this;
		gridScript = objGrid.GetComponent<UIGrid>();
		arrSkins = Resources.LoadAll<Texture2D>("Skins");
		settings.updateOfflineMode(false);
	}

	private void Start()
	{
		if (settings.soundEnabled)
		{
			butSoundOn.SetActive(true);
			butSoundOff.SetActive(false);
		}
		else
		{
			butSoundOn.SetActive(false);
			butSoundOff.SetActive(true);
		}
		if (settings.musicEnabled)
		{
			base.audio.Play();
			butMusicOn.SetActive(true);
			butMusicOff.SetActive(false);
		}
		else
		{
			base.audio.Pause();
			butMusicOn.SetActive(false);
			butMusicOff.SetActive(true);
		}
		if (settings.includePreloadingSectors)
		{
			Application.backgroundLoadingPriority = ThreadPriority.High;
		}
		showMainMenu();
	}

	private void Update()
	{
		/*
		if (mKeyboard != null)
		{
			if (mKeyboard.done && !mKeyboard.wasCanceled)
			{
				closeKeyboard();
				if (panelEnterPassword.alpha > 0.5f)
				{
					connectRoomWithPassword();
				}
			}
			else if (mKeyboard.wasCanceled)
			{
				closeKeyboard();
			}
		}*/ // TMP
		if (!controllerConnectPhoton.thisScript.filterForSearchRoom.Equals(lbFilterRoom.text))
		{
			controllerConnectPhoton.thisScript.filterForSearchRoom = lbFilterRoom.text;
			controllerConnectPhoton.thisScript.updateFilteredRoomList(controllerConnectPhoton.thisScript.filterForSearchRoom);
		}
	}

	public void closeKeyboard()
	{
		/*if (mKeyboard != null)
		{
			mKeyboard.active = false;
			mKeyboard = null;
		}*/ // TMP
	}

	public void createKeyboard()
	{
		// TMP mKeyboard = TouchScreenKeyboard.Open(string.Empty, TouchScreenKeyboardType.Default, false, false);
	}

	private void OnDestroy()
	{
		closeKeyboard();
	}

	public void showMainMenu()
	{
		objPlayer.SetActive(false);
		shopController.thisScript.hidePanelCoins();
		panelMainMenu.gameObject.SetActive(true);
		panelCustomGame.gameObject.SetActive(false);
		panelScrollRoom.gameObject.SetActive(false);
		panelHeroRoom.gameObject.SetActive(false);
		panelSettings.gameObject.SetActive(false);
		panelEnterPassword.gameObject.SetActive(false);
		panelCreateRoom.gameObject.SetActive(false);
		panelSettings.gameObject.SetActive(false);
		ActivityIndicator.activEnabled = false;
	}

	public void showCustomGame()
	{
		showIndicator();
		settings.updateOfflineMode(false);
		if (PhotonNetwork.connected)
		{
			connectCreate_showCustomGame();
			return;
		}
		controllerConnectPhoton.thisScript.tekConnect = controllerConnectPhoton.connectTo.customGame;
		controllerConnectPhoton.thisScript._initializeWorldwide();
	}

	public void connectCreate_showCustomGame()
	{
		hideIndicator();
		clickOnButton = false;
		controllerConnectPhoton.thisScript.tekConnect = controllerConnectPhoton.connectTo.customGame;
		controllerConnectPhoton.thisScript._initializeWorldwide();
		shopController.thisScript.hidePanelCoins();
		panelCustomGame.gameObject.SetActive(true);
		panelScrollRoom.gameObject.SetActive(true);
		panelMainMenu.gameObject.SetActive(false);
		panelCreateRoom.gameObject.SetActive(false);
		panelEnterPassword.gameObject.SetActive(false);
	}

	public void showCreateRoom()
	{
		lbPassword.text = string.Empty;
		Save.SaveString(settings.keyEnterPassword, string.Empty);
		panelCustomGame.gameObject.SetActive(false);
		panelScrollRoom.gameObject.SetActive(false);
		panelCreateRoom.gameObject.SetActive(true);
	}

	public void showUpload()
	{
		panelUpload.gameObject.SetActive(true);
		panelHeroRoom.gameObject.SetActive(false);
	}

	public void showHeroRoom()
	{
		lbKolDead.text = string.Empty + settings.tekKolDead;
		lbKolKill.text = string.Empty + settings.tekKolKill;
		lbKolPoints.text = string.Empty + settings.maxKolPoints;
		nomTekSkin = settings.tekNomSkin;
		objPlayer.SetActive(true);
		activateCurSkinAfterDelay();
		Invoke("activateCurSkinAfterDelay", 0.05f);
		lbName.text = settings.tekName;
		shopController.thisScript.hidePanelCoins();
		panelMainMenu.gameObject.SetActive(false);
		panelUpload.gameObject.SetActive(false);
		panelHeroRoom.gameObject.SetActive(true);
		shopController.thisScript.showPanelCoins();
	}

	private void activateCurSkinAfterDelay()
	{
		activateSkinWithNom(nomTekSkin);
	}

	public void showSettings()
	{
		shopController.thisScript.hidePanelCoins();
		panelMainMenu.gameObject.SetActive(false);
		panelSettings.gameObject.SetActive(true);
		shopController.thisScript.tekExitTo = shopController.exitTo.heroRoom;
	}

	public void showEnterPassword()
	{
		panelCustomGame.gameObject.SetActive(false);
		panelScrollRoom.gameObject.SetActive(false);
		panelEnterPassword.gameObject.SetActive(true);
	}

	public void hideAllPanelMenu()
	{
		panelCustomGame.gameObject.SetActive(false);
		panelScrollRoom.gameObject.SetActive(false);
		objPlayer.SetActive(false);
		panelHeroRoom.gameObject.SetActive(false);
		panelMainMenu.gameObject.SetActive(false);
	}

	public void startRandomGame()
	{
		showIndicator();
		settings.updateOfflineMode(false);
		controllerConnectPhoton.thisScript.connectRandomRoom();
	}

	public void uvelKolPlayers()
	{
		int num = Convert.ToInt32(lbKolPlayers.text);
		if (num < 30)
		{
			num++;
			lbKolPlayers.text = string.Empty + num;
		}
	}

	public void umenKolPlayers()
	{
		int num = Convert.ToInt32(lbKolPlayers.text);
		if (num > 4)
		{
			num--;
			lbKolPlayers.text = string.Empty + num;
		}
	}

	public void createNewRoom()
	{
		Save.SaveString(settings.keyEnterPassword, lbPassword.text);
		controllerConnectPhoton.thisScript.createNewRoom(lbNameRoom.text, int.Parse(lbKolPlayers.text), Load.LoadString(settings.keyEnterPassword));
	}

	private void activateSkinWithNom(int curNomSkin)
	{
		izmenitTexturuNa(objPlayer, arrSkins[curNomSkin]);
		infoSkin infoSkinWithNom = getInfoSkinWithNom(curNomSkin);
		if (infoSkinWithNom != null && infoSkinWithNom.forBuy && !Load.LoadBool(settings.keyForBuySkin + infoSkinWithNom.skin.name))
		{
			activateButBuySkin();
			lbPriceSkin.text = string.Empty + infoSkinWithNom.price;
		}
		else if (curNomSkin == settings.tekNomSkin)
		{
			activateButSetSkins(false);
		}
		else
		{
			activateButSetSkins(true);
		}
	}

	private void activateButSetSkins(bool val)
	{
		butSetSkin.SetActive(val);
		sprChooseSkinOff.SetActive(val);
		butSetSkinOff.SetActive(!val);
		sprChooseSkinOn.SetActive(!val);
		plaskaPriceHeroRoom.gameObject.SetActive(false);
		butBuySkin.SetActive(false);
	}

	private void activateButBuySkin()
	{
		butSetSkin.SetActive(false);
		butSetSkinOff.SetActive(false);
		sprChooseSkinOff.SetActive(false);
		sprChooseSkinOn.SetActive(false);
		plaskaPriceHeroRoom.gameObject.SetActive(true);
		butBuySkin.SetActive(true);
	}

	private infoSkin getInfoSkinWithNom(int nomCurSkin)
	{
		string text = arrSkins[nomCurSkin].name;
		infoSkin[] array = arrInfoSkin;
		foreach (infoSkin infoSkin2 in array)
		{
			if (text == infoSkin2.skin.name)
			{
				return infoSkin2;
			}
		}
		return null;
	}

	private int getCurPriceSkin()
	{
		infoSkin infoSkinWithNom = getInfoSkinWithNom(nomTekSkin);
		if (infoSkinWithNom == null)
		{
			return 0;
		}
		return infoSkinWithNom.price;
	}

	public void buyAndSetCurSkins()
	{
		infoSkin infoSkinWithNom = getInfoSkinWithNom(nomTekSkin);
		FlurryWrapper.LogEvent(FlurryWrapper.EV_BUY_SKIN + infoSkinWithNom.skin.name);
		if (settings.tekKolCoins < infoSkinWithNom.price)
		{
			shopController.thisScript.showMessageGoToShop();
			return;
		}
		Save.SaveBool(settings.keyForBuySkin + infoSkinWithNom.skin.name, true);
		setNewSkin();
		settings.updateKolCoins(settings.tekKolCoins - infoSkinWithNom.price);
		settings.updateAllKeychain();
		shopController.thisScript.showPanelCoins();
	}

	public void sledSkins()
	{
		if (nomTekSkin >= arrSkins.Length - 1)
		{
			nomTekSkin = 0;
		}
		else
		{
			nomTekSkin++;
		}
		activateSkinWithNom(nomTekSkin);
	}

	public void predSkins()
	{
		if (nomTekSkin <= 0)
		{
			nomTekSkin = arrSkins.Length - 1;
		}
		else
		{
			nomTekSkin--;
		}
		activateSkinWithNom(nomTekSkin);
	}

	public void izmenitTexturuNa(GameObject player, Texture newTex)
	{
		Debug.Log("izmenitTexturuNa = " + newTex.name + " objPlayer=" + objPlayer.activeSelf);
		Renderer[] componentsInChildren = player.GetComponentsInChildren<Renderer>();
		if (componentsInChildren.Length != 0)
		{
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				renderer.material.mainTexture = newTex;
			}
		}
	}

	public void setNewSkin()
	{
		settings.setNomSkin(nomTekSkin);
		activateButSetSkins(false);
	}

	public void saveProfile()
	{
		settings.setNewName(lbName.text);
	}

	public void upload()
	{
		Texture2D t = arrSkins[nomTekSkin];
		SkinsManager.SaveTextureWithName(t, "upload_Skin.png", false);
		string text = Path.Combine(SkinsManager._PathBase, "upload_Skin.png");
		Debug.Log("upload path=" + text);
		using (new WebClient())
		{
			CookieContainer cookies = new CookieContainer();
			string text2 = UploadFileEx(text, "http://d-games.org/uploadimg.php", "userfile", "image/png", cookies);
			string url = "http://www.minecraft.net/profile/skin/remote?url=" + text2;
			Application.OpenURL(url);
		}
	}

	public static string UploadFileEx(string uploadfile, string url, string fileFormName, string contenttype, CookieContainer cookies)
	{
		if (fileFormName == null || fileFormName.Length == 0)
		{
			fileFormName = "file";
		}
		if (contenttype == null || contenttype.Length == 0)
		{
			contenttype = "application/octet-stream";
		}
		Uri requestUri = new Uri(url);
		string text = "----------" + DateTime.Now.Ticks.ToString("x");
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
		Debug.Log("webrequest=" + httpWebRequest);
		httpWebRequest.CookieContainer = cookies;
		httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
		httpWebRequest.Method = "POST";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("--");
		stringBuilder.Append(text);
		stringBuilder.Append("\r\n");
		stringBuilder.Append("Content-Disposition: form-data; name=\"");
		stringBuilder.Append(fileFormName);
		stringBuilder.Append("\"; filename=\"");
		stringBuilder.Append(Path.GetFileName(uploadfile));
		stringBuilder.Append("\"");
		stringBuilder.Append("\r\n");
		stringBuilder.Append("Content-Type: ");
		stringBuilder.Append(contenttype);
		stringBuilder.Append("\r\n");
		stringBuilder.Append("\r\n");
		string s = stringBuilder.ToString();
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		byte[] bytes2 = Encoding.UTF8.GetBytes("\r\n--" + text + "\r\n");
		FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read);
		long contentLength = bytes.Length + fileStream.Length + bytes2.Length;
		httpWebRequest.ContentLength = contentLength;
		Stream requestStream = httpWebRequest.GetRequestStream();
		requestStream.Write(bytes, 0, bytes.Length);
		byte[] array = new byte[checked((uint)Math.Min(4096, (int)fileStream.Length))];
		int num = 0;
		while ((num = fileStream.Read(array, 0, array.Length)) != 0)
		{
			requestStream.Write(array, 0, num);
		}
		requestStream.Write(bytes2, 0, bytes2.Length);
		WebResponse response = httpWebRequest.GetResponse();
		Stream responseStream = response.GetResponseStream();
		StreamReader streamReader = new StreamReader(responseStream);
		return streamReader.ReadToEnd();
	}

	public void pereklSoundEnabled()
	{
		settings.setSoundEnbled(!settings.soundEnabled);
		if (settings.soundEnabled)
		{
			butSoundOn.SetActive(true);
			butSoundOff.SetActive(false);
		}
		else
		{
			butSoundOn.SetActive(false);
			butSoundOff.SetActive(true);
		}
	}

	public void pereklMusicEnabled()
	{
		settings.setMusicEnbled(!settings.musicEnabled);
		if (settings.musicEnabled)
		{
			base.audio.Play();
			butMusicOn.SetActive(true);
			butMusicOff.SetActive(false);
		}
		else
		{
			base.audio.Pause();
			butMusicOn.SetActive(false);
			butMusicOff.SetActive(true);
		}
	}

	public void showShopGunsFromMenu()
	{
		panelMainMenu.gameObject.SetActive(false);
		shopController.showShopGuns(shopController.exitTo.menu);
	}

	public void connectRoomWithPassword()
	{
		string text = Load.LoadString(settings.keyRoomPass);
		if (text.Equals(lbEnterPassword.text))
		{
			settings.updateOfflineMode(false);
			controllerConnectPhoton.thisScript.joinRoomName(Load.LoadString(settings.keyRoomName));
		}
		else
		{
			showMessagErrorPassword();
		}
	}

	public void showMessagErrorPassword()
	{
		messageErrorPass.SetActive(true);
		Invoke("hideMessagErrorPassword", 2f);
	}

	public void hideMessagErrorPassword()
	{
		messageErrorPass.SetActive(false);
	}

	public void showIndicator()
	{
		ActivityIndicator.activEnabled = true;
		colliderIndicator.SetActive(true);
	}

	public void hideIndicator()
	{
		ActivityIndicator.activEnabled = false;
		colliderIndicator.SetActive(false);
	}

	public void showMessage(string msg)
	{
		lbMessage.text = msg;
		Invoke("hideMessage", 3f);
		panelMessage.alpha = 1f;
	}

	public void hideMessage()
	{
		panelMessage.alpha = 0f;
	}

	public void playOfflineMode()
	{
		settings.updateOfflineMode(true);
		Application.LoadLevel("LoadingGame");
	}
}
