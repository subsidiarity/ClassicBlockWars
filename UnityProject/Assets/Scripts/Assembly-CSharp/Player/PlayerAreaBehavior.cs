using System.Collections.Generic;
using UnityEngine;

public class PlayerAreaBehavior : MonoBehaviour
{
	private GameObject objPlayer;

	public CarBehavior nearCar;

	public HelicopterBehavior nearHelicopter;

	public List<CarBehavior> listNearCar = new List<CarBehavior>();

	public List<HelicopterBehavior> listNearHelicopter = new List<HelicopterBehavior>();

	private PhotonView photonView;

	private float distanceFindCar = 4.5f;

	private float lastTime;

	private float timeDelay = 0.3f;

	private void Start()
	{
		objPlayer = base.gameObject;
		photonView = objPlayer.GetComponent<PhotonView>();
		lastTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (lastTime + timeDelay < Time.realtimeSinceStartup)
		{
			updateListNearCar();
			lastTime = Time.realtimeSinceStartup;
		}
	}

	private void updateListNearCar()
	{
		foreach (CarBehavior item in GameController.thisScript.arrAllCar)
		{
			if (Vector3.Distance(item.transform.position, objPlayer.transform.position) < distanceFindCar)
			{
				if (!listNearCar.Contains(item))
				{
					listNearCar.Add(item);
				}
			}
			else if (listNearCar.Contains(item))
			{
				listNearCar.Remove(item);
			}
		}
		foreach (HelicopterBehavior item2 in GameController.thisScript.listAllHelicopter)
		{
			if (Vector3.Distance(item2.transform.position, objPlayer.transform.position) < distanceFindCar)
			{
				if (!listNearHelicopter.Contains(item2))
				{
					listNearHelicopter.Add(item2);
				}
			}
			else if (listNearHelicopter.Contains(item2))
			{
				listNearHelicopter.Remove(item2);
			}
		}
		findCar();
	}

	private void findCar()
	{
		if ((settings.offlineMode || photonView.isMine) && (!settings.offlineMode || settings.isLearned))
		{
			nearCar = findNearCar();
			nearHelicopter = findNearHelicopter();
			bool flag = false;
			bool flag2 = false;
			flag = ((nearCar != null && !nearCar.playerInCar) ? true : false);
			flag2 = ((nearHelicopter != null && !nearHelicopter.playerInHelic) ? true : false);
			if (!GameController.thisScript.butInCar.activeSelf && (flag || flag2))
			{
				GameController.thisScript.butInCar.SetActive(true);
			}
			if (GameController.thisScript.butInCar.activeSelf && !flag && !flag2)
			{
				GameController.thisScript.butInCar.SetActive(false);
			}
		}
	}

	private HelicopterBehavior findNearHelicopter()
	{
		float num = 10000f;
		HelicopterBehavior result = null;
		foreach (HelicopterBehavior item in listNearHelicopter)
		{
			if (item != null && !item.isDead)
			{
				float num2 = Vector3.Distance(item.transform.position, objPlayer.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	private CarBehavior findNearCar()
	{
		float num = 10000f;
		CarBehavior result = null;
		foreach (CarBehavior item in listNearCar)
		{
			if (item != null && !item.isDead)
			{
				float num2 = Vector3.Distance(item.transform.position, objPlayer.transform.position);
				if (num2 < num)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	public bool HelicBlijeCar()
	{
		if (nearHelicopter != null)
		{
			if (nearCar != null)
			{
				if (Vector3.Distance(nearHelicopter.transform.position, objPlayer.transform.position) < Vector3.Distance(nearCar.transform.position, objPlayer.transform.position))
				{
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}
}
