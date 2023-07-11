using UnityEngine;

public class CollectAllCoins : Mission
{
	private const int MAX_COIN_COUNT = 10;

	private int timeLimit;

	private bool wasKilled;

	private UILabel indicatorLabel;

	private UISprite indicatorSprite;

	private CoinCollecter mCoinCollecter;

	private UILabel timeLabel;

	private GameObject panelTime;

	public CollectAllCoins()
	{
		mTitle = "Collect All Coins";
		mDescription = "Collect 10 coins hidden across\n the entire city! Try to collect\n them all, they could be anywhere!";
		mId = MissionID.CollectCoins;
	}

	private void AttachCoinCollecter()
	{
		mCoinCollecter = GameController.thisScript.myPlayer.AddComponent<CoinCollecter>();
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
		GameObject gameObject = GameObject.FindGameObjectWithTag("Respawn_Stars");
		SetMissionParam("Coins", 10);
		panelTime = MissionManager.Instance.panelTime;
		if (panelTime != null)
		{
			panelTime.SetActive(true);
			timeLabel = panelTime.transform.Find("LabelTimeGame").GetComponent<UILabel>();
			timeLimit = 300;
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
			indicatorSprite.spriteName = "icon_stars";
		}
		InitCoins(gameObject.transform);
		SetMissionParam("Time", 300);
		AttachCoinCollecter();
	}

	public Transform getRandomChild(Transform parent)
	{
		Transform[] componentsInChildren = parent.GetComponentsInChildren<Transform>();
		int num = Random.Range(0, componentsInChildren.Length);
		return componentsInChildren[num];
	}

	private void InitCoins(Transform root)
	{
		Object original = Resources.Load("Bonuse/Star");
		for (int i = 0; i < 10; i++)
		{
			GameObject gameObject = (GameObject)Object.Instantiate(original, getRandomChild(root).position, Quaternion.identity);
		}
	}

	private void ResetCoins()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Pickable_Coin");
		GameObject[] array2 = array;
		foreach (GameObject obj in array2)
		{
			Object.Destroy(obj);
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

	private void CheckMission()
	{
		int num = 10 - GetMissionParam<int>("Coins");
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
		indicatorLabel.text = 10 - GetMissionParam<int>("Coins") + "/" + 10;
		if (GameController.thisScript.playerScript.isDead && panelTime != null)
		{
			panelTime.SetActive(false);
			wasKilled = true;
			CancelInvoke("DecTime");
			SwitchStatus(MissionStatus.MissionFailed);
		}
		if (GetMissionParam<int>("Coins") == 0)
		{
			CancelInvoke("DecTime");
			CheckMission();
		}
	}

	public override void OnMissionComplete()
	{
		base.OnMissionComplete();
		mDescription = "Collected coins: " + (10 - GetMissionParam<int>("Coins")) + "/" + 10 + "\nYour reward: " + reward + "$";
		MissionManager.Instance.mView.ShowMissionEnd(this, false);
	}

	public override void OnMissionEnd()
	{
		MissionManager.Instance.indicatorPoints.SetActive(false);
		base.OnMissionEnd();
		Object.Destroy(mCoinCollecter);
		panelTime.SetActive(false);
		ResetCoins();
	}

	public override void OnMissionFailed()
	{
		base.OnMissionFailed();
		mDescription = ((!wasKilled) ? "You've got zero stars!\nCollect at least 4 coins!" : "You are dead!");
		MissionManager.Instance.mView.ShowMissionEnd(this, true);
	}
}
