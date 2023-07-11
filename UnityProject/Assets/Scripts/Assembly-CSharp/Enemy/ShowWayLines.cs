using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ShowWayLines : MonoBehaviour
{
	public bool show;

	public Color color = Color.magenta;

	private AIDriverController aiDriverController;

	public void OnDrawGizmos()
	{
		if (Application.isPlaying && !show)
		{
			return;
		}
		aiDriverController = base.gameObject.GetComponent("AIDriverController") as AIDriverController;
		List<Transform> waypoints = aiDriverController.waypoints;
		Vector3 vector = Vector3.zero;
		foreach (Transform item in waypoints)
		{
			if (vector != Vector3.zero)
			{
				Debug.DrawRay(vector, item.position, color);
			}
			vector = item.position;
		}
	}
}
