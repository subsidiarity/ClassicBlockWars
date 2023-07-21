using UnityEngine;

public class BarrierBehaviour : MonoBehaviour
{
	public bool show;

	public bool triggerCollider = true;

	private void Awake()
	{
		base.gameObject.GetComponent<Collider>().isTrigger = triggerCollider;
		base.gameObject.transform.GetComponent<Renderer>().enabled = show;
	}
}
