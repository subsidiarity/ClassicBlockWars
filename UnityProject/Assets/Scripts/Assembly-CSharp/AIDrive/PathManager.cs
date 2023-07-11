using System;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	public Transform[] waypoints;

	public bool drawStraight = true;

	public bool drawCurved = true;

	public Color color1 = new Color(1f, 0f, 1f, 0.5f);

	public Color color2 = new Color(1f, 47f / 51f, 0.015686275f, 0.5f);

	public Color color3 = new Color(1f, 47f / 51f, 0.015686275f, 0.5f);

	private float radius = 0.4f;

	private Vector3 size = new Vector3(0.7f, 0.7f, 0.7f);

	public GameObject waypointPrefab;

	private Vector3[] points;

	private void OnDrawGizmos()
	{
		foreach (Transform item in base.transform)
		{
			if (item.name == "Waypoint")
			{
				Gizmos.color = color2;
				Gizmos.DrawWireSphere(item.position, radius);
			}
			else if (item.name == "WaypointStart" || item.name == "WaypointEnd")
			{
				Gizmos.color = color1;
				Gizmos.DrawWireCube(item.position, size);
			}
		}
		if (drawStraight)
		{
			DrawStraight();
		}
		if (drawCurved)
		{
			DrawCurved();
		}
	}

	private void DrawStraight()
	{
		iTween.DrawLine(waypoints, color2);
	}

	private void DrawCurved()
	{
		if (waypoints.Length >= 2)
		{
			points = new Vector3[waypoints.Length + 2];
			for (int i = 0; i < waypoints.Length; i++)
			{
				points[i + 1] = waypoints[i].position;
			}
			points[0] = points[1];
			points[points.Length - 1] = points[points.Length - 2];
			Gizmos.color = color3;
			int num = points.Length * 10;
			Vector3[] array = new Vector3[num + 1];
			for (int j = 0; j <= num; j++)
			{
				float t = (float)j / (float)num;
				Vector3 point = GetPoint(t);
				array[j] = point;
			}
			Vector3 to = array[0];
			for (int k = 1; k < array.Length; k++)
			{
				Vector3 point = array[k];
				Gizmos.DrawLine(point, to);
				to = point;
			}
		}
	}

	private Vector3 GetPoint(float t)
	{
		int num = points.Length - 3;
		int num2 = (int)Math.Floor(t * (float)num);
		int num3 = num - 1;
		if (num3 > num2)
		{
			num3 = num2;
		}
		float num4 = t * (float)num - (float)num3;
		Vector3 vector = points[num3];
		Vector3 vector2 = points[num3 + 1];
		Vector3 vector3 = points[num3 + 2];
		Vector3 vector4 = points[num3 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num4 * num4 * num4) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num4 * num4) + (-vector + vector3) * num4 + 2f * vector2);
	}
}
