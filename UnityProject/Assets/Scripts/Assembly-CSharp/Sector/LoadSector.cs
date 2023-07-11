using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSector : MonoBehaviour
{
	public string nameSector;

	private string pathForLoading = "Resources/";

	private List<Object> listObjectsSector = new List<Object>();

	private Object curObj;

	public bool sectorIsCreate;

	private void Start()
	{
		if (nameSector == null || nameSector == string.Empty)
		{
			nameSector = base.gameObject.name;
		}
		sectorManager.thisScript.addSectorToList(this);
	}

	public void createSector()
	{
		if (!sectorIsCreate)
		{
			if (nameSector == null)
			{
				Debug.LogError("Set name Sector");
			}
			else
			{
				StartCoroutine(createPartSector());
			}
		}
	}

	private IEnumerator createPartSector()
	{
		sectorIsCreate = true;
		Object[] arrPartSector = Resources.LoadAll<Object>("Resources/" + nameSector);
		Object[] array = arrPartSector;
		foreach (Object curPart in array)
		{
			curObj = Object.Instantiate(curPart);
			if (curObj != null)
			{
				listObjectsSector.Add(curObj);
			}
			yield return new WaitForSeconds(0.03f);
		}
		yield return new WaitForSeconds(0.03f);
	}

	public void removeSector()
	{
		if (!sectorIsCreate)
		{
			return;
		}
		sectorIsCreate = false;
		foreach (Object item in listObjectsSector)
		{
			Object.Destroy(item);
		}
		listObjectsSector.Clear();
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			sectorManager.thisScript.removeAllSectorBesidesCurrent(this);
			createSector();
		}
	}
}
