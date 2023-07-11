using System;
using UnityEngine;

public class Carmageddon : DestroyAllEnemies
{
	private const int MAX_ENEMY_COUNT = 20;

	private int timeLimit;

	private bool wasKilled;

	private UILabel indicatorLabel;

	private UISprite indicatorSprite;

	private GameObject getInCarLabel;

	private GameObject car;

	private UILabel timeLabel;

	private GameObject panelTime;

	public Carmageddon()
	{
		mTitle = "Car armageddon";
		mDescription = "Kill 20 enemies with car\nin a limited time.";
		mId = MissionID.Carmageddon;
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
		Debug.Log(getInCarLabel);
		getInCarLabel = MissionManager.Instance.centralLabel;
		getInCarLabel.GetComponent<UILabel>().text = "Get in to the car and crush the enemies!";
		getInCarLabel.SetActive(true);
		car = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Cars/CarSport"), new Vector3(-16f + GameController.thisScript.myPlayer.transform.position.x, 0f, GameController.thisScript.myPlayer.transform.position.z), Quaternion.identity);
		car.transform.parent = GameController.thisScript.spisokCars.transform;
	}

	public override void OnMissionEnd()
	{
		base.OnMissionEnd();
		getInCarLabel.GetComponent<UILabel>().text = string.Empty;
		getInCarLabel.SetActive(false);
		if (GameController.thisScript.playerScript.inCar && GameController.thisScript.carScript.gameObject.Equals(car))
		{
			GameController.thisScript.playerScript.GetOutOfCar();
		}
		UnityEngine.Object.Destroy(car);
	}

	public new Type GetMissionType()
	{
		return typeof(Carmageddon);
	}

	protected new virtual void ResetEnemies()
	{
		EnemyGenerator.Instance.makeHellNotAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			UnityEngine.Object.Destroy(item.GetComponent<EnemyMission>());
		}
	}

	protected override void InitEnemies()
	{
		EnemyGenerator.Instance.makeHellNotAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			item.AddComponent<EnemyMission>().how = Destroyed.ByCar;
		}
	}

	public override void OnMission()
	{
		base.OnMission();
		bool flag = GameController.thisScript.playerScript.inCar && !GameController.thisScript.carScript.carWithWeapon;
		getInCarLabel.SetActive(!flag);
	}
}
