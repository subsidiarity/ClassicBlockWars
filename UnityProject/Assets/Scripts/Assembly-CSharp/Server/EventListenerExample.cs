using System;
using System.Collections;
using UnityEngine;

public class EventListenerExample : MonoBehaviour
{
	public GameObject respawnEffect;

	private string message;

	private void OnEnable()
	{
		AIDriverController.onLastWaypoint = (AIDriverController.LastWaypointHandler)Delegate.Combine(AIDriverController.onLastWaypoint, new AIDriverController.LastWaypointHandler(onLastWaypoint));
		AIRespawnController.onRespawnWaypoint = (AIRespawnController.RespawnHandler)Delegate.Combine(AIRespawnController.onRespawnWaypoint, new AIRespawnController.RespawnHandler(onRespawnWaypoint));
	}

	private void OnDisable()
	{
		AIDriverController.onLastWaypoint = (AIDriverController.LastWaypointHandler)Delegate.Remove(AIDriverController.onLastWaypoint, new AIDriverController.LastWaypointHandler(onLastWaypoint));
		AIRespawnController.onRespawnWaypoint = (AIRespawnController.RespawnHandler)Delegate.Remove(AIRespawnController.onRespawnWaypoint, new AIRespawnController.RespawnHandler(onRespawnWaypoint));
	}

	private void onLastWaypoint(AIEventArgs e)
	{
	}

	private void onRespawnWaypoint(AIEventArgs e)
	{
		UnityEngine.Object.Instantiate(respawnEffect, e.position, e.rotation);
	}

	private IEnumerator ShowMessage(string text, float seconds)
	{
		message = text;
		yield return new WaitForSeconds(seconds);
		message = string.Empty;
	}

	private void OnGUI()
	{
		GUILayout.Space(20f);
		GUILayout.Label(message);
	}
}
