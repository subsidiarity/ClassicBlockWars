using System.Collections.Generic;
using Holoville.HOTween;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
	public static readonly Dictionary<string, PathManager> Paths = new Dictionary<string, PathManager>();

	private void Awake()
	{
		foreach (Transform item in base.transform)
		{
			AddPath(item.gameObject);
		}
		HOTween.Init(true, true, true);
		HOTween.EnableOverwriteManager();
		HOTween.showPathGizmos = true;
	}

	public static void AddPath(GameObject path)
	{
		if (path.name.Contains("Clone"))
		{
			path.name = path.name.Replace("(Clone)", string.Empty);
		}
		if (Paths.ContainsKey(path.name))
		{
			Debug.LogWarning("Called AddPath() but Scene already contains Path " + path.name + ".");
			return;
		}
		PathManager componentInChildren = path.GetComponentInChildren<PathManager>();
		if (componentInChildren == null)
		{
			Debug.LogWarning("Called AddPath() but Transform " + path.name + " has no PathManager attached.");
		}
		else
		{
			Paths.Add(path.name, componentInChildren);
		}
	}

	private void OnDestroy()
	{
		Paths.Clear();
	}
}
