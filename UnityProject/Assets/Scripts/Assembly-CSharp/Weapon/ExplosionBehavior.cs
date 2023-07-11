using UnityEngine;

public class ExplosionBehavior : MonoBehaviour
{
	private GameObject[] players;

	private GameObject[] enemies;

	private GameObject[] cars;

	private int idMachine = -9999;

	public bool isPlayed;

	private void Start()
	{
	}

	private void Update()
	{
		if (isPlayed)
		{
			return;
		}
		if (settings.offlineMode)
		{
			enemies = GameObject.FindGameObjectsWithTag("enemy");
			GameObject[] array = enemies;
			foreach (GameObject gameObject in array)
			{
				if (base.collider.bounds.Contains(gameObject.gameObject.transform.position))
				{
					EnemyBehavior component = gameObject.GetComponent<EnemyBehavior>();
					if (component != null && !component.isDead)
					{
						component.getDamage(1000);
					}
				}
			}
		}
		players = GameObject.FindGameObjectsWithTag("Player");
		GameObject[] array2 = players;
		foreach (GameObject gameObject2 in array2)
		{
			if (!base.collider.bounds.Contains(gameObject2.gameObject.transform.position))
			{
				continue;
			}
			PlayerBehavior component2 = gameObject2.GetComponent<PlayerBehavior>();
			if (component2 != null && !component2.isDead)
			{
				if (settings.offlineMode)
				{
					component2.getDamage(1000);
					continue;
				}
				component2.photonView.RPC("getDamage", PhotonTargets.All, 1000, idMachine);
			}
		}
		cars = GameObject.FindGameObjectsWithTag("Car");
		GameObject[] array3 = cars;
		foreach (GameObject gameObject3 in array3)
		{
			if (!base.gameObject.transform.parent.gameObject.Equals(gameObject3) && base.collider.bounds.Contains(gameObject3.gameObject.transform.position))
			{
				CarBehavior component3 = gameObject3.GetComponent<CarBehavior>();
				if (component3 != null && !component3.isDead)
				{
					component3.deadWithDelay(1f);
				}
			}
		}
		isPlayed = true;
	}
}
