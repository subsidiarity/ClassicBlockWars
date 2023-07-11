using UnityEngine;

[ExecuteInEditMode]
public class ShowControllerRaycasts : MonoBehaviour
{
	public bool show;

	private AIDriverController aiDriverController;

	private AIMotorMapping aIMotorMapping;

	public void OnDrawGizmos()
	{
		if (!Application.isPlaying || show)
		{
			aiDriverController = base.gameObject.GetComponent("AIDriverController") as AIDriverController;
			aIMotorMapping = base.gameObject.GetComponent("AIMotorMapping") as AIMotorMapping;
			if (aiDriverController.useObstacleAvoidance && aIMotorMapping.flWheelMesh != null)
			{
				Vector3 dir = aiDriverController.viewPoint.TransformDirection(Vector3.forward * aiDriverController.oADistance);
				Vector3 position = aiDriverController.viewPoint.transform.position;
				Vector3 position2 = aiDriverController.viewPoint.transform.position;
				position += aiDriverController.viewPoint.TransformDirection(Vector3.right * aIMotorMapping.flWheelMesh.localPosition.x);
				position2 += aiDriverController.viewPoint.TransformDirection(Vector3.right * aIMotorMapping.frWheelMesh.localPosition.x);
				float oAWidth = aiDriverController.oAWidth;
				Vector3 dir2 = aiDriverController.viewPoint.TransformDirection(Vector3.left * oAWidth + Vector3.forward * aiDriverController.oADistance);
				Vector3 dir3 = aiDriverController.viewPoint.TransformDirection(Vector3.right * oAWidth + Vector3.forward * aiDriverController.oADistance);
				Vector3 dir4 = aiDriverController.viewPoint.TransformDirection(Vector3.left * aiDriverController.oASideDistance);
				Vector3 dir5 = aiDriverController.viewPoint.TransformDirection(Vector3.right * aiDriverController.oASideDistance);
				Debug.DrawRay(position, dir2, Color.cyan);
				Debug.DrawRay(position2, dir3, Color.cyan);
				Debug.DrawRay(aiDriverController.viewPoint.position, dir, Color.green);
				Debug.DrawRay(position, dir, Color.green);
				Debug.DrawRay(position2, dir, Color.green);
				Vector3 start = base.transform.position + base.transform.TransformDirection(Vector3.left * aiDriverController.oASideOffset);
				start.y = aiDriverController.viewPoint.position.y;
				start += base.transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
				Debug.DrawRay(start, dir4, Color.magenta);
				Vector3 start2 = base.transform.position + base.transform.TransformDirection(Vector3.left * aiDriverController.oASideOffset);
				start2.y = aiDriverController.viewPoint.position.y;
				start2 -= base.transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
				Debug.DrawRay(start2, dir4, Color.magenta);
				Vector3 start3 = base.transform.position + base.transform.TransformDirection(Vector3.right * aiDriverController.oASideOffset);
				start3.y = aiDriverController.viewPoint.position.y;
				start3 += base.transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
				Debug.DrawRay(start3, dir5, Color.magenta);
				Vector3 start4 = base.transform.position + base.transform.TransformDirection(Vector3.right * aiDriverController.oASideOffset);
				start4.y = aiDriverController.viewPoint.position.y;
				start4 -= base.transform.TransformDirection(Vector3.forward * aiDriverController.oASideFromMid);
				Debug.DrawRay(start4, dir5, Color.magenta);
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			aiDriverController = base.gameObject.GetComponent("AIDriverController") as AIDriverController;
			Transform transform = base.transform.FindChild("Colliders/ColliderBottom");
			if (transform != null)
			{
				aiDriverController.oASideOffset = Mathf.Abs(transform.localPosition.x) + transform.localScale.x / 2f + 0.1f;
				Vector3 localPosition = aiDriverController.viewPoint.localPosition;
				localPosition.z = transform.localPosition.z + transform.localScale.z / 2f + 0.1f;
				localPosition.x = 0f;
				aiDriverController.viewPoint.localPosition = localPosition;
			}
		}
	}
}
