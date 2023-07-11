using UnityEngine;

public class SplinePathWaypoints : SplinePath
{
	public new bool active = true;

	public bool show;

	private string m_waypointPreName = "MyWaypoint";

	private string m_waypointFolder = "MyWaypoints";

	private Transform parent;

	protected override void Awake()
	{
		if (active)
		{
			Init();
		}
	}

	private void Start()
	{
		if (show && active)
		{
			SetRenderer(true);
		}
	}

	protected new virtual void OnDrawGizmos()
	{
		if (active && (!Application.isPlaying || show))
		{
			GetWaypointNames();
			FillPath();
			FillSequence();
			DrawGizmos();
		}
		if (Application.isPlaying)
		{
		}
	}

	private void Init()
	{
		GetWaypointNames();
		FillPath();
		FillSequence();
		parent = GameObject.Find(m_waypointFolder).transform;
		CreateNewWaypoints();
		RenamePathObjects();
	}

	private void CreateNewWaypoints()
	{
		int num = 0;
		GameObject original = Resources.LoadAssetAtPath("Assets/AIDriverToolkit/Prefabs/Waypoint.prefab", typeof(GameObject)) as GameObject;
		foreach (Vector3 item in sequence)
		{
			num++;
			if (num < sequence.Count || !loop)
			{
				GameObject waypoint = Object.Instantiate(original) as GameObject;
				waypoint.transform.position = item;
				waypoint.name = m_waypointPreName + num;
				waypoint.transform.parent = parent;
				AIWaypoint aIWaypoint = waypoint.GetComponent("AIWaypoint") as AIWaypoint;
				CopyParameters(ref waypoint, num);
			}
		}
	}

	private void CopyParameters(ref GameObject waypoint, int newIndex)
	{
		float num = newIndex / (steps + 1);
		int num2 = ((newIndex % (steps + 1) != 0) ? (1 + newIndex / (steps + 1)) : (newIndex / (steps + 1)));
		AIWaypoint aIWaypoint = path[num2 - 1].GetComponent("AIWaypoint") as AIWaypoint;
		AIWaypoint aIWaypoint2 = waypoint.GetComponent("AIWaypoint") as AIWaypoint;
		aIWaypoint2.speed = aIWaypoint.speed;
		aIWaypoint2.useTrigger = aIWaypoint.useTrigger;
		if (aIWaypoint2.useTrigger)
		{
			BoxCollider boxCollider = waypoint.AddComponent<BoxCollider>();
			boxCollider.isTrigger = true;
			waypoint.layer = 2;
		}
		waypoint.transform.localScale = path[num2 - 1].localScale;
		waypoint.tag = path[num2 - 1].gameObject.tag;
	}

	private void RenamePathObjects()
	{
		foreach (Transform item in path)
		{
			item.gameObject.name = item.gameObject.name + "_original";
		}
	}

	private void FillPath()
	{
		bool flag = true;
		int num = 1;
		path.Clear();
		while (flag)
		{
			string text = "/" + m_waypointFolder + "/" + m_waypointPreName + num;
			GameObject gameObject = GameObject.Find(text);
			if (gameObject != null)
			{
				path.Add(gameObject.transform);
				num++;
			}
			else
			{
				flag = false;
			}
		}
	}

	private void GetWaypointNames()
	{
		AIWaypointEditor aIWaypointEditor = GetComponent("AIWaypointEditor") as AIWaypointEditor;
		if (aIWaypointEditor != null)
		{
			m_waypointPreName = aIWaypointEditor.preName + "_";
			m_waypointFolder = aIWaypointEditor.folderName;
		}
	}

	private void SetRenderer(bool active)
	{
		bool flag = true;
		int num = 1;
		path.Clear();
		while (flag)
		{
			string text = "/" + m_waypointFolder + "/" + m_waypointPreName + num;
			GameObject gameObject = GameObject.Find(text);
			if (gameObject != null)
			{
				gameObject.renderer.enabled = active;
				num++;
			}
			else
			{
				flag = false;
			}
		}
	}

	private void SetDrawLineToNext()
	{
		if (active)
		{
		}
		bool flag = true;
		int num = 1;
		path.Clear();
		while (flag)
		{
			string text = "/" + m_waypointFolder + "/" + m_waypointPreName + num;
			GameObject gameObject = GameObject.Find(text);
			if (gameObject != null)
			{
				DrawLineToNext component = gameObject.GetComponent<DrawLineToNext>();
				if (component != null)
				{
					if (active)
					{
						component.active = false;
					}
					else
					{
						component.active = true;
					}
				}
			}
			else
			{
				flag = false;
			}
		}
	}
}
