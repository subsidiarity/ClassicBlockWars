using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : EntityBehavior
{
	public GameObject myCollidePoint;

	public GameObject playerMesh;

	private GameObject buttonEnableCar;

	private GameObject nearestCar;

	public GameObject prefabPlayer;

	private GameObject aimObject;

	private Transform aimer;

	[HideInInspector]
	public GameObject enemyCollider;

	[HideInInspector]
	public EnemyWatcher enemyWatcher;

	[HideInInspector]
	public GameObject currentTarget;

	public GameObject deadHands;

	private CharacterController cController;

	public ThirdPersonController tController;

	private int idMachine = -9999;

	public WeaponManager weaponManager;

	public bool isImmortal;

	public AudioClip[] arrDamageScream;

	private AudioClip curAudioDamage;

	private float timeLastPlayDamage;

	private float timeDelayBetweenPlayDamage = 0.3f;

	public GameObject playerNameLabel;

	[HideInInspector]
	public NameLabel scriptPlayerName;

	public GameObject bloodParticle;

	public GameObject wallParticle;

	public GameObject hole;

	public GameObject objArmor;

	public LayerMask collisionLayer;

	[HideInInspector]
	public string nick = "Unname";

	public int health = 100;

	public int armor;

	public int points;

	public int indPlayer = -1;

	public int ownerIDPlayer = -1;

	public bool isShooting;

	public bool inCar;

	public bool inHelic;

	public bool damageFromCar;

	public CarBehavior currentCar;

	public HelicopterBehavior currentHelic;

	public GameObject[] arrGameObjInPlayer;

	[HideInInspector]
	public PlayerAreaBehavior areaBehavior;

	private carDamageEnemy carDemage;

	[HideInInspector]
	public CapsuleCollider colliderEnemyPlayer;

	private bool highCarDemage;

	public lerpTransformPhoton lerpScript;

	public BoxCollider colliderForHelicopter;

	public List<weaponGrenade> listDetonatorGrenade = new List<weaponGrenade>();

	private NJGMapItem iconMiniMap;

	private int predSpeedCarForIgnoreEnemy;

	private bool PlayerMine
	{
		get
		{
			return (settings.offlineMode || (!settings.offlineMode && base.photonView.isMine));
		}
	}

	public int Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if (settings.offlineMode || base.photonView.isMine)
			{
				GameController.thisScript.healthValue = health;
			}
		}
	}

	public int Armor
	{
		get
		{
			return armor;
		}
		set
		{
			armor = value;
			if (settings.offlineMode || base.photonView.isMine)
			{
				Save.SaveInt(settings.keyArmor, armor);
				if (armor <= 0)
				{
					settings.updateKeychainArmor();
				}
				GameController.thisScript.armorValue = armor;
				settings.updateArmorEnabled();
			}
			if (settings.offlineMode)
			{
				if (needChangeActivArmor())
				{
					setActivObjArmor(Load.LoadBool(settings.keyArmorEnabled));
				}
			}
			else if (base.photonView.isMine && needChangeActivArmor())
			{
				base.photonView.RPC("setActivObjArmor", PhotonTargets.AllBuffered, Load.LoadBool(settings.keyArmorEnabled));
			}
		}
	}

	public int Points
	{
		get
		{
			return points;
		}
		set
		{
			points = value;
			if (points < 0)
			{
				points = 0;
			}
			if (PlayerMine)
			{
				GameController.thisScript.lbPoints.text = string.Empty + points;
			}
			GameController.thisScript.obnovitSpisokButForPlayers();
		}
	}

	public string Nick
	{
		get
		{
			return nick;
		}
		set
		{
			nick = value;
		}
	}

	private void Awake()
	{
		colliderEnemyPlayer = GetComponent<CapsuleCollider>();
		lerpScript = GetComponent<lerpTransformPhoton>();
		cController = GetComponent<CharacterController>();
		weaponManager = GetComponent<WeaponManager>();
		areaBehavior = GetComponent<PlayerAreaBehavior>();
		iconMiniMap = GetComponent<NJGMapItem>();
	}

	private void Start()
	{
		iconMiniMap.type = 1;
		timeLastPlayDamage = Time.timeSinceLevelLoad;
		curAudioDamage = arrDamageScream[Random.Range(0, arrDamageScream.Length)];

		if (settings.offlineMode)
		{
			initOfflinePlayer();
			InitSticky();
			return;
		}
		else
		{
			indPlayer = base.photonView.viewID;
			ownerIDPlayer = base.photonView.owner.ID;
		}

		initPlayerObject();

		if (base.photonView.isMine)
		{
			InitSticky();
			base.photonView.RPC("setName", PhotonTargets.AllBuffered, settings.tekName);
		}
		else
		{
			iconMiniMap.type = 2;
		}
	}

	private void InitSticky()
	{
		// TODO: This should be enabling and disabling the script too.
		GameObject.Find("FeetTrigger").GetComponent<BoxCollider>().enabled = true;
		Debug.Log("Started trigger");
	}

	private void initOfflinePlayer()
	{
		initSkin(settings.tekNomSkin);
		cController = GetComponent<CharacterController>();
		prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
		enemyCollider = base.transform.Find("enemyCollider").gameObject;
		enemyWatcher = enemyCollider.GetComponent<EnemyWatcher>();
		collisionLayer = enemyWatcher.GetComponent<EnemyWatcher>().collisionLayer;
		tController = GetComponent<ThirdPersonController>();
		deadHands = weaponManager.weaponHolder.transform.Find("deadHands").gameObject;
		aimObject = enemyCollider.transform.Find("AimLabel").gameObject;
		deadHands.SetActive(false);
		GetComponent<NavMeshObstacle>().enabled = true;
		tController.jetpack.Activate();
		colliderEnemyPlayer.enabled = false;
		initIntefaceBindings();
		Health = 100;
		Armor = Load.LoadInt(settings.keyArmor);
	}

	private void Update()
	{
		updateTarget();
		updateWeapon();

		KeyboardManager.Update(GameController.thisScript.playerScript);

		if (CompilationSettings.UseMouseShoot)
		{
			isShooting = Input.GetMouseButton(0);
		}
	}

	public void TryGetIntoOrOutVehicle()
	{
		if (!inCar && !inHelic)
		{
			if (areaBehavior.HelicBlijeCar())
			{
				GetInHelicopter();
			}
			else
			{
				GetInCar();
			}
			return;
		}

		if (inCar)
		{
			GetOutOfCar();
		}

		if (inHelic)
		{
			GetOutOfHelic();
		}
	}

	private void updateWeapon()
	{
		if (isShooting && PlayerMine)
		{
			weaponManager.currentWeaponScript.shoot();
		}
	}

	[RPC]
	private void broadcastShoot()
	{
		Debug.Log("broadcastShoot");
		if (weaponManager == null)
		{
			weaponManager = GetComponent<WeaponManager>();
		}
		weaponManager.currentWeaponScript.broadcastShoot();
	}

	[RPC]
	private void broadcastReload()
	{
		if (weaponManager == null)
		{
			weaponManager = GetComponent<WeaponManager>();
		}
		weaponManager.currentWeaponScript.broadcastReload();
	}

	private void initIntefaceBindings()
	{
		RPG_Camera component = Camera.main.gameObject.GetComponent<RPG_Camera>();
		component.cameraPivot = base.transform.Find("CameraPoint");
	}

	private void updateTarget()
	{
		if (enemyWatcher != null && weaponManager != null && weaponManager.currentWeaponScript != null && !weaponManager.currentWeaponScript.areaDamage)
		{
			currentTarget = enemyWatcher.targetPlayer;
		}
	}

	[RPC]
	private void initSkin(int skinNumber)
	{
		if (prefabPlayer == null)
		{
			prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
		}
		if (deadHands == null)
		{
			deadHands = weaponManager.weaponHolder.transform.Find("deadHands").gameObject;
		}
		switchTextureOn(prefabPlayer, settings.getTekSkin(skinNumber));
		switchTextureOn(deadHands, settings.getTekSkin(skinNumber));
	}

	private void initPlayerObject()
	{
		if (base.photonView.isMine)
		{
			base.photonView.RPC("initSkin", PhotonTargets.AllBuffered, settings.tekNomSkin);
			prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
			enemyCollider = base.transform.Find("enemyCollider").gameObject;
			enemyWatcher = enemyCollider.GetComponent<EnemyWatcher>();
			collisionLayer = enemyWatcher.GetComponent<EnemyWatcher>().collisionLayer;
			tController = GetComponent<ThirdPersonController>();
			deadHands = weaponManager.weaponHolder.transform.FindChild("deadHands").gameObject;
			aimObject = enemyCollider.transform.Find("AimLabel").gameObject;
			deadHands.SetActive(false);
			colliderEnemyPlayer.enabled = false;
			initIntefaceBindings();
			Health = 100;
			Armor = Load.LoadInt(settings.keyArmor);
			GetComponent<NavMeshObstacle>().enabled = false;
			base.photonView.RPC("ActivateJetpack", PhotonTargets.All);
			return;
		}
		aimer = base.transform.Find("Aimer");
		if (aimer != null)
		{
			Object.Destroy(aimer.gameObject);
		}
		CharacterController component = GetComponent<CharacterController>();
		GameObject gameObject = base.transform.Find("enemyCollider").gameObject;
		tController = GetComponent<ThirdPersonController>();
		gameObject.GetComponent<EnemyWatcher>().enabled = false;
		component.enabled = false;
		if (deadHands == null)
		{
			deadHands = base.transform.Find("deadHands").gameObject;
		}
		switchTextureOn(deadHands, settings.getTekSkin(settings.tekNomSkin));
		deadHands.SetActive(false);
		GameObject gameObject2 = (GameObject)Object.Instantiate(playerNameLabel, base.transform.position + new Vector3(0f, 2f, 0f), base.transform.rotation);
		scriptPlayerName = gameObject2.GetComponent<NameLabel>();
		scriptPlayerName.target = base.gameObject.transform;
	}

	public void addNameLabel(string name)
	{
		GameObject gameObject = (GameObject)Object.Instantiate(playerNameLabel, base.transform.position + new Vector3(0f, 2f, 0f), base.transform.rotation);
		scriptPlayerName = gameObject.GetComponent<NameLabel>();
		scriptPlayerName.target = base.transform;
	}

	public int healthAfterDamage(int damage)
	{
		int num = health;
		int num2 = armor - damage;
		if (num2 < 0)
		{
			num += num2;
		}
		return num;
	}

	public void updateValueAfterDamage(int damage)
	{
		if (settings.offlineMode || base.photonView.isMine)
		{
			int num = Armor - damage;
			if (num < 0 && Armor > 0)
			{
				Armor = 0;
			}
			if (num < 0)
			{
				Health += num;
			}
			else
			{
				Armor -= damage;
			}
		}
	}

	[RPC]
	public void getDamage(int damage, int idKiller)
	{
		if (settings.soundEnabled && Time.timeSinceLevelLoad - timeLastPlayDamage > timeDelayBetweenPlayDamage)
		{
			timeLastPlayDamage = Time.timeSinceLevelLoad;
			if (curAudioDamage != null)
			{
				AudioSource.PlayClipAtPoint(curAudioDamage, base.transform.position);
			}
		}
		if (!base.photonView.isMine || isDead || isImmortal)
		{
			return;
		}
		if (healthAfterDamage(damage) <= 0)
		{
			Health = 0;
			Armor = 0;
			isImmortal = true;
			if (base.photonView.isMine)
			{
				base.photonView.RPC("dead", PhotonTargets.All, idKiller);
			}
		}
		else
		{
			updateValueAfterDamage(damage);
			GameController.thisScript.animDamage();
		}
	}

	public override void getDamage(int damage)
	{
		if (isDead)
		{
			return;
		}
		if (settings.soundEnabled && Time.timeSinceLevelLoad - timeLastPlayDamage > timeDelayBetweenPlayDamage)
		{
			timeLastPlayDamage = Time.timeSinceLevelLoad;
			if (curAudioDamage != null)
			{
				AudioSource.PlayClipAtPoint(curAudioDamage, base.transform.position);
			}
		}
		if (settings.offlineMode || base.photonView.isMine)
		{
			if (healthAfterDamage(damage) <= 0)
			{
				Health = 0;
				Armor = 0;
				dead();
			}
			else
			{
				updateValueAfterDamage(damage);
				GameController.thisScript.animDamage();
			}
		}
	}

	public void switchTextureOn(GameObject player, Texture newTex)
	{
		Renderer[] componentsInChildren = player.GetComponentsInChildren<Renderer>();
		if (componentsInChildren.Length == 0)
		{
			return;
		}
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (renderer.gameObject.tag != "Weapon")
			{
				renderer.material.mainTexture = newTex;
			}
		}
	}

	[RPC]
	public void setName(string name)
	{
		if (!settings.offlineMode)
		{
			name = FilterBadWorld.FilterString(name);
			nick = name;
			GameController.thisScript.addPlayerToList(this);
			if (!base.photonView.isMine)
			{
				GameController.thisScript.addMessageToList(nick + "joined the game.");
			}
		}
	}

	[RPC]
	public void dead(int idKiller)
	{
		if (settings.offlineMode || isDead)
		{
			return;
		}
		isDead = true;
		Points -= 3;
		if (Points < 0)
		{
			Points = 0;
		}
		PhotonView photonView = PhotonView.Find(idKiller);
		if (!base.photonView.isMine)
		{
			if (idKiller != idMachine && idKiller != base.photonView.viewID && photonView != null)
			{
				photonView.gameObject.GetComponent<PlayerBehavior>().addCoins(idKiller);
			}
		}
		else
		{
			settings.updateKolDead(settings.tekKolDead + 1);
		}
		if (prefabPlayer == null)
		{
			prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
		}
		if (deadHands == null)
		{
			deadHands = base.transform.Find("deadHands").gameObject;
		}
		deadHands.SetActive(true);
		if (weaponManager == null)
		{
			weaponManager = GetComponent<WeaponManager>();
		}
		weaponManager.CurrentWeapon.SetActive(false);
		if (damageFromCar)
		{
			damageFromCar = false;
			Invoke("animDeadFromCarAfterDamage", 0.7f);
		}
		else
		{
			prefabPlayer.animation.Play("Dead");
			deadHands.transform.GetChild(0).animation.Play("Dead");
		}
		if (idKiller != idMachine && photonView != null)
		{
			GameController.thisScript.addMessageToList(string.Empty + photonView.gameObject.GetComponent<PlayerBehavior>().nick + " killed " + nick);
		}
		Invoke("reset", 1f);
	}

	private void animDeadFromCarAfterDamage()
	{
		prefabPlayer.animation.Play("Dead_Lay");
		deadHands.transform.GetChild(0).animation.Play("Dead_Lay");
	}

	public override void dead()
	{
		if (!isDead)
		{
			isDead = true;
			settings.updateKolDead(settings.tekKolDead + 1);
			if (prefabPlayer == null)
			{
				prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
			}
			if (deadHands == null)
			{
				deadHands = base.transform.Find("deadHands").gameObject;
			}
			deadHands.SetActive(true);
			if (weaponManager == null)
			{
				weaponManager = GetComponent<WeaponManager>();
			}
			weaponManager.CurrentWeapon.SetActive(false);
			prefabPlayer.animation.Play("Dead");
			deadHands.transform.GetChild(0).animation.Play("Dead");
			GameController.thisScript.offlineKolDied++;
			Invoke("reset", 1f);
		}
	}

	public void addCoins(int killerID)
	{
		if (base.photonView.viewID == killerID && base.photonView.isMine)
		{
			Points += 10;
			settings.updateKolCoins(settings.tekKolCoins + 10);
			shopController.thisScript.lbKolCoins.text = string.Empty + settings.tekKolCoins;
			settings.updateKeychainCoins();
			base.photonView.RPC("updatePoints", PhotonTargets.OthersBuffered, Points);
			settings.updateKolKill(settings.tekKolKill + 1);
			settings.updateKolMaxPoints(Points);
		}
	}

	[RPC]
	public void updatePoints(int p)
	{
		Points = p;
	}

	public void reset()
	{
		reset(controllerConnectGame.thisScript.getRandomPointNearPlayer().position);
	}

	public void reset(Vector3 position)
	{
		CancelInvoke("reset");
		deadHands.SetActive(false);
		damageFromCar = false;
		inCar = false;
		tController.isPlayAnimation = true;
		base.transform.eulerAngles = Vector3.zero;
		tController.verticalSpeed = 0f;
		tController.moveSpeed = 0f;
		tController.inAirVelocity = Vector3.zero;
		tController.gravity = 20f;
		weaponManager.CurrentWeapon.SetActive(true);
		if (PlayerMine)
		{
			if (GameController.thisScript.carScript != null && inCar)
			{
				GameController.thisScript.carScript.exitFromCarOnline();
			}
			base.transform.position = position;
			prefabPlayer.animation.Play("Idle");
			health = 100;
			GameController.thisScript.lbHealth.text = health.ToString();
		}
		else
		{
			colliderEnemyPlayer.enabled = true;
			prefabPlayer.animation.Play("Idle");
		}
		isDead = false;
		Invoke("ressurect", 3f);
	}

	private void ressurect()
	{
		isImmortal = false;
	}

	public void GetInCar()
	{
		if (!inCar && !isDead && !areaBehavior.nearCar.playerInCar)
		{
			CancelInvoke("switchOffRigidBodyCar");
			inCar = true;
			cController.enabled = false;
			Debug.Log("GetInCar");
			GameController.thisScript.switchInterface(inCar);
			areaBehavior.nearCar.GetComponent<NewDriving>().enabled = true;
			GameController.thisScript.newDrivingScript = areaBehavior.nearCar.GetComponent<NewDriving>();
			GameController.thisScript.myCar = areaBehavior.nearCar.gameObject;
			currentCar = areaBehavior.nearCar.GetComponent<CarBehavior>();
			carDemage = GameController.thisScript.myCar.AddComponent<carDamageEnemy>();
			GameController.thisScript.curCarRigidBody = areaBehavior.nearCar.GetComponent<Rigidbody>();
			GameController.thisScript.carScript = areaBehavior.nearCar;
			Debug.Log("carScript=" + GameController.thisScript.carScript);
			GameController.thisScript.carScript.clearAllPrepiadstvia();
			GameController.thisScript.carScript.newDrivingScript.isGo = false;
			GameController.thisScript.carScript.newDrivingScript.isBack = false;
			GameController.thisScript.carScript.enabled = true;
			GameController.thisScript.carScript.setIconMiniMapCarWithPlayer(true);
			GameController.thisScript.carScript.soundCarEnabled(true);
			GameController.thisScript.carScript.playerInCar = true;
			GameController.thisScript.carScript.objPlayerInCar = base.gameObject;
			GameController.thisScript.carScript.scriptPlayerInCar = this;
			GameController.thisScript.carScript.CancelResetCarWithDelay();
			tController.stamina = 1f;
			GameController.thisScript.interfaceForCarWithWeapon.SetActive(GameController.thisScript.carScript.carWithWeapon);
			GameController.thisScript.camScriptCar.target = areaBehavior.nearCar.cameraPoint;
			GameController.thisScript.curCarRigidBody.isKinematic = false;
			base.transform.parent = areaBehavior.nearCar.transform;
			base.gameObject.transform.localRotation = Quaternion.identity;
			if (settings.offlineMode)
			{
				base.gameObject.SetActive(false);
			}
			areaBehavior.listNearCar.Remove(GameController.thisScript.carScript);
			if (!settings.offlineMode)
			{
				GameController.thisScript.carScript.sitInCarOnline(base.photonView.viewID);
				SetActiveComponentBesidesControl(false);
			}
			isShooting = false;
		}
	}

	public void SetActiveComponentBesidesControl(bool Val)
	{
		tController.enabled = Val;
		cController.enabled = Val;
		base.enabled = Val;
		weaponManager.enabled = Val;
		GetComponent<bonuseManager>().enabled = Val;
		areaBehavior.enabled = Val;
		GetComponent<LODGroup>().enabled = Val;
		iconMiniMap.enabled = Val;
		GameObject[] array = arrGameObjInPlayer;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(Val);
			}
		}
	}

	public void SetVisiblePlayerBesidesControl(bool Val)
	{
		GetComponent<LODGroup>().enabled = Val;
		GameObject[] array = arrGameObjInPlayer;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(Val);
			}
		}
	}

	public void GetOutOfCar()
	{
		if (!inCar)
		{
			return;
		}
		inCar = false;
		Object.Destroy(carDemage);
		GameController.thisScript.switchInterface(inCar);
		if (areaBehavior.nearCar != null)
		{
			areaBehavior.nearCar.GetComponent<NewDriving>().enabled = false;
		}
		GameController.thisScript.newDrivingScript = null;
		if (GameController.thisScript.carScript != null)
		{
			GameController.thisScript.carScript.soundCarEnabled(false);
			GameController.thisScript.carScript.clearAllPrepiadstvia();
			GameController.thisScript.carScript.playerInCar = false;
			GameController.thisScript.carScript.vklColliderEnemy(true);
			base.gameObject.transform.position = GameController.thisScript.carScript.pointForExit;
			GameController.thisScript.carScript.setIconMiniMapCarWithPlayer(false);
			GameController.thisScript.carScript.newDrivingScript.FullStopCar();
			GameController.thisScript.carScript.newDrivingScript.isGo = false;
			GameController.thisScript.carScript.newDrivingScript.isBack = false;
			GameController.thisScript.carScript.ResetCarWithDelay();
			GameController.thisScript.carScript.enabled = false;
		}
		if (settings.offlineMode)
		{
			cController.enabled = true;
			base.gameObject.SetActive(true);
		}
		GameController.thisScript.myCar = null;
		currentCar = null;
		GameController.thisScript.camScriptCar.target = null;
		Invoke("switchOffRigidBodyCar", 3f);
		base.transform.parent = null;
		if (!settings.offlineMode)
		{
			SetActiveComponentBesidesControl(true);
			if (GameController.thisScript.carScript != null)
			{
				GameController.thisScript.carScript.leftCarOnline(base.photonView.viewID);
			}
		}
	}

	private void switchOffRigidBodyCar()
	{
		if (GameController.thisScript.curCarRigidBody != null)
		{
			GameController.thisScript.curCarRigidBody.isKinematic = true;
		}
	}

	public void GetInHelicopter()
	{
		if (!inCar && !isDead && !areaBehavior.nearHelicopter.playerInHelic)
		{
			Debug.Log("GetInHelicopter");
			inHelic = true;
			myCollidePoint.SetActive(false);
			colliderForHelicopter.enabled = true;
			tController.jetpack.modeOfADonkey = true;
			GameController.thisScript.CheckInterfaceHelicopter(inHelic);
			GameController.thisScript.helicopterScript = areaBehavior.nearHelicopter;
			SetVisiblePlayerBesidesControl(false);
			if (GameController.thisScript.helicopterScript != null)
			{
				currentHelic = areaBehavior.nearHelicopter.GetComponent<HelicopterBehavior>();
				currentHelic.animationHelicUp();
				base.gameObject.transform.rotation = GameController.thisScript.helicopterScript.transform.rotation;
				base.gameObject.transform.position = GameController.thisScript.helicopterScript.transform.position;
				GameController.thisScript.helicopterScript.clearAllPrepiadstvia();
				GameController.thisScript.helicopterScript.allPointsForExit.SetActive(true);
				GameController.thisScript.camScriptCar.CheckSettingsHelicopter(true);
				GameController.thisScript.camScriptCar.target = areaBehavior.nearHelicopter.cameraPoint;
				GameController.thisScript.helicopterScript.setIconMiniMapCarWithPlayer(true);
				iconMiniMap.enabled = false;
				GameController.thisScript.myHelic = GameController.thisScript.helicopterScript.gameObject;
				GameController.thisScript.helicopterScript.enabled = true;
				GameController.thisScript.helicopterScript.playerInHelic = true;
				GameController.thisScript.helicopterScript.objPlayerInHelic = base.gameObject;
				GameController.thisScript.helicopterScript.scriptPlayerInHelic = this;
				GameController.thisScript.helicopterScript.GetComponent<AudioSource>().Play();
				GameController.thisScript.helicopterScript.colliderHelicopter.enabled = false;
				areaBehavior.listNearHelicopter.Remove(GameController.thisScript.helicopterScript);
			}
			if (!settings.offlineMode)
			{
				GameController.thisScript.helicopterScript.sitInHelicOnline(base.photonView.viewID);
			}

			isShooting = false;
		}
	}

	public void GetOutOfHelic()
	{
		if (inHelic)
		{
			Debug.Log("GetOutOfHelic");
			inHelic = false;
			myCollidePoint.SetActive(true);
			colliderForHelicopter.enabled = false;
			tController.jetpack.modeOfADonkey = false;
			SetVisiblePlayerBesidesControl(true);
			if (GameController.thisScript.helicopterScript != null)
			{
				GameController.thisScript.helicopterScript.enabled = false;
				GameController.thisScript.helicopterScript.clearAllPrepiadstvia();
				GameController.thisScript.helicopterScript.playerInHelic = false;
				GameController.thisScript.helicopterScript.allPointsForExit.SetActive(false);
				GameController.thisScript.myHelic = null;
				base.gameObject.transform.position = GameController.thisScript.helicopterScript.pointForExit;
				GameController.thisScript.helicopterScript.GetComponent<AudioSource>().Stop();
				GameController.thisScript.camScriptCar.CheckSettingsHelicopter(false);
				GameController.thisScript.helicopterScript.setIconMiniMapCarWithPlayer(false);
				GameController.thisScript.helicopterScript.colliderHelicopter.enabled = true;
				GameController.thisScript.helicopterScript.animationHelicDown();
			}
			iconMiniMap.enabled = true;
			currentHelic = null;
			if (!settings.offlineMode && GameController.thisScript.helicopterScript != null)
			{
				GameController.thisScript.helicopterScript.leftHelicOnline(base.photonView.viewID);
			}
			GameController.thisScript.CheckInterfaceHelicopter(inHelic);
		}
	}

	public void reload()
	{
		if (settings.offlineMode)
		{
			broadcastReload();
		}
		else
		{
			base.photonView.RPC("broadcastReload", PhotonTargets.Others);
		}
	}

	[RPC]
	public void ActivateJetpack()
	{
		if (tController != null && tController.jetpack != null)
		{
			tController.jetpack.Activate();
		}
	}

	[RPC]
	public void EnableJetpackParticle()
	{
		if (tController != null && tController.jetpack != null)
		{
			tController.jetpack.reactiveFire.SetActive(true);
		}
	}

	[RPC]
	public void PlayJetpackSound()
	{
		if (!base.photonView.isMine)
		{
			GetComponent<AudioSource>().enabled = true;
		}
	}

	[RPC]
	public void StopJetpackSound()
	{
		if (!settings.offlineMode)
		{
			if (!base.photonView.isMine)
			{
				GetComponent<AudioSource>().enabled = false;
			}
		}
		else
		{
			GetComponent<AudioSource>().enabled = false;
		}
	}

	[RPC]
	public void DisableJetpackParticle()
	{
		if (tController != null && tController.jetpack != null)
		{
			tController.jetpack.reactiveFire.SetActive(false);
		}
	}

	[RPC]
	public void HoleRPC(bool _isBloodParticle, Vector3 _pos, Quaternion _rot)
	{
		if (_isBloodParticle)
		{
			Object.Instantiate(bloodParticle, _pos, _rot);
			return;
		}
		Object.Instantiate(hole, _pos, _rot);
		Object.Instantiate(wallParticle, _pos, _rot);
	}

	private bool DamageOnStrikeAccuracy()
	{
		if (!settings.offlineMode && !base.photonView.isMine)
		{
			return true;
		}
		if (weaponManager.currentWeaponScript.areaDamage)
		{
			return true;
		}
		if (currentTarget == null || weaponManager.currentWeaponScript == null)
		{
			return false;
		}
		float num = Vector3.Distance(currentTarget.transform.position, base.transform.position);
		float num2 = (num * (weaponManager.currentWeaponScript.accuracyMaxDistance - weaponManager.currentWeaponScript.accuracyWeapon) + (float)weaponManager.currentWeaponScript.maxDistance * weaponManager.currentWeaponScript.accuracyWeapon) / (float)weaponManager.currentWeaponScript.maxDistance;
		int num3 = Random.Range(0, 100);
		if ((float)num3 < num2)
		{
			return true;
		}
		return false;
	}

	public void shootTarget(int damage, GameObject target)
	{
		if (target != null && target.transform.GetComponent<EntityBehavior>().isDead)
		{
			return;
		}
		if (target != null && !isSniping())
		{
			if (!DamageOnStrikeAccuracy())
			{
				return;
			}
			if (damage > 0)
			{
				Ray ray = new Ray(base.transform.position + new Vector3(0f, 1f, 0f), target.transform.position - base.transform.position);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer) && !target.tag.Equals("Car"))
				{
					if (settings.offlineMode)
					{
						weaponManager.currentWeaponScript.hit();
						HoleRPC(true, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					}
					else
					{
						base.photonView.RPC("HoleRPC", PhotonTargets.All, true, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
					}
				}
			}
			switch (target.tag)
			{
			case "enemy":
				target.GetComponent<EnemyBehavior>().getDamage(damage);
				break;
			case "Helicopter":
				if (settings.offlineMode)
				{
					target.GetComponent<HelicopterBehavior>().getDamage(damage);
					break;
				}
				target.GetComponent<HelicopterBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage);
				break;
			case "Car":
				if (settings.offlineMode)
				{
					target.GetComponent<CarBehavior>().getDamage(damage);
					break;
				}
				target.GetComponent<CarBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage);
				break;
			case "Player":
				target.GetComponent<PlayerBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage, base.photonView.viewID);
				break;
			}
		}
		else if (isSniping())
		{
			shootByRaycast(damage);
		}
		else
		{
			if (damage <= 0)
			{
				return;
			}
			Ray ray2 = new Ray(base.transform.position + new Vector3(0f, 1f, 0f), base.transform.forward);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, float.PositiveInfinity, collisionLayer) && !weaponManager.currentWeaponScript.isMelee && !isSniping())
			{
				if (settings.offlineMode)
				{
					HoleRPC(false, hitInfo2.point + hitInfo2.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo2.normal));
					return;
				}
				base.photonView.RPC("HoleRPC", PhotonTargets.All, false, hitInfo2.point + hitInfo2.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo2.normal));
			}
		}
	}

	public void SendBroadcastShoot()
	{
		if (settings.offlineMode)
		{
			broadcastShoot();
		}
		else
		{
			base.photonView.RPC("broadcastShoot", PhotonTargets.Others);
		}
	}

	public void shoot(int damage)
	{
		if (!weaponManager.currentWeaponScript.areaDamage)
		{
			shootTarget(damage, currentTarget);
		}
		else
		{
			foreach (GameObject enemyAvailable in enemyWatcher.enemyAvailableList)
			{
				shootTarget(damage, enemyAvailable.transform.parent.gameObject);
			}
		}
		SendBroadcastShoot();
	}

	public bool isSniping()
	{
		return weaponManager.currentWeaponScript.name.Equals("SniperRifle") && weaponManager.CurrentWeapon.GetComponent<OpticalAimer>().sniperMode;
	}

	public void shootByRaycast(int damage)
	{
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer))
		{
			return;
		}
		Debug.Log(hitInfo.transform.name);
		if ((float)weaponManager.currentWeaponScript.maxDistance > Vector3.Distance(base.transform.position, hitInfo.point))
		{
			if (hitInfo.transform.tag.Equals("collidePoint"))
			{
				switch (hitInfo.transform.parent.tag)
				{
				case "enemy":
					hitInfo.transform.parent.GetComponent<EnemyBehavior>().getDamage(damage);
					break;
				case "Player":
					hitInfo.transform.root.GetComponent<PlayerBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage, base.photonView.viewID);
					break;
				case "Car":
					if (settings.offlineMode)
					{
						hitInfo.transform.parent.GetComponent<CarBehavior>().getDamage(damage);
						break;
					}
					hitInfo.transform.parent.GetComponent<CarBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage);
					break;
				case "Helicopter":
					if (settings.offlineMode)
					{
						hitInfo.transform.parent.GetComponent<HelicopterBehavior>().getDamage(damage);
						break;
					}
					hitInfo.transform.parent.GetComponent<HelicopterBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage);
					break;
				}
				if (settings.offlineMode)
				{
					weaponManager.currentWeaponScript.hit();
					HoleRPC(true, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
				else
				{
					base.photonView.RPC("HoleRPC", PhotonTargets.All, true, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
			}
			else if (hitInfo.transform.tag.Equals("Car"))
			{
				if (settings.offlineMode)
				{
					hitInfo.transform.GetComponent<CarBehavior>().getDamage(damage);
					weaponManager.currentWeaponScript.hit();
					HoleRPC(false, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
				else
				{
					hitInfo.transform.GetComponent<CarBehavior>().photonView.RPC("getDamage", PhotonTargets.All, damage);
					base.photonView.RPC("HoleRPC", PhotonTargets.All, false, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
			}
		}
		if (settings.offlineMode)
		{
			broadcastShoot();
		}
		else
		{
			base.photonView.RPC("broadcastShoot", PhotonTargets.Others);
		}
	}

	private bool needChangeActivArmor()
	{
		bool flag = Load.LoadBool(settings.keyArmorEnabled);
		if (objArmor.activeSelf && !flag)
		{
			return true;
		}
		if (!objArmor.activeSelf && flag)
		{
			return true;
		}
		return false;
	}

	[RPC]
	public void setActivObjArmor(bool val)
	{
		objArmor.SetActive(val);
	}

	[RPC]
	public void lowDamageCar(int demag, int idKiller)
	{
		if (!damageFromCar)
		{
			damageFromCar = true;
			colliderEnemyPlayer.enabled = false;
			highCarDemage = false;
			getDamage(demag, idKiller);
			if (weaponManager == null)
			{
				weaponManager = GetComponent<WeaponManager>();
			}
			weaponManager.CurrentWeapon.SetActive(false);
			deadHands.SetActive(true);
			prefabPlayer.animation.Play("Hit_By_Car_Little");
			deadHands.transform.GetChild(0).animation.Play("Hit_By_Car_Little");
			Invoke("wakeUpAfterCar", 3f);
		}
	}

	[RPC]
	public void highDamageCar(int demag, int idKiller)
	{
		if (!damageFromCar)
		{
			damageFromCar = true;
			colliderEnemyPlayer.enabled = false;
			highCarDemage = true;
			getDamage(demag, idKiller);
			prefabPlayer.animation.Play("Hit_By_Car_Big");
			deadHands.transform.GetChild(0).animation.Play("Hit_By_Car_Big");
		}
	}

	private void wakeUpAfterCar()
	{
		if (damageFromCar)
		{
			if (!PlayerMine)
			{
				colliderEnemyPlayer.enabled = true;
			}
			prefabPlayer.animation.Play("Wake_Up");
			deadHands.transform.GetChild(0).animation.Play("Wake_Up");
			Invoke("showOurGun", 1.5f);
		}
	}

	private void showOurGun()
	{
		deadHands.SetActive(false);
		weaponManager.CurrentWeapon.SetActive(true);
		damageFromCar = false;
	}

	public void animFlyDown()
	{
		if (!prefabPlayer.animation.IsPlaying("InFly"))
		{
			if (settings.offlineMode)
			{
				animFlyDownOnline();
			}
			else if (base.photonView.isMine)
			{
				base.photonView.RPC("animFlyDownOnline", PhotonTargets.All);
			}
		}
	}

	[RPC]
	private void animFlyDownOnline()
	{
		if (tController != null)
		{
			tController.isPlayAnimation = false;
		}
		prefabPlayer.animation.Play("InFly", AnimationPlayMode.Stop);
	}

	public void animDropAfterFlyDown()
	{
		if (settings.offlineMode)
		{
			animDropAfterFlyDownOnline();
		}
		else if (base.photonView.isMine)
		{
			base.photonView.RPC("animDropAfterFlyDownOnline", PhotonTargets.All);
		}
	}

	[RPC]
	private void animDropAfterFlyDownOnline()
	{
		prefabPlayer.animation.Play("Drop");
		if (settings.offlineMode || base.photonView.isMine)
		{
			Invoke("alreadyDontFly", 0.8f);

			if (CompilationSettings.EdgeBugFix)
			{
				tController.moveDirection = base.transform.forward.normalized * 10f;
			}
			else
			{
				tController.moveDirection = base.transform.forward * 10f;
			}

			tController.moveSpeed = 5f;
		}
	}

	public void alreadyDontFly()
	{
		if (settings.offlineMode)
		{
			alreadyDontFlyOnline();
		}
		else if (base.photonView.isMine)
		{
			base.photonView.RPC("alreadyDontFlyOnline", PhotonTargets.All);
		}
	}

	[RPC]
	private void alreadyDontFlyOnline()
	{
		CancelInvoke("moveToForward");
		if (tController != null)
		{
			tController.isPlayAnimation = true;
			tController.needPlayAnimDrop = false;
		}
	}

	public void AddDentonatorGrenadeToList(weaponGrenade addGrenade)
	{
		if (!listDetonatorGrenade.Contains(addGrenade))
		{
			listDetonatorGrenade.Add(addGrenade);
			CheckActiveButDetonator(addGrenade);
		}
	}

	public void RemoveGrenadeFromList(weaponGrenade removeGrenade)
	{
		if (listDetonatorGrenade.Contains(removeGrenade))
		{
			listDetonatorGrenade.Remove(removeGrenade);
			if (weaponManager.currentWeaponScript.scriptGrenade != null)
			{
				CheckActiveButDetonator(weaponManager.currentWeaponScript.scriptGrenade);
			}
		}
	}

	public void ExplosionGrenade(weaponGrenade bangGrenade)
	{
		Debug.Log("ExplosionGrenade");
		List<weaponGrenade> list = new List<weaponGrenade>();
		foreach (weaponGrenade item in listDetonatorGrenade)
		{
			if (settings.offlineMode || item.idPlayer == GameController.thisScript.playerScript.photonView.viewID)
			{
				if (item == null)
				{
					list.Add(item);
				}
				else if (item.objGrenade.name.Equals(bangGrenade.objGrenade.name))
				{
					list.Add(item);
					item.getDamage();
				}
			}
		}
		foreach (weaponGrenade item2 in list)
		{
			listDetonatorGrenade.Remove(item2);
		}
		CheckActiveButDetonator(bangGrenade);
	}

	public void ExplosionAllGrenade()
	{
		foreach (weaponGrenade item in listDetonatorGrenade)
		{
			item.getDamage();
		}
		listDetonatorGrenade.Clear();
	}

	public void RemoveAllGrenade()
	{
		foreach (weaponGrenade item in listDetonatorGrenade)
		{
			if (item != null && base.photonView.isMine)
			{
				item.RemoveByDisconnect();
			}
		}
		listDetonatorGrenade.Clear();
		CheckActiveButDetonator(null);
	}

	public void DetonateGrenade()
	{
		if (weaponManager.currentWeaponScript.scriptGrenade != null)
		{
			ExplosionGrenade(weaponManager.currentWeaponScript.scriptGrenade);
		}
	}

	public void CheckActiveButDetonator(weaponGrenade bangGrenade)
	{
		if (settings.offlineMode || base.photonView.isMine)
		{
			Debug.Log("CheckActiveButDetonator");
			bool activeButDetonator = GetActiveButDetonator(bangGrenade);
			if (!activeButDetonator && weaponManager.currentWeaponScript.throwingWeapon && weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount <= 0)
			{
				weaponManager.currentWeaponScript.switchNextWeapon();
			}
			GameController.thisScript.butDetonator.SetActive(activeButDetonator);
		}
	}

	public bool GetActiveButDetonator(weaponGrenade bangGrenade)
	{
		bool result = false;
		if (bangGrenade == null || bangGrenade.objGrenade == null)
		{
			return result;
		}
		foreach (weaponGrenade item in listDetonatorGrenade)
		{
			if (item.objGrenade.name.Equals(bangGrenade.objGrenade.name))
			{
				return true;
			}
		}
		return result;
	}

	public void otklButDetonator()
	{
		if (settings.offlineMode || base.photonView.isMine)
		{
			Debug.Log("otklButDetonator");
			GameController.thisScript.butDetonator.SetActive(false);
		}
	}

	private void OnDestroy()
	{
		try
		{
			if (!settings.offlineMode)
			{
				GameController.thisScript.obnovitSpisokButForPlayersWithDelay();
			}
		}
		catch (UnityException)
		{
		}
	}
}
