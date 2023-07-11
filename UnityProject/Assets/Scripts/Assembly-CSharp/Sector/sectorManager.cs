using System.Collections.Generic;
using UnityEngine;

public class sectorManager : MonoBehaviour
{
	public static sectorManager thisScript;

	private List<LoadSector> listSectors = new List<LoadSector>();

	private void Awake()
	{
		thisScript = this;
	}

	private void Update()
	{
	}

	public void addSectorToList(LoadSector curSector)
	{
		if (!listSectors.Contains(curSector))
		{
			listSectors.Add(curSector);
		}
	}

	public void removeAllSectorBesidesCurrent(LoadSector curSector)
	{
		foreach (LoadSector listSector in listSectors)
		{
			if (listSector != curSector)
			{
				listSector.removeSector();
			}
		}
	}

	public void removeSector(LoadSector curSector)
	{
		if (listSectors.Contains(curSector))
		{
			curSector.removeSector();
		}
	}

	private void OnDestroy()
	{
		if (thisScript == this)
		{
			thisScript = null;
		}
		listSectors.Clear();
	}
}
