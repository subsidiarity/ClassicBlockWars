using UnityEngine;

public class ControllerNavMesh : MonoBehaviour
{
	public BoxCollider[] allzone;

	private PlayerBehavior playerBeh;

	private WeaponManager weaponMan;

	private UnityEngine.AI.NavMeshAgent navMeshScript;

	public Vector3 goToPoint;

	private bool vistavlen;

	private float timerForce = 0.5f;

	public void Start()
	{
		allzone = EnemyGenerator.Instance.arrNavMeshzone;
		playerBeh = base.gameObject.GetComponent<PlayerBehavior>();
		weaponMan = base.gameObject.GetComponent<WeaponManager>();
		navMeshScript = base.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
		startMoveNavMesh();
	}

	private void Update()
	{
		if (Vector3.Distance(base.transform.position, goToPoint) < 2f)
		{
			chooseAndGoToNextPoint();
		}
		timerForce -= Time.deltaTime;
	}

	public void startMoveNavMesh()
	{
		chooseAndGoToNextPoint();
	}

	public void stopMoveNavMesh()
	{
		navMeshScript.ResetPath();
	}

	public void chooseAndGoToNextPoint()
	{
		BoxCollider boxCollider = allzone[Random.Range(0, allzone.Length)];
		float num = boxCollider.size.x * boxCollider.transform.localScale.x;
		float num2 = boxCollider.size.z * boxCollider.transform.localScale.z;
		Vector3 vector = new Vector3(boxCollider.transform.position.x + Random.Range((0f - num) * 0.5f, num * 0.5f), boxCollider.transform.position.y, boxCollider.transform.position.z + Random.Range((0f - num2) * 0.5f, num2 * 0.5f));
		if (vistavlen)
		{
			navMeshGoToPoint(vector);
			return;
		}
		base.transform.position = vector;
		vistavlen = true;
		chooseAndGoToNextPoint();
	}

	public void GoToPointForce(Vector3 point)
	{
		if (timerForce <= 0f)
		{
			timerForce = 0.5f;
			navMeshGoToPoint(point);
		}
	}

	private void navMeshGoToPoint(Vector3 point)
	{
		goToPoint = point;
		navMeshScript.SetDestination(goToPoint);
	}
}
