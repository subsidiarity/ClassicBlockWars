public class CheckpointRunCity : CheckpointRunPort
{
	public CheckpointRunCity()
	{
		mTitle = "Parkour and Flight!";
		mDescription = "Pass all the check points with\na limited time!\nYou need the Jetpack to\ncomplete the mission.";
		mId = MissionID.CheckpointRunCity;
	}

	public CheckpointRunCity(string title, string descr, MissionID mId)
	{
		mTitle = title;
		mDescription = descr;
		mId = MissionID.CheckpointRunCity;
	}

	protected override void InitCheckpoints()
	{
		rootPoint = MissionManager.Instance.checkpointRoot;
		rootPoint.SetActive(true);
		checkPoints = rootPoint.transform.Find("m3").gameObject;
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
	}

	protected override void CheckMission()
	{
		int num = checkpointCount - GetMissionParam<int>("PassedPoints");
		rateStars = 0;
		bool flag = true;
		switch (num)
		{
		case 19:
			rateStars = 3;
			flag = false;
			break;
		case 15:
		case 16:
		case 17:
		case 18:
			rateStars = 2;
			flag = false;
			break;
		default:
			if (num >= 10 && num < 15)
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
