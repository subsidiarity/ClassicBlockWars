using UnityEngine;

public class SectorCreate : MonoBehaviour
{
	private void OnEnable()
	{
		Debug.Log("Sector create enabled");
		if (ManagerPreloadingSectors.thisScript != null)
		{
			ManagerPreloadingSectors.thisScript.AddCreateSectorToList(this);
		}
	}
}
