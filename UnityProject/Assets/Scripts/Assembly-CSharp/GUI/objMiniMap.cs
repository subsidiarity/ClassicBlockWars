using UnityEngine;

internal class objMiniMap
{
	public GameObject objToWorld;

	public GameObject sprMiniMap;

	public objMiniMap(GameObject curGameObj, GameObject curSprMiniMap)
	{
		objToWorld = curGameObj;
		sprMiniMap = curSprMiniMap;
	}
}
