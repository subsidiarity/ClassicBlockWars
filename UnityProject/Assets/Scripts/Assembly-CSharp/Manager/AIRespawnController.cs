using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRespawnController : MonoBehaviour
{
	public delegate void RespawnHandler(AIEventArgs e);

	public float heightOffset;

	private Transform currentRespawnPoint;

	[HideInInspector]
	public int currentRespawnPointInt;

	private bool isStartingRespawn;

	private AIDriverController aiDriverControllerScript;

	private List<Transform> waypoints;

	public float timeTillRespawn = 5f;

	[HideInInspector]
	public float lastTimeToReachNextWP;

	public static RespawnHandler onRespawnWaypoint;

	private void Awake()
	{
	}

	private void Start()
	{
		aiDriverControllerScript = base.gameObject.GetComponent("AIDriverController") as AIDriverController;
		waypoints = aiDriverControllerScript.waypoints;
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
		StartCoroutine(Freeze(1f));
		int currentWaypoint = aiDriverControllerScript.currentWaypoint;
		currentWaypoint = ((currentWaypoint != 0) ? (currentWaypoint - 1) : (waypoints.Count - 1));
		currentRespawnPoint = waypoints[currentWaypoint];
		Vector3 position = currentRespawnPoint.position;
		position.y += heightOffset;
		base.transform.position = position;
		base.transform.rotation = currentRespawnPoint.rotation;
		aiDriverControllerScript.aiSteerAngle = 0f;
		aiDriverControllerScript.currentAngle = 0f;
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

	private IEnumerator Freeze(float seconds)
	{
		base.gameObject.GetComponent<Rigidbody>().freezeRotation = true;
		base.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		yield return new WaitForSeconds(seconds);
		base.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
	}
}
