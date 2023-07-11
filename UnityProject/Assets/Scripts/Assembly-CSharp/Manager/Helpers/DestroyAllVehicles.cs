using System.Collections.Generic;
using UnityEngine;

public class DestroyAllVehicles : Mission
{
	private const int MAX_CARS = 10;

	private List<CarBehavior> forRemoving;

	private List<CarBehavior> carBehaviors;

	private int timeLimit;

	private bool wasKilled;

	private int currentCars;

	private UILabel indicatorLabel;

	private UISprite indicatorSprite;

	private UILabel timeLabel;

	private GameObject panelTime;

	public DestroyAllVehicles()
	{
		mTitle = "Rides destroyer";
		mDescription = "Try to blow up 10 cars \nwithin a time limit. Be careful, \nthe police won't like it!";
		mId = MissionID.DestroyVehicles;
	}

	public override void OnMissionStart()
	{
		base.OnMissionStart();
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
			indicatorSprite.spriteName = "icon_car";
		}
		carBehaviors = new List<CarBehavior>();
		forRemoving = new List<CarBehavior>();
		InitCars();
		SetMissionParam("Time", 300);
	}

	private void InitCars()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Car");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			CarBehavior component = gameObject.GetComponent<CarBehavior>();
			if (component != null)
			{
				component.gameObject.transform.position = component.initialCarPosition;
				component.gameObject.transform.rotation = component.initialRotation;
				component.resetCarEnabled(false);
				carBehaviors.Add(component);
			}
		}
	}

	private void ResetCars()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Car");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			CarBehavior component = gameObject.GetComponent<CarBehavior>();
			if (component != null)
			{
				component.resetCarEnabled(true);
				component.reset();
			}
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
		int num = currentCars;
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
		forRemoving.Clear();
		foreach (CarBehavior carBehavior in carBehaviors)
		{
			if (carBehavior.isDead)
			{
				forRemoving.Add(carBehavior);
			}
		}
		foreach (CarBehavior item in forRemoving)
		{
			currentCars++;
			carBehaviors.Remove(item);
			Debug.Log("Estimated car count: " + currentCars);
		}
		indicatorLabel.text = currentCars + "/" + 10;
		if (GameController.thisScript.playerScript.isDead && panelTime != null)
		{
			currentCars = 0;
			panelTime.SetActive(false);
			wasKilled = true;
			CancelInvoke("DecTime");
			SwitchStatus(MissionStatus.MissionFailed);
		}
		if (currentCars == 10)
		{
			CancelInvoke("DecTime");
			CheckMission();
		}
	}

	public override void OnMissionComplete()
	{
		base.OnMissionComplete();
		mDescription = "Blown-up cars " + currentCars + "/" + 10 + "\nYour reward: " + reward + "$";
		MissionManager.Instance.mView.ShowMissionEnd(this, false);
	}

	public override void OnMissionEnd()
	{
		MissionManager.Instance.indicatorPoints.SetActive(false);
		base.OnMissionEnd();
		panelTime.SetActive(false);
		ResetCars();
	}

	public override void OnMissionFailed()
	{
		base.OnMissionFailed();
		mDescription = ((!wasKilled) ? "You've got zero stars!\nBlow up at least 4 cars!" : "You are dead!");
		MissionManager.Instance.mView.ShowMissionEnd(this, true);
	}
}
