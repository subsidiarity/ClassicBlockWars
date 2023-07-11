using System.Collections.Generic;
using UnityEngine;

public class AssetBundleContainer
{
	private AssetBundle thisAssetBundle;

	private string bundleName;

	private List<GameObject> objectList = new List<GameObject>();

	public AssetBundle ThisAssetBundle
	{
		get
		{
			return thisAssetBundle;
		}
		set
		{
			thisAssetBundle = value;
		}
	}

	public List<GameObject> ObjectList
	{
		get
		{
			return objectList;
		}
	}

	public string BundleName
	{
		get
		{
			return bundleName;
		}
		set
		{
			bundleName = value;
		}
	}

	public bool IsListEmpty()
	{
		if (objectList.Count == 0)
		{
			return true;
		}
		return false;
	}

	public void ClearEmptyObjects()
	{
		for (int num = objectList.Count - 1; num >= 0; num--)
		{
			if (objectList[num] == null)
			{
				objectList.RemoveAt(num);
			}
		}
	}

	public void Unload()
	{
		Debug.Log("Objects that holds a reference to " + bundleName + ": " + objectList.Count);
		Debug.Log("Unloading AssetBundle(true):" + bundleName);
		thisAssetBundle.Unload(true);
	}
}
