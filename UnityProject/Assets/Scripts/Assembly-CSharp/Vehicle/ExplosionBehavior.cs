using UnityEngine;
using System.Collections;

// TODO: It would be best to just remove this file and do the explosion calling
// logic in the car's script.
/* This handles explosions for vehicles. */
public class ExplosionBehavior : MonoBehaviour
{
	private int idMachine = -9999;

	private void Start()
	{
		ExplosionManager.Explode(
			gameObject.transform.position,
			gameObject.GetComponent<SphereCollider>().radius,
			1000,
			idMachine,
			false,
			GetComponent<Collider>().gameObject
		);

		this.enabled = false;
	}
}
