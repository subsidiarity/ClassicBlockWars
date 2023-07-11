using System.Collections;
using System.Net;
using UnityEngine;

public class Mission : MonoBehaviour
{
	private MissionStatus mStatus;

	public string mDescription;

	public string mTitle;

	protected int rateStars;

	protected int reward;

	public MissionID mId;

	protected Hashtable missionParams;

	protected void InitTimeLimit(int t)
	{
	}

	private MissionStatus GetMissionStatus()
	{
		return mStatus;
	}

	public virtual void Init()
	{
		missionParams = new Hashtable();
		MissionManager.Instance.eOnMissionStart += OnMissionStart;
		MissionManager.Instance.eOnMission += OnMission;
		MissionManager.Instance.eOnMissionEnd += OnMissionEnd;
		MissionManager.Instance.eOnMissionComplete += OnMissionComplete;
		MissionManager.Instance.eOnMissionFailed += OnMissionFailed;
	}

	public virtual void Cancel()
	{
		MissionManager.Instance.eOnMissionStart -= OnMissionStart;
		MissionManager.Instance.eOnMission -= OnMission;
		MissionManager.Instance.eOnMissionEnd -= OnMissionEnd;
		MissionManager.Instance.eOnMissionComplete -= OnMissionComplete;
		MissionManager.Instance.eOnMissionFailed -= OnMissionFailed;
	}

	public virtual T GetMissionParam<T>(string param)
	{
		return (T)missionParams[param];
	}

	public virtual void SetMissionParam<T>(string param, T value)
	{
		missionParams[param] = value;
	}

	public virtual void SaveRating()
	{
		if (rateStars > Load.LoadInt(mTitle))
		{
			reward = (rateStars - Load.LoadInt(mTitle)) * 10;
			settings.updateKolCoins(settings.tekKolCoins + reward);
			shopController.thisScript.lbKolCoins.text = string.Empty + settings.tekKolCoins;
			settings.updateKeychainCoins();
			Save.SaveInt(mTitle, rateStars);
		}
	}

	public virtual void OnMissionComplete()
	{
		SaveRating();
		Debug.Log("Mission Complete.");
		OnMissionEnd();
	}

	public virtual void OnMissionFailed()
	{
		Debug.Log("Mission Failed.");
		OnMissionEnd();
	}

	public virtual void OnMissionPause()
	{
		Debug.Log("Mission " + mTitle + " paused.");
	}

	public virtual void OnMissionStart()
	{
		Debug.Log("Mission " + mTitle + " started.");
	}

	public virtual void OnMission()
	{
	}

	public virtual void CalculateRate()
	{
	}

	public void AnimateStars()
	{
	}

	protected void SwitchStatus(MissionStatus mS)
	{
		switch (mS)
		{
		case MissionStatus.MissionCompleted:
			OnMissionComplete();
			break;
		case MissionStatus.MissionFailed:
			OnMissionFailed();
			break;
		case MissionStatus.MissionInProgress:
			break;
		}
	}

	public virtual void OnMissionEnd()
	{
		if (GameController.thisScript.playerScript.isShooting)
		{
			GameController.thisScript.playerScript.isShooting = false;
		}
		MissionManager.Instance.SetMarkerState(true);
		MissionManager.Instance.lastMissionID = mId;
		Cancel();
		Object.Destroy(this);
		Debug.Log("Mission Ended.");
	}
}
