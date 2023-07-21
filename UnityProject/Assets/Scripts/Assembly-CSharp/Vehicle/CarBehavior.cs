using Holoville.HOTween;
using UnityEngine;

public class CarBehavior : EntityBehavior
{
	public bool isShooting;

	public bool playerInCar;

	public bool exitFromCarEnabled;

	public bool respawnCar = true;

	public bool carWithWeapon;

	[HideInInspector]
	public NewDriving newDrivingScript;

	[HideInInspector]
	public GameObject butOutCar;

	public int idPlayerInCar = -1;

	public GameObject objPlayerInCar;

	public PlayerBehavior scriptPlayerInCar;

	public int health = 200;

	private int startHealth;

	public int countBulletCar;

	public float timeBetweenShoot = 1f;

	private bool iCanShoot = true;

	private int predSpeedCarForIgnoreEnemy;

	public LayerMask collisionLayer;

	public areaDamage[] arrDamage;

	private int startCountBulletCar;

	public AudioClip soundBonuse;

	public GameObject myCollidePoint;

	public Vector3 pointForExit;

	public GameObject allPointsForExit;

	public pointExitFromCar leftPoint;

	public pointExitFromCar rightPoint;

	public pointExitFromCar topPoint;

	public GameObject explosionObject;

	[HideInInspector]
	public Vector3 initialCarPosition;

	[HideInInspector]
	public Quaternion initialRotation;

	public Transform cameraPoint;

	[HideInInspector]
	public UnityEngine.AI.NavMeshObstacle navMeshCollider;

	[HideInInspector]
	public BoxCollider colliderEnemy;

	public Texture textCar;

	public Texture textCarDead;

	public GameObject particlLightSmoke;

	public GameObject particlHardSmoke;

	public GameObject particlFire;

	public GameObject particlBoom;

	public GameObject particleExposionWeapon;

	public GameObject objCar;

	public bool carPerevernuta;

	public GameObject particleShoot;

	public NJGMapItem iconMiniMap;

	public NJGMapItem iconMiniMapWithPlayer;

	public visibleObjPhoton ScriptVisibleForLerp;

	private PhotonView photonViewPlayerInCar;

	private void Awake()
	{
		newDrivingScript = GetComponent<NewDriving>();
		navMeshCollider = GetComponent<UnityEngine.AI.NavMeshObstacle>();
		colliderEnemy = GetComponent<BoxCollider>();
		explosionObject.SetActive(false);
		initialCarPosition = base.transform.position;
		initialRotation = base.transform.rotation;
		ScriptVisibleForLerp = GetComponentInChildren<visibleObjPhoton>();
		startHealth = health;
		startCountBulletCar = countBulletCar;
		HOTween.Init();
	}

	private void Start()
	{
		if (!carWithWeapon)
		{
			iconMiniMap.type = 3;
			iconMiniMapWithPlayer.type = 4;
		}
		else
		{
			iconMiniMap.type = 6;
			iconMiniMapWithPlayer.type = 7;
		}
		butOutCar = GameController.thisScript.butOutCar;
		GameController.thisScript.arrAllCar.Add(this);
		base.enabled = false;
	}

	private void OnEnable()
	{
		if (carWithWeapon)
		{
			iCanShoot = true;
			if (GameController.thisScript != null)
			{
				GameController.thisScript.countCarBullets = countBulletCar;
			}
			predSpeedCarForIgnoreEnemy = settings.speedCarForIgnoreEnemy;
			settings.speedCarForIgnoreEnemy = 5;
		}
	}

	private void OnDestroy()
	{
		GameController.thisScript.arrAllCar.Remove(this);
	}

	private void OnDisable()
	{
		if (carWithWeapon)
		{
			settings.speedCarForIgnoreEnemy = predSpeedCarForIgnoreEnemy;
		}
	}

	public void soundCarEnabled(bool val)
	{
		AudioSource[] components = GetComponents<AudioSource>();
		AudioSource[] array = components;
		foreach (AudioSource audioSource in array)
		{
			if (val)
			{
				if (settings.soundEnabled)
				{
					audioSource.Play();
				}
			}
			else
			{
				audioSource.Stop();
			}
		}
	}

	public void setIconMiniMapCarWithPlayer(bool val)
	{
		if (iconMiniMap != null && iconMiniMapWithPlayer != null)
		{
			iconMiniMap.enabled = !val;
			iconMiniMapWithPlayer.enabled = val;
		}
	}

	[RPC]
	public void reset()
	{
		if (respawnCar)
		{
			if (objPlayerInCar != null)
			{
				scriptPlayerInCar.GetOutOfCar();
			}
			stopAllPatricle();
			switchTextureOn(objCar, textCar);
			health = startHealth;
			countBulletCar = startCountBulletCar;
			exitFromCar();
			CancelInvoke("reset");
			CancelResetCarWithDelay();
			CancelInvoke("invokBlowUp");
			newDrivingScript.enabled = true;
			base.transform.position = initialCarPosition;
			base.transform.rotation = initialRotation;
			isDead = false;
		}
	}

	public void resetCarEnabled(bool val)
	{
		if (val)
		{
			respawnCar = true;
			return;
		}
		respawnCar = false;
		stopResetWithDelay();
	}

	public void sitInCarOnline(int idPlayer)
	{
		base.photonView.RPC("setIdPlayerInCar", PhotonTargets.All, idPlayer, true);
	}

	public void leftCarOnline(int idPlayer)
	{
		Debug.Log("leftCarOnline leftCarOnline=" + idPlayer);
		base.photonView.RPC("setIdPlayerInCar", PhotonTargets.All, idPlayer, false);
	}

	[RPC]
	private void setIdPlayerInCar(int idPl, bool inCar)
	{
		if (inCar)
		{
			idPlayerInCar = idPl;
			playerInCar = true;
			setIconMiniMapCarWithPlayer(playerInCar);
			CancelResetCarWithDelay();
			{
				foreach (PlayerBehavior listPlayer in GameController.thisScript.listPlayers)
				{
					if (listPlayer.indPlayer == idPlayerInCar)
					{
						objPlayerInCar = listPlayer.gameObject;
						scriptPlayerInCar = listPlayer;
						scriptPlayerInCar.currentCar = this;
						scriptPlayerInCar.inCar = true;
						newDrivingScript.isGo = false;
						newDrivingScript.isBack = false;
						base.enabled = true;
						scriptPlayerInCar.SetActiveComponentBesidesControl(false);
						if (ScriptVisibleForLerp != null)
						{
							ScriptVisibleForLerp.lerpScript = scriptPlayerInCar.GetComponent<lerpTransformPhoton>();
							scriptPlayerInCar.lerpScript.sglajEnabled = true;
							scriptPlayerInCar.lerpScript.objVistavlen = true;
						}
						OtklControlFromMain();
						CancelInvoke("OtklControlFromMain");
						objPlayerInCar.transform.position = base.transform.position;
						objPlayerInCar.transform.rotation = base.transform.rotation;
						Invoke("setPosPlayerInCar", 0.5f);
						break;
					}
				}
				return;
			}
		}
		foreach (CarBehavior item in GameController.thisScript.arrAllCar)
		{
			if (item.idPlayerInCar == idPl)
			{
				item.exitFromCar();
			}
		}
	}

	public void SendPositionCarOther()
	{
		if (!settings.offlineMode && base.photonView.isMine && !playerInCar)
		{
			base.photonView.RPC("SetPositionCarOnline", PhotonTargets.Others, base.transform.position, base.transform.rotation);
		}
	}

	[RPC]
	private void SetPositionCarOnline(Vector3 pos, Quaternion rot)
	{
		if (!settings.offlineMode && !base.photonView.isMine && !playerInCar)
		{
			base.transform.position = pos;
			base.transform.rotation = rot;
		}
	}

	public void SendSetIdPlayerInCar()
	{
		if (!settings.offlineMode && base.photonView.isMine && playerInCar)
		{
			base.photonView.RPC("setIdPlayerInCar", PhotonTargets.Others, idPlayerInCar, true);
		}
	}

	private void VklControlFromMain()
	{
		base.photonView.synchronization = ViewSynchronization.Unreliable;
	}

	private void OtklControlFromMain()
	{
		base.photonView.synchronization = ViewSynchronization.Off;
	}

	private void setPosPlayerInCar()
	{
		if (objPlayerInCar != null)
		{
			objPlayerInCar.transform.position = base.transform.position;
			objPlayerInCar.transform.rotation = base.transform.rotation;
			if (!settings.offlineMode)
			{
				scriptPlayerInCar.lerpScript.sglajEnabled = true;
				scriptPlayerInCar.lerpScript.objVistavlen = true;
			}
		}
	}

	[RPC]
	public override void getDamage(int damage)
	{
		if (!isDead)
		{
			if (health - damage <= 0)
			{
				dead();
				return;
			}
			health -= damage;
			startParticleDamage();
		}
	}

	public new bool isAlive()
	{
		return !isDead;
	}

	public override void dead()
	{
		if (isDead)
		{
			return;
		}
		isDead = true;
		health = 0;
		startParticleDamage();
		switchTextureOn(objCar, textCarDead);
		explosionObject.SetActive(true);
		if (!settings.offlineMode)
		{
			photonViewPlayerInCar = null;
			if (playerInCar && idPlayerInCar >= 0)
			{
				photonViewPlayerInCar = PhotonView.Find(idPlayerInCar);
			}
			Debug.Log("playerInCar=" + playerInCar + " idPlayerInCar=" + idPlayerInCar + " photonViewPlayerInCar=" + photonViewPlayerInCar);
			if (photonViewPlayerInCar != null)
			{
				Debug.Log("dead player in car");
				if (photonViewPlayerInCar.isMine)
				{
					photonViewPlayerInCar.GetComponent<PlayerBehavior>().GetOutOfCar();
				}
				photonViewPlayerInCar.GetComponent<PlayerBehavior>().getDamage(1000, -9999);
			}
			else if (playerInCar && objPlayerInCar != null && scriptPlayerInCar.photonView.isMine)
			{
				resetCarOnline();
			}
		}
		else if (playerInCar)
		{
			GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>().GetOutOfCar();
			GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>().getDamage(1000);
		}
		Invoke("respawn", 1f);
	}

	private void deadCarIfInverted()
	{
		if (settings.offlineMode)
		{
			deadWithDelay(5f);
		}
		else if (base.photonView.isMine)
		{
			base.photonView.RPC("deadWithDelay", PhotonTargets.All, 5f);
		}
	}

	[RPC]
	public void deadWithDelay(float time)
	{
		stopAllPatricle();
		particlFire.SetActive(true);
		Invoke("invokDead", time);
	}

	private void invokDead()
	{
		Debug.Log("invokDead");
		if (settings.offlineMode)
		{
			getDamage(1000);
			return;
		}
		base.photonView.RPC("getDamage", PhotonTargets.All, 1000);
	}

	private void respawn()
	{
		Invoke("reset", 5f);
		explosionObject.SetActive(false);
	}

	private void Update()
	{
		// TODO: This is terrible to being doing every update.
		getPointExitFromCar();
		if (!settings.offlineMode && playerInCar && objPlayerInCar != null && !scriptPlayerInCar.photonView.isMine)
		{
			base.transform.position = objPlayerInCar.transform.position;
			base.transform.rotation = objPlayerInCar.transform.rotation;
		}
		if (!settings.offlineMode && (!(objPlayerInCar != null) || !scriptPlayerInCar.photonView.isMine))
		{
			return;
		}
		if (playerInCar && !allPointsForExit.activeSelf)
		{
			allPointsForExit.SetActive(true);
		}
		if (!playerInCar && allPointsForExit.activeSelf)
		{
			allPointsForExit.SetActive(false);
		}
		if (newDrivingScript != null && playerInCar)
		{
			if ((float)Mathf.Abs(newDrivingScript.currentSpeedReal) < 20f && !butOutCar.activeSelf)
			{
				butOutCar.SetActive(true);
			}

			if ((float)Mathf.Abs(newDrivingScript.currentSpeedReal) >= 20f && butOutCar.activeSelf)
			{
				butOutCar.SetActive(false);
			}

			if (newDrivingScript.currentSpeedReal < settings.speedCarForIgnoreEnemy && !navMeshCollider.enabled)
			{
				navMeshCollider.enabled = true;
			}

			if (newDrivingScript.currentSpeedReal >= settings.speedCarForIgnoreEnemy && navMeshCollider.enabled)
			{
				navMeshCollider.enabled = false;
			}
		}
		if (topPoint.colliderWithGround && !carPerevernuta)
		{
			carPerevernuta = true;
			Invoke("deadCarIfInverted", 5f);
		}
		if (!topPoint.colliderWithGround && carPerevernuta)
		{
			carPerevernuta = false;
			CancelInvoke("deadCarIfInverted");
		}
		if (carWithWeapon && GameController.thisScript.bonuseManagerScript != null)
		{
			GameController.thisScript.bonuseManagerScript.getNearBonuseInCar(base.gameObject, 2f);
		}
	}

	public void resetWithDelay()
	{
		if (settings.offlineMode)
		{
			Invoke("reset", 5f);
		}
	}

	public void stopResetWithDelay()
	{
		CancelInvoke("reset");
	}

	private void getPointExitFromCar()
	{
		if (playerInCar)
		{
			if (carPerevernuta)
			{
				pointForExit = base.transform.position + new Vector3(0f, 3f, 0f);
			}
			else if (leftPoint.exitVozmojen)
			{
				pointForExit = leftPoint.transform.position;
			}
			else if (rightPoint.exitVozmojen)
			{
				pointForExit = rightPoint.transform.position;
			}
			else if (topPoint.exitVozmojen)
			{
				pointForExit = topPoint.transform.position;
			}
			else
			{
				pointForExit = base.transform.position + new Vector3(0f, 3f, 0f);
			}
		}
	}

	public void clearAllPrepiadstvia()
	{
		leftPoint.clearListPrepiadstvii();
		rightPoint.clearListPrepiadstvii();
		topPoint.clearListPrepiadstvii();
	}

	private void stopAllPatricle()
	{
		particlLightSmoke.SetActive(false);
		particlHardSmoke.SetActive(false);
		particlFire.SetActive(false);
		particlBoom.SetActive(false);
	}

	[RPC]
	public void startParticleDamage()
	{
		if (health <= 0)
		{
			if (!particlBoom.activeSelf)
			{
				stopAllPatricle();
				particlBoom.SetActive(true);
			}
		}
		else if (health <= 50)
		{
			if (!particlFire.activeSelf)
			{
				stopAllPatricle();
				blowUpCarWithDelay();
				particlFire.SetActive(true);
			}
		}
		else if ((float)health <= (float)startHealth * 0.5f)
		{
			if (!particlHardSmoke.activeSelf)
			{
				stopAllPatricle();
				particlHardSmoke.SetActive(true);
			}
		}
		else if ((float)health <= (float)startHealth * 0.75f && !particlLightSmoke.activeSelf)
		{
			stopAllPatricle();
			particlLightSmoke.SetActive(true);
		}
	}

	private void blowUpCarWithDelay()
	{
		if (!IsInvoking("invokBlowUp"))
		{
			Invoke("invokBlowUp", 8f);
		}
	}

	private void invokBlowUp()
	{
		CancelInvoke("invokBlowUp");
		if (!isDead)
		{
			if (settings.offlineMode)
			{
				getDamage(1000);
				return;
			}
			base.photonView.RPC("getDamage", PhotonTargets.All, 1000);
		}
	}

	public void switchTextureOn(GameObject player, Texture newTex)
	{
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

	public void vklColliderEnemy(bool val)
	{
		navMeshCollider.enabled = val;
	}

	public void resetPlayervklInCar()
	{
		playerInCar = false;
		setIconMiniMapCarWithPlayer(playerInCar);
		idPlayerInCar = -1;
	}

	public void exitFromCar()
	{
		playerInCar = false;
		setIconMiniMapCarWithPlayer(playerInCar);
		if (settings.offlineMode)
		{
			return;
		}
		VklControlFromMain();
		Invoke("OtklControlFromMain", 3f);
		if (objPlayerInCar != null)
		{
			objPlayerInCar.transform.parent = null;
			scriptPlayerInCar.lerpScript.objVistavlen = false;
			if (pointForExit != Vector3.zero && scriptPlayerInCar.photonView.isMine)
			{
				objPlayerInCar.transform.position = pointForExit;
			}
			newDrivingScript.FullStopCar();
			newDrivingScript.isGo = false;
			newDrivingScript.isBack = false;
			scriptPlayerInCar.currentCar = null;
			scriptPlayerInCar.inCar = false;
			scriptPlayerInCar.SetActiveComponentBesidesControl(true);
		}
		objPlayerInCar = null;
		idPlayerInCar = -1;
		scriptPlayerInCar = null;
		base.enabled = false;
	}

	public void ResetCarWithDelay()
	{
		if (settings.offlineMode)
		{
			ResetCarWithDelayOnline();
		}
		else
		{
			base.photonView.RPC("ResetCarWithDelayOnline", PhotonTargets.MasterClient);
		}
	}

	[RPC]
	private void ResetCarWithDelayOnline()
	{
		CancelResetCarWithDelay();
		Invoke("resetCarOnline", 9f);
	}

	public void CancelResetCarWithDelay()
	{
		if (settings.offlineMode)
		{
			CancelResetCarWithDelayOnline();
		}
		else
		{
			base.photonView.RPC("CancelResetCarWithDelayOnline", PhotonTargets.MasterClient);
		}
	}

	[RPC]
	private void CancelResetCarWithDelayOnline()
	{
		CancelInvoke("resetCarOnline");
	}

	public void exitFromCarOnline()
	{
		if (!settings.offlineMode)
		{
			leftCarOnline(idPlayerInCar);
		}
	}

	public void resetCarOnline()
	{
		Debug.Log("= resetCarOnline =");
		if (respawnCar)
		{
			if (settings.offlineMode)
			{
				reset();
			}
			else
			{
				base.photonView.RPC("reset", PhotonTargets.All);
			}
		}
	}

	public void startShoot()
	{
		isShooting = true;
		InvokeRepeating("shootFromCar", 0.03f, 0.3f);
	}

	public void cancelShoot()
	{
		isShooting = false;
		CancelInvoke("shootFromCar");
	}

	public void shootFromCar()
	{
		Debug.Log("shootFromCar =" + idPlayerInCar);
		if (!playerInCar)
		{
			cancelShoot();
		}
		else if (iCanShoot)
		{
			if (isDead)
			{
				cancelShoot();
				return;
			}

			if (settings.offlineMode)
			{
				shootFromCarOnline(true);
				return;
			}

			shootFromCarOnline(true);
			base.photonView.RPC("shootFromCarOnline", PhotonTargets.Others, false);
		}
	}

	[RPC]
	private void shootFromCarOnline(bool isMinePlayer)
	{
		if (!iCanShoot)
		{
			return;
		}
		iCanShoot = false;
		Invoke("vklShoot", timeBetweenShoot);
		if (countBulletCar <= 0)
		{
			return;
		}
		countBulletCar--;
		if (particleShoot != null)
		{
			particleShoot.SetActive(true);
		}
		Debug.Log("PlayShoooot");
		objCar.GetComponent<Animation>().Play("Shoot");
		if (!settings.offlineMode && !isMinePlayer)
		{
			return;
		}
		GameController.thisScript.countCarBullets = countBulletCar;
		Ray ray = new Ray(base.transform.position + new Vector3(0f, 1f, 0f), base.transform.forward);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer) ? true : false)
		{
			if (settings.offlineMode)
			{
				explosionFromWeapon(hitInfo.point);
				return;
			}
			base.photonView.RPC("explosionFromWeapon", PhotonTargets.All, hitInfo.point);
		}
	}

	[RPC]
	private void explosionFromWeapon(Vector3 pointForExplosion)
	{
		Debug.Log("explosionFromWeapon " + base.photonView.viewID);
		Object.Instantiate(particleExposionWeapon, pointForExplosion, Quaternion.identity);

		foreach (areaDamage area_damage in arrDamage)
		{
			ExplosionManager.Explode(
				pointForExplosion,
				area_damage.radius,
				area_damage.damage,
				idPlayerInCar,
				false
			);
		}
	}

	private void vklShoot()
	{
		iCanShoot = true;
		if (particleShoot != null)
		{
			particleShoot.SetActive(false);
		}
	}

	[RPC]
	public void setCountBullets(int val)
	{
		countBulletCar = val;
		if (settings.offlineMode || base.photonView.isMine)
		{
			GameController.thisScript.countCarBullets = countBulletCar;
		}
	}

	private void playSoundBonuse()
	{
		if (settings.soundEnabled)
		{
			NGUITools.PlaySound(soundBonuse);
		}
	}
}
