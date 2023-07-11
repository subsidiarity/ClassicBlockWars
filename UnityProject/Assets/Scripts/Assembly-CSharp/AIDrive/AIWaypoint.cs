using UnityEngine;

public class AIWaypoint : MonoBehaviour
{
	public int speed = 100;

	public bool useTrigger;

	private void Awake()
	{
		base.renderer.enabled = false;
		if (useTrigger)
		{
			BoxCollider boxCollider = base.gameObject.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			base.gameObject.layer = 2;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!useTrigger)
		{
			return;
		}
		AIWaypointEditor componentInChildren = other.gameObject.transform.root.gameObject.GetComponentInChildren<AIWaypointEditor>();
		if (componentInChildren != null && componentInChildren.folderName == base.gameObject.transform.parent.name)
		{
			AIDriverController componentInChildren2 = other.gameObject.transform.root.gameObject.GetComponentInChildren<AIDriverController>();
			if (componentInChildren2 != null && componentInChildren2.waypoints.Count > componentInChildren2.currentWaypoint && componentInChildren2.waypoints[componentInChildren2.currentWaypoint].gameObject.name == base.gameObject.name)
			{
				componentInChildren2.NextWaypoint();
			}
		}
	}
}
