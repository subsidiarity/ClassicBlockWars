using UnityEngine;

[ExecuteInEditMode]
public class ShowRaycasts : MonoBehaviour
{
	public bool show;

	private AIDriver aiDriver;

	public void OnDrawGizmos()
	{
		if (!Application.isPlaying || show)
		{
			aiDriver = base.gameObject.GetComponent("AIDriver") as AIDriver;
			if (aiDriver.useObstacleAvoidance)
			{
				Vector3 dir = aiDriver.viewPoint.TransformDirection(Vector3.forward * aiDriver.oADistance);
				Vector3 start = base.transform.position + base.transform.TransformDirection(Vector3.left * aiDriver.oASideOffset);
				start.y = aiDriver.viewPoint.position.y;
				Vector3 start2 = base.transform.position + base.transform.TransformDirection(Vector3.right * aiDriver.oASideOffset);
				start2.y = aiDriver.viewPoint.position.y;
				Vector3 position = aiDriver.viewPoint.transform.position;
				Vector3 position2 = aiDriver.viewPoint.transform.position;
				position += aiDriver.viewPoint.TransformDirection(Vector3.right * aiDriver.flWheel.localPosition.x);
				position2 += aiDriver.viewPoint.TransformDirection(Vector3.right * aiDriver.frWheel.localPosition.x);
				float oAWidth = aiDriver.oAWidth;
				Vector3 dir2 = aiDriver.viewPoint.TransformDirection(Vector3.left * oAWidth + Vector3.forward * aiDriver.oADistance);
				Vector3 dir3 = aiDriver.viewPoint.TransformDirection(Vector3.right * oAWidth + Vector3.forward * aiDriver.oADistance);
				Vector3 dir4 = aiDriver.viewPoint.TransformDirection(Vector3.left * aiDriver.oASideDistance);
				Vector3 dir5 = aiDriver.viewPoint.TransformDirection(Vector3.right * aiDriver.oASideDistance);
				Debug.DrawRay(position, dir2, Color.cyan);
				Debug.DrawRay(position2, dir3, Color.cyan);
				Debug.DrawRay(aiDriver.viewPoint.position, dir, Color.green);
				Debug.DrawRay(position, dir, Color.green);
				Debug.DrawRay(position2, dir, Color.green);
				Debug.DrawRay(start, dir4, Color.magenta);
				Debug.DrawRay(start2, dir5, Color.magenta);
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			aiDriver = base.gameObject.GetComponent("AIDriver") as AIDriver;
			Transform transform = base.transform.FindChild("Colliders/ColliderBottom");
			aiDriver.oASideOffset = Mathf.Abs(transform.localPosition.x) + transform.localScale.x / 2f + 0.1f;
			Vector3 localPosition = aiDriver.viewPoint.localPosition;
			localPosition.z = transform.localPosition.z + transform.localScale.z / 2f + 0.1f;
			localPosition.x = 0f;
			aiDriver.viewPoint.localPosition = localPosition;
		}
	}
}
