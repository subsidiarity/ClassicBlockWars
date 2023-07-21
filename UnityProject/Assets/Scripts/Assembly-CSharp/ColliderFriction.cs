using UnityEngine;

public class ColliderFriction : MonoBehaviour
{
	public float frictionValue = 1f;

	private void Start()
	{
		base.GetComponent<Collider>().material.staticFriction = frictionValue;
		//base.GetComponent<Collider>().material.staticFriction2 = frictionValue;
		base.GetComponent<Collider>().material.dynamicFriction = frictionValue;
		//base.GetComponent<Collider>().material.dynamicFriction2 = frictionValue;
		base.GetComponent<Collider>().material.bounciness = 0f;
	}
}
