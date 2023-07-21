using UnityEngine;

public class EnemyBehavior : EntityBehavior
{
	private UnityEngine.AI.NavMeshAgent nAgent;

	private GameObject currentTarget;

	private GameObject prefabPlayer;

	private bool playerHasDead;

	private GameObject enemyCollider;

	public LayerMask collisionLayer;

	private GameObject deadHands;

	private GameObject arms;

	private GameObject aimObject;

	private bool isShooting;

	private Weapon weaponScript;

	[HideInInspector]
	public GameObject enemyWeapon;

	private ControllerNavMesh cNavMesh;

	private PlayerBehavior pBehavior;

	private int skinNumber;

	public bool isPolice;

	public bool isCoward;

	public int accuracy;

	public GameObject myCollidePoint;

	public AudioClip[] arrDamageScream;

	private AudioClip curAudioDamage;

	private float timeLastPlayDamage;

	private float timeDelayBetweenPlayDamage = 0.3f;

	public int MAX_DISTANCE_FROM_PLAYER = 70;

	public float damageMultiplier;

	public float normalSpeed;

	public float chaisingSpeed;

	public float rotSpeed;

	public bool isAggressive;

	public EnemyState currentState;

	public GameObject weaponPrefab;

	public int[] skinSet;

	public int health;

	public int reward;

	public GameObject bloodParticle;

	public GameObject wallParticle;

	public GameObject hole;

	public GameObject playerTarget;

	public GameObject weaponHolder;

	public EnemyType enemyType;

	[HideInInspector]
	public CapsuleCollider capsCollider;

	public bool carDamage;

	private int getSkinFromSet()
	{
		if (skinSet.Length > 0)
		{
			return skinSet[Random.Range(0, skinSet.Length - 1)];
		}
		return -1;
	}

	private void Awake()
	{
		capsCollider = GetComponent<CapsuleCollider>();
	}

	private void Start()
	{
		timeLastPlayDamage = Time.timeSinceLevelLoad;
		curAudioDamage = arrDamageScream[Random.Range(0, arrDamageScream.Length)];
		initEnemy();
		initWeapon();
		toggleGun(isAggressive);
	}

	private void OnEnable()
	{
		if (cNavMesh != null)
		{
			cNavMesh.Start();
		}
	}

	private void initWeapon()
	{
		if (weaponHolder != null && enemyWeapon == null)
		{
			enemyWeapon = (GameObject)Object.Instantiate(weaponPrefab, weaponHolder.transform.position, weaponHolder.transform.rotation);
			enemyWeapon.transform.parent = weaponHolder.transform;
			weaponScript = enemyWeapon.GetComponent<Weapon>();
			weaponScript.damage = (int)((float)weaponScript.damage * damageMultiplier);
			switchWeaponTexture(skinNumber);
		}
	}

	public void SwitchEnemyType(EnemyType eType)
	{
		if (eType > EnemyType.WhitePerson)
		{
			eType = EnemyType.Casual;
		}
		switch (eType)
		{
		case EnemyType.Bandit:
			isAggressive = true;
			isCoward = false;
			isPolice = false;
			skinNumber = GetRandomIntFromArray(new int[1] { 2 });
			break;
		case EnemyType.Beach:
			isAggressive = false;
			isCoward = true;
			isPolice = false;
			skinNumber = GetRandomIntFromArray(new int[3] { 4, 9, 24 });
			break;
		case EnemyType.Casual:
			isAggressive = false;
			isCoward = true;
			isPolice = false;
			skinNumber = GetRandomIntFromArray(new int[4] { 3, 5, 11, 14 });
			break;
		case EnemyType.Clerk:
			skinNumber = GetRandomIntFromArray(new int[2] { 33, 31 });
			isAggressive = false;
			isPolice = false;
			isCoward = true;
			break;
		case EnemyType.WhitePerson:
			isAggressive = false;
			isCoward = true;
			isPolice = false;
			skinNumber = GetRandomIntFromArray(new int[1] { 35 });
			break;
		case EnemyType.BlackPerson:
			skinNumber = GetRandomIntFromArray(new int[2] { 1, 12 });
			isAggressive = false;
			isCoward = false;
			isPolice = false;
			break;
		case EnemyType.Officer:
			isPolice = false;
			skinNumber = GetRandomIntFromArray(new int[2] { 29, 28 });
			isAggressive = false;
			isCoward = false;
			break;
		case EnemyType.Police:
			isCoward = false;
			isAggressive = false;
			isPolice = true;
			skinNumber = GetRandomIntFromArray(new int[3] { 20, 21, 13 });
			break;
		case EnemyType.Army:
			isCoward = false;
			isAggressive = false;
			isPolice = true;
			skinNumber = GetRandomIntFromArray(new int[3] { 34, 32, 30 });
			break;
		}
		enemyType = eType;
		if (weaponHolder != null)
		{
			initWeapon();
			initSkin(skinNumber);
		}
		toggleGun(isAggressive);
		if (!isAggressive && deadHands != null)
		{
			switchTextureOn(deadHands, settings.getTekSkin(skinNumber));
		}
	}

	private int GetRandomIntFromArray(int[] arr)
	{
		return arr[Random.Range(0, arr.Length)];
	}

	private void initEnemy()
	{
		skinNumber = getSkinFromSet();
		initSkin(skinNumber);
		nAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
		if (base.transform.Find("enemyCollider") != null)
		{
			enemyCollider = base.transform.Find("enemyCollider").gameObject;
		}
		if (weaponHolder != null)
		{
			deadHands = weaponHolder.transform.Find("deadHands").gameObject;
			arms = deadHands.transform.GetChild(0).gameObject;
		}
		if (enemyCollider != null && enemyCollider.transform.Find("AimLabel") != null)
		{
			aimObject = enemyCollider.transform.Find("AimLabel").gameObject;
		}
		cNavMesh = GetComponent<ControllerNavMesh>();
		health = 100;
		currentState = EnemyState.Passive;
		pBehavior = GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>();
	}

	private void debugupdate()
	{
		// TODO: This has to be removed.
		// if (Input.GetAxis("Mouse ScrollWheel") > 0f)
		// {
		// 	SwitchEnemyType(++enemyType);
		// }
	}

	private void FixedUpdate()
	{
		if (!carDamage)
		{
			updateBehavior();
			updateTarget();
		}
	}

	private void updateTarget()
	{
		if (!GameController.thisScript.myCar && !GameController.thisScript.myHelic)
		{
			if (!pBehavior.isDead)
			{
				playerTarget = GameController.thisScript.myPlayer;
			}
		}
		else if (GameController.thisScript.myCar != null)
		{
			playerTarget = GameController.thisScript.myCar;
		}
		else if (GameController.thisScript.myHelic != null)
		{
			playerTarget = GameController.thisScript.helicopterScript.gameObject;
		}
	}

	private void animateWalk()
	{
		prefabPlayer.GetComponent<Animation>().Play("Slow_Walk");
		if (deadHands != null)
		{
			deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Slow_Walk");
		}
	}

	private void animateRun()
	{
		prefabPlayer.GetComponent<Animation>().Play("Walk");
		if (deadHands != null)
		{
			deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Walk");
		}
	}

	private void Passive()
	{
		animateWalk();
		if (playerHasDead != pBehavior.isDead)
		{
			CheckPlayerDeath();
		}
		playerHasDead = pBehavior.isDead;
	}

	private void Suspicious()
	{
	}

	public void lowDamageCar(int damag)
	{
		if (!carDamage)
		{
			carDamage = true;
			cNavMesh.stopMoveNavMesh();
			toggleGun(false);
			getDamage(damag);
			prefabPlayer.GetComponent<Animation>().Play("Hit_By_Car_Little");
			if (deadHands != null)
			{
				deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Hit_By_Car_Little");
			}
			Invoke("wakeUpAfterCar", 3f);
		}
	}

	public void highDamageCar(int damag)
	{
		if (!carDamage)
		{
			carDamage = true;
			toggleGun(false);
			getDamage(damag);
			prefabPlayer.GetComponent<Animation>().Play("Hit_By_Car_Big");
			if (deadHands != null)
			{
				deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Hit_By_Car_Big");
			}
		}
	}

	private void wakeUpAfterCar()
	{
		if (carDamage)
		{
			prefabPlayer.GetComponent<Animation>().Play("Wake_Up");
			if (deadHands != null)
			{
				deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Wake_Up");
			}
			Invoke("showOurGun", 1.5f);
		}
	}

	private void showOurGun()
	{
		cNavMesh.startMoveNavMesh();
		carDamage = false;
		toggleGun(true);
	}

	private void toggleGun(bool show)
	{
		if (show)
		{
			if (deadHands != null)
			{
				deadHands.SetActive(false);
			}
			if (enemyWeapon != null)
			{
				enemyWeapon.SetActive(true);
			}
		}
		else
		{
			if (deadHands != null)
			{
				deadHands.SetActive(true);
			}
			if (enemyWeapon != null)
			{
				enemyWeapon.SetActive(false);
			}
		}
	}

	public void SwitchEnemyState(EnemyState state)
	{
		currentState = state;
		if (!(nAgent == null))
		{
			switch (currentState)
			{
			case EnemyState.Chasing:
				nAgent.speed = chaisingSpeed;
				break;
			case EnemyState.Passive:
				nAgent.speed = normalSpeed;
				break;
			case EnemyState.Escaping:
				nAgent.speed = chaisingSpeed;
				break;
			}
		}
	}

	private void Aggressive()
	{
		if (!(playerTarget != null))
		{
			return;
		}
		if (isPlayerVisible())
		{
			rotateToTarget(playerTarget.transform.position);
			cNavMesh.stopMoveNavMesh();
			prefabPlayer.GetComponent<Animation>().Play("Idle");
			if (weaponScript != null)
			{
				shoot(weaponScript.damage);
			}
			CheckPlayerDeath();
		}
		else
		{
			currentState = EnemyState.Chasing;
			animateRun();
		}
	}

	private bool isPlayerVisible()
	{
		if (playerTarget != null)
		{
			if (GameController.thisScript.myCar != null)
			{
				return true;
			}
			if ((bool)GameController.thisScript.myHelic)
			{
				return true;
			}
			Ray ray = new Ray(base.transform.position + new Vector3(0f, 1f, 0f), playerTarget.transform.position - base.transform.position);
			Debug.DrawRay(base.transform.position, playerTarget.transform.position - base.transform.position);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer))
			{
				if (hitInfo.collider.transform.root.tag == "Player")
				{
					return true;
				}
				return false;
			}
			return false;
		}
		return false;
	}

	public void CheckPlayerDeath()
	{
		if (currentState == EnemyState.Passive || !(playerTarget != null))
		{
			return;
		}
		if (GameController.thisScript.myCar == null)
		{
			if (GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>().isDead)
			{
				prefabPlayer.GetComponent<Animation>().Play("Walk");
				currentState = EnemyState.Passive;
				playerTarget = null;
				cNavMesh.startMoveNavMesh();
			}
		}
		else if (GameController.thisScript.myCar.GetComponent<CarBehavior>().isDead)
		{
			prefabPlayer.GetComponent<Animation>().Play("Walk");
			currentState = EnemyState.Passive;
			playerTarget = null;
			cNavMesh.startMoveNavMesh();
		}
	}

	private void Chase()
	{
		if (playerTarget != null)
		{
			rotateToTarget(playerTarget.transform.position);
			prefabPlayer.GetComponent<Animation>().Play("Walk");
			cNavMesh.GoToPointForce(playerTarget.transform.position);
			CheckPlayerDeath();
		}
	}

	private void Escape()
	{
		if (playerTarget != null)
		{
			animateRun();
			CheckPlayerDeath();
		}
	}

	private void updateBehavior()
	{
		if (Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(GameController.thisScript.playerScript.transform.position.x, GameController.thisScript.playerScript.transform.position.z)) > (float)MAX_DISTANCE_FROM_PLAYER)
		{
			EnemyGenerator.Instance.GoToHell(base.gameObject);
		}
		if (!isDead)
		{
			switch (currentState)
			{
			case EnemyState.Passive:
				Passive();
				break;
			case EnemyState.Suspicious:
				break;
			case EnemyState.Aggressive:
				Aggressive();
				break;
			case EnemyState.Chasing:
				Chase();
				break;
			case EnemyState.Escaping:
				Escape();
				break;
			}
		}
	}

	private void initSkin(int skinNumber)
	{
		if (skinNumber != -1)
		{
			Texture2D tekSkin = settings.getTekSkin(skinNumber);
			if (prefabPlayer == null)
			{
				prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
			}
			if (deadHands == null)
			{
				deadHands = weaponHolder.transform.Find("deadHands").gameObject;
			}
			switchTextureOn(prefabPlayer, tekSkin);
			switchTextureOn(deadHands, tekSkin);
			if (enemyWeapon != null)
			{
				switchWeaponTexture(skinNumber);
			}
		}
	}

	public override void getDamage(int damage)
	{
		if (isDead)
		{
			return;
		}
		if (damage != 0)
		{
			PoliceManager.Instance.SwitchWarningLevel(WarningLevel.Chasing);
		}
		if (damage < 0)
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
		if (!isAggressive && !isCoward)
		{
			isAggressive = true;
			toggleGun(true);
		}
		if (isCoward)
		{
			SwitchEnemyState(EnemyState.Escaping);
		}
		if (health - damage <= 0)
		{
			isDead = true;
			dead();
		}
		else
		{
			health -= damage;
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
			if (!(renderer.gameObject.tag == "Weapon"))
			{
				renderer.material.mainTexture = newTex;
			}
		}
	}

	public void switchWeaponTexture(int tekNomSkin)
	{
		Texture2D tekSkin = settings.getTekSkin(tekNomSkin);
		Renderer[] componentsInChildren = enemyWeapon.GetComponentsInChildren<Renderer>(true);
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

	public override void dead()
	{
		isDead = true;
		if (prefabPlayer == null)
		{
			prefabPlayer = base.transform.Find("prefabPlayer").gameObject;
		}
		if (enemyWeapon != null)
		{
			enemyWeapon.SetActive(false);
		}
		cNavMesh.stopMoveNavMesh();
		prefabPlayer.GetComponent<Animation>().Stop();
		prefabPlayer.GetComponent<Animation>().Play("Dead");
		if (deadHands != null)
		{
			deadHands.SetActive(true);
			deadHands.transform.GetChild(0).GetComponent<Animation>().Play("Dead");
		}
		GameController.thisScript.offlineKolKill++;
		Invoke("reset", 1f);
	}

	public void addCoins(int killerID)
	{
		shopController.thisScript.startMiganieCoins();
		settings.updateKolCoins(settings.tekKolCoins + 1);
		shopController.thisScript.lbKolCoins.text = string.Empty + settings.tekKolCoins;
		settings.updateKeychainCoins();
		settings.updateKolKill(settings.tekKolKill + 1);
	}

	private void reset()
	{
		toggleGun(isAggressive);
		animateWalk();
		cNavMesh.startMoveNavMesh();
		base.transform.position = controllerConnectGame.thisScript.getRandomPointAllMap().position;
		SwitchEnemyState(EnemyState.Passive);
		health = 100;
		isDead = false;
		carDamage = false;
	}

	public void reload()
	{
	}

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

	public void rotateToTarget(Vector3 target)
	{
		Quaternion to = Quaternion.LookRotation(target - base.transform.position);
		base.transform.rotation = Quaternion.Slerp(base.transform.rotation, to, rotSpeed * Time.deltaTime);
	}

	private void disableParticle()
	{
		if (weaponScript != null)
		{
			weaponScript.shootParticle.SetActive(false);
		}
	}

	private bool checkAccuracy()
	{
		return Random.Range(0, 100) <= accuracy;
	}

	public void shoot(int damage)
	{
		if (!(playerTarget != null) || currentState != 0)
		{
			return;
		}
		Ray ray = new Ray(base.transform.position + new Vector3(0f, 1f, 0f), playerTarget.transform.position - base.transform.position);
		if (enemyWeapon.GetComponent<Animation>().IsPlaying("Shoot"))
		{
			return;
		}
		enemyWeapon.GetComponent<Animation>().Play("Shoot");
		weaponScript.shootParticle.SetActive(true);
		weaponScript.GetComponent<AudioSource>().PlayOneShot(weaponScript.soundShoot);
		if (checkAccuracy())
		{
			if (GameController.thisScript.myCar == null && GameController.thisScript.myHelic == null && !playerTarget.GetComponent<PlayerBehavior>().inCar)
			{
				GameController.thisScript.playerScript.getDamage(damage);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer))
				{
					HoleRPC(true, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
				}
			}
			else if (GameController.thisScript.myHelic == null)
			{
				GameController.thisScript.carScript.getDamage(damage);
			}
			else
			{
				GameController.thisScript.helicopterScript.getDamage(damage);
			}
		}
		Invoke("disableParticle", 0.1f);
	}

	public void ChangeTriggerColliderEnemy()
	{
		CancelInvoke("VklIsTrigColliderEnemy");
		capsCollider.isTrigger = false;
		Invoke("VklIsTrigColliderEnemy", 0.05f);
	}

	private void VklIsTrigColliderEnemy()
	{
		capsCollider.isTrigger = true;
	}

	private void OnDestroy()
	{
	}
}
