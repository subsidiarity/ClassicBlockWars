using System;
using UnityEngine;

public class TankDestroyer : Mission
{
	private const int MAX_ENEMY_COUNT = 15;

	private int timeLimit;

	private bool wasKilled;

	private UILabel indicatorLabel;

	private UISprite indicatorSprite;

	private GameObject getInCarLabel;

	private UILabel timeLabel;

	private GameObject panelTime;

	private GameObject tank;

	public TankDestroyer()
	{
		mTitle = "Tank Madness";
		mDescription = "Kill 15 enemies with Tank\nin a limited time.";
		mId = MissionID.TankDestroyer;
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
		SetMissionParam("Enemies", 15);
		panelTime = MissionManager.Instance.panelTime;
		if (panelTime != null)
		{
			panelTime.SetActive(true);
			timeLabel = panelTime.transform.Find("LabelTimeGame").GetComponent<UILabel>();
			timeLimit = 120;
			bool flag = timeLimit % 60 < 10;
			string text = timeLimit / 60 + ":" + ((!flag) ? (string.Empty + timeLimit % 60) : ("0" + timeLimit % 60));
			timeLabel.text = text;
			InvokeRepeating("DecTime", 1f, 1f);
		}
		MissionManager.Instance.indicatorPoints.SetActive(true);
		indicatorSprite = MissionManager.Instance.indicatorPoints.transform.Find("icon_Points").GetComponent<UISprite>();
		indicatorLabel = MissionManager.Instance.indicatorPoints.transform.Find("LabelPoints").GetComponent<UILabel>();
		if (indicatorSprite != null)
		{
			indicatorSprite.spriteName = "icon_bot";
		}
		InitEnemies();
		SetMissionParam("Time", 300);
		getInCarLabel = MissionManager.Instance.centralLabel;
		getInCarLabel.GetComponent<UILabel>().text = "Get in to the tank and crush the enemies!";
		getInCarLabel.SetActive(true);
		InitTank();
	}

	protected void CheckMission()
	{
		int num = 15 - GetMissionParam<int>("Enemies");
		rateStars = 0;
		bool flag = true;
		switch (num)
		{
		case 15:
			rateStars = 3;
			flag = false;
			break;
		case 12:
		case 13:
		case 14:
			rateStars = 2;
			flag = false;
			break;
		default:
			if (num >= 8 && num < 12)
			{
				rateStars = 1;
				flag = false;
			}
			break;
		}
		if (flag)
		{
			SwitchStatus(MissionStatus.MissionFailed);
		}
		else
		{
			SwitchStatus(MissionStatus.MissionCompleted);
		}
	}

	public void DecTime()
	{
		if (timeLimit - 1 > 0)
		{
			timeLimit--;
			bool flag = timeLimit % 60 < 10;
			string text = timeLimit / 60 + ":" + ((!flag) ? (string.Empty + timeLimit % 60) : ("0" + timeLimit % 60));
			timeLabel.text = text;
		}
		else
		{
			CancelInvoke("DecTime");
			CheckMission();
		}
	}

	public override void OnMissionEnd()
	{
		base.OnMissionEnd();
		MissionManager.Instance.indicatorPoints.SetActive(false);
		panelTime.SetActive(false);
		ResetEnemies();
		getInCarLabel.GetComponent<UILabel>().text = string.Empty;
		getInCarLabel.SetActive(false);
		if (GameController.thisScript.playerScript.inCar && GameController.thisScript.myCar.Equals(tank))
		{
			GameController.thisScript.playerScript.GetOutOfCar();
		}
		UnityEngine.Object.Destroy(tank);
	}

	public Type GetMissionType()
	{
		return typeof(Carmageddon);
	}

	protected void ResetEnemies()
	{
		EnemyGenerator.Instance.makeHellNotAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			UnityEngine.Object.Destroy(item.GetComponent<EnemyMission>());
		}
	}

	protected void InitTank()
	{
		tank = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Cars/CarTank"), new Vector3(-16f + GameController.thisScript.myPlayer.transform.position.x, 0f, GameController.thisScript.myPlayer.transform.position.z), Quaternion.identity);
		tank.transform.parent = GameController.thisScript.spisokCars.transform;
		tank.GetComponent<CarBehavior>().setCountBullets(35);
	}

	protected void InitEnemies()
	{
		EnemyGenerator.Instance.makeHellAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			item.AddComponent<EnemyMission>().how = Destroyed.ByTank;
		}
	}

	public override void OnMissionComplete()
	{
		base.OnMissionComplete();
		mDescription = "Destroyed enemies: " + (15 - GetMissionParam<int>("Enemies")) + "/" + 15 + "\nYour reward: " + reward + "$";
		MissionManager.Instance.mView.ShowMissionEnd(this, false);
	}

	public override void OnMissionFailed()
	{
		base.OnMissionFailed();
		mDescription = ((!wasKilled) ? "You've got zero stars!\nDestroy at least 8 enemies!" : "You are dead!");
		MissionManager.Instance.mView.ShowMissionEnd(this, true);
	}

	public override void OnMission()
	{
		base.OnMission();
		indicatorLabel.text = 15 - GetMissionParam<int>("Enemies") + "/" + 15;
		if (GameController.thisScript.playerScript.isDead && panelTime != null)
		{
			panelTime.SetActive(false);
			wasKilled = true;
			CancelInvoke("DecTime");
			SwitchStatus(MissionStatus.MissionFailed);
		}
		if (GetMissionParam<int>("Enemies") == 0)
		{
			CancelInvoke("DecTime");
			CheckMission();
		}
		bool flag = GameController.thisScript.playerScript.inCar && GameController.thisScript.carScript.carWithWeapon;
		getInCarLabel.SetActive(!flag);
	}
}
