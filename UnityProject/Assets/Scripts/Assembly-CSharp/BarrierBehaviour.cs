using UnityEngine;

public class BarrierBehaviour : MonoBehaviour
{
	public bool show;

	public bool triggerCollider = true;

	private void Awake()
	{
		base.gameObject.collider.isTrigger = triggerCollider;
		base.gameObject.transform.renderer.enabled = show;
	}
}
