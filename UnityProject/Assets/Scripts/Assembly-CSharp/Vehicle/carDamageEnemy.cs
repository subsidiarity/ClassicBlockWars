using UnityEngine;

public class carDamageEnemy : MonoBehaviour
{
	private int minDamageSpeed = 30;

	private NewDriving newDrivingScript;

	private CarBehavior carScript;

	private void Awake()
	{
		newDrivingScript = GetComponent<NewDriving>();
		carScript = GetComponent<CarBehavior>();
	}

	private void OnTriggerEnter(Collider other)
	{
		string text;
		if (settings.offlineMode)
		{
			text = other.tag;
			if (text.Equals("enemy") && newDrivingScript.currentSpeedReal > settings.speedCarForIgnoreEnemy)
			{
				EnemyBehavior component = other.GetComponent<EnemyBehavior>();
				if (newDrivingScript.currentSpeedReal < settings.speedCarForHighDemageEnemy)
				{
					component.lowDamageCar(35);
				}
				else
				{
					component.highDamageCar(10000);
				}
			}
		}
		else if (other.transform.parent != null)
		{
			text = other.transform.parent.gameObject.tag;
			if (text.Equals("Player") && newDrivingScript.currentSpeedReal > settings.speedCarForIgnoreEnemy)
			{
				PlayerBehavior component2 = other.transform.parent.gameObject.GetComponent<PlayerBehavior>();
				if (component2 == null)
				{
					return;
				}
				if (newDrivingScript.currentSpeedReal < settings.speedCarForHighDemageEnemy)
				{
					component2.photonView.RPC("lowDamageCar", PhotonTargets.All, 35, carScript.idPlayerInCar);
				}
				else
				{
					component2.photonView.RPC("highDamageCar", PhotonTargets.All, 10000, carScript.idPlayerInCar);
				}
			}
		}
		text = other.tag;
		if (other.gameObject != base.gameObject && carScript.objPlayerInCar != null && !text.Equals("ground") && !text.Equals("enemy") && !text.Equals("collidePoint") && !text.Equals("pointExitCar") && newDrivingScript.currentSpeedReal >= minDamageSpeed)
		{
			if (settings.offlineMode)
			{
				carScript.getDamage((int)((float)newDrivingScript.currentSpeedReal * 0.04f));
				return;
			}
			Debug.Log("DEBUUUUUG: " + (float)newDrivingScript.currentSpeedReal * 0.04f);
			carScript.photonView.RPC("getDamage", PhotonTargets.All, (int)((float)newDrivingScript.currentSpeedReal * 0.04f));
		}
	}
}
