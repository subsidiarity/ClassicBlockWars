using UnityEngine;

public class ColliderFriction : MonoBehaviour
{
	public float frictionValue = 1f;

	private void Start()
	{
		base.collider.material.staticFriction = frictionValue;
		base.collider.material.staticFriction2 = frictionValue;
		base.collider.material.dynamicFriction = frictionValue;
		base.collider.material.dynamicFriction2 = frictionValue;
		base.collider.material.bounciness = 0f;
	}
}
