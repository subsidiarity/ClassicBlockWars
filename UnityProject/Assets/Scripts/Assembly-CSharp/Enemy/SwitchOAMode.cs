using UnityEngine;

public class SwitchOAMode : MonoBehaviour
{
	public string tagName = "Untagged";

	public bool switchUseOaTo;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.transform.root.gameObject.tag == tagName)
		{
			AIDriverController componentInChildren = other.gameObject.transform.root.gameObject.GetComponentInChildren<AIDriverController>();
			if (componentInChildren != null)
			{
				componentInChildren.SwitchOaMode(switchUseOaTo);
			}
		}
	}
}
