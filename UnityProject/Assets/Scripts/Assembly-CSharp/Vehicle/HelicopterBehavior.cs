using Holoville.HOTween;
using UnityEngine;

public class HelicopterBehavior : EntityBehavior
{
	[HideInInspector]
	public GameObject butExitFromHelic;

	public bool playerInHelic;

	public bool exitFromHelicEnabled;

	public bool respawnHelic = true;

	public int health = 200;

	private int startHealth;

	public GameObject myCollidePoint;

	public int idPlayerInHelic = -1;

	public GameObject objPlayerInHelic;

	public PlayerBehavior scriptPlayerInHelic;

	public int countBulletHelic;

	public float timeBetweenShoot = 1f;

	private bool iCanShoot = true;

	public Transform cameraPoint;

	public Vector3 pointForExit;

	public GameObject allPointsForExit;

	public pointExitFromCar leftPoint;

	public pointExitFromCar rightPoint;

	public pointExitFromCar topPoint;

	public GameObject explosionObject;

	[HideInInspector]
	public Vector3 initialHelicPosition;

	[HideInInspector]
	public Quaternion initialRotation;

	public Texture textHelic;

	public Texture textHelicDead;

	public GameObject particlLightSmoke;

	public GameObject particlHardSmoke;

	public GameObject particlFire;

	public GameObject particlBoom;

	public GameObject particleExposionWeapon;

	public GameObject objHelic;

	public MeshCollider colliderHelicopter;

	public TriggerOnGround triggerOnGroundSript;

	public visibleObjPhoton ScriptVisibleForLerp;

	private PhotonView photonViewPlayerInHelic;

	public NJGMapItem iconMiniMap;

	public NJGMapItem iconMiniMapWithPlayer;

	private AnimationState animVjigVjig;

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

	private void Awake()
	{
		explosionObject.SetActive(false);
		initialHelicPosition = base.transform.position;
		initialRotation = base.transform.rotation;
		startHealth = health;
		animVjigVjig = objHelic.animation["Engine_on"];
		animVjigVjig.speed = 0f;
	}

	public void setIconMiniMapCarWithPlayer(bool val)
	{
		if (iconMiniMap != null && iconMiniMapWithPlayer != null)
		{
			iconMiniMap.enabled = !val;
			iconMiniMapWithPlayer.enabled = val;
		}
	}

	private void Start()
	{
		butExitFromHelic = GameController.thisScript.butOutHelicopter;
		GameController.thisScript.listAllHelicopter.Add(this);
		base.enabled = false;
		HOTween.Init();
	}

	private void Update()
	{
		if (!playerInHelic || !(objPlayerInHelic != null))
		{
			return;
		}
		base.transform.position = objPlayerInHelic.transform.position;
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, objPlayerInHelic.transform.rotation, Time.deltaTime * 1f);
		if (settings.offlineMode || scriptPlayerInHelic.photonView.isMine)
		{
			getPointExitFromHelic();
			if (triggerOnGroundSript.isOnGround && !butExitFromHelic.activeSelf)
			{
				butExitFromHelic.SetActive(true);
			}
			if (!triggerOnGroundSript.isOnGround && butExitFromHelic.activeSelf)
			{
				butExitFromHelic.SetActive(false);
			}
			if (triggerOnGroundSript.isOnGround && GameController.thisScript.panelJoystick.gameObject.activeSelf)
			{
				GameController.thisScript.panelJoystick.gameObject.SetActive(false);
				GameController.thisScript.joystickWalk.position = Vector2.zero;
			}
			if (!triggerOnGroundSript.isOnGround && !GameController.thisScript.panelJoystick.gameObject.activeSelf)
			{
				GameController.thisScript.panelJoystick.gameObject.SetActive(true);
			}
		}
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
		switchTextureOn(objHelic, textHelicDead);
		explosionObject.SetActive(true);
		if (!settings.offlineMode)
		{
			photonViewPlayerInHelic = null;
			if (playerInHelic && idPlayerInHelic >= 0)
			{
				photonViewPlayerInHelic = PhotonView.Find(idPlayerInHelic);
			}
			Debug.Log("playerInHelic=" + playerInHelic + " idPlayerInHelic=" + idPlayerInHelic + " photonViewPlayerInHelic=" + photonViewPlayerInHelic);
			if (photonViewPlayerInHelic != null)
			{
				Debug.Log("dead player in Helic");
				if (photonViewPlayerInHelic.isMine)
				{
					photonViewPlayerInHelic.GetComponent<PlayerBehavior>().GetOutOfHelic();
				}
				photonViewPlayerInHelic.GetComponent<PlayerBehavior>().getDamage(1000, -9999);
			}
			else if (playerInHelic && objPlayerInHelic != null && scriptPlayerInHelic.photonView.isMine)
			{
				resetHelicOnline();
			}
		}
		else if (playerInHelic)
		{
			GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>().GetOutOfHelic();
			GameController.thisScript.myPlayer.GetComponent<PlayerBehavior>().getDamage(1000);
		}
		Invoke("respawn", 1f);
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

	public void exitFromHelic()
	{
		playerInHelic = false;
		if (settings.offlineMode)
		{
			return;
		}
		Debug.Log("exitFromHelic " + base.photonView.viewID);
		VklControlFromMain();
		Invoke("OtklControlFromMain", 3f);
		if (objPlayerInHelic != null)
		{
			scriptPlayerInHelic.lerpScript.objVistavlen = false;
			if (pointForExit != Vector3.zero && scriptPlayerInHelic.photonView.isMine)
			{
				objPlayerInHelic.transform.position = pointForExit;
			}
			scriptPlayerInHelic.SetVisiblePlayerBesidesControl(true);
		}
		objPlayerInHelic = null;
		idPlayerInHelic = -1;
		scriptPlayerInHelic = null;
		base.enabled = false;
		Invoke("resetHeliopterOnline", 20f);
	}

	public void exitFromHelicOnline()
	{
		if (!settings.offlineMode)
		{
			leftHelicOnline(idPlayerInHelic);
		}
	}

	public void resetHelicOnline()
	{
		if (respawnHelic)
		{
			base.photonView.RPC("reset", PhotonTargets.All);
		}
	}

	public void sitInHelicOnline(int idPlayer)
	{
		base.photonView.RPC("setIdPlayerInHelic", PhotonTargets.All, idPlayer, true);
	}

	public void leftHelicOnline(int idPlayer)
	{
		Debug.Log("leftHelicOnline =" + idPlayer);
		base.photonView.RPC("setIdPlayerInHelic", PhotonTargets.All, idPlayer, false);
	}

	public void clearAllPrepiadstvia()
	{
		leftPoint.clearListPrepiadstvii();
		rightPoint.clearListPrepiadstvii();
		topPoint.clearListPrepiadstvii();
	}

	private void getPointExitFromHelic()
	{
		if (playerInHelic)
		{
			if (leftPoint.exitVozmojen)
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
				pointForExit = base.transform.position + new Vector3(0f, 5f, 0f);
			}
		}
	}

	public void resetHeliopterOnline()
	{
		if (respawnHelic)
		{
			base.photonView.RPC("reset", PhotonTargets.All);
		}
	}

	public void animationHelicUp()
	{
		if (settings.offlineMode)
		{
			animationHelicUpOnline();
		}
		else
		{
			base.photonView.RPC("animationHelicUpOnline", PhotonTargets.AllBuffered);
		}
	}

	public void animationHelicDown()
	{
		if (settings.offlineMode)
		{
			animationHelicDownOnline();
		}
		else
		{
			base.photonView.RPC("animationHelicDownOnline", PhotonTargets.AllBuffered);
		}
	}

	private void StopAnimationFly()
	{
		HOTween.Kill(animVjigVjig);
		if (objHelic != null)
		{
			objHelic.animation.Stop();
		}
	}

	private void stopAllPatricle()
	{
		particlLightSmoke.SetActive(false);
		particlHardSmoke.SetActive(false);
		particlFire.SetActive(false);
		particlBoom.SetActive(false);
	}

	private void blowUpHelicWithDelay()
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

	private void respawn()
	{
		Invoke("reset", 5f);
		explosionObject.SetActive(false);
	}

	private void VklControlFromMain()
	{
		base.photonView.synchronization = ViewSynchronization.Unreliable;
	}

	private void OtklControlFromMain()
	{
		base.photonView.synchronization = ViewSynchronization.Off;
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
				blowUpHelicWithDelay();
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

	[RPC]
	public void reset()
	{
		if (respawnHelic)
		{
			if (objPlayerInHelic != null)
			{
				scriptPlayerInHelic.GetOutOfHelic();
			}
			stopAllPatricle();
			switchTextureOn(objHelic, textHelic);
			health = startHealth;
			exitFromHelic();
			CancelInvoke("reset");
			CancelInvoke("resetHeliopterOnline");
			CancelInvoke("invokBlowUp");
			base.transform.position = initialHelicPosition;
			base.transform.rotation = initialRotation;
			isDead = false;
		}
	}

	[RPC]
	private void setIdPlayerInHelic(int idPl, bool inHelic)
	{
		if (inHelic)
		{
			idPlayerInHelic = idPl;
			playerInHelic = true;
			CancelInvoke("resetHeliopterOnline");
			{
				foreach (PlayerBehavior listPlayer in GameController.thisScript.listPlayers)
				{
					if (listPlayer.indPlayer == idPlayerInHelic)
					{
						objPlayerInHelic = listPlayer.gameObject;
						scriptPlayerInHelic = listPlayer;
						base.enabled = true;
						if (ScriptVisibleForLerp != null)
						{
							Debug.Log("setIdPlayerInHelic = " + idPl);
							ScriptVisibleForLerp.lerpScript = scriptPlayerInHelic.GetComponent<lerpTransformPhoton>();
						}
						scriptPlayerInHelic.SetVisiblePlayerBesidesControl(false);
						OtklControlFromMain();
						CancelInvoke("OtklControlFromMain");
						setPosPlayerInHelic();
						Invoke("setPosPlayerInHelic", 0.5f);
						break;
					}
				}
				return;
			}
		}
		foreach (HelicopterBehavior item in GameController.thisScript.listAllHelicopter)
		{
			if (item.idPlayerInHelic == idPl)
			{
				item.exitFromHelic();
			}
		}
	}

	private void setPosPlayerInHelic()
	{
		if (objPlayerInHelic != null)
		{
			objPlayerInHelic.transform.position = base.transform.position;
			objPlayerInHelic.transform.rotation = base.transform.rotation;
			if (!settings.offlineMode)
			{
				scriptPlayerInHelic.lerpScript.sglajEnabled = true;
				scriptPlayerInHelic.lerpScript.objVistavlen = true;
			}
		}
	}

	[RPC]
	private void animationHelicUpOnline()
	{
		Debug.Log("animationHelicUpOnline");
		objHelic.animation.CrossFade(animVjigVjig.name);
		HOTween.Kill(animVjigVjig);
		HOTween.To(animVjigVjig, 2f, new TweenParms().Prop("speed", 4f).Ease(EaseType.Linear));
		CancelInvoke("StopAnimationFly");
	}

	[RPC]
	private void animationHelicDownOnline()
	{
		Debug.Log("animationHelicDownOnline");
		float num = 2f;
		HOTween.Kill(animVjigVjig);
		HOTween.To(animVjigVjig, num, new TweenParms().Prop("speed", 0f).Ease(EaseType.Linear));
		Invoke("StopAnimationFly", num);
	}

	public void SendPositionHelicOther()
	{
		if (!settings.offlineMode && base.photonView.isMine && !playerInHelic)
		{
			base.photonView.RPC("SetPositionHelicOnline", PhotonTargets.Others, base.transform.position, base.transform.rotation);
		}
	}

	[RPC]
	private void SetPositionHelicOnline(Vector3 pos, Quaternion rot)
	{
		if (!settings.offlineMode && !base.photonView.isMine && !playerInHelic)
		{
			base.transform.position = pos;
			base.transform.rotation = rot;
		}
	}

	public void SendSetIdPlayerInHelic()
	{
		if (!settings.offlineMode && base.photonView.isMine && playerInHelic)
		{
			base.photonView.RPC("setIdPlayerInHelic", PhotonTargets.Others, idPlayerInHelic, true);
		}
	}
}
