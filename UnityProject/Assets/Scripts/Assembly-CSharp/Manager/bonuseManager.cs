using System.Collections.Generic;
using Photon;
using UnityEngine;

public class bonuseManager : Photon.MonoBehaviour
{
	public static bonuseManager thisScript;

	private GameObject bonuseRespawn;

	private GameObject spisokBonuse;

	private PlayerBehavior playerBeh;

	private WeaponManager weaponMan;

	public BoxCollider[] arrBonuseRespawn;

	public List<GameObject> listBonuse = new List<GameObject>();

	private GameObject removeBonuse;

	private string[] arrNamePrefBonuse = new string[2] { "Bullet", "Health" };

	public AudioClip soundBonuse;

	private float distGetBonuse = 1.5f;

	private void Awake()
	{
		if (settings.offlineMode || base.photonView.isMine)
		{
			GameController.thisScript.bonuseManagerScript = this;
			thisScript = this;
		}
		else
		{
			thisScript = null;
		}
	}

	private void Start()
	{
		bonuseRespawn = GameController.thisScript.bonuseRespawn;
		spisokBonuse = GameController.thisScript.spisokBonuse;
		playerBeh = base.gameObject.GetComponent<PlayerBehavior>();
		weaponMan = base.gameObject.GetComponent<WeaponManager>();
		if (settings.offlineMode || base.photonView.isMine)
		{
			createArrBonuseRespawn();
			listBonuse.Clear();
			GameObject[] array = GameObject.FindGameObjectsWithTag("BonusBullet");
			foreach (GameObject item in array)
			{
				listBonuse.Add(item);
			}
			GameObject[] array2 = GameObject.FindGameObjectsWithTag("BonusHealth");
			foreach (GameObject item2 in array2)
			{
				listBonuse.Add(item2);
			}
			InvokeRepeating("addNewBonuse", settings.timeAddBonuse, settings.timeAddBonuse);
		}
		else
		{
			listBonuse = GameController.thisScript.bonuseManagerScript.listBonuse;
		}
	}

	public void createArrBonuseRespawn()
	{
		arrBonuseRespawn = bonuseRespawn.GetComponentsInChildren<BoxCollider>();
	}

	public void addNewBonuse()
	{
		if ((settings.offlineMode || base.photonView.isMine) && listBonuse.Count < settings.maxKolBonuse)
		{
			BoxCollider boxCollider = arrBonuseRespawn[Random.Range(0, arrBonuseRespawn.Length)];
			float num = boxCollider.size.x * boxCollider.transform.localScale.x;
			float num2 = boxCollider.size.z * boxCollider.transform.localScale.z;
			Vector3 position = new Vector3(boxCollider.transform.position.x + Random.Range((0f - num) * 0.5f, num * 0.5f), boxCollider.transform.position.y, boxCollider.transform.position.z + Random.Range((0f - num2) * 0.5f, num2 * 0.5f));
			string text = arrNamePrefBonuse[Random.Range(0, arrNamePrefBonuse.Length)];
			Object @object = Resources.Load("Bonuse/" + text);
			if (@object != null)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(@object, position, Quaternion.identity);
				listBonuse.Add(gameObject);
				gameObject.transform.parent = spisokBonuse.transform;
			}
			else
			{
				Debug.Log("error create bonuse");
			}
		}
	}

	private void FixedUpdate()
	{
		getNearBonuse(base.gameObject, distGetBonuse);
	}

	public void getNearBonuseInCar(GameObject objCar, float radius)
	{
		getBonuse(objCar, radius, true);
	}

	public void getNearBonuse(GameObject objPlayer, float radius)
	{
		getBonuse(objPlayer, radius, false);
	}

	private void getBonuse(GameObject objPlayer, float radius, bool inCar)
	{
		foreach (GameObject item in listBonuse)
		{
			if (!(Vector3.Distance(item.transform.position, objPlayer.transform.position) < radius))
			{
				continue;
			}
			string text = item.tag;
			if (!settings.offlineMode && !base.photonView.isMine && (text == "BonusBullet" || text == "BonusHealth"))
			{
				removeBonuse = item;
				break;
			}
			if (text == "BonusBullet")
			{
				if (inCar)
				{
					CarBehavior component = objPlayer.GetComponent<CarBehavior>();
					int num = component.countBulletCar + 5;
					if (settings.offlineMode)
					{
						component.setCountBullets(num);
					}
					else
					{
						component.photonView.RPC("setCountBullets", PhotonTargets.All, num);
					}
					removeBonuse = item;
					break;
				}
				if (!weaponMan.currentWeaponScript.isMelee && !weaponMan.currentWeaponScript.throwingWeapon)
				{
					Debug.Log("get ammo");
					weaponMan.getBulletBonus();
					removeBonuse = item;
					break;
				}
			}
			if (text == "BonusHealth")
			{
				Debug.Log("get health");
				int num2 = playerBeh.Health + 20;
				if (num2 > 100)
				{
					num2 = 100;
				}
				playerBeh.Health = num2;
				removeBonuse = item;
				break;
			}
		}
		if (removeBonuse != null)
		{
			listBonuse.Remove(removeBonuse);
			Object.Destroy(removeBonuse);
			playSoundBonuse();
			removeBonuse = null;
		}
	}

	private void playSoundBonuse()
	{
		if ((settings.offlineMode || base.photonView.isMine) && settings.soundEnabled)
		{
			NGUITools.PlaySound(soundBonuse);
		}
	}
}
