using System.Collections.Generic;
using UnityEngine;

public class PreloadSector : MonoBehaviour
{
	public string nameSector = string.Empty;

	public int radiusForLoad = 130;

	public int radiusForUnload = 130;

	public statusSector curStatus = statusSector.unload;

	[HideInInspector]
	public bool isUnloadSector;

	private GameObject objPlayer;

	public List<Vector2> listPointForLoad = new List<Vector2>();

	public EnemyType[] possibleEnemyTypes;

	private void Start()
	{
		curStatus = statusSector.unload;
		if (nameSector == null || nameSector == string.Empty)
		{
			nameSector = base.gameObject.name;
		}
		if (!settings.includePreloadingSectors)
		{
			SectorCreate[] array = Object.FindObjectsOfType<SectorCreate>();
			SectorCreate[] array2 = array;
			foreach (SectorCreate sectorCreate in array2)
			{
				if (sectorCreate.name.Equals(nameSector))
				{
					curStatus = statusSector.load;
					return;
				}
			}
			ManagerPreloadingSectors.PreloadAssets(this);
			return;
		}
		Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>();
		Transform[] array3 = componentsInChildren;
		foreach (Transform transform in array3)
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
		if (!settings.includePreloadingSectors)
		{
			if (curStatus != 0 && curStatus != statusSector.process_load && curStatus != statusSector.process_unload)
			{
				ManagerPreloadingSectors.PreloadAssets(this);
			}
		}
		else
		{
			proveritStatusSector();
		}
	}

	public void proveritStatusSector()
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
		bool flag = false;
		foreach (Vector2 item in listPointForLoad)
		{
			if (Vector2.Distance(a, item) < (float)radiusForLoad)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			if (curStatus == statusSector.unload)
			{
				ManagerPreloadingSectors.thisScript.AddSectorToStackList(this);
			}
			if (curStatus != 0 && curStatus != statusSector.process_load && curStatus != statusSector.process_unload)
			{
				if (!ManagerPreloadingSectors.isLoading)
				{
					ManagerPreloadingSectors.PreloadAssets(this);
				}
				return;
			}
		}
		else if (curStatus != statusSector.process_unload && curStatus != statusSector.unload)
		{
			isUnloadSector = true;
		}
		if (isUnloadSector && curStatus == statusSector.load)
		{
			ManagerPreloadingSectors.thisScript.unloadSector(this);
		}
	}
}
