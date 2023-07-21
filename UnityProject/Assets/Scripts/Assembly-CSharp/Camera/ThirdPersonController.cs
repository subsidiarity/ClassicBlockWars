using System;
using Photon;
using UnityEngine;

public class ThirdPersonController : Photon.MonoBehaviour
{
	private enum CharacterState
	{
		Idle = 0,
		Walking = 1,
		Trotting = 2,
		Running = 3,
		Jumping = 4
	}

	public AnimationClip idleAnimation;

	public AnimationClip walkAnimation;

	public AnimationClip runAnimation;

	public AnimationClip jumpPoseAnimation;

	public float walkMaxAnimationSpeed = 0.75f;

	public float trotMaxAnimationSpeed = 1f;

	public float runMaxAnimationSpeed = 1f;

	public float jumpAnimationSpeed = 1.15f;

	public float landAnimationSpeed = 1f;

	public GameObject weaponAnimationObject;

	public GameObject joystick;

	private GameObject animationObject;

	private CharacterController controller;

	[HideInInspector]
	public UIJoystick joyScript;

	private Animation _animation;

	public float jetpackAcceleration;

	public PlayerBehavior pBehavior;

	public JetpackBehavior jetpack;

	public AudioClip jetpackSound;

	public AudioClip[] arrWalkSound;

	public float stamina = 1f;

	public bool isRunning;

	public bool isPlayAnimation = true;

	public bool needPlayAnimDrop;

	private CharacterState _characterState;

	private CharacterState pred_CharacterState;

	public float walkSpeed = 2f;

	public float trotSpeed = 4f;

	public float runSpeed = 6f;

	public float inAirControlAcceleration = 3f;

	public float jumpHeight = 0.5f;

	public float gravity = 20f;

	public float speedSmoothing = 10f;

	public float rotateSpeed = 500f;

	public float trotAfterSeconds = 3f;

	public bool canJump = true;

	private float jumpRepeatTime = 0.05f;

	private float jumpTimeout = 0.15f;

	private float groundedTimeout = 0.25f;

	private float lockCameraTimer;

	public Vector3 moveDirection = Vector3.zero;

	public float verticalSpeed;

	public float moveSpeed;

	private CollisionFlags collisionFlags;

	public bool jumping;

	private bool jumpingReachedApex;

	private bool movingBack;

	public bool isMoving;

	private float walkTimeStart;

	public float lastJumpButtonTime = -10f;

	private float lastJumpTime = -1f;

	private bool gotWM;

	private float lastJumpStartHeight;

	public Vector3 inAirVelocity = Vector3.zero;

	private float lastGroundedTime;

	private bool isControllable = true;

	public float downSpeedMove = 1f;

	private float dist;

	private Vector3 predPos = Vector3.zero;

	private bool predIsGround = true;

	private float porogSpeedDown = 70f;

	private float porogSpeedFly = 50f;

	private bool propProv;

	private float timeLastPlayWalk;

	private float timeDelayPalyWalk = 0.4f;

	private bool PlayerMine
	{
		get
		{
			return (settings.offlineMode
			|| (!settings.offlineMode && base.photonView.isMine));
		}
	}

	private void Awake()
	{
		controller = GetComponent<CharacterController>();
		moveDirection = base.transform.TransformDirection(Vector3.forward);
		animationObject = base.transform.Find("prefabPlayer").gameObject;
		timeLastPlayWalk = Time.timeSinceLevelLoad;
		_animation = animationObject.GetComponent<Animation>();
		idleAnimation = _animation.GetClip("Idle");
		walkAnimation = _animation.GetClip("Walk");
		runAnimation = _animation.GetClip("Walk");
		jumpPoseAnimation = _animation.GetClip("Jump");
		if (!_animation)
		{
			Debug.Log("The character you would like to control doesn't have animations. Moving her might look weird.");
		}
		if (!idleAnimation)
		{
			_animation = null;
			Debug.Log("No idle animation found. Turning off animations.");
		}
		if (!walkAnimation)
		{
			_animation = null;
			Debug.Log("No walk animation found. Turning off animations.");
		}
		if (!runAnimation)
		{
			_animation = null;
			Debug.Log("No run animation found. Turning off animations.");
		}
		if (!jumpPoseAnimation && canJump)
		{
			_animation = null;
			Debug.Log("No jump animation found and the character has canJump enabled. Turning off animations.");
		}
	}

	private void Start()
	{
		if (pBehavior == null)
		{
			pBehavior = GetComponent<PlayerBehavior>();
		}

		if (PlayerMine)
		{
			grabControls();
		}

		if (jetpack == null)
		{
			jetpack = base.transform.Find("JetPack").gameObject.GetComponent<JetpackBehavior>();
		}

		if (jetpack != null)
		{
			jetpack.gameObject.SetActive(false);
		}

		predPos = base.gameObject.transform.position;
	}

	private void Update()
	{
		if (PlayerMine && !pBehavior.inHelic)
		{
			if (!predIsGround)
			{
				if (IsGrounded() && !propProv)
				{
					if (downSpeedMove > porogSpeedDown && !pBehavior.isDead)
					{
						int damage = (int)((downSpeedMove - porogSpeedDown) * 0.5f);
						pBehavior.getDamage(damage);
						predIsGround = true;
						downSpeedMove = 0f;
					}
					if (!pBehavior.isDead && !isPlayAnimation && needPlayAnimDrop)
					{
						needPlayAnimDrop = false;
						pBehavior.animDropAfterFlyDown();
					}
				}
				else
				{
					dist = predPos.y - base.gameObject.transform.position.y;
					downSpeedMove = 3.6f * dist / Time.deltaTime;
					if (downSpeedMove > porogSpeedFly)
					{
						pBehavior.animFlyDown();
						needPlayAnimDrop = true;
					}
				}
			}

			propProv = pBehavior.isDead;
			predPos = base.gameObject.transform.position;
			predIsGround = IsGrounded();
		}

		if (!PlayerMine || pBehavior.isDead || pBehavior.damageFromCar)
		{
			return;
		}

		isRunning = Input.GetButton("Sprint");

		if (!isControllable)
		{
			Input.ResetInputAxes();
		}

		if (CompilationSettings.UseKeyboard)
		{
			SetJumping(Input.GetButton("Jump"));
		}

		UpdateSmoothedMovementDirection();
		ApplyGravity();
		ApplyJumping();
		Vector3 vector = (moveDirection * moveSpeed) + new Vector3(0f, verticalSpeed, 0f) + inAirVelocity;
		vector *= Time.deltaTime;

		if (controller.enabled)
		{
			collisionFlags = controller.Move(vector);
		}

		if (PlayerMine)
		{
			updateAnimations();
		}

		if (IsGrounded())
		{
			base.transform.rotation = Quaternion.LookRotation(moveDirection);
		}
		else
		{
			Vector3 forward = vector;
			forward.y = 0f;
			if ((double)forward.sqrMagnitude > 0.001)
			{
				base.transform.rotation = Quaternion.LookRotation(forward);
			}
		}

		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public void ActivateJetpack()
	{
		if (!settings.offlineMode)
		{
			pBehavior.photonView.RPC("ActivateJetpack", PhotonTargets.AllBuffered);
		}
		else
		{
			jetpack.Activate();
		}
	}

	public void SetJumping(bool IsJumping)
	{
		if (IsJumping)
		{
			lastJumpButtonTime = Time.time;

			if (jetpack.activated)
			{
				if (!settings.offlineMode)
				{
					pBehavior.photonView.RPC("EnableJetpackParticle", PhotonTargets.AllBuffered);
				}

				if (jumping)
				{
					jetpack.isFlying = true;
					pBehavior.alreadyDontFly();
				}
			}

			if (!jumping)
			{
				jumping = true;
			}
			else if (CompilationSettings.UseKeyboard && !IsGrounded())
			{
				ActivateJetpack();
			}
		}
		else
		{
			if (!settings.offlineMode)
			{
				pBehavior.photonView.RPC("DisableJetpackParticle", PhotonTargets.AllBuffered);
			}
			jetpack.isFlying = false;
		}
	}

	private void UpdateSmoothedMovementDirection()
	{
		Transform transform = Camera.main.transform;
		bool flag = IsGrounded();
		Vector3 vector = transform.TransformDirection(Vector3.forward);
		vector.y = 0f;
		vector = vector.normalized;
		Vector3 vector2 = new Vector3(vector.z, 0f, 0f - vector.x);
		float num = 0f;
		float num2 = 0f;
		if (joyScript != null)
		{
			if (GameController.thisScript.useLestncu)
			{
				num = 0f;
				num2 = 0f;
				lastGroundedTime = Time.time;
				inAirVelocity = Vector3.zero;
				moveSpeed = 0f;
			}
			else if (CompilationSettings.UseMotionMovment)
			{
				Vector2 vec = KeyboardManager.GetMovmentVector();
				num2 = vec.x;
				num = vec.y;
			}
			else
			{
				num2 = joyScript.position.x;
				num = joyScript.position.y;
			}
		}
		if (num < 0f)
		{
			movingBack = true;
		}
		else
		{
			movingBack = false;
		}
		bool flag2 = isMoving;
		isMoving = (double)Mathf.Abs(num2) > 0.1 || (double)Mathf.Abs(num) > 0.1;
		Vector3 vector3 = num2 * vector2 + num * vector;
		if (flag)
		{
			lockCameraTimer += Time.deltaTime;

			if (isMoving != flag2)
			{
				lockCameraTimer = 0f;
			}

			if (vector3 != Vector3.zero)
			{
				if ((double)moveSpeed < (double)walkSpeed * 0.9 && flag)
				{
					moveDirection = vector3.normalized;
				}
				else
				{
					moveDirection = Vector3.RotateTowards(moveDirection, vector3, rotateSpeed * ((float)Math.PI / 180f) * Time.deltaTime, 1000f);
					moveDirection = moveDirection.normalized;
				}
			}

			float t = speedSmoothing * Time.deltaTime;
			float num3 = Mathf.Min(vector3.magnitude, 1f);
			_characterState = CharacterState.Idle;

			if (!isRunning && stamina < 1f)
			{
				stamina += 0.05f * Time.deltaTime;
			}

			if (stamina > 0f && isRunning && isMoving)
			{
				stamina -= 0.2f * Time.deltaTime;
				num3 *= runSpeed;
				_characterState = CharacterState.Running;
			}
			else if (Time.time - trotAfterSeconds > walkTimeStart)
			{
				num3 *= trotSpeed;
				_characterState = CharacterState.Trotting;
			}
			else
			{
				num3 *= walkSpeed;
				_characterState = CharacterState.Walking;
			}

			moveSpeed = moveSpeed * (float)pBehavior.weaponManager.currentWeaponScript.mobility / 100f;
			moveSpeed = Mathf.Lerp(moveSpeed, num3, t);
			if ((double)moveSpeed < (double)walkSpeed * 0.3)
			{
				walkTimeStart = Time.time;
			}
		}
		else if (isMoving)
		{
			inAirVelocity *= 0.95f;
			if (pBehavior.inHelic)
			{
				Debug.Log("pBehavior.inHelic");
				inAirVelocity += vector3.normalized * Time.deltaTime * inAirControlAcceleration * jetpackAcceleration;
				moveDirection *= 0.99f;
			}
			else
			{
				inAirVelocity += vector3.normalized * Time.deltaTime * inAirControlAcceleration * ((!jetpack.isFlying) ? 1f : jetpackAcceleration);
				moveDirection *= ((!jetpack.isFlying) ? 1f : 0.99f);
			}
		}
	}

	private void ApplyJumping()
	{
		if (lastJumpTime + jumpRepeatTime > Time.time)
		{
			return;
		}
		if (IsGrounded() && jetpack != null && !jetpack.activated)
		{
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (jetpack != null && jetpack.activated && jetpack.isFlying && !IsGrounded())
		{
			if (jetpack.isFlying)
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (jetpack != null && jetpack.activated && IsGrounded() && canJump && Time.time < lastJumpButtonTime + jumpTimeout)
		{
			verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
			SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void ApplyGravity()
	{
		if (isControllable)
		{
			bool button = Input.GetButton("Jump");
			if (jumping && !jumpingReachedApex && (double)verticalSpeed <= 0.0)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}

			if (IsGrounded())
			{
				verticalSpeed = 0f;
			}
			else
			{
				verticalSpeed -= gravity * Time.deltaTime;
			}
		}
	}

	private float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt(2f * targetJumpHeight * gravity);
	}

	private void DidJump()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = base.transform.position.y;
		lastJumpButtonTime = -10f;
		_characterState = CharacterState.Jumping;
	}

	private string getNameAnimation(int curAnim)
	{
		switch (curAnim)
		{
		case 0:
			return idleAnimation.name;
		case 4:
			return jumpPoseAnimation.name;
		case 3:
			return runAnimation.name;
		case 2:
			return walkAnimation.name;
		case 1:
			return walkAnimation.name;
		default:
			return null;
		}
	}

	[RPC]
	private void broadcastAnimation(int anim)
	{
		if (!base.photonView.isMine)
		{
			string nameAnimation = getNameAnimation(anim);
			if (pBehavior == null)
			{
				pBehavior = GetComponent<PlayerBehavior>();
			}
			if (!pBehavior.damageFromCar && nameAnimation != null)
			{
				animationObject.GetComponent<Animation>().CrossFade(nameAnimation);
			}
			broadcastWeaponAnimation(nameAnimation);
		}
	}

	private void broadcastWeaponAnimation(string anim)
	{
		if (!(anim == jumpPoseAnimation.name) && !base.photonView.isMine && checkForShootNotPlaying())
		{
			weaponAnimationObject.GetComponent<Animation>().Play(anim);
		}
	}

	private bool checkForShootNotPlaying()
	{
		return (weaponAnimationObject != null && !weaponAnimationObject.GetComponent<Animation>().IsPlaying("Shoot") && !weaponAnimationObject.GetComponent<Animation>().IsPlaying("Reload") && !weaponAnimationObject.GetComponent<Animation>().IsPlaying("Shoot1"));
	}

	private void updateAnimations()
	{
		if (!_animation || pBehavior.isDead || !isPlayAnimation)
		{
			return;
		}

		if (_characterState == CharacterState.Jumping)
		{
			if (!jumpingReachedApex)
			{
				_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
				_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade(jumpPoseAnimation.name);
				if (!settings.offlineMode && pred_CharacterState != _characterState)
				{
					base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
				}
			}
			else
			{
				_animation[jumpPoseAnimation.name].speed = 0f - landAnimationSpeed;
				_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade(jumpPoseAnimation.name);
				if (!settings.offlineMode && pred_CharacterState != _characterState)
				{
					base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
				}
			}
		}
		else if ((double)controller.velocity.sqrMagnitude < 0.1)
		{
			_animation.CrossFade(idleAnimation.name);
			if (checkForShootNotPlaying())
			{
				weaponAnimationObject.GetComponent<Animation>().CrossFade(idleAnimation.name);
			}
			_characterState = CharacterState.Idle;
			if (!settings.offlineMode && pred_CharacterState != _characterState)
			{
				base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
			}
		}
		else if (_characterState == CharacterState.Running)
		{
			_animation[runAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, runMaxAnimationSpeed);
			_animation.CrossFade(runAnimation.name);
			if (!settings.offlineMode && pred_CharacterState != _characterState)
			{
				base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
			}
		}
		else if (_characterState == CharacterState.Trotting)
		{
			_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, trotMaxAnimationSpeed);
			_animation.CrossFade(walkAnimation.name);
			if (checkForShootNotPlaying())
			{
				weaponAnimationObject.GetComponent<Animation>().CrossFade(walkAnimation.name);
			}
			if (!settings.offlineMode && pred_CharacterState != _characterState)
			{
				base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
			}
		}
		else if (_characterState == CharacterState.Walking)
		{
			_animation[walkAnimation.name].speed = Mathf.Clamp(controller.velocity.magnitude, 0f, walkMaxAnimationSpeed);
			_animation.CrossFade(walkAnimation.name);
			if (checkForShootNotPlaying())
			{
				weaponAnimationObject.GetComponent<Animation>().CrossFade(walkAnimation.name);
			}
			if (!settings.offlineMode && pred_CharacterState != _characterState)
			{
				base.photonView.RPC("broadcastAnimation", PhotonTargets.Others, _characterState);
			}
		}
		pred_CharacterState = _characterState;
	}

	private void PlaySoundWalk()
	{
		if (settings.soundEnabled && arrWalkSound != null && Time.timeSinceLevelLoad - timeLastPlayWalk > timeDelayPalyWalk)
		{
			timeLastPlayWalk = Time.timeSinceLevelLoad;
			AudioClip audioClip = arrWalkSound[UnityEngine.Random.Range(0, arrWalkSound.Length)];
			if (audioClip != null)
			{
				AudioSource.PlayClipAtPoint(audioClip, base.transform.position, UnityEngine.Random.Range(0.3f, 1f) * _animation[walkAnimation.name].speed);
			}
		}
	}

	private void grabControls()
	{
		if (PlayerMine)
		{
			joyScript = GameController.thisScript.joystickWalk;
		}
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (!((double)hit.moveDirection.y > 0.01))
		{
		}
	}

	public float GetSpeed()
	{
		return moveSpeed;
	}

	public bool IsJumping()
	{
		return jumping;
	}

	public bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.Below) != 0;
	}

	public Vector3 GetDirection()
	{
		return moveDirection;
	}

	public bool IsMovingBackwards()
	{
		return movingBack;
	}

	public float GetLockCameraTimer()
	{
		return lockCameraTimer;
	}

	public bool IsMoving()
	{
		return (double)(Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal"))) > 0.1;
	}

	private bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}

	private bool IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	private void Reset()
	{
		base.gameObject.tag = "Player";
	}
}
