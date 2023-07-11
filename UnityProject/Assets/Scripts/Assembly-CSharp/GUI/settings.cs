using UnityEngine;

public class settings : MonoBehaviour
{
	public enum typeStore
	{
		PlayStore = 0,
		AmazonStore = 1
	}

	public static string verConnectPhoton = "GTAAndroid_1.15";

	public static settings thisScript;

	public static typeStore currentStore = typeStore.PlayStore;

	public static string keyVersionGame = "keyVersionGame";

	public static int currentVersionGame = 10;

	public static bool isLearned;

	public static bool includePreloadingSectors = true;

	public static string keyNameShooseRoom = "keyNameShooseRoom";

	public static string keyEnterPassword = "keyEnterPassword";

	public static string keyShooseSkin = "keyShooseSkin";

	public static string keyName = "keyName";

	public static string keyMusic = "keyMusic";

	public static string keySound = "keySound";

	public static string keyRoomPass = "keyRoomPass";

	public static string keyRoomName = "keyRoomName";

	public static string keyOfflineMode = "keyOfflineMode";

	public static string keyKolDead = "keyKolDead";

	public static string keyKolKill = "keyKolKill";

	public static string keyKolMaxPoints = "keyKolMaxPoints";

	public static string keyKolCoins = "keyKolCoins";

	public static string keyArmor = "keyArmor";

	public static string keyKeychainArmor = "keyKeychainArmor";

	public static string keyArmorEnabled = "keyArmorEnabled";

	public static string keyEquipGun = "EquipGun_";

	public static string keyKeychainKolCoins = "keyKeychainKolCoins";

	public static string keyKeychainGun1 = "keyKeychainGun1";

	public static string keyKeychainGun2 = "keyKeychainGun2";

	public static string keyKeychainGun3 = "keyKeychainGun3";

	public static string keyKeychainGunBat = "keyKeychainGunBat";

	public static string keyKeychainGunSniper = "keyKeychainGunSniper";

	public static string keyKeychainGunMinigun = "keyKeychainGunMinigun";

	public static string keyKeychainJetpack = "keyKeychainJetpack";

	public static string keyKeychainSpas12 = "keyKeychainSpas12";

	public static string keyKeychainBrass = "keyKeychainBrass";

	public static string keyKeychainRevolver = "keyKeychainRevolver";

	public static string keyKeychainKnife = "keyKeychainKnife";

	public static string keyKeychainChainsaw = "keyKeychainChainsaw";

	public static string keyKeychainGlock = "keyKeychainGlock";

	public static string keyCountBullets = "keyCountBullets_";

	public static string keyBulletsSaved = "keyBulletsSaved_";

	public static string keyArrAllWeapons = "keyArrAllWeapons";

	public static string keyBuyGunUzi = "SMG";

	public static string keyBuyGunShotgun = "shotgun";

	public static string keyBuyGunAk47 = "ak47";

	public static string keyBuyGunSniperRifle = "SniperRifle";

	public static string keyBuyGunBat = "bat";

	public static string keyBuyGunMinigun = "Minigun";

	public static string keyBuyGunGrenade = "grenade";

	public static string keyBuyGunMolotov = "molotov";

	public static string keyBuyGunC4 = "c4";

	public static string keyBuyRevolver = "Revolver";

	public static string keyBuyKnife = "Knife";

	public static string keyBuyChainsaw = "Chainsaw";

	public static string keyBuyGlock = "glock18";

	public static string keyBuySpas12 = "SPAS12";

	public static string keyBuyBrass = "Brass_knuckles";

	private string[] arrKeyPostGuns = new string[5] { "knuckles", "eagl", "grenade", "molotov", "c4" };

	public static string[] arrAllWeapons = new string[21]
	{
		"knuckles", "Brass_knuckles", "Knife", "bat", "Chainsaw", "eagl", "Revolver", "glock18", "SMG", "shotgun",
		"SPAS12", "ak47", "m16", "thompson", "SniperRifle", "Minigun", "rpg", "grenade", "molotov", "c4",
		"flamethrower"
	};

	public static string noNamePlayer = "No name";

	public static int maxKolBonuse = 10;

	public static int tekKolBonuse = 0;

	public static float timeAddBonuse = 1f;

	public static int kolStarOnLevel = 10;

	public static float timeGame = 420f;

	public static int tekKolDead = 0;

	public static int tekKolKill = 0;

	public static int maxKolPoints = 0;

	public static int speedCarForIgnoreEnemy = 20;

	public static int speedCarForHighDemageEnemy = 60;

	public static string keyForBuySkin = "keyForBuySkin_";

	public static int tekNomSkin = 0;

	public static string tekName = string.Empty;

	public static int tekKolCoins = 0;

	public static bool soundEnabled;

	public static bool musicEnabled;

	public static float jetpackFuel;

	public static string keyJetpackBought = "Jetpack";

	public static Texture2D tekSkin;

	public static bool testBuilding = false;

	public static bool offlineMode = false;

	public static int maxKolVragov = 14;

	public static int tekKolVragov = 0;

	public AudioClip soundButton;

	private void RestoreJetpack()
	{
		if (Load.LoadBool(keyJetpackBought))
		{
			Save.SaveBool(keyJetpackBought, false);
		}
	}

	private void Awake()
	{
		thisScript = this;
		int num = Load.LoadInt(keyVersionGame);
		if (num < currentVersionGame)
		{
			if (num <= 3)
			{
				Storager.createInt(0, keyKeychainGunSniper);
			}
			if (num <= 4)
			{
				Storager.createInt(0, keyKeychainArmor);
			}
			if (num <= 5)
			{
				RestoreJetpack();
			}
			if (num <= 6)
			{
				Storager.createBool(false, keyKeychainRevolver);
				Storager.createBool(false, keyKeychainChainsaw);
				Storager.createBool(false, keyKeychainKnife);
			}
			if (num <= 7)
			{
				Storager.createBool(false, keyKeychainGlock);
			}
			if (num <= 10)
			{
				Storager.createBool(false, keyKeychainSpas12);
				Storager.createBool(false, keyKeychainBrass);
			}
			Save.SaveInt(keyVersionGame, currentVersionGame);
		}
		if (!Load.LoadBool("firstLoad"))
		{
			Storager.createInt(100, keyKeychainKolCoins);
			Storager.createInt(0, keyKeychainGun1);
			Storager.createInt(0, keyKeychainGun2);
			Storager.createInt(0, keyKeychainGun3);
			Storager.createInt(0, keyKeychainGunBat);
			Storager.createInt(0, keyKeychainGunSniper);
			Save.SaveInt(keyShooseSkin, 0);
			Save.SaveString(keyName, "Player");
			Save.SaveBool(keySound, true);
			Save.SaveBool(keyMusic, true);
			Save.SaveBool(keyOfflineMode, false);
			Save.SaveInt(keyKolDead, 0);
			Save.SaveInt(keyKolKill, 0);
			Save.SaveInt(keyKolMaxPoints, 0);
			Save.SaveBool("firstLoad", true);
		}
		if (!Load.LoadBool("secondLoad"))
		{
			Storager.createInt(0, keyKeychainGunMinigun);
			Save.SaveBool("secondLoad", true);
		}
		bool flag = Load.LoadBool("thirdLoad");
		if (!flag)
		{
			Save.SaveBool("isLearned", false);
			Save.SaveBool(keyJetpackBought, true);
			Save.SaveFloat("fuel", 100f);
			Save.SaveBool("thirdLoad", true);
		}
		bool flag2 = Load.LoadBool("Load4");
		if (!flag)
		{
			Save.SaveInt(keyShooseSkin, 0);
			Save.SaveBool("Load4", true);
		}
		loadPrefsFromKeychain();
		Save.SaveInt(keyKolCoins, CompilationSettings.StartingCash);
		updateArmorEnabled();
		for (int i = 0; i < arrKeyPostGuns.Length; i++)
		{
			Save.SaveBool(arrKeyPostGuns[i], true);
			Save.SaveBool(keyEquipGun + arrKeyPostGuns[i], true);
		}
		soundEnabled = Load.LoadBool(keySound);
		musicEnabled = Load.LoadBool(keyMusic);
		tekNomSkin = Load.LoadInt(keyShooseSkin);
		tekName = Load.LoadString(keyName);
		tekKolCoins = Load.LoadInt(keyKolCoins);
		tekKolDead = Load.LoadInt(keyKolDead);
		tekKolKill = Load.LoadInt(keyKolKill);
		maxKolPoints = Load.LoadInt(keyKolMaxPoints);
		offlineMode = Load.LoadBool(keyOfflineMode);
		tekSkin = getTekSkin();
		isLearned = Load.LoadBool("isLearned");
	}

	public static void updateOfflineMode(bool val)
	{
		offlineMode = val;
		Save.SaveBool(keyOfflineMode, val);
	}

	public static void updateKolCoins(int coins)
	{
		tekKolCoins = coins;
		Save.SaveInt(keyKolCoins, coins);
	}

	public static void setNomSkin(int nom)
	{
		tekNomSkin = nom;
		Save.SaveInt(keyShooseSkin, tekNomSkin);
		tekSkin = getTekSkin();
	}

	public static void setNewName(string name)
	{
		tekName = name;
		Save.SaveString(keyName, tekName);
	}

	public static void playSoundButton()
	{
		if (soundEnabled)
		{
			NGUITools.PlaySound(thisScript.soundButton);
		}
	}

	public static void setMusicEnbled(bool enabled)
	{
		musicEnabled = enabled;
		Save.SaveBool(keyMusic, musicEnabled);
	}

	public static void setSoundEnbled(bool enabled)
	{
		soundEnabled = enabled;
		Save.SaveBool(keySound, soundEnabled);
	}

	public static void updateKolDead(int newKol)
	{
		tekKolDead = newKol;
		Save.SaveInt(keyKolDead, tekKolDead);
	}

	public static void updateKolKill(int newKol)
	{
		tekKolKill = newKol;
		Save.SaveInt(keyKolKill, tekKolKill);
	}

	public static void updateKolMaxPoints(int tekKolPoints)
	{
		if (tekKolPoints > maxKolPoints)
		{
			maxKolPoints = tekKolPoints;
			Save.SaveInt(keyKolMaxPoints, maxKolPoints);
		}
	}

	public static bool isWeaponBought(string namePrefab)
	{
		return Load.LoadBool(namePrefab);
	}

	public static Texture2D getTekSkin()
	{
		string text = "Skins/shablon_";
		text = ((tekNomSkin > 9) ? (text + tekNomSkin) : (text + "0" + tekNomSkin));
		return Resources.Load<Texture2D>(text);
	}

	public static Texture2D getTekSkin(int nomSkin)
	{
		string text = "Skins/shablon_";
		text = ((nomSkin > 9) ? (text + nomSkin) : (text + "0" + nomSkin));
		return Resources.Load<Texture2D>(text);
	}

	public static void addGrenade(int count)
	{
		if (!Load.LoadBool(keyBulletsSaved + keyBuyGunGrenade))
		{
			Save.SaveBool(keyBulletsSaved + keyBuyGunGrenade, true);
			Save.SaveInt(keyCountBullets + keyBuyGunGrenade, 0);
		}
		Save.SaveInt(keyCountBullets + keyBuyGunGrenade, Load.LoadInt(keyCountBullets + keyBuyGunGrenade) + count);
		Debug.Log("addGrenade = " + Load.LoadInt(keyCountBullets + keyBuyGunGrenade));
	}

	public static void addMolotov(int count)
	{
		if (!Load.LoadBool(keyBulletsSaved + keyBuyGunMolotov))
		{
			Save.SaveBool(keyBulletsSaved + keyBuyGunMolotov, true);
			Save.SaveInt(keyCountBullets + keyBuyGunMolotov, 0);
		}
		Save.SaveInt(keyCountBullets + keyBuyGunMolotov, Load.LoadInt(keyCountBullets + keyBuyGunMolotov) + count);
		Debug.Log("addMolotov = " + Load.LoadInt(keyCountBullets + keyBuyGunMolotov));
	}

	public static void addC4(int count)
	{
		if (!Load.LoadBool(keyBulletsSaved + keyBuyGunMolotov))
		{
			Save.SaveBool(keyBulletsSaved + keyBuyGunC4, true);
			Save.SaveInt(keyCountBullets + keyBuyGunC4, 0);
		}
		Save.SaveInt(keyCountBullets + keyBuyGunC4, Load.LoadInt(keyCountBullets + keyBuyGunC4) + count);
		Debug.Log("addC4 = " + Load.LoadInt(keyCountBullets + keyBuyGunC4));
	}

	public static void updateArmorEnabled()
	{
		if (Load.LoadInt(keyArmor) > 0)
		{
			Save.SaveBool(keyArmorEnabled, true);
		}
		else
		{
			Save.SaveBool(keyArmorEnabled, false);
		}
	}

	public static void loadPrefsFromKeychain()
	{
		Save.SaveInt(keyKolCoins, Storager.getInt(keyKeychainKolCoins));
		Save.SaveBool(keyBuyGunUzi, Storager.getBool(keyKeychainGun1));
		Save.SaveBool(keyBuyGunShotgun, Storager.getBool(keyKeychainGun2));
		Save.SaveBool(keyBuyGunAk47, Storager.getBool(keyKeychainGun3));
		Save.SaveBool(keyBuyGunBat, Storager.getBool(keyKeychainGunBat));
		Save.SaveBool(keyBuyGunMinigun, Storager.getBool(keyKeychainGunMinigun));
		Save.SaveBool(keyBuyGunSniperRifle, Storager.getBool(keyKeychainGunSniper));
		Save.SaveBool(keyBuyChainsaw, Storager.getBool(keyKeychainChainsaw));
		Save.SaveBool(keyBuyRevolver, Storager.getBool(keyKeychainRevolver));
		Save.SaveBool(keyBuyKnife, Storager.getBool(keyKeychainKnife));
		Save.SaveBool(keyBuyGlock, Storager.getBool(keyKeychainGlock));
		Save.SaveInt(keyArmor, Storager.getInt(keyKeychainArmor));
	}

	public static void updateKeychainCoins()
	{
		Storager.setInt(Load.LoadInt(keyKolCoins), keyKeychainKolCoins);
	}

	public static void updateKeychainGun1()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunUzi), keyKeychainGun1);
	}

	public static void updateKeychainGun2()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunShotgun), keyKeychainGun2);
	}

	public static void updateKeychainGun3()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunAk47), keyKeychainGun3);
	}

	public static void updateKeychainGunBat()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunBat), keyKeychainGunBat);
	}

	public static void updateKeychainGunMinigun()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunMinigun), keyKeychainGunMinigun);
	}

	public static void updateKeychainSniperRifle()
	{
		Storager.setBool(Load.LoadBool(keyBuyGunSniperRifle), keyKeychainGunSniper);
	}

	public static void updateKeychainJetpack()
	{
		Storager.setBool(Load.LoadBool(keyJetpackBought), keyKeychainJetpack);
	}

	public static void updateKeychainArmor()
	{
		Storager.setInt(Load.LoadInt(keyArmor), keyKeychainArmor);
		updateArmorEnabled();
	}

	public static void updateKeychainRevolver()
	{
		Storager.setBool(Load.LoadBool(keyBuyRevolver), keyKeychainRevolver);
	}

	public static void updateKeychainKnife()
	{
		Storager.setBool(Load.LoadBool(keyBuyKnife), keyKeychainKnife);
	}

	public static void updateKeychainChainsaw()
	{
		Storager.setBool(Load.LoadBool(keyBuyChainsaw), keyKeychainChainsaw);
	}

	public static void updateKeychainGlock()
	{
		Storager.setBool(Load.LoadBool(keyBuyGlock), keyKeychainGlock);
	}

	public static void updateKeychainSpas12()
	{
		Storager.setBool(Load.LoadBool(keyBuySpas12), keyKeychainSpas12);
	}

	public static void updateKeychainBrass()
	{
		Debug.Log("keyBuySpas12 prefs=" + Load.LoadBool(keyBuyBrass));
		Storager.setBool(Load.LoadBool(keyBuyBrass), keyKeychainBrass);
		Debug.Log("keyKeychainSpas12 =" + Storager.getBool(keyKeychainBrass));
	}

	public static void updateAllKeychain()
	{
		updateKeychainCoins();
		updateKeychainGun1();
		updateKeychainGun2();
		updateKeychainGun3();
		updateKeychainGunBat();
		updateKeychainGunMinigun();
		updateKeychainSniperRifle();
		updateKeychainArmor();
		updateKeychainChainsaw();
		updateKeychainKnife();
		updateKeychainRevolver();
		updateKeychainGlock();
		updateKeychainSpas12();
		updateKeychainBrass();
	}
}
