using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Note: Preloading means loading from disk. */
public class ManagerPreloadingSectors : MonoBehaviour
{
	public delegate void OnLoaded();

	/* Unity OOP. Must anything else be said? */
	public static ManagerPreloadingSectors thisScript;

	public static bool isLoading;

	// TODO: Why are there these two bools that seem to do the same kinda thing?
	public static bool isStopLoading;

	public static string nameAssetsForLoad = string.Empty;

	public static PreloadSector loadSector;

	public bool sectorsArePreloading = true;

	[HideInInspector]
	public bool isFirstLoading = true;

	/* A list of the sectors loaded in game. */
	public List<SectorCreate> listCreateSectors = new List<SectorCreate>();

	/* A list of sectors that need to be loaded. */
	public List<PreloadSector> listStackCreateSectors = new List<PreloadSector>();

	// TODO: SectorCreate should have an int for it's index so it doesn't need to be parsed a
	// hundred times over.
	/* The sectors that have been cached after being loaded from disk then removed. */
	private static SectorCreate[] cachedSectors = new SectorCreate[22];

	/* The number of currently cached sectors. */
	private static int cachedSectorLength;

	private void Awake()
	{
		thisScript = this;
	}

	/* Starts loading the assets from the level of the inputted sector. */
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

			if (CompilationSettings.SectorCacheSize > 0)
			{
				string sector_num = nameAssetsForLoad.Substring(7);
				SectorCreate cached_sector = cachedSectors[Int32.Parse(sector_num)-1];
				if (cached_sector != null)
				{
					cachedSectors[Int32.Parse(sector_num)-1] = null;
					cached_sector.gameObject.SetActive(true);
					cachedSectorLength--;
					LoadSectorFinish();
					return;
				}
			}

			if (CompilationSettings.Debug && thisScript == null)
			{
				Debug.LogError("PreloadingFromAsset didn't find in Scene");
			}
			else
			{
				thisScript.StartCoroutine(LoadSectorsFromScene());
			}
		}
	}

	private static IEnumerator LoadSectorsFromScene()
	{
		Debug.Log("Loading from disk.");
		yield return Resources.UnloadUnusedAssets();
		yield return Application.LoadLevelAdditiveAsync(nameAssetsForLoad);
		LoadSectorFinish();
	}

	private static void LoadSectorFinish()
	{
		if (settings.includePreloadingSectors)
		{
			loadSector.curStatus = statusSector.loaded;
			thisScript.RemoveSectorFromStackList(loadSector);
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
		CheckLoadingStatus();
	}

	public void RemoveSectorFromStackList(PreloadSector curSector)
	{
		if (listStackCreateSectors.Contains(curSector))
		{
			listStackCreateSectors.Remove(curSector);
		}
		CheckLoadingStatus();
	}

	public void CheckLoadingStatus()
	{
		if (listStackCreateSectors.Count > 0)
		{
			sectorsArePreloading = true;
			return;
		}

		sectorsArePreloading = false;

		// TODO: This is kinda bad.
		if (isFirstLoading)
		{
			isFirstLoading = false;
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
		// TODO: Would this function every be called while a sector was in this
		// state?
		/* If the sector is unloading or already unloaded. */
		if (curSector.curStatus >= statusSector.unloaded)
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

		if (CompilationSettings.Debug && sectorCreate == null)
		{
			Debug.Log("Sector " + curSector.nameSector + " is not found.");
		}
		else
		{
			listCreateSectors.Remove(sectorCreate);

			if (CompilationSettings.SectorCacheSize > 0)
			{
				/*
				 * If there's not enough space for this sector remove the sector that is the
				 * furthest away from the player.
				 */
				if (cachedSectorLength >= CompilationSettings.SectorCacheSize)
				{
					float furthest_distance = 0.0f;
					int furthest_sector_index = 0;
					Vector3 player_pos = GameController.thisScript.myPlayer.transform.position;

					for (int i = 0; i < 22; i++)
					{
						if (cachedSectors[i] == null)
						{
							continue;
						}

						float distance = Vector3.Distance(
							cachedSectors[i].gameObject.transform.position,
							player_pos
						);

						if (distance > furthest_distance)
						{
							furthest_sector_index = i;
							furthest_distance = distance;
						}
					}

					Debug.Log(furthest_sector_index+1);
					UnityEngine.Object.Destroy(cachedSectors[furthest_sector_index].gameObject);
					cachedSectors[furthest_sector_index] = null;
				}
				else
				{
					cachedSectorLength++;
				}

				/* Adding the sector to the cache. */
				string sector_num = sectorCreate.gameObject.name.Substring(7);
				cachedSectors[Int32.Parse(sector_num)-1] = sectorCreate;

				/* Disabling the sector. */
				sectorCreate.gameObject.SetActive(false);
			}
			else
			{
				UnityEngine.Object.Destroy(sectorCreate.gameObject);
			}
		}

		curSector.shouldUnloadSector = false;
		curSector.curStatus = statusSector.unloaded;
	}

	private void OnDestroy()
	{
		isLoading = false;
		isStopLoading = false;
		thisScript = null;
	}
}
