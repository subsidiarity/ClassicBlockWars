using UnityEngine;

public class deadToWater : MonoBehaviour
{
	private void Update()
	{
		if (GameController.thisScript.myPlayer != null
		&& (settings.offlineMode || GameController.thisScript.playerScript.photonView.isMine)
		&& !GameController.thisScript.playerScript.isDead && GameController.thisScript.myPlayer.transform.position.y < -3.5f)
		{
			dead();
		}
	}

	private void dead()
	{
		if (!settings.offlineMode && !GameController.thisScript.playerScript.photonView.isMine)
		{
			return;
		}

		if (GameController.thisScript.playerScript.inCar)
		{
			if (settings.offlineMode)
			{
				GameController.thisScript.carScript.getDamage(1000);
				return;
			}
			GameController.thisScript.carScript.photonView.RPC("getDamage", PhotonTargets.All, 1000);
		}
		else if (GameController.thisScript.playerScript.inHelic)
		{
			if (settings.offlineMode)
			{
				GameController.thisScript.helicopterScript.getDamage(1000);
				return;
			}
			GameController.thisScript.helicopterScript.photonView.RPC("getDamage", PhotonTargets.All, 1000);
		}
		else if (settings.offlineMode)
		{
			GameController.thisScript.playerScript.getDamage(1000);
		}
		else
		{
			GameController.thisScript.playerScript.photonView.RPC("getDamage", PhotonTargets.All, 1000, -9999);
		}
	}
}
