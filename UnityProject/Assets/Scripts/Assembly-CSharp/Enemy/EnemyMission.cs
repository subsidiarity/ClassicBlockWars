using UnityEngine;

public class EnemyMission : MonoBehaviour
{
	private EnemyBehavior eBehavior;

	private bool wasKilled;

	private bool enemyCanBeKilled = true;

	[HideInInspector]
	public Destroyed how;

	private void OnEnable()
	{
		eBehavior = GetComponent<EnemyBehavior>();
	}

	private void Update()
	{
		if (!(eBehavior != null) || !enemyCanBeKilled || !eBehavior.isDead)
		{
			return;
		}
		int num = 0;
		switch (how)
		{
		case Destroyed.ByGun:
			num = MissionManager.Instance.GetComponent<DestroyAllEnemies>().GetMissionParam<int>("Enemies");
			MissionManager.Instance.GetComponent<DestroyAllEnemies>().SetMissionParam("Enemies", --num);
			break;
		case Destroyed.ByCar:
			if (GameController.thisScript.playerScript.inCar && !GameController.thisScript.carScript.carWithWeapon)
			{
				num = MissionManager.Instance.GetComponent<Carmageddon>().GetMissionParam<int>("Enemies");
				MissionManager.Instance.GetComponent<Carmageddon>().SetMissionParam("Enemies", --num);
			}
			break;
		case Destroyed.ByTank:
			if (GameController.thisScript.playerScript.inCar && GameController.thisScript.carScript.carWithWeapon)
			{
				num = MissionManager.Instance.GetComponent<TankDestroyer>().GetMissionParam<int>("Enemies");
				MissionManager.Instance.GetComponent<TankDestroyer>().SetMissionParam("Enemies", --num);
			}
			break;
		}
		enemyCanBeKilled = false;
		Invoke("Reset", 2f);
	}

	private void OnDestroy()
	{
		CancelInvoke("Reset");
	}

	private void Reset()
	{
		if (this != null)
		{
			enemyCanBeKilled = true;
		}
	}
}
