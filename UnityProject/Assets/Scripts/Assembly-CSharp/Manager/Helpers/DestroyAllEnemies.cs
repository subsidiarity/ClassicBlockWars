using System;
using UnityEngine;

public class DestroyAllEnemies : Mission
{
	private const int MAX_ENEMY_COUNT = 20;

	private int timeLimit;

	private bool wasKilled;

	private UILabel indicatorLabel;

	private UISprite indicatorSprite;

	private UILabel timeLabel;

	private GameObject panelTime;

	public DestroyAllEnemies()
	{
		mTitle = "Serial Killer";
		mDescription = "Kill 20 enemies \nwith a limited time. \nBe careful, they will resist!";
		mId = MissionID.DestroyEnemies;
	}

	public Type GetMissionType()
	{
		return typeof(DestroyAllEnemies);
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
		SetMissionParam("Enemies", 20);
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
			indicatorSprite.spriteName = "icon_bot";
		}
		InitEnemies();
		SetMissionParam("Time", 300);
	}

	public Transform getRandomChild(Transform parent)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		int num = UnityEngine.Random.Range(0, componentsInChildren.Length);
		return componentsInChildren[num];
	}

	protected virtual void InitEnemies()
	{
		EnemyGenerator.Instance.makeHellAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			item.AddComponent<EnemyMission>().how = Destroyed.ByGun;
		}
	}

	protected virtual void ResetEnemies()
	{
		EnemyGenerator.Instance.makeHellNotAggressive();
		foreach (GameObject item in EnemyGenerator.Instance.enemiesInHell)
		{
			UnityEngine.Object.Destroy(item.GetComponent<EnemyMission>());
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

	protected void CheckMission()
	{
		int num = 20 - GetMissionParam<int>("Enemies");
		rateStars = 0;
		bool flag = true;
		switch (num)
		{
		case 20:
			rateStars = 3;
			flag = false;
			break;
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
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

	public override void OnMission()
	{
		base.OnMission();
		indicatorLabel.text = 20 - GetMissionParam<int>("Enemies") + "/" + 20;
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
	}

	public override void OnMissionComplete()
	{
		base.OnMissionComplete();
		mDescription = "Destroyed enemies: " + (20 - GetMissionParam<int>("Enemies")) + "/" + 20 + "\nYour reward: " + reward + "$";
		MissionManager.Instance.mView.ShowMissionEnd(this, false);
	}

	public override void OnMissionEnd()
	{
		MissionManager.Instance.indicatorPoints.SetActive(false);
		base.OnMissionEnd();
		panelTime.SetActive(false);
		ResetEnemies();
	}

	public override void OnMissionFailed()
	{
		base.OnMissionFailed();
		mDescription = ((!wasKilled) ? "You've got zero stars!\nDestroy at least 10 enemies!" : "You are dead!");
		MissionManager.Instance.mView.ShowMissionEnd(this, true);
	}
}
