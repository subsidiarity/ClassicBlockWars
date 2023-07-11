using UnityEngine;

public class CheckpointRunDriver2 : CheckpointRunPort
{
	private GameObject car;

	private GameObject getInCarLabel;

	public CheckpointRunDriver2()
	{
		mTitle = "Crazy driver 2";
		mDescription = "Pass all the check points with\na limited time!\nYou need get into the car\n to complete the mission.";
		mId = MissionID.CheckpointRunDriver2;
	}

	public CheckpointRunDriver2(string title, string descr, MissionID mId)
	{
		mTitle = title;
		mDescription = descr;
		mId = MissionID.CheckpointRunDriver2;
	}

	public override void OnMissionEnd()
	{
		base.OnMissionEnd();
		getInCarLabel.GetComponent<UILabel>().text = string.Empty;
		getInCarLabel.SetActive(false);
	}

	public override void OnMissionStart()
	{
		getInCarLabel = MissionManager.Instance.centralLabel;
		getInCarLabel.GetComponent<UILabel>().text = "Get in to the car and collect check points!";
		getInCarLabel.SetActive(true);
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
			indicatorSprite.spriteName = "icon_check";
		}
		InitCheckpoints();
		SetMissionParam("Time", 300);
		car = (GameObject)Object.Instantiate(Resources.Load("Cars/CarSport"), new Vector3(-16f + GameController.thisScript.myPlayer.transform.position.x, 0f, GameController.thisScript.myPlayer.transform.position.z), Quaternion.identity);
		car.transform.parent = GameController.thisScript.spisokCars.transform;
	}

	public override void OnMission()
	{
		base.OnMission();
		bool flag = GameController.thisScript.playerScript.inCar && !GameController.thisScript.carScript.carWithWeapon;
		getInCarLabel.SetActive(!flag);
	}

	protected override void InitCheckpoints()
	{
		rootPoint = MissionManager.Instance.checkpointRoot;
		rootPoint.SetActive(true);
		checkPoints = rootPoint.transform.FindChild("m7").gameObject;
		checkPoints.SetActive(true);
		checkPoints.transform.GetChild(0).gameObject.SetActive(true);
		checkPoints.transform.GetChild(0).GetComponent<CheckPointBehavior>().canBeVisited = true;
		checkpointCount = checkPoints.transform.childCount;
		SetMissionParam("PassedPoints", checkpointCount);
	}

	protected override void ResetCheckpoints()
	{
		CheckPointBehavior[] componentsInChildren = checkPoints.GetComponentsInChildren<CheckPointBehavior>();
		foreach (CheckPointBehavior checkPointBehavior in componentsInChildren)
		{
			checkPointBehavior.canBeVisited = false;
			checkPointBehavior.gameObject.SetActive(false);
		}
		if (GameController.thisScript.playerScript.inCar && GameController.thisScript.carScript.gameObject.Equals(car))
		{
			GameController.thisScript.playerScript.GetOutOfCar();
		}
		Object.Destroy(car);
	}

	protected override void CheckMission()
	{
		int num = checkpointCount - GetMissionParam<int>("PassedPoints");
		rateStars = 0;
		bool flag = true;
		switch (num)
		{
		case 41:
			rateStars = 3;
			flag = false;
			break;
		case 30:
		case 31:
		case 32:
		case 33:
		case 34:
		case 35:
		case 36:
		case 37:
		case 38:
		case 39:
		case 40:
			rateStars = 2;
			flag = false;
			break;
		default:
			if (num >= 20 && num < 30)
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
}
