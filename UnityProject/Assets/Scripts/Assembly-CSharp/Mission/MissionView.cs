using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionView : MonoBehaviour
{
	public Transform mRoot;

	public float offsetX = 2f;

	public Shader blurShader;

	public GameObject prefabMissionItem;

	public UITexture pauseTexture;

	public GameObject missionEndGamePanel;

	public GameObject listOfMissionsLabel;

	private GameObject bRestart;

	private GameObject bToCity;

	private GameObject bStartMission;

	private GameObject bBack;

	private string MISSION_FAILED_TITLE = "Mission failed";

	private string MISSION_COMPLETE_TITLE = "Mission complete";

	private List<GameObject> MissionItemGameObjectList;

	public IEnumerator grabScreen()
	{
		yield return new WaitForEndOfFrame();
		MissionManager.Instance.panelMissions.GetComponent<UIPanel>().alpha = 0f;
		Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		RenderTexture renderTexture = null;
		Camera mainCamera = Camera.main;
		if (mainCamera != null)
		{
			mainCamera.targetTexture = renderTexture;
			RenderTexture.active = renderTexture;
			mainCamera.Render();
			screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			screenTexture.Apply(false);
			RenderTexture.active = null;
			mainCamera.targetTexture = null;
		}
		GameObject cameraObject = Camera.main.gameObject;
		if (cameraObject != null)
		{
			BlurEffect be = cameraObject.GetComponent<BlurEffect>();
			Object.Destroy(be);
		}
		pauseTexture.mainTexture = screenTexture;
		pauseTexture.height += 2;
		MissionManager.Instance.panelMissions.GetComponent<UIPanel>().alpha = 1f;
	}

	public void HideMissions()
	{
		if (MissionItemGameObjectList != null)
		{
			foreach (GameObject missionItemGameObject in MissionItemGameObjectList)
			{
				Object.Destroy(missionItemGameObject);
			}
			MissionItemGameObjectList.Clear();
		}
		Time.timeScale = 1f;
		MissionManager.Instance.panelMissions.gameObject.SetActive(false);
	}

	public void HideAllInterface()
	{
		GameController.thisScript.hideAllPanels();
	}

	public void ShowGameInterface()
	{
		if (GameController.thisScript.playerScript.inCar)
		{
			GameController.thisScript.carInteface.gameObject.SetActive(true);
		}
		else
		{
			GameController.thisScript.walkInterface.gameObject.SetActive(true);
		}
		GameController.thisScript.panelGameTop.gameObject.SetActive(true);
		GameController.thisScript.panelMiniMap.gameObject.SetActive(true);
		GameController.thisScript.panelJoystick.gameObject.SetActive(true);
	}

	public void StartSelectedMission()
	{
		UICenterOnChild component = mRoot.GetComponent<UICenterOnChild>();
		if (component != null)
		{
			GameObject centeredObject = component.centeredObject;
			if (centeredObject != null)
			{
				MissionItem component2 = centeredObject.GetComponent<MissionItem>();
				if (component2 != null)
				{
					MissionManager.Instance.StartMission(component2.mID);
				}
			}
			else
			{
				GameObject gameObject = mRoot.GetChild(0).gameObject;
				if (gameObject != null)
				{
					MissionItem component3 = gameObject.GetComponent<MissionItem>();
					if (component3 != null)
					{
						MissionManager.Instance.StartMission(component3.mID);
					}
				}
			}
		}
		ShowGameInterface();
		HideMissions();
	}

	public void ShowMissionEnd(Mission miss, bool isFailed)
	{
		listOfMissionsLabel.SetActive(false);
		HideAllInterface();
		GameObject gameObject = MissionManager.Instance.panelMissions.gameObject;
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		ShowButtons(true);
		GameObject gameObject2 = Camera.main.gameObject;
		if (gameObject2 != null)
		{
			BlurEffect blurEffect = gameObject2.AddComponent<BlurEffect>();
			blurEffect.blurShader = blurShader;
			StartCoroutine(grabScreen());
		}
		float num = 0f;
		missionEndGamePanel = NGUITools.AddChild(mRoot.gameObject, prefabMissionItem);
		MissionItem component = missionEndGamePanel.GetComponent<MissionItem>();
		component.title.text = ((!isFailed) ? MISSION_COMPLETE_TITLE : MISSION_FAILED_TITLE);
		component.description.text = miss.mDescription;
		component.transform.position = Vector3.zero;
		if (isFailed)
		{
			component.starGroup.SetActive(false);
		}
		else
		{
			component.starGroup.GetComponent<showResaultStars>().updateShowKolStars(Load.LoadInt(miss.mTitle));
		}
		Time.timeScale = 0f;
	}

	public void ShowMissions(MissionData mData, bool blurEnabled)
	{
		listOfMissionsLabel.SetActive(true);
		HideAllInterface();
		GameObject gameObject = MissionManager.Instance.panelMissions.gameObject;
		if (!gameObject.activeSelf)
		{
			gameObject.SetActive(true);
		}
		ShowButtons(false);
		if (blurEnabled)
		{
			GameObject gameObject2 = Camera.main.gameObject;
			if (gameObject2 != null)
			{
				BlurEffect blurEffect = gameObject2.AddComponent<BlurEffect>();
				blurEffect.blurShader = blurShader;
				StartCoroutine(grabScreen());
			}
		}
		float num = 0f;
		List<Mission> list = new List<Mission>();
		MissionID[] missionIds = mData.missionIds;
		foreach (MissionID key in missionIds)
		{
			if (MissionManager.Instance.allMissionsDict.ContainsKey(key))
			{
				list.Add(MissionManager.Instance.allMissionsDict[key]);
			}
		}
		MissionItemGameObjectList = new List<GameObject>();
		int num2 = 0;
		foreach (Mission item in list)
		{
			int kol = Load.LoadInt(item.mTitle);
			GameObject gameObject3 = NGUITools.AddChild(mRoot.gameObject, prefabMissionItem);
			gameObject3.name = string.Empty + ++num2;
			Transform transform = gameObject3.transform.Find("groupStars");
			if (transform != null)
			{
				transform.GetComponent<showResaultStars>().updateShowKolStars(kol);
			}
			MissionItem component = gameObject3.GetComponent<MissionItem>();
			component.title.text = item.mTitle;
			component.description.text = item.mDescription;
			component.mID = item.mId;
			MissionItemGameObjectList.Add(gameObject3);
		}
		SortGrid();
		GetComponent<UIPanel>().clipOffset = new Vector2(0f, GetComponent<UIPanel>().clipOffset.y);
		base.transform.position = new Vector3(0f, base.transform.position.y, base.transform.position.z);
		Time.timeScale = 0f;
	}

	private void SortGrid()
	{
		UIGrid component = mRoot.GetComponent<UIGrid>();
		if (mRoot.GetComponent<UIGrid>() != null)
		{
			component.repositionNow = true;
		}
	}

	private void ShowButtons(bool completeButtons)
	{
		if (bRestart == null)
		{
			bRestart = MissionManager.Instance.panelMissions.Find("AnchorLeftBottom/butRestart").gameObject;
		}
		if (bToCity == null)
		{
			bToCity = MissionManager.Instance.panelMissions.Find("AnchorRightBottom/butToCity").gameObject;
		}
		if (bStartMission == null)
		{
			bStartMission = MissionManager.Instance.panelMissions.Find("AnchorRightBottom/butStartMission").gameObject;
		}
		if (bBack == null)
		{
			bBack = MissionManager.Instance.panelMissions.Find("AnchorLeftBottom/butBack").gameObject;
		}
		bRestart.SetActive(completeButtons);
		bToCity.SetActive(completeButtons);
		bStartMission.SetActive(!completeButtons);
		bBack.SetActive(!completeButtons);
	}
}
