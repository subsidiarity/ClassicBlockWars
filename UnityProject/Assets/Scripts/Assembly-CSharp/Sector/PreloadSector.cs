using System.Collections.Generic;
using UnityEngine;

public class PreloadSector : MonoBehaviour
{
	public string nameSector = string.Empty;

	public int radiusForLoad = 130;

	public int radiusForUnload = 130;

	public statusSector curStatus = statusSector.unloaded;

	[HideInInspector]
	public bool shouldUnloadSector;

	private GameObject objPlayer;

	public List<Vector2> listPointForLoad = new List<Vector2>();

	public EnemyType[] possibleEnemyTypes;

	private bool loadedOnStart = false;

	private void Start()
	{
		curStatus = statusSector.unloaded;

		if (nameSector == null || nameSector == string.Empty)
		{
			nameSector = base.gameObject.name;
		}

		// TODO: This should be a compilation settings not a runtime settings.
		if (!settings.includePreloadingSectors)
		{
			foreach (SectorCreate sectorCreate in Object.FindObjectsOfType<SectorCreate>())
			{
				if (sectorCreate.name.Equals(nameSector))
				{
					curStatus = statusSector.loaded;
					return;
				}
			}
			ManagerPreloadingSectors.PreloadAssets(this);
			return;
		}

		foreach (Transform transform in base.gameObject.GetComponentsInChildren<Transform>())
		{
			if (!transform.gameObject.Equals(base.gameObject))
			{
				listPointForLoad.Add(new Vector2(transform.position.x, transform.position.z));
			}
		}

		if (CompilationSettings.OverideSectorLoads)
		{
			radiusForLoad = CompilationSettings.SectorLoadDistance;
			radiusForUnload = CompilationSettings.SectorUnloadDistance;
		}
	}

	private void Update()
	{
		if (CompilationSettings.LoadSectorsOnStart
		&& !loadedOnStart)
		{
			if (curStatus != statusSector.loaded)
			{
				ManagerPreloadingSectors.PreloadAssets(this);
			}
			else
			{
				loadedOnStart = true;
			}
		}

		if (!settings.includePreloadingSectors)
		{
			if (curStatus == statusSector.unloaded)
			{
				ManagerPreloadingSectors.PreloadAssets(this);
			}
		}
		else
		{
			UpdateSectorStatus();
		}
	}

	public void UpdateSectorStatus()
	{
		if (objPlayer == null && GameController.thisScript != null)
		{
			objPlayer = GameController.thisScript.myPlayer;
		}

		if (objPlayer == null)
		{
			return;
		}

		Vector2 a = new Vector2(objPlayer.transform.position.x, objPlayer.transform.position.z);
		bool in_distance_for_load = false;

		foreach (Vector2 item in listPointForLoad)
		{
			if (Vector2.Distance(a, item) < (float)radiusForLoad)
			{
				in_distance_for_load = true;
				break;
			}
		}

		if (in_distance_for_load)
		{
			if (curStatus == statusSector.unloaded)
			{
				ManagerPreloadingSectors.thisScript.AddSectorToStackList(this);

				if (!ManagerPreloadingSectors.isLoading)
				{
					ManagerPreloadingSectors.PreloadAssets(this);
				}
				return;
			}
		}
		else if ((shouldUnloadSector && curStatus == statusSector.loaded)
		|| curStatus < statusSector.unloaded)
		{
			shouldUnloadSector = true;
			ManagerPreloadingSectors.thisScript.unloadSector(this);
		}
	}
}
