using System.Collections.Generic;
using NJG;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[ExecuteInEditMode]
[AddComponentMenu("NJG MiniMap/Map Zone")]
public class NJGMapZone : MonoBehaviour
{
	public static List<NJGMapZone> list = new List<NJGMapZone>();

	public static int id = 0;

	public string triggerTag = "Player";

	public string zone;

	public string level;

	public int colliderRadius = 10;

	public int mId;

	private SphereCollider mCollider;

	private NJGMapBase map;

	public Color color
	{
		get
		{
			return (!(map == null)) ? map.GetZoneColor(level, zone) : Color.white;
		}
	}

	private void Awake()
	{
		map = NJGMapBase.instance;
		id++;
		mId = id;
		mCollider = GetComponent<SphereCollider>();
		mCollider.isTrigger = true;
		mCollider.radius = colliderRadius;
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.CompareTag(triggerTag) && map != null)
		{
			map.zoneColor = color;
			map.worldName = zone;
		}
	}

	private void OnEnable()
	{
		list.Add(this);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void OnDestroy()
	{
		id--;
	}
}
