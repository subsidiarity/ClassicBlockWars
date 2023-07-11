using Holoville.HOTween;
using Photon;
using UnityEngine;

public class weaponGrenade : Photon.MonoBehaviour
{
	public bool isRocket;

	public bool damageAtCollision;

	public bool isDetonator;

	public LayerMask collisionLayer;

	public float explosionTimer = 3f;

	public float timeDelay = 0.7f;

	public float timeBeforeCreateNextObj = 1f;

	public float timeBetweenDamage = 0.5f;

	public float forceThrow = 100f;

	public areaDamage[] arrDamage;

	public AudioClip soundShoot;

	public GameObject particlExplosion;

	public GameObject particlShoot;

	public GameObject objGrenade;

	private PhotonView photonPlayer;

	public int idPlayer;

	private bool IsStartLongDamage;

	private bool isThrow;

	private EntityBehavior target;

	private SphereCollider colliderGrenade;

	private void Start()
	{
		HOTween.Init();
		colliderGrenade = GetComponent<SphereCollider>();
		if (base.photonView.isMine || !isThrow)
		{
		}
		if (!settings.offlineMode)
		{
			if (!base.photonView.isMine)
			{
				colliderGrenade.enabled = false;
			}
			photonPlayer = GameController.thisScript.playerScript.photonView;
			idPlayer = photonPlayer.viewID;
		}
	}

	private void Update()
	{
		if (target != null && target.isDead)
		{
			remove();
		}
	}

	public void throwGrenade()
	{
		if (isRocket)
		{
			Shoot();
		}
		else
		{
			Invoke("throwGrenadeWithDelay", timeDelay);
		}
	}

	private void Shoot()
	{
		if (particlShoot != null)
		{
			particlShoot.SetActive(true);
		}
		if (settings.soundEnabled && soundShoot != null)
		{
			base.audio.PlayOneShot(soundShoot);
		}
		if ((settings.offlineMode || base.photonView.isMine) && GameController.thisScript.playerScript.currentTarget != null)
		{
			Vector3 worldPosition = GameController.thisScript.playerScript.currentTarget.transform.position + new Vector3(0f, 1.45f, 0f);
			base.transform.LookAt(worldPosition);
		}
		base.rigidbody.isKinematic = false;
		InvokeRepeating("constantForceForFly", 0f, 1f);
		isThrow = true;
		Invoke("ExplosionRocket", explosionTimer);
	}

	private void constantForceForFly()
	{
		if (isRocket)
		{
			base.rigidbody.velocity = Vector3.zero;
			base.rigidbody.AddRelativeForce(new Vector3(0f, 0f, 35f), ForceMode.VelocityChange);
		}
	}

	private void throwGrenadeWithDelay()
	{
		IsStartLongDamage = false;
		if (settings.offlineMode || base.photonView.isMine)
		{
			if (!settings.offlineMode)
			{
				base.photonView.RPC("GrenadeThrowOnline", PhotonTargets.AllBuffered);
			}
			else
			{
				GrenadeThrowOnline();
			}
			base.gameObject.transform.localRotation = GameController.thisScript.myPlayer.transform.localRotation;
			base.rigidbody.isKinematic = false;
			base.rigidbody.AddRelativeForce(new Vector3(0f, 1f, 3.5f) * forceThrow, ForceMode.Impulse);
			base.rigidbody.angularVelocity = new Vector3(0f, 1f, 3f) * 90f;
			if (isDetonator)
			{
				GameController.thisScript.playerScript.AddDentonatorGrenadeToList(this);
			}
			if (!damageAtCollision || isDetonator)
			{
				Invoke("getDamage", explosionTimer);
			}
		}
	}

	[RPC]
	private void GrenadeThrowOnline()
	{
		isThrow = true;
		base.transform.parent = null;
	}

	public void getDamage()
	{
		if (colliderGrenade != null && colliderGrenade.enabled)
		{
			colliderGrenade.enabled = false;
		}

		if (!settings.offlineMode && !base.photonView.isMine)
		{
			return;
		}

		foreach (areaDamage area_damage in arrDamage)
		{
			ExploisonManager.Explode(
				gameObject.transform.position,
				area_damage.radius,
				area_damage.damage,
				idPlayer
			);
		}

		if (isRocket)
		{
			CancelInvoke("getDamage");
			CancelInvoke("constantForceForFly");
		}

		if (!damageAtCollision || isRocket || isDetonator)
		{
			if (settings.offlineMode)
			{
				explosion();
			}
			else if (base.photonView.isMine)
			{
				base.photonView.RPC("explosion", PhotonTargets.All);
			}
		}
	}

	[RPC]
	private void explosion()
	{
		if (colliderGrenade != null && colliderGrenade.enabled)
		{
			colliderGrenade.enabled = false;
		}

		if (particlExplosion != null)
		{
			Object.Instantiate(particlExplosion, base.transform.position, Quaternion.identity);
		}

		if (objGrenade != null)
		{
			objGrenade.SetActive(false);
		}

		Invoke("remove", 1f);
	}

	public void remove()
	{
		if (settings.offlineMode)
		{
			Object.Destroy(base.gameObject);
		}
		else if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}

		if (isDetonator)
		{
			GameController.thisScript.playerScript.RemoveGrenadeFromList(this);
		}
	}

	public void RemoveByDisconnect()
	{
		if (settings.offlineMode)
		{
			Object.Destroy(base.gameObject);
		}
		else if (base.photonView.isMine)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!damageAtCollision || !isThrow || isRocket || IsStartLongDamage)
		{
			return;
		}
		Debug.Log("OnCollisionEnter = " + collision.gameObject.tag);
		if (!settings.offlineMode && !base.photonView.isMine)
		{
			return;
		}
		ContactPoint[] contacts = collision.contacts;
		for (int i = 0; i < contacts.Length; i++)
		{
			ContactPoint contactPoint = contacts[i];
			if (IsStartLongDamage)
			{
				break;
			}
			IsStartLongDamage = true;
			base.gameObject.transform.localRotation = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
			if (isDetonator)
			{
				Debug.Log("isDetonator");
				target = collision.gameObject.GetComponent<EntityBehavior>();
				base.gameObject.transform.parent = collision.gameObject.transform;
				colliderGrenade.enabled = false;
				base.rigidbody.isKinematic = true;
				if (colliderGrenade != null && colliderGrenade.enabled)
				{
					colliderGrenade.enabled = false;
				}
			}
			else if (settings.offlineMode)
			{
				StartLongDamage(contactPoint.point, contactPoint.normal);
			}
			else if (base.photonView.isMine)
			{
				base.photonView.RPC("StartLongDamage", PhotonTargets.All, contactPoint.point, contactPoint.normal);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isRocket && !isDetonator)
		{
			return;
		}
		if (isDetonator)
		{
			if (colliderGrenade == null)
			{
				colliderGrenade = GetComponent<SphereCollider>();
			}
			if (colliderGrenade == null)
			{
				return;
			}
			colliderGrenade.isTrigger = false;
			if (other.tag.Equals("enemy"))
			{
				EnemyBehavior component = other.GetComponent<EnemyBehavior>();
				if (component != null)
				{
					component.ChangeTriggerColliderEnemy();
				}
			}
		}
		else if (GameController.thisScript.myPlayer != null && !other.gameObject.Equals(GameController.thisScript.myPlayer))
		{
			ExplosionRocket();
		}
	}

	private void ExplosionRocket()
	{
		getDamage();
		base.rigidbody.isKinematic = false;
		base.rigidbody.velocity = Vector3.zero;
		base.rigidbody.isKinematic = true;
	}

	[RPC]
	private void StartLongDamage(Vector3 point, Vector3 vectorNormal)
	{
		base.rigidbody.isKinematic = true;
		if (colliderGrenade != null && colliderGrenade.enabled)
		{
			colliderGrenade.enabled = false;
		}
		if (particlExplosion != null)
		{
			GameObject gameObject = Object.Instantiate(particlExplosion, point, Quaternion.FromToRotation(Vector3.up, vectorNormal)) as GameObject;
			gameObject.transform.parent = base.gameObject.transform;
		}
		if (objGrenade != null)
		{
			objGrenade.SetActive(false);
		}
		if (settings.offlineMode || base.photonView.isMine)
		{
			Invoke("EndLongDamage", explosionTimer);
			InvokeRepeating("getDamage", timeBetweenDamage, timeBetweenDamage);
		}
	}

	private void EndLongDamage()
	{
		CancelInvoke("getDamage");
		IsStartLongDamage = false;
		remove();
	}
}
