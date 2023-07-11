using UnityEngine;

public class SectorCreate : MonoBehaviour
{
	private void Start()
	{
		if (ManagerPreloadingSectors.thisScript != null)
		{
			ManagerPreloadingSectors.thisScript.AddCreateSectorToList(this);
		}
	}
}
