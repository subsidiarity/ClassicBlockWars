using System.Collections.Generic;
using UnityEngine;

public class HighAssetsLoader : MonoBehaviour
{
	public static HighAssetsLoader thisScript;

	public static readonly string LightmapsFolder = "Lightmap";

	public static readonly string HighFolder = "High";

	public static readonly string AtlasFolder = "Atlas";

	public static string atlasesPath;

	private void Awake()
	{
		thisScript = this;
		atlasesPath = Combine(Combine(AtlasFolder, Application.loadedLevelName), HighFolder);
	}

	private void OnLevelWasLoaded(int lev)
	{
		if (Device.isWeakDevice)
		{
			Debug.LogWarning("low quality");
			List<LightmapData> list = new List<LightmapData>();
			LightmapSettings.lightmaps = list.ToArray();
		}
		else
		{
			Debug.LogWarning("high quality");
		}
	}

	public static string Combine(string a, string b)
	{
		if (a == null)
		{
			a = string.Empty;
		}
		if (b == null)
		{
			b = string.Empty;
		}
		return a + "/" + b;
	}
}
