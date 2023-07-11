using System.Collections.Generic;
using UnityEngine;

public class AIRespawn : MonoBehaviour
{
	public delegate void RespawnHandler(AIEventArgs e);

	private Transform currentRespawnPoint;

	public int currentRespawnPointInt;

	public List<WheelCollider> myWcs;

	private bool isStartingRespawn;

	private AIDriver aiDriverScript;

	private List<Transform> waypoints;

	public float timeTillRespawn = 5f;

	public float lastTimeToReachNextWP;

	public static RespawnHandler onRespawnWaypoint;

	private void Awake()
	{
	}

	private void Start()
	{
		aiDriverScript = base.gameObject.GetComponent("AIDriver") as AIDriver;
		waypoints = aiDriverScript.waypoints;
	}

	public void StartRespawn()
	{
		if (!isStartingRespawn)
		{
			isStartingRespawn = true;
			Respawn();
		}
	}

	private void Update()
	{
		if (!IsCorrectMoving())
		{
			StartRespawn();
		}
	}

	private void Respawn()
	{
		int currentWaypoint = aiDriverScript.currentWaypoint;
		currentWaypoint = ((currentWaypoint != 0) ? (currentWaypoint - 1) : (waypoints.Count - 1));
		currentRespawnPoint = waypoints[currentWaypoint];
		base.transform.position = currentRespawnPoint.position;
		base.transform.rotation = currentRespawnPoint.rotation;
		aiDriverScript.aiSteerAngle = 0f;
		aiDriverScript.currentAngle = 0f;
		isStartingRespawn = false;
		lastTimeToReachNextWP = 0f;
		if (onRespawnWaypoint != null)
		{
			AIEventArgs aIEventArgs = new AIEventArgs();
			aIEventArgs.name = base.gameObject.name;
			aIEventArgs.currentWaypointIndex = currentWaypoint;
			aIEventArgs.currentWaypointName = waypoints[currentWaypoint].name;
			aIEventArgs.position = base.gameObject.transform.position;
			aIEventArgs.rotation = base.gameObject.transform.rotation;
			aIEventArgs.tag = base.gameObject.tag;
			onRespawnWaypoint(aIEventArgs);
		}
	}

	private bool IsCorrectMoving()
	{
		bool result = true;
		lastTimeToReachNextWP += Time.deltaTime;
		if (lastTimeToReachNextWP >= timeTillRespawn)
		{
			result = false;
		}
		return result;
	}
}
