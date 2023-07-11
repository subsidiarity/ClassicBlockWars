using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	public delegate void OnMissionStartEvent();

	public delegate void OnMissionEndEvent();

	public delegate void OnMissionEvent();

	public delegate void OnMissionCompleteEvent();

	public delegate void OnMissionFailedEvent();

	public Dictionary<MissionID, Mission> allMissionsDict;

	public MissionID lastMissionID;

	public MissionData currentMissionData;

	public Transform panelMissions;

	public GameObject checkpointRoot;

	public GameObject pauseRestartButton;

	public GameObject pauseCancelButton;

	public GameObject indicatorPoints;

	public GameObject panelMissionsButton;

	public GameObject panelTime;

	public GameObject centralLabel;

	private GameObject[] missionPoints;

	private float missionDistance = 3f;

	public MissionView mView;

	public Mission cMission;

	private static MissionManager instance;

	public static MissionManager Instance
	{
		get
		{
			if (instance == null)
			{
				instance = UnityEngine.Object.FindObjectOfType(typeof(MissionManager)) as MissionManager;
			}
			return instance;
		}
	}

	public event OnMissionStartEvent eOnMissionStart;

	public event OnMissionEvent eOnMission;

	public event OnMissionEndEvent eOnMissionEnd;

	public event OnMissionCompleteEvent eOnMissionComplete;

	public event OnMissionFailedEvent eOnMissionFailed;

	public T SetDataForCurrentMission<T>(string name, T data)
	{
		cMission.SetMissionParam(name, data);
		return data;
	}

	public T GetDataFromCurrentMission<T>(string name)
	{
		return cMission.GetMissionParam<T>(name);
	}

	public void SetMarkerState(bool isActive)
	{
		GameObject[] array = missionPoints;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(isActive);
		}
	}

	public void RestartLastMission()
	{
		if (GameController.thisScript.playerScript.inCar)
		{
			GameController.thisScript.playerScript.GetOutOfCar();
		}
		GameController.thisScript.playerScript.reset(currentMissionData.transform.position);
		cMission.OnMissionEnd();
		StartMission(lastMissionID);
	}

	public Mission GetCurrentMission()
	{
		return cMission;
	}

	public void LoadCurrentProgress()
	{
	}

	public void SaveCurrentProgress()
	{
	}

	private void InitObjects()
	{
		checkpointRoot = GameObject.FindGameObjectWithTag("Mission_CheckpointRoot");
		checkpointRoot.SetActive(false);
		missionPoints = GameObject.FindGameObjectsWithTag("Mission_Point");
		if (!settings.offlineMode)
		{
			GameObject[] array = missionPoints;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
		}
		panelTime = GameController.thisScript.indicatorTimeGame;
		if (GameController.thisScript != null)
		{
			centralLabel = GameController.thisScript.lbInfo;
		}
		if (centralLabel != null)
		{
			centralLabel.SetActive(false);
		}
		if (settings.offlineMode)
		{
			if (GameController.thisScript != null)
			{
				indicatorPoints = GameController.thisScript.indicatorPoints;
			}
			if (indicatorPoints != null)
			{
				indicatorPoints.SetActive(false);
			}
		}
		panelMissions = GameObject.FindGameObjectWithTag("GUI_Mission").transform;
		panelMissions.gameObject.SetActive(false);
		mView = panelMissions.transform.Find("AnchorCenter/PanelMissionSelect").GetComponent<MissionView>();
		mView.listOfMissionsLabel = panelMissions.transform.Find("AnchorTop/Label").gameObject;
		panelMissionsButton = GameObject.FindGameObjectWithTag("GUI_MissionButton");
		panelMissionsButton.SetActive(false);
	}

	private void Init()
	{
		allMissionsDict = new Dictionary<MissionID, Mission>();
		allMissionsDict.Add(MissionID.DestroyVehicles, new DestroyAllVehicles());
		allMissionsDict.Add(MissionID.CollectCoins, new CollectAllCoins());
		allMissionsDict.Add(MissionID.DestroyEnemies, new DestroyAllEnemies());
		allMissionsDict.Add(MissionID.CheckpointRunPort, new CheckpointRunPort());
		allMissionsDict.Add(MissionID.CheckpointRunPort2, new CheckpointRunPort2());
		allMissionsDict.Add(MissionID.CheckpointRunCity, new CheckpointRunCity());
		allMissionsDict.Add(MissionID.Carmageddon, new Carmageddon());
		allMissionsDict.Add(MissionID.TankDestroyer, new TankDestroyer());
		allMissionsDict.Add(MissionID.CheckpointRunBeach, new CheckpointRunBeach());
		allMissionsDict.Add(MissionID.CheckpointRunBeach2, new CheckpointRunBeach2());
		allMissionsDict.Add(MissionID.CheckpointRunDriver, new CheckpointRunDriver());
		allMissionsDict.Add(MissionID.CheckpointRunDriver2, new CheckpointRunDriver2());
		InitObjects();
	}

	public void ShowMissionView()
	{
		if (currentMissionData != null)
		{
			mView.ShowMissions(currentMissionData, true);
		}
	}

	public void CheckPauseButtons()
	{
		if (pauseCancelButton != null && pauseRestartButton != null)
		{
			if (cMission != null)
			{
				pauseCancelButton.SetActive(true);
				pauseRestartButton.SetActive(true);
			}
			else
			{
				pauseCancelButton.SetActive(false);
				pauseRestartButton.SetActive(false);
			}
		}
	}

	public void StartMission(MissionID missionID)
	{
		if (cMission != null)
		{
			cMission.Cancel();
		}
		cMission = GetMissionById(missionID);
		cMission.Init();
		this.eOnMissionStart();
		FlurryWrapper.LogEvent(FlurryWrapper.EV_LAUNCH_MISSION + cMission.mTitle);
		SetMarkerState(false);
	}

	public void ResetPlayerPosition()
	{
		if (GameController.thisScript.playerScript.inCar)
		{
			GameController.thisScript.playerScript.GetOutOfCar();
		}
		GameController.thisScript.myPlayer.transform.position = currentMissionData.transform.position;
	}

	public Mission GetMissionById(MissionID missionID)
	{
		Type type = allMissionsDict[missionID].GetType();
		Debug.Log("Mission got: " + type.Name);
		return (Mission)base.gameObject.AddComponent(type.Name);
	}

	public void EndCurrentMission()
	{
		if (this.eOnMissionEnd != null)
		{
			this.eOnMissionEnd();
		}
	}

	public void FailCurrentMission()
	{
		if (this.eOnMissionFailed != null)
		{
			this.eOnMissionFailed();
		}
	}

	public void CancelCurrentMission()
	{
		if (cMission != null)
		{
			cMission.OnMissionEnd();
			cMission.Cancel();
		}
		cMission = null;
	}

	private void Start()
	{
		Init();
	}

	private void Update()
	{
		if (this.eOnMission != null)
		{
			this.eOnMission();
		}
		if (!settings.offlineMode)
		{
			return;
		}
		bool flag = false;
		GameObject[] array = missionPoints;
		foreach (GameObject gameObject in array)
		{
			if (Vector3.Distance(gameObject.transform.position, GameController.thisScript.myPlayer.transform.position) <= missionDistance && !GameController.thisScript.playerScript.inCar && cMission == null)
			{
				currentMissionData = gameObject.GetComponent<MissionData>();
				flag = true;
			}
			if (flag)
			{
				panelMissionsButton.SetActive(true);
			}
			else
			{
				panelMissionsButton.SetActive(false);
			}
		}
	}
}
