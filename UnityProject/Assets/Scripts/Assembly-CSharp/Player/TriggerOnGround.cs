using System.Collections.Generic;
using UnityEngine;

public class TriggerOnGround : MonoBehaviour
{
	public bool isOnGround;

	public List<GameObject> listTriggerObject = new List<GameObject>();

	private void Start()
	{
		HelicopterBehavior helicopterBehavior = NGUITools.FindInParents<HelicopterBehavior>(base.transform);
		if (helicopterBehavior != null)
		{
			helicopterBehavior.triggerOnGroundSript = this;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((other.tag.Equals("Untagged") || other.tag.Equals("ground")) && !listTriggerObject.Contains(other.gameObject))
		{
			listTriggerObject.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((other.tag.Equals("Untagged") || other.tag.Equals("ground")) && listTriggerObject.Contains(other.gameObject))
		{
			listTriggerObject.Remove(other.gameObject);
		}
	}

	private void FixedUpdate()
	{
		if (listTriggerObject.Count > 0)
		{
			isOnGround = true;
		}
		else
		{
			isOnGround = false;
		}
	}
}
