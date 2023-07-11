using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWatcher : MonoBehaviour
{
	public LayerMask collisionLayer;

	public List<GameObject> enemyList;

	public List<GameObject> enemyAvailableList;

	private BetterList<GameObject> enemiesToDelete;

	public GameObject targetPlayer;

	private Ray toEnemy;

	public GameObject test;

	public GameObject aimLabel;

	public PlayerBehavior pBehavior;

	private GUITexture aimTexture;

	private ObjectLabel oLabel;

	private bool enemyListIsDirty;

	private bool photonIsMine;

	public GameObject aimer;

	[HideInInspector]
	public bool manualAiming;

	private ManualAimerZone manualAimerZone;

	private void Start()
	{
		pBehavior = base.transform.root.GetComponent<PlayerBehavior>();
		enemyList = new List<GameObject>();
		enemyAvailableList = new List<GameObject>();
		enemiesToDelete = new BetterList<GameObject>();
		aimTexture = aimLabel.GetComponent<GUITexture>();
		oLabel = aimLabel.GetComponent<ObjectLabel>();
		photonIsMine = base.transform.root.gameObject.GetComponent<PhotonView>().isMine;
		if (photonIsMine || settings.offlineMode)
		{
			aimTexture.enabled = true;
		}
	}

	private void FixedUpdate()
	{
		CheckArea();
		updateAvailableList();
		if (!pBehavior.weaponManager.currentWeaponScript.areaDamage)
		{
			updateTarget();
			return;
		}
		aimTexture.enabled = false;
		aimer.SetActive(false);
	}

	private void updateTarget()
	{
		if (enemyAvailableList.Count != 0)
		{
			if (manualAimerZone == null)
			{
				manualAimerZone = GameObject.FindGameObjectWithTag("GUI_CameraZone").GetComponent<ManualAimerZone>();
			}
			if (manualAimerZone.touchedObject == null)
			{
				targetPlayer = getClosestEnemy(enemyAvailableList);
			}
			else
			{
				Vector3 normalized = (manualAimerZone.touchedObject.transform.position - base.transform.position).normalized;
				if (inCone(normalized, manualAimerZone.touchedObject.transform))
				{
					targetPlayer = manualAimerZone.touchedObject;
				}
			}
			if (targetPlayer != null)
			{
				aimTexture.enabled = true;
				aimer.SetActive(false);
				oLabel.target = targetPlayer.transform;
			}
			return;
		}
		targetPlayer = null;
		aimTexture.enabled = false;
		Ray ray = new Ray(base.transform.root.position + new Vector3(0f, 1f, 0f), base.transform.root.forward);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, float.PositiveInfinity, collisionLayer) && pBehavior != null && pBehavior.weaponManager.currentWeaponScript != null && !pBehavior.weaponManager.currentWeaponScript.isMelee)
		{
			if (Vector3.Distance(base.transform.position, hitInfo.point) < (float)pBehavior.weaponManager.currentWeaponScript.maxDistance)
			{
				aimer.SetActive(true);
				aimer.transform.position = hitInfo.point - new Vector3(0f, 1.45f, 0f);
			}
			else
			{
				aimer.SetActive(false);
			}
		}
		else
		{
			aimer.SetActive(false);
		}
	}

	private void updateAvailableList()
	{
		enemyAvailableList.Clear();
		foreach (GameObject enemy in enemyList)
		{
			if (enemy.activeSelf && isShootable(enemy))
			{
				enemyAvailableList.Add(enemy);
			}
		}
	}

	private bool isShootable(GameObject target)
	{
		if (target == null)
		{
			return false;
		}
		toEnemy = new Ray(base.transform.position, target.transform.position - base.transform.position);
		Debug.DrawRay(base.transform.position, target.transform.position - base.transform.position);
		RaycastHit hitInfo;
		if (Physics.Raycast(toEnemy, out hitInfo, float.PositiveInfinity, collisionLayer))
		{
			test = hitInfo.transform.gameObject;
			if ((hitInfo.transform.gameObject.transform.tag.Equals("collidePoint") || hitInfo.transform.gameObject.transform.tag.Equals("Car")) && Vector3.Distance(base.transform.position, hitInfo.transform.position) < (float)pBehavior.weaponManager.currentWeaponScript.maxDistance)
			{
				EntityBehavior entityBehavior = (hitInfo.transform.gameObject.transform.tag.Equals("collidePoint") ? ((EntityBehavior)hitInfo.transform.parent.gameObject.GetComponent(typeof(EntityBehavior))) : ((!hitInfo.transform.gameObject.transform.tag.Equals("Car")) ? null : ((EntityBehavior)hitInfo.transform.gameObject.GetComponent(typeof(EntityBehavior)))));
				if (entityBehavior != null && entityBehavior.isAlive())
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	public GameObject getClosestEnemy(List<GameObject> list)
	{
		float num = 10000f;
		GameObject gameObject = null;
		foreach (GameObject item in list)
		{
			if (Vector3.Distance(item.transform.position, base.transform.position) < num)
			{
				num = Vector3.Distance(item.transform.position, base.transform.position);
				gameObject = item;
			}
		}
		return gameObject.transform.parent.gameObject;
	}

	private bool inCone(Vector3 heading, Transform t)
	{
		float num = Mathf.Cos(pBehavior.weaponManager.currentWeaponScript.shootingAngle * ((float)Math.PI / 180f));
		return Vector3.Dot(heading, base.transform.forward) > num && Vector3.Distance(base.transform.position, t.position) < (float)pBehavior.weaponManager.currentWeaponScript.maxDistance;
	}

	private void CheckArea()
	{
		Vector3 position = base.transform.position;
		enemyList.Clear();
		if (settings.offlineMode)
		{
			foreach (EnemyBehavior item in EnemyGenerator.Instance.listEnemy)
			{
				Vector3 normalized = (item.myCollidePoint.transform.position - position).normalized;
				if (inCone(normalized, item.transform))
				{
					enemyList.Add(item.myCollidePoint);
				}
			}
		}
		else
		{
			foreach (PlayerBehavior listPlayer in GameController.thisScript.listPlayers)
			{
				if (listPlayer != null && listPlayer.gameObject.activeSelf)
				{
					Vector3 normalized = (listPlayer.myCollidePoint.transform.position - position).normalized;
					if (inCone(normalized, listPlayer.transform))
					{
						enemyList.Add(listPlayer.myCollidePoint);
					}
				}
			}
		}
		foreach (CarBehavior item2 in GameController.thisScript.arrAllCar)
		{
			Vector3 normalized = (item2.myCollidePoint.transform.position - position).normalized;
			if (inCone(normalized, item2.transform))
			{
				enemyList.Add(item2.myCollidePoint);
			}
		}
		foreach (HelicopterBehavior item3 in GameController.thisScript.listAllHelicopter)
		{
			Vector3 normalized = (item3.myCollidePoint.transform.position - position).normalized;
			if (inCone(normalized, item3.transform))
			{
				enemyList.Add(item3.myCollidePoint);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
	}
}
