using UnityEngine;

public class CheckpointRunPort : Mission
{
	protected int checkpointCount;

	protected int timeLimit;

	protected bool wasKilled;

	protected UILabel indicatorLabel;

	protected UISprite indicatorSprite;

	protected UILabel timeLabel;

	protected GameObject panelTime;

	protected GameObject rootPoint;

	protected GameObject checkPoints;

	public CheckpointRunPort()
	{
		mTitle = "Easy Parkour";
		mDescription = "Pass all the check points\n with a limited time!";
		mId = MissionID.CheckpointRunPort;
	}

	public CheckpointRunPort(string title, string descr, MissionID mId)
	{
		mTitle = title;
		mDescription = descr;
		mId = MissionID.CheckpointRunPort;
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
		panelTime = MissionManager.Instance.panelTime;
		if (panelTime != null)
		{
			panelTime.SetActive(true);
			timeLabel = panelTime.transform.Find("LabelTimeGame").GetComponent<UILabel>();
			timeLimit = 180;
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

	public Transform getRandomChild(Transform parent)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		int num = Random.Range(0, componentsInChildren.Length);
		return componentsInChildren[num];
	}

	protected virtual void InitCheckpoints()
	{
		rootPoint = MissionManager.Instance.checkpointRoot;
		rootPoint.SetActive(true);
		checkPoints = rootPoint.transform.Find("m1").gameObject;
		checkPoints.SetActive(true);
		checkPoints.transform.GetChild(0).gameObject.SetActive(true);
		checkPoints.transform.GetChild(0).GetComponent<CheckPointBehavior>().canBeVisited = true;
		checkpointCount = checkPoints.transform.childCount;
		SetMissionParam("PassedPoints", checkpointCount);
	}

	protected virtual void ResetCheckpoints()
	{
		CheckPointBehavior[] componentsInChildren = checkPoints.GetComponentsInChildren<CheckPointBehavior>();
		foreach (CheckPointBehavior checkPointBehavior in componentsInChildren)
		{
			checkPointBehavior.canBeVisited = false;
			checkPointBehavior.gameObject.SetActive(false);
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

	protected virtual void CheckMission()
	{
		int num = checkpointCount - GetMissionParam<int>("PassedPoints");
		rateStars = 0;
		bool flag = true;
		switch (num)
		{
		case 10:
			rateStars = 3;
			flag = false;
			break;
		case 7:
		case 8:
		case 9:
			rateStars = 2;
			flag = false;
			break;
		default:
			if (num >= 4 && num < 7)
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

	public override void OnMission()
	{
		base.OnMission();
		indicatorLabel.text = checkpointCount - GetMissionParam<int>("PassedPoints") + "/" + checkpointCount;
		if (GameController.thisScript.playerScript.isDead && panelTime != null)
		{
			panelTime.SetActive(false);
			wasKilled = true;
			CancelInvoke("DecTime");
			SwitchStatus(MissionStatus.MissionFailed);
		}
		if (GetMissionParam<int>("PassedPoints") == 0)
		{
			CancelInvoke("DecTime");
			CheckMission();
		}
	}

	public override void OnMissionComplete()
	{
		base.OnMissionComplete();
		mDescription = "Passed checkpoints: " + (checkpointCount - GetMissionParam<int>("PassedPoints")) + "/" + checkpointCount + "\nYour reward: " + reward + "$";
		MissionManager.Instance.mView.ShowMissionEnd(this, false);
	}

	public override void OnMissionEnd()
	{
		MissionManager.Instance.indicatorPoints.SetActive(false);
		base.OnMissionEnd();
		panelTime.SetActive(false);
		ResetCheckpoints();
	}

	public override void OnMissionFailed()
	{
		base.OnMissionFailed();
		mDescription = ((!wasKilled) ? "Wrong Check point." : "You are dead!");
		MissionManager.Instance.mView.ShowMissionEnd(this, true);
	}
}
