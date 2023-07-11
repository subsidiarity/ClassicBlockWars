using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour
{
	private static AssetBundleManager instance;

	private Dictionary<string, AssetBundleContainer> assetBundles = new Dictionary<string, AssetBundleContainer>();

	public static AssetBundleManager Instance
	{
		get
		{
			if (instance == null)
			{
				Debug.Log("Creating an AssetBundle manager instance");
				GameObject gameObject = new GameObject();
				instance = gameObject.AddComponent<AssetBundleManager>();
				gameObject.name = "AssetBundleManager";
				Object.DontDestroyOnLoad(gameObject);
			}
			return instance;
		}
	}

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
		InvokeRepeating("CheckForUnusedBundles", 5f, 5f);
	}

	private void CheckForUnusedBundles()
	{
		if (assetBundles.Count <= 0)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, AssetBundleContainer> assetBundle in assetBundles)
		{
			assetBundle.Value.ClearEmptyObjects();
			if (assetBundle.Value.IsListEmpty())
			{
				assetBundle.Value.Unload();
				list.Add(assetBundle.Key);
			}
		}
		foreach (string item in list)
		{
			assetBundles.Remove(item);
		}
	}

	public void AddBundle(string bundleName, AssetBundle assetBundle, GameObject instantiatedObject)
	{
		if (!assetBundles.ContainsKey(bundleName))
		{
			AssetBundleContainer assetBundleContainer = new AssetBundleContainer();
			assetBundleContainer.ThisAssetBundle = assetBundle;
			assetBundleContainer.ObjectList.Add(instantiatedObject);
			assetBundleContainer.BundleName = bundleName;
			assetBundles.Add(bundleName, assetBundleContainer);
			return;
		}
		AssetBundleContainer value = null;
		assetBundles.TryGetValue(bundleName, out value);
		if (value != null)
		{
			value.ObjectList.Add(instantiatedObject);
			return;
		}
		Debug.LogError("AssetBundleManager.cs: Couldn't get the container for assetbundle: " + bundleName + ". Removal Management for object:" + instantiatedObject.name + " will not work");
	}

	public AssetBundleContainer GetAssetBundle(string bundleName)
	{
		AssetBundleContainer value = null;
		assetBundles.TryGetValue(bundleName, out value);
		return value;
	}

	public void DestroyAssetBundle(string bundleName)
	{
		AssetBundleContainer value = null;
		assetBundles.TryGetValue(bundleName, out value);
		if (value == null)
		{
			return;
		}
		foreach (GameObject @object in value.ObjectList)
		{
			if (@object != null)
			{
				Object.Destroy(@object);
			}
		}
		value.ObjectList.Clear();
		value.Unload();
		assetBundles.Remove(bundleName);
	}

	public void DestroyAllBundles()
	{
		foreach (KeyValuePair<string, AssetBundleContainer> assetBundle in assetBundles)
		{
			foreach (GameObject @object in assetBundle.Value.ObjectList)
			{
				if (@object != null)
				{
					Object.Destroy(@object);
				}
			}
			assetBundle.Value.ObjectList.Clear();
			assetBundle.Value.Unload();
		}
		assetBundles.Clear();
	}
}
