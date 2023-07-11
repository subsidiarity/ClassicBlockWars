using UnityEngine;

// TODO: Is this used or is just the optical amier used?
public class ManualAimerZone : MonoBehaviour
{
	[HideInInspector]
	public GameObject touchedObject;

	private void Start()
	{
	}

	private void OnPress(bool isDown)
	{
		if (!isDown)
		{
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(UICamera.currentTouch.pos);
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo))
		{
			return;
		}
		if (hitInfo.transform.tag.Equals("collidePoint") || hitInfo.transform.tag.Equals("Car"))
		{
			if (!hitInfo.transform.root.gameObject.Equals(GameController.thisScript.myPlayer))
			{
				if (hitInfo.transform.tag.Equals("collidePoint"))
				{
					touchedObject = hitInfo.transform.parent.gameObject;
				}
				else
				{
					touchedObject = hitInfo.transform.gameObject;
				}
				Debug.Log(touchedObject.name);
				GameController.thisScript.playerScript.tController.moveDirection.x = (hitInfo.transform.position - GameController.thisScript.playerScript.transform.position).normalized.x;
				GameController.thisScript.playerScript.tController.moveDirection.z = (hitInfo.transform.position - GameController.thisScript.playerScript.transform.position).normalized.z;
			}
		}
		else
		{
			touchedObject = null;
		}
	}
}
