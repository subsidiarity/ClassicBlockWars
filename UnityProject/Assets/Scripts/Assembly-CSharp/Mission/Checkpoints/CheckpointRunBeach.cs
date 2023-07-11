public class CheckpointRunBeach : CheckpointRunPort
{
	public CheckpointRunBeach()
	{
		mTitle = "Beach Parkour";
		mDescription = "Pass all the check points with\na limited time!";
		mId = MissionID.CheckpointRunBeach;
	}

	public CheckpointRunBeach(string title, string descr, MissionID mId)
	{
		mTitle = title;
		mDescription = descr;
		mId = MissionID.CheckpointRunBeach;
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
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
	}

	protected override void InitCheckpoints()
	{
		rootPoint = MissionManager.Instance.checkpointRoot;
		rootPoint.SetActive(true);
		checkPoints = rootPoint.transform.FindChild("m4").gameObject;
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
		case 12:
			rateStars = 3;
			flag = false;
			break;
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
			rateStars = 2;
			flag = false;
			break;
		default:
			if (num >= 5 && num < 7)
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
