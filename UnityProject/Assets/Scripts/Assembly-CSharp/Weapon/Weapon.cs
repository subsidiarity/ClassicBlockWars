using UnityEngine;

public class Weapon : MonoBehaviour
{
	public new string name;

	public string nameSpritePreview;

	public bool showWeaponThanNullBullets;

	public bool isMelee;

	public bool throwingWeapon;

	public bool rocketWeapon;

	public bool saveCountBullets;

	public bool showFullInfoAmmo = true;

	public bool automaticReload;

	public bool areaDamage;

	public int bulletAllCount;

	public int bulletPackCount;

	public int damage;

	public int maxDistance = 100;

	public int addCountBulletBonuse;

	public int addCountBulletShop;

	public float accuracyWeapon = 100f;

	public float accuracyMaxDistance = 100f;

	public float timeDisabledShootParticle = 0.1f;

	public string[] shootAnimArray;

	public int currentShootIndex;

	private UILabel ammoLabel;

	public weaponGrenade scriptGrenade;

	public weaponGrenade curGrenade;

	private float timeDelayThrowingWeapon;

	public GameObject shootParticle;

	private PhotonView photonView;

	private Animation weaponAnimation;

	public WeaponManager weaponManager;

	public AudioClip soundHit;

	public AudioClip soundReload;

	public AudioClip soundShoot;

	public AudioClip soundEmpty;

	public float shootingAngle = 30f;

	public int mobility = 100;

	public GameObject pointForThrowingWeapon;

	private PlayerBehavior currentPlayer;

	public bool equipped
	{
		get
		{
			return Load.LoadBool(settings.keyEquipGun + name);
		}
		set
		{
			Save.SaveBool(settings.keyEquipGun + name, value);
		}
	}

	private void Start()
	{
		currentPlayer = base.transform.root.gameObject.GetComponent<PlayerBehavior>();
		if (!settings.offlineMode)
		{
			photonView = base.transform.root.GetComponent<PhotonView>();
		}
		weaponAnimation = GetComponent<Animation>();
		if (base.transform.root.GetComponent<ThirdPersonController>() != null)
		{
			base.transform.root.GetComponent<ThirdPersonController>().weaponAnimationObject = base.gameObject;
		}
		ammoLabel = GameController.thisScript.lbAmmo;
		if (shootParticle != null)
		{
			shootParticle.SetActive(false);
		}
		weaponManager = base.transform.root.GetComponent<WeaponManager>();
		if (shootParticle != null)
		{
			shootParticle.SetActive(false);
		}
		if (currentPlayer != null && (settings.offlineMode || currentPlayer.photonView.isMine))
		{
			if (scriptGrenade != null && scriptGrenade.isDetonator)
			{
				GameController.thisScript.playerScript.CheckActiveButDetonator(scriptGrenade);
			}
			else
			{
				GameController.thisScript.playerScript.otklButDetonator();
			}
		}
		createThrowObj();
	}

	private void Update()
	{
		if (throwingWeapon)
		{
			CheckActiveGrenade();
		}
		if (automaticReload)
		{
			AutomaticReloadCurrentWeapon();
		}
	}

	public void createThrowObj()
	{
		if ((settings.offlineMode || (!(currentPlayer.photonView == null) && currentPlayer.photonView.isMine)) && (!(weaponManager != null) || weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount >= 0) && throwingWeapon && curGrenade == null && scriptGrenade != null && pointForThrowingWeapon != null)
		{
			GameObject gameObject;
			if (settings.offlineMode)
			{
				gameObject = Object.Instantiate(scriptGrenade.gameObject) as GameObject;
				gameObject.transform.position = pointForThrowingWeapon.transform.position;
			}
			else
			{
				gameObject = PhotonNetwork.Instantiate("Grenade/" + scriptGrenade.name, pointForThrowingWeapon.transform.position, Quaternion.identity, 0);
			}
			curGrenade = gameObject.GetComponent<weaponGrenade>();
			gameObject.transform.parent = pointForThrowingWeapon.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			timeDelayThrowingWeapon = curGrenade.timeBeforeCreateNextObj;
		}
	}

	public void CheckActiveGrenade()
	{
		if ((settings.offlineMode || (!(currentPlayer.photonView == null) && currentPlayer.photonView.isMine)) && (!(weaponManager != null) || weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount >= 0) && curGrenade != null)
		{
			if (weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount <= 0 && curGrenade.gameObject.activeSelf)
			{
				curGrenade.gameObject.SetActive(false);
			}
			if (weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount > 0 && !curGrenade.gameObject.activeSelf)
			{
				curGrenade.gameObject.SetActive(true);
			}
		}
	}

	private bool isWeaponReady()
	{
		if (weaponAnimation == null)
		{
			weaponAnimation = GetComponent<Animation>();
			ammoLabel = GameController.thisScript.lbAmmo;
			return true;
		}
		return true;
	}

	public void hit()
	{
		if (isWeaponReady() && soundHit != null && settings.soundEnabled)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundHit);
		}
	}

	public void broadcastHit()
	{
		if (!settings.offlineMode && !currentPlayer.photonView.isMine && !shootIsPlaying() && settings.soundEnabled)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundHit);
		}
	}

	public void walk()
	{
		if (isWeaponReady() && !weaponAnimation.IsPlaying("Walk"))
		{
			weaponAnimation.Play("Walk");
		}
	}

	public void idle()
	{
		if (isWeaponReady() && !weaponAnimation.isPlaying)
		{
			weaponAnimation.Play("Idle");
		}
	}

	public void empty()
	{
		if (isWeaponReady() && !weaponAnimation.isPlaying)
		{
			base.GetComponent<AudioSource>().PlayOneShot(soundEmpty);
			weaponAnimation.Play("Empty");
		}
	}

	public void reload()
	{
		if (isWeaponReady() && bulletAllCount > 0 && !weaponAnimation.IsPlaying("Reload"))
		{
			if (settings.soundEnabled)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundReload);
			}
			weaponAnimation.Play("Reload");
		}
	}

	private void disableParticle()
	{
		shootParticle.SetActive(false);
	}

	private void nextShootAnim()
	{
		currentShootIndex++;
		if (currentShootIndex > shootAnimArray.Length - 1)
		{
			currentShootIndex = 0;
		}
	}

	private bool shootIsPlaying()
	{
		if (weaponAnimation.IsPlaying("Shoot") || weaponAnimation.IsPlaying("Shoot1"))
		{
			return true;
		}
		return false;
	}

	public void broadcastShoot()
	{
		if (currentPlayer.photonView != null && !settings.offlineMode && !currentPlayer.photonView.isMine && !shootIsPlaying())
		{
			if (shootParticle != null)
			{
				shootParticle.SetActive(false);
				shootParticle.SetActive(true);
				CancelInvoke("disableParticle");
				Invoke("disableParticle", timeDisabledShootParticle);
			}
			weaponAnimation.Play("Shoot");
			if (settings.soundEnabled)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundShoot);
			}
		}
	}

	public void broadcastReload()
	{
		if (currentPlayer.photonView != null && !settings.offlineMode && !currentPlayer.photonView.isMine && !weaponAnimation.IsPlaying("Reload"))
		{
			if (settings.soundEnabled)
			{
				base.GetComponent<AudioSource>().PlayOneShot(soundReload);
			}
			weaponAnimation.Play("Reload");
		}
	}

	public void shoot()
	{
		if (!isWeaponReady())
		{
			return;
		}
		if (weaponManager == null)
		{
			weaponManager = base.transform.root.GetComponent<WeaponManager>();
		}
		if (currentPlayer == null)
		{
			currentPlayer = base.transform.root.gameObject.GetComponent<PlayerBehavior>();
		}
		if (!shootIsPlaying() && !weaponAnimation.IsPlaying("Reload"))
		{
			if (rocketWeapon && weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].getCurrentBulletsToPack() > 0)
			{
				if (shootParticle != null)
				{
					shootParticle.SetActive(false);
					shootParticle.SetActive(true);
					Invoke("otklPartiklShoot", scriptGrenade.timeBeforeCreateNextObj);
				}
				if (settings.soundEnabled)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundShoot);
				}
				weaponAnimation.Play("Shoot");
				if (!settings.offlineMode && !currentPlayer.photonView.isMine)
				{
					return;
				}
				minusOneBullet();
				if (scriptGrenade != null && pointForThrowingWeapon != null)
				{
					GameObject gameObject;
					if (settings.offlineMode)
					{
						gameObject = (GameObject)Object.Instantiate(scriptGrenade.gameObject);
						gameObject.transform.position = pointForThrowingWeapon.transform.position;
						gameObject.transform.rotation = GameController.thisScript.myPlayer.transform.rotation;
					}
					else
					{
						gameObject = PhotonNetwork.Instantiate("Grenade/" + scriptGrenade.name, pointForThrowingWeapon.transform.position, GameController.thisScript.myPlayer.transform.rotation, 0);
					}
					curGrenade = gameObject.GetComponent<weaponGrenade>();
					if (curGrenade != null)
					{
						curGrenade.throwGrenade();
					}
					curGrenade = null;
					currentPlayer.shoot(0);
				}
			}
			else if (isMelee)
			{
				if (settings.soundEnabled)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundShoot);
				}
				weaponAnimation.Play(shootAnimArray[currentShootIndex]);
				nextShootAnim();
				currentPlayer.shoot(damage);
				ammoLabel.text = string.Empty;
			}
			else if (throwingWeapon)
			{
				if (curGrenade != null)
				{
					CheckActiveGrenade();
					if (weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount <= 0)
					{
						return;
					}
					if (shootParticle != null)
					{
						shootParticle.SetActive(false);
						shootParticle.SetActive(true);
					}
					if (settings.soundEnabled)
					{
						base.GetComponent<AudioSource>().PlayOneShot(soundShoot);
					}
					weaponAnimation.Play("Shoot");
					if (!settings.offlineMode && !currentPlayer.photonView.isMine)
					{
						return;
					}
					if (curGrenade != null)
					{
						weaponGrenade weaponGrenade2 = curGrenade;
						minusOneBullet();
						curGrenade.throwGrenade();
						curGrenade = null;
						currentPlayer.shoot(0);
						if (weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount >= 0)
						{
							Invoke("createThrowObj", weaponGrenade2.timeBeforeCreateNextObj);
						}
					}
				}
			}
			else if (!weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].needReload)
			{
				if (shootParticle != null)
				{
					shootParticle.SetActive(false);
					shootParticle.SetActive(true);
				}
				if (settings.soundEnabled)
				{
					base.GetComponent<AudioSource>().PlayOneShot(soundShoot);
				}
				weaponAnimation.Play("Shoot");
				currentPlayer.shoot(damage);
				minusOneBullet();
				CancelInvoke("disableParticle");
				Invoke("disableParticle", timeDisabledShootParticle);
			}
			else if (weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].getRestBullets() > 0)
			{
				reload();
				weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].reloadBullets();
				currentPlayer.reload();
			}
			else
			{
				empty();
			}
			showCountBulletCurrentWeapon();
		}
		switchNextWeapon();
	}

	private void showCountBulletCurrentWeapon()
	{
		if (!isMelee)
		{
			if (showFullInfoAmmo)
			{
				ammoLabel.text = weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].getCountBullets();
			}
			else
			{
				ammoLabel.text = weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].getCountBulletsOnly();
			}
		}
	}

	private void AutomaticReloadCurrentWeapon()
	{
		if (automaticReload && weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].needReload && weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].getRestBullets() > 0 && !shootIsPlaying() && !weaponAnimation.IsPlaying("Reload"))
		{
			reload();
			weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].reloadBullets();
			base.transform.root.gameObject.GetComponent<PlayerBehavior>().reload();
			showCountBulletCurrentWeapon();
		}
	}

	public void switchNextWeapon()
	{
		if ((!(scriptGrenade != null) || !GameController.thisScript.playerScript.GetActiveButDetonator(scriptGrenade)) && weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].bulletAllCount <= 0 && !showWeaponThanNullBullets)
		{
			float num = 0.2f;
			if (throwingWeapon)
			{
				num += timeDelayThrowingWeapon;
			}
			Invoke("switchNextWeaponWithDelay", num);
		}
	}

	private void switchNextWeaponWithDelay()
	{
		weaponManager.switchToNext();
	}

	public void initSavedBullets()
	{
		if (saveCountBullets && !Load.LoadBool(settings.keyBulletsSaved + base.gameObject.name))
		{
			Save.SaveInt(settings.keyCountBullets + name, bulletAllCount);
			Save.SaveBool(settings.keyBulletsSaved + name, true);
		}
	}

	public void minusOneBullet()
	{
		if (saveCountBullets)
		{
			Save.SaveInt(settings.keyCountBullets + name, Load.LoadInt(settings.keyCountBullets + name) - 1);
		}
		weaponManager.myWeaponsBullets[weaponManager.currentWeaponIndex].minusOneBullet();
	}

	public void otklPartiklShoot()
	{
		if (shootParticle != null)
		{
			shootParticle.SetActive(false);
		}
	}

	public void RemoveWeapon()
	{
		if (curGrenade != null)
		{
			curGrenade.remove();
		}
		Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		CancelInvoke("minusOneBullet");
		CancelInvoke("createThrowObj");
	}
}
