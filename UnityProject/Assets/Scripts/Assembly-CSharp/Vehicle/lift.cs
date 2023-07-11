using Holoville.HOTween;
using UnityEngine;

public class lift : MonoBehaviour
{
	public Vector3 downPoint = Vector3.zero;

	public Vector3 topPoint = Vector3.zero;

	public float speedMove = 5f;

	public float timeDalay = 5f;

	private PlayerBehavior myPlayerScript;

	private float timeMove = 10f;

	private void Start()
	{
		HOTween.Init();
		timeMove = topPoint.y - downPoint.y;
		timeMove /= speedMove;
		base.gameObject.transform.localPosition = downPoint;
		moveUp();
	}

	private void moveUp()
	{
		HOTween.To(base.transform, timeMove, new TweenParms().Delay(timeDalay).Prop("localPosition", topPoint).Ease(EaseType.Linear)
			.OnComplete(moveDown)
			.OnUpdate(resetPlayerAnim));
	}

	private void moveDown()
	{
		HOTween.To(base.transform, timeMove, new TweenParms().Delay(timeDalay).Prop("localPosition", downPoint).Ease(EaseType.Linear)
			.OnComplete(moveUp)
			.OnUpdate(resetPlayerAnim));
	}

	private void resetPlayerAnim()
	{
		if (myPlayerScript != null)
		{
			myPlayerScript.tController.inAirVelocity = Vector3.zero;
			myPlayerScript.tController.verticalSpeed = 0f;
			myPlayerScript.tController.moveSpeed = 0f;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Debug.Log("Enter");
			myPlayerScript = GameController.thisScript.playerScript;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player")
		{
			myPlayerScript = null;
		}
	}
}
