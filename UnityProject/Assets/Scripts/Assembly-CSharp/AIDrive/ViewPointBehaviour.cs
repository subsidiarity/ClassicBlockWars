using UnityEngine;

[ExecuteInEditMode]
public class ViewPointBehaviour : MonoBehaviour
{
	private AIDriver aiDriver;

	private AIDriverController aiDriverController;

	public void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			return;
		}
		AIDriver component = base.gameObject.transform.parent.GetComponent<AIDriver>();
		if (component != null)
		{
			if (component.useObstacleAvoidance)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(base.gameObject.transform.position, 0.1f);
			}
			return;
		}
		AIDriverController component2 = base.gameObject.transform.parent.GetComponent<AIDriverController>();
		if (component2 != null && component2.useObstacleAvoidance)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(base.gameObject.transform.position, 0.1f);
		}
	}
}
