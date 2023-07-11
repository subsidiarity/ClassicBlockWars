using UnityEngine;

public class SetStartPosition : MonoBehaviour
{
	public int startWaypoint;

	public int heightOffset = 2;

	private void Start()
	{
		if (startWaypoint <= 0)
		{
			return;
		}
		AIDriverController aIDriverController = base.gameObject.GetComponent("AIDriverController") as AIDriverController;
		if (aIDriverController.waypoints.Count > startWaypoint)
		{
			Vector3 position = aIDriverController.waypoints[startWaypoint].position;
			position.y += heightOffset;
			base.gameObject.transform.position = position;
			base.gameObject.transform.rotation = aIDriverController.waypoints[startWaypoint].rotation;
			if (aIDriverController.waypoints.Count > startWaypoint + 1)
			{
				aIDriverController.currentWaypoint = startWaypoint + 1;
			}
			else
			{
				aIDriverController.currentWaypoint = 0;
			}
		}
		else
		{
			Debug.LogError("StartWaypoint number is to high. Maximum is" + (aIDriverController.waypoints.Count - 1) + ").");
		}
	}
}
