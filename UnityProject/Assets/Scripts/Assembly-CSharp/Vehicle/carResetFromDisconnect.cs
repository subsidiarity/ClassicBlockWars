using Photon;
using UnityEngine;

public class carResetFromDisconnect : Photon.MonoBehaviour
{
	private CarBehavior carScript;

	private HelicopterBehavior helicScript;

	private void Start()
	{
		if (settings.offlineMode)
		{
			Object.Destroy(this);
		}
		carScript = GetComponent<CarBehavior>();
		helicScript = GetComponent<HelicopterBehavior>();
	}

	private void FixedUpdate()
	{
		if (carScript != null)
		{
			if (carScript.objPlayerInCar == null && carScript.playerInCar)
			{
				Debug.Log("reset car from disconnect =" + base.photonView.owner);
				carScript.reset();
			}
		}
		else if (helicScript.objPlayerInHelic == null && helicScript.playerInHelic)
		{
			Debug.Log("reset helic from disconnect =" + base.photonView.owner);
			helicScript.reset();
		}
	}
}
