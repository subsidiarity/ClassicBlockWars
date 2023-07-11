using NJG;
using UnityEngine;

public class ThirdPersonController2 : MonoBehaviour
{
	private const float inputThreshold = 0.01f;

	private const float groundDrag = 5f;

	private const float directionalJumpFactor = 0.7f;

	private const float groundedDistance = 0.5f;

	public Rigidbody target;

	public float speed = 1f;

	public float walkSpeedDownscale = 2f;

	public float turnSpeed = 2f;

	public float mouseTurnSpeed = 0.3f;

	public float jumpSpeed = 1f;

	public LayerMask groundLayers = -1;

	public float groundedCheckOffset = 0.7f;

	public bool showGizmos = true;

	public bool requireLock = true;

	public bool controlLock;

	public JumpDelegate onJump;

	private bool grounded;

	private bool walking;

	public bool Grounded
	{
		get
		{
			return grounded;
		}
	}

	private float SidestepAxisInput
	{
		get
		{
			float num = 0f;
			if (Input.GetKeyDown(KeyCode.Q))
			{
				num = -1f;
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				num = 1f;
			}
			if (Input.GetMouseButton(1))
			{
				float num2 = num;
				float axis = Input.GetAxis("Horizontal");
				return (!(Mathf.Abs(num2) > Mathf.Abs(axis))) ? axis : num2;
			}
			return num;
		}
	}

	private void Reset()
	{
		Setup();
	}

	private void Setup()
	{
		if (target == null)
		{
			target = GetComponent<Rigidbody>();
		}
	}

	private void Start()
	{
		Setup();
		if (target == null)
		{
			Debug.LogError("No target assigned. Please correct and restart.");
			base.enabled = false;
		}
		else
		{
			target.freezeRotation = true;
			walking = false;
		}
	}

	private void Update()
	{
		float angle;
		if (Input.GetMouseButton(0) && (!requireLock || controlLock || Screen.lockCursor) && !NJGMapBase.instance.isMouseOver)
		{
			if (controlLock)
			{
			}
			angle = Input.GetAxis("Mouse X") * mouseTurnSpeed * Time.deltaTime;
		}
		else
		{
			if (controlLock)
			{
			}
			angle = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;
		}
		target.transform.Rotate(target.transform.up, angle);
		walking = !Input.GetKey(KeyCode.LeftShift);
	}

	private void FixedUpdate()
	{
		grounded = Physics.Raycast(target.transform.position + target.transform.up * (0f - groundedCheckOffset), target.transform.up * -1f, 0.5f, groundLayers);
		if (grounded)
		{
			target.drag = 5f;
			if (Input.GetButton("Jump"))
			{
				target.AddForce(jumpSpeed * target.transform.up + target.velocity.normalized * 0.7f, ForceMode.VelocityChange);
				if (onJump != null)
				{
					onJump();
				}
				return;
			}
			Vector3 vector = Input.GetAxis("Vertical") * target.transform.forward + SidestepAxisInput * target.transform.right;
			float num = ((!walking) ? speed : (speed / walkSpeedDownscale));
			if (Input.GetAxis("Vertical") < 0f)
			{
				num /= walkSpeedDownscale;
			}
			if (vector.magnitude > 0.01f)
			{
				target.AddForce(vector.normalized * num, ForceMode.VelocityChange);
			}
			else
			{
				target.velocity = new Vector3(0f, target.velocity.y, 0f);
			}
		}
		else
		{
			target.drag = 0f;
		}
	}

	private void OnDrawGizmos()
	{
		if (showGizmos && !(target == null))
		{
			Gizmos.color = ((!grounded) ? Color.red : Color.blue);
			Gizmos.DrawLine(target.transform.position + target.transform.up * (0f - groundedCheckOffset), target.transform.position + target.transform.up * (0f - (groundedCheckOffset + 0.5f)));
		}
	}
}
