using UnityEngine;

public class Ladder : MonoBehaviour
{
	public GameObject objPlayer;

	private bool nazemle = true;

	private bool vCollidere;

	private Vector3 vectorUp = new Vector3(0f, 40f, 0f);

	private Vector3 vactorDown = new Vector3(0f, -0.3f, 0f);

	private float nachVisota;

	private float climbSpeed = 1f;

	private bool playerIsNear;

	private bool isClimbing;

	private float jumpedBt;

	private ThirdPersonController moveControl;

	private CharacterController charControl;

	private void OnTriggerEnter(Collider other)
	{
		if (!other.tag.Equals("collidePoint"))
		{
			return;
		}
		objPlayer = other.transform.root.gameObject;
		if (objPlayer.Equals(GameController.thisScript.myPlayer))
		{
			charControl = objPlayer.GetComponent<CharacterController>();
			moveControl = objPlayer.GetComponent<ThirdPersonController>();
			if (charControl != null && moveControl != null)
			{
				playerIsNear = true;
				moveControl.verticalSpeed = 0f;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag.Equals("collidePoint"))
		{
			objPlayer = null;
			isClimbing = false;
			playerIsNear = false;
		}
	}

	private void exitCollider()
	{
		objPlayer = null;
		vCollidere = false;
		GameController.thisScript.curScriptLadder = null;
	}

	private void FixedUpdate()
	{
		if (GameController.thisScript.myPlayer == null)
		{
			return;
		}
		if (GameController.thisScript.myPlayer.Equals(objPlayer) && moveControl.joyScript.position.y >= 0.94f && playerIsNear)
		{
			isClimbing = true;
			moveControl.jumping = false;
		}
		if (playerIsNear && isClimbing)
		{
			if (moveControl.jumping)
			{
				isClimbing = false;
			}
			if (!moveControl.IsGrounded())
			{
				moveControl.inAirVelocity = Vector3.zero;
				moveControl.moveSpeed = 0f;
			}
			if (Mathf.Abs(moveControl.joyScript.position.y) <= 0.1f)
			{
				moveControl.verticalSpeed = 0.6f;
			}
			if (moveControl.verticalSpeed < -6f)
			{
				moveControl.verticalSpeed = -6f;
			}
			else if (Mathf.Abs(moveControl.verticalSpeed) <= Mathf.Abs(moveControl.gravity * 0.5f))
			{
				moveControl.verticalSpeed += moveControl.joyScript.position.y;
			}
		}
	}

	public void dontUselLestnicu()
	{
		GameController.thisScript.useLestncu = false;
		moveControl.gravity = 20f;
		objPlayer = null;
		nazemle = true;
		GameController.thisScript.curScriptLadder = null;
	}
}
