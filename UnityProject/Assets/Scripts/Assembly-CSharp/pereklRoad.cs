using UnityEngine;

public class pereklRoad : MonoBehaviour
{
	public PathManager nachRoad;

	public PathManager Road;

	public hoMove nachRoadMove;

	public hoMove RoadMove;

	private void Start()
	{
		SetRoadsAndStart();
	}

	public void SetRoadsAndStart()
	{
		if (Road != null)
		{
			RoadMove.SetPath(Road);
			if (nachRoad == null)
			{
				RoadMove.StartMove();
			}
		}
		if (nachRoad != null)
		{
			nachRoadMove.SetPath(nachRoad);
			nachRoadMove.InitializeMessageOptions();
			nachRoadMove._messages[nachRoadMove._messages.Count - 1].message[0] = "perekluchitRoad";
			nachRoadMove.StartMove();
		}
	}

	private void perekluchitRoad()
	{
		if (nachRoad != null)
		{
			Object.Destroy(nachRoadMove);
			RoadMove.StartMove();
		}
	}
}
