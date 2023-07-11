using UnityEngine;

public class UserInput : MonoBehaviour
{
	public PathManager pathContainer;

	public float speed = 10f;

	public float sizeToAdd = 1f;

	private Transform[] waypoints;

	public int currentPoint;

	private Transform[] currentPath = new Transform[2];

	public float progress;

	private float avgSpeed;

	private void Start()
	{
		waypoints = pathContainer.waypoints;
		currentPath[0] = waypoints[currentPoint];
		currentPath[1] = waypoints[currentPoint + 1];
		avgSpeed = speed / Vector3.Distance(currentPath[0].position, currentPath[1].position) * 100f;
	}

	private void Update()
	{
		if (Input.GetKey("right"))
		{
			progress += Time.deltaTime * avgSpeed;
		}
		if (Input.GetKey("left"))
		{
			progress -= Time.deltaTime * avgSpeed;
		}
		if (progress < 0f && currentPoint > 0)
		{
			currentPath[0] = waypoints[currentPoint - 1];
			currentPath[1] = waypoints[currentPoint];
			currentPoint--;
			progress = 100f;
			avgSpeed = speed / Vector3.Distance(currentPath[0].position, currentPath[1].position) * 100f;
		}
		else if (progress > 100f && currentPoint < waypoints.Length - 2)
		{
			currentPoint++;
			currentPath[0] = waypoints[currentPoint];
			currentPath[1] = waypoints[currentPoint + 1];
			progress = 0f;
			avgSpeed = speed / Vector3.Distance(currentPath[0].position, currentPath[1].position) * 100f;
		}
		else
		{
			if (progress <= 0f)
			{
				progress = 0f;
			}
			if (progress >= 100f)
			{
				progress = 100f;
			}
		}
		PointOnPath(progress / 100f);
	}

	private void PointOnPath(float number)
	{
		base.transform.position = iTween.PointOnPath(currentPath, number) + new Vector3(0f, sizeToAdd, 0f);
	}
}
