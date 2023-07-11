using UnityEngine;

public class ExploisonManager
{
	/*
	 * This function takes these odd parameters to keep compliant with the original explosion
	 * logic of the game.
	 */
    public static void Explode(Vector3 pos, float radius, int damage, int id,
	bool damage_helicopters = false, GameObject skip_car = null)
    {
        Collider[] colliders = Physics.OverlapSphere(pos, radius);
		foreach (Collider collider in colliders)
		{
			switch (collider.gameObject.tag)
			{
			case "enemy":
				EnemyBehavior enemy = collider.gameObject.GetComponent<EnemyBehavior>();
				if (enemy != null && !enemy.isDead)
				{
					enemy.getDamage(damage);
				}
				break;

			case "Player":
				PlayerBehavior player = collider.gameObject.GetComponent<PlayerBehavior>();
				if (player != null && !player.isDead)
				{
					if (settings.offlineMode)
					{
						player.getDamage(damage);
					}
					else
					{
						player.photonView.RPC("getDamage", PhotonTargets.All, damage, id);
					}
				}
				break;

			case "Car":
				CarBehavior car = collider.gameObject.GetComponent<CarBehavior>();

				if (car == null)
				{
					continue;
				}

				if (skip_car == null)
				{
					if (settings.offlineMode)
					{
						car.getDamage(damage);
					}
					else
					{
						car.photonView.RPC("getDamage", PhotonTargets.All, damage);
					}
				}
				else if (!skip_car.Equals(collider.gameObject)
				&& !car.isDead)
				{
					car.deadWithDelay(1f);
				}
				break;

            case "Helicopter":
                if (!damage_helicopters)
                {
                    continue;
                }

                HelicopterBehavior helicopter = collider.gameObject.GetComponent<HelicopterBehavior>();
				if (!(helicopter == null))
				{
					if (settings.offlineMode)
					{
						helicopter.getDamage(damage);
					}
					else
					{
						helicopter.photonView.RPC("getDamage", PhotonTargets.All, damage);
					}
					break;
				}
                break;
			}
		}
    }
}