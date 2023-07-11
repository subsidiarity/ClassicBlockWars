using UnityEngine;

public class CheckPointBehavior : MonoBehaviour
{
	public Material nextPointMaterial;

	public Material currentMaterial;

	public bool canBeVisited;

	private bool pointCollected;

	public GameObject nextCheckPoint;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!col.tag.Equals("Player") && !col.tag.Equals("Car"))
		{
			return;
		}
		if (canBeVisited)
		{
			int dataFromCurrentMission = MissionManager.Instance.GetDataFromCurrentMission<int>("PassedPoints");
			MissionManager.Instance.SetDataForCurrentMission("PassedPoints", --dataFromCurrentMission);
			if (nextCheckPoint != null)
			{
				nextCheckPoint.GetComponent<CheckPointBehavior>().canBeVisited = true;
				NJGMapItem[] components = nextCheckPoint.GetComponents<NJGMapItem>();
				foreach (NJGMapItem nJGMapItem in components)
				{
					nJGMapItem.enabled = !nJGMapItem.enabled;
				}
				nextCheckPoint.SetActive(true);
				nextCheckPoint.renderer.material = currentMaterial;
				GameObject gameObject = nextCheckPoint.GetComponent<CheckPointBehavior>().nextCheckPoint;
				if (gameObject != null)
				{
					gameObject.SetActive(true);
					gameObject.renderer.material = nextPointMaterial;
				}
			}
			base.gameObject.SetActive(false);
		}
		else
		{
			MissionManager.Instance.FailCurrentMission();
		}
	}

	private void Update()
	{
	}
}
