using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerPreloadingSectors : MonoBehaviour
{
	public delegate void OnLoaded();

	public static ManagerPreloadingSectors thisScript;

	public static bool isLoading;

	public static bool isStopLoading;

	public static string nameAssetsForLoad = string.Empty;

	public static PreloadSector loadSector;

	public bool sectorsIsPreloading = true;

	[HideInInspector]
	public bool isFirstLoading = true;

	public List<SectorCreate> listCreateSectors = new List<SectorCreate>();

	public List<PreloadSector> listStackCreateSectors = new List<PreloadSector>();

	private void Awake()
	{
		thisScript = this;
	}

	private void SetPriorityNextLoading()
	{
		if (Device.isWeakDevice)
		{
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}
		else
		{
			Application.backgroundLoadingPriority = ThreadPriority.Low;
		}
	}

	public static void PreloadAssets(PreloadSector curSector)
	{
		if (!isLoading && !isStopLoading)
		{
			if (settings.includePreloadingSectors)
			{
				isLoading = true;
			}
			loadSector = curSector;
			loadSector.curStatus = statusSector.process_load;
			nameAssetsForLoad = loadSector.nameSector;
			if (thisScript != null)
			{
				thisScript.StartCoroutine(LoadSectorsFormScene(nameAssetsForLoad));
			}
			else
			{
				Debug.LogError("PreloadingFromAsset dont find on Scene");
			}
		}
	}

	private static IEnumerator LoadSectorsFormScene(string curNameSector)
	{
		yield return Resources.UnloadUnusedAssets();
		yield return Application.LoadLevelAdditiveAsync(curNameSector);
		if (settings.includePreloadingSectors)
		{
			loadSector.curStatus = statusSector.load;
			thisScript.RemoveSectorToStackList(loadSector);
			isLoading = false;
			loadSector = null;
			nameAssetsForLoad = string.Empty;
		}
	}

	public void AddCreateSectorToList(SectorCreate curSector)
	{
		if (!listCreateSectors.Contains(curSector))
		{
			listCreateSectors.Add(curSector);
		}
	}

	public void AddSectorToStackList(PreloadSector curSector)
	{
		if (!listStackCreateSectors.Contains(curSector))
		{
			listStackCreateSectors.Add(curSector);
		}
		proveritStatusLoading();
	}

	public void RemoveSectorToStackList(PreloadSector curSector)
	{
		if (listStackCreateSectors.Contains(curSector))
		{
			listStackCreateSectors.Remove(curSector);
		}
		proveritStatusLoading();
	}

	public void proveritStatusLoading()
	{
		if (listStackCreateSectors.Count > 0)
		{
			sectorsIsPreloading = true;
			return;
		}
		sectorsIsPreloading = false;
		if (isFirstLoading)
		{
			isFirstLoading = false;
			SetPriorityNextLoading();
			GameController.thisScript.playerScript.tController.enabled = true;
			GameController.thisScript.playerScript.tController.gravity = 20f;
			GameController.thisScript.showWindowGame();
			Invoke("resetBoundsMap", 0.2f);
		}
	}

	private void resetBoundsMap()
	{
		GameController.thisScript.mapScript.UpdateBounds();
	}

	public void unloadSector(PreloadSector curSector)
	{
		if (curSector.curStatus == statusSector.unload || curSector.curStatus == statusSector.process_unload)
		{
			return;
		}
		curSector.curStatus = statusSector.process_unload;
		SectorCreate sectorCreate = null;
		foreach (SectorCreate listCreateSector in listCreateSectors)
		{
			if (listCreateSector.gameObject.name.Equals(curSector.nameSector))
			{
				sectorCreate = listCreateSector;
				break;
			}
		}
		if (sectorCreate != null)
		{
			listCreateSectors.Remove(sectorCreate);
			Object.Destroy(sectorCreate.gameObject);
		}
		else
		{
			Debug.Log("Sector " + curSector.nameSector + " no find!!!");
		}
		curSector.isUnloadSector = false;
		curSector.curStatus = statusSector.unload;
	}

	private void OnDestroy()
	{
		isLoading = false;
		isStopLoading = false;
		thisScript = null;
	}
}
