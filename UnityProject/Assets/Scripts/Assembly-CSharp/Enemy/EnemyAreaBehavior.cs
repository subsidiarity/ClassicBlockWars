using UnityEngine;

public class EnemyAreaBehavior : MonoBehaviour
{
	public AreaLevel currentLevel;

	public float aggressiveDistance = 10f;

	public float chasingDistance = 40f;

	public float passiveDistance = 50f;

	public float distance;

	private EnemyBehavior enemyBehavior;

	private void Start()
	{
		enemyBehavior = GetComponent<EnemyBehavior>();
	}

	private void Update()
	{
		UpdateAreaLevel();
	}

	private void UpdateAreaLevel()
	{
		if (!(enemyBehavior.playerTarget != null))
		{
			return;
		}
		distance = Vector3.Distance(base.transform.position, enemyBehavior.playerTarget.transform.position);
		if (enemyBehavior.isAggressive)
		{
			if (distance < aggressiveDistance)
			{
				enemyBehavior.SwitchEnemyState(EnemyState.Aggressive);
			}
			else if (distance < passiveDistance)
			{
				enemyBehavior.SwitchEnemyState(EnemyState.Chasing);
			}
			else
			{
				enemyBehavior.SwitchEnemyState(EnemyState.Passive);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, aggressiveDistance);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, chasingDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, passiveDistance);
	}
}
