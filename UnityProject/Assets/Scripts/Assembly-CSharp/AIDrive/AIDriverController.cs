using System.Collections.Generic;
using UnityEngine;

public class AIDriverController : MonoBehaviour
{
	public enum DriveMode
	{
		OneWay = 0,
		Laps = 1
	}

	public enum SteeringMode
	{
		Cautious = 0,
		Tough = 1
	}

	public delegate void LastWaypointHandler(AIEventArgs e);

	private float m_calcMaxSpeed = 200f;

	[HideInInspector]
	public float steerAngle = 20f;

	public float hsSteerAngle = 5f;

	public float steeringSpeed = 100f;

	private float m_maxSpeed;

	private float m_currentSpeed;

	private bool m_isBraking;

	private float m_leftRightDistanceLength;

	private float m_frontDistanceLength;

	private float m_leftRightSideDistanceLength;

	[HideInInspector]
	public AIMotorMapping aiPreMotor;

	public DriveMode driveMode = DriveMode.Laps;

	public bool steerAbsolute;

	private string m_waypointPreName = "MyWaypoint";

	private string m_waypointFolder = "MyWaypoints";

	[HideInInspector]
	public List<Transform> waypoints = new List<Transform>();

	[HideInInspector]
	public float currentAngle;

	private float m_targetAngle;

	private AIRespawnController aiRespawnControllerScript;

	[HideInInspector]
	public int currentWaypoint;

	[HideInInspector]
	public float aiSteerAngle;

	[HideInInspector]
	public float aiSpeedPedal = 1f;

	[HideInInspector]
	public Transform flWheel;

	[HideInInspector]
	public Transform frWheel;

	[HideInInspector]
	public Transform viewPoint;

	public bool useObstacleAvoidance = true;

	public float oADistance = 10f;

	public float oAWidth = 4f;

	public float oASideDistance = 3f;

	public float oASideOffset = 1f;

	public float oASideFromMid = 1.5f;

	public SteeringMode steeringMode;

	public float roadMaxWidth = 20f;

	public LayerMask visibleLayers = -1;

	[HideInInspector]
	public GameObject viewPointLeftGO;

	[HideInInspector]
	public GameObject viewPointRightGO;

	[HideInInspector]
	public GameObject viewPointEndGO;

	[HideInInspector]
	public GameObject viewPointLeftEndGO;

	[HideInInspector]
	public GameObject viewPointRightEndGO;

	private float m_sqrDistanceToWaypoint = 4f;

	private float m_sqrDistanceToWpOa;

	private float m_sqrDistanceToWpNoOa = 4f;

	private Vector3 m_leftDirection;

	private Vector3 m_rightDirection;

	private Vector3 m_centerPointL;

	private Vector3 m_centerPointR;

	private float m_obstacleAvoidanceWidth;

	private bool m_backwardDriving;

	private bool m_isBackwardDriving;

	private float m_currentMaxSteerAngle;

	private float m_lastSqrDistanceNextWp;

	private float m_lastSqrDistanceAfterNextWp;

	[HideInInspector]
	public GameObject leftDirectionGO;

	[HideInInspector]
	public GameObject rightDirectionGO;

	[HideInInspector]
	public GameObject centerPointLGO;

	[HideInInspector]
	public GameObject centerPointRGO;

	[HideInInspector]
	public GameObject centerPointEndLGO;

	[HideInInspector]
	public GameObject centerPointEndRGO;

	[HideInInspector]
	public Transform frontCollider;

	[HideInInspector]
	public GameObject leftFrontSideGO;

	[HideInInspector]
	public GameObject rightFrontSideGO;

	[HideInInspector]
	public GameObject leftRearSideGO;

	[HideInInspector]
	public GameObject rightRearSideGO;

	[HideInInspector]
	public GameObject leftFrontSideEndGO;

	[HideInInspector]
	public GameObject rightFrontSideEndGO;

	[HideInInspector]
	public GameObject leftRearSideEndGO;

	[HideInInspector]
	public GameObject rightRearSideEndGO;

	private Vector3 m_leftFrontSidePos;

	private Vector3 m_rightFrontSidePos;

	private Vector3 m_leftRearSidePos;

	private Vector3 m_rightRearSidePos;

	public static LastWaypointHandler onLastWaypoint;

	private void Awake()
	{
		aiRespawnControllerScript = base.gameObject.GetComponent<AIRespawnController>();
		aiPreMotor = GetComponent<AIMotorMapping>();
		flWheel = aiPreMotor.flWheelMesh;
		frWheel = aiPreMotor.frWheelMesh;
	}

	private void Start()
	{
		GetWaypointNames();
		FillWaypointList();
		steerAngle = aiPreMotor.steerMax;
		m_calcMaxSpeed = aiPreMotor.speedMax;
		m_maxSpeed = m_calcMaxSpeed;
		m_sqrDistanceToWpOa = roadMaxWidth * roadMaxWidth;
		if (steerAngle < hsSteerAngle)
		{
			Debug.LogError("hsSteerAngle is bigger then aiPreMotor.steerMax. It has to be lower or equal.");
		}
		if (useObstacleAvoidance)
		{
			m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa;
			viewPointLeftGO = new GameObject("viewPointLeftGO");
			viewPointLeftGO.transform.parent = base.transform;
			viewPointLeftGO.transform.position = viewPoint.transform.position;
			viewPointLeftGO.transform.position += viewPoint.TransformDirection(Vector3.right * flWheel.localPosition.x);
			viewPointLeftGO.transform.rotation = base.transform.rotation;
			viewPointRightGO = new GameObject("viewPointRightGO");
			viewPointRightGO.transform.parent = base.transform;
			viewPointRightGO.transform.position = viewPoint.transform.position;
			viewPointRightGO.transform.position += viewPoint.TransformDirection(Vector3.right * frWheel.localPosition.x);
			viewPointRightGO.transform.rotation = base.transform.rotation;
			m_centerPointL = base.transform.position + base.transform.TransformDirection(Vector3.left * oASideOffset);
			m_centerPointL.y = viewPoint.position.y;
			m_centerPointR = base.transform.position + base.transform.TransformDirection(Vector3.right * oASideOffset);
			m_centerPointR.y = viewPoint.position.y;
			centerPointRGO = new GameObject("centerPointRGO");
			centerPointRGO.transform.parent = base.transform;
			centerPointRGO.transform.position = m_centerPointR;
			centerPointRGO.transform.rotation = base.transform.rotation;
			viewPointEndGO = new GameObject("viewPointEndGO");
			viewPointEndGO.transform.parent = base.transform;
			viewPointEndGO.transform.position = viewPoint.position + viewPoint.TransformDirection(Vector3.forward * oADistance);
			viewPointEndGO.transform.rotation = base.transform.rotation;
			viewPointLeftEndGO = new GameObject("viewPointLeftEndGO");
			viewPointLeftEndGO.transform.parent = base.transform;
			viewPointLeftEndGO.transform.position = viewPointLeftGO.transform.position + viewPointLeftGO.transform.TransformDirection(Vector3.forward * oADistance);
			viewPointLeftEndGO.transform.rotation = base.transform.rotation;
			viewPointRightEndGO = new GameObject("viewPointRightEndGO");
			viewPointRightEndGO.transform.parent = base.transform;
			viewPointRightEndGO.transform.position = viewPointRightGO.transform.position + viewPointRightGO.transform.TransformDirection(Vector3.forward * oADistance);
			viewPointRightEndGO.transform.rotation = base.transform.rotation;
			centerPointEndRGO = new GameObject("centerPointEndRGO");
			centerPointEndRGO.transform.parent = base.transform;
			centerPointEndRGO.transform.position = m_centerPointR + base.transform.TransformDirection(Vector3.right * oASideDistance);
			centerPointEndRGO.transform.rotation = base.transform.rotation;
			m_leftFrontSidePos = base.transform.position + base.transform.TransformDirection(Vector3.left * oASideOffset);
			m_leftFrontSidePos.y = viewPoint.position.y;
			m_leftFrontSidePos += base.transform.TransformDirection(Vector3.forward * oASideFromMid);
			leftFrontSideGO = new GameObject("leftFrontSideGO");
			leftFrontSideGO.transform.parent = base.transform;
			leftFrontSideGO.transform.position = m_leftFrontSidePos;
			leftFrontSideGO.transform.rotation = base.transform.rotation;
			leftFrontSideEndGO = new GameObject("leftFrontSideEndGO");
			leftFrontSideEndGO.transform.parent = base.transform;
			leftFrontSideEndGO.transform.position = m_leftFrontSidePos + base.transform.TransformDirection(Vector3.left * oASideDistance);
			leftFrontSideEndGO.transform.rotation = base.transform.rotation;
			m_leftRearSidePos = base.transform.position + base.transform.TransformDirection(Vector3.left * oASideOffset);
			m_leftRearSidePos.y = viewPoint.position.y;
			m_leftRearSidePos -= base.transform.TransformDirection(Vector3.forward * oASideFromMid);
			leftRearSideGO = new GameObject("leftRearSideGO");
			leftRearSideGO.transform.parent = base.transform;
			leftRearSideGO.transform.position = m_leftRearSidePos;
			leftRearSideGO.transform.rotation = base.transform.rotation;
			leftRearSideEndGO = new GameObject("leftRearSideEndGO");
			leftRearSideEndGO.transform.parent = base.transform;
			leftRearSideEndGO.transform.position = m_leftRearSidePos + base.transform.TransformDirection(Vector3.left * oASideDistance);
			leftRearSideEndGO.transform.rotation = base.transform.rotation;
			m_rightFrontSidePos = base.transform.position + base.transform.TransformDirection(Vector3.right * oASideOffset);
			m_rightFrontSidePos.y = viewPoint.position.y;
			m_rightFrontSidePos += base.transform.TransformDirection(Vector3.forward * oASideFromMid);
			rightFrontSideGO = new GameObject("rightFrontSideGO");
			rightFrontSideGO.transform.parent = base.transform;
			rightFrontSideGO.transform.position = m_rightFrontSidePos;
			rightFrontSideGO.transform.rotation = base.transform.rotation;
			rightFrontSideEndGO = new GameObject("rightFrontSideEndGO");
			rightFrontSideEndGO.transform.parent = base.transform;
			rightFrontSideEndGO.transform.position = m_rightFrontSidePos + base.transform.TransformDirection(Vector3.right * oASideDistance);
			rightFrontSideEndGO.transform.rotation = base.transform.rotation;
			m_rightRearSidePos = base.transform.position + base.transform.TransformDirection(Vector3.right * oASideOffset);
			m_rightRearSidePos.y = viewPoint.position.y;
			m_rightRearSidePos -= base.transform.TransformDirection(Vector3.forward * oASideFromMid);
			rightRearSideGO = new GameObject("rightRearSideGO");
			rightRearSideGO.transform.parent = base.transform;
			rightRearSideGO.transform.position = m_rightRearSidePos;
			rightRearSideGO.transform.rotation = base.transform.rotation;
			rightRearSideEndGO = new GameObject("rightRearSideEndGO");
			rightRearSideEndGO.transform.parent = base.transform;
			rightRearSideEndGO.transform.position = m_rightRearSidePos + base.transform.TransformDirection(Vector3.right * oASideDistance);
			rightRearSideEndGO.transform.rotation = base.transform.rotation;
			leftDirectionGO = new GameObject("leftDirectionGO");
			leftDirectionGO.transform.parent = base.transform;
			leftDirectionGO.transform.position = viewPointLeftEndGO.transform.position + viewPointLeftEndGO.transform.TransformDirection(Vector3.left * oAWidth);
			leftDirectionGO.transform.rotation = base.transform.rotation;
			rightDirectionGO = new GameObject("rightDirectionGO");
			rightDirectionGO.transform.parent = base.transform;
			rightDirectionGO.transform.position = viewPointRightEndGO.transform.position + viewPointRightEndGO.transform.TransformDirection(Vector3.right * oAWidth);
			rightDirectionGO.transform.rotation = base.transform.rotation;
			m_leftRightDistanceLength = Vector3.Distance(viewPointRightGO.transform.position, rightDirectionGO.transform.position);
			m_leftRightSideDistanceLength = Vector3.Distance(centerPointRGO.transform.position, centerPointEndRGO.transform.position);
			m_frontDistanceLength = Vector3.Distance(viewPoint.transform.position, viewPointEndGO.transform.position);
			frontCollider = base.transform.FindChild("ViewPointCollider");
			Vector3 localPosition = viewPoint.transform.localPosition;
			localPosition.y += 0.1f;
			frontCollider.transform.localPosition = localPosition;
			frontCollider.transform.rotation = base.transform.rotation;
			frontCollider.transform.localScale = new Vector3(frWheel.localPosition.x * 2f + 0.1f, 0.05f, 0.05f);
		}
		else
		{
			m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa;
		}
	}

	private void FixedUpdate()
	{
		m_currentSpeed = Mathf.Round(base.rigidbody.velocity.magnitude * 3.6f);
		if (m_currentSpeed > m_maxSpeed + 10f)
		{
			m_isBraking = true;
		}
		else
		{
			m_isBraking = false;
			aiPreMotor.brakeInput = 0f;
		}
		if (!m_isBraking)
		{
			if (m_currentSpeed < m_maxSpeed)
			{
				aiPreMotor.motorInput = aiSpeedPedal;
			}
			else
			{
				aiPreMotor.motorInput = 0f;
			}
		}
		else
		{
			aiPreMotor.motorInput = 0f;
			aiPreMotor.brakeInput = 1f;
		}
		AI();
	}

	private void AI()
	{
		if (currentWaypoint < waypoints.Count)
		{
			Vector3 position = waypoints[currentWaypoint].position;
			Vector3 vector = position - base.transform.position;
			Vector3 vector2 = base.transform.InverseTransformPoint(waypoints[currentWaypoint].position);
			float value = m_currentSpeed / m_calcMaxSpeed;
			value = Mathf.Clamp(value, 0f, 1f);
			m_currentMaxSteerAngle = steerAngle - (steerAngle - hsSteerAngle) * value;
			if (!useObstacleAvoidance)
			{
				m_targetAngle = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
			}
			else
			{
				m_targetAngle = ObstacleAvoidanceSteering();
			}
			if (currentAngle < m_targetAngle)
			{
				currentAngle += Time.deltaTime * steeringSpeed;
				if (currentAngle > m_targetAngle)
				{
					currentAngle = m_targetAngle;
				}
			}
			else if (currentAngle > m_targetAngle)
			{
				currentAngle -= Time.deltaTime * steeringSpeed;
				if (currentAngle < m_targetAngle)
				{
					currentAngle = m_targetAngle;
				}
			}
			aiSteerAngle = Mathf.Clamp(currentAngle, -1f * m_currentMaxSteerAngle, m_currentMaxSteerAngle);
			aiPreMotor.steerInput = aiSteerAngle / m_currentMaxSteerAngle;
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < m_sqrDistanceToWaypoint)
			{
				NextWaypoint();
			}
		}
		else if (driveMode == DriveMode.Laps)
		{
			currentWaypoint = 0;
		}
		else
		{
			aiSpeedPedal = 0f;
			aiRespawnControllerScript.enabled = false;
		}
	}

	public void NextWaypoint()
	{
		AIWaypoint aIWaypoint = waypoints[currentWaypoint].GetComponent("AIWaypoint") as AIWaypoint;
		if (aIWaypoint != null)
		{
			m_maxSpeed = aIWaypoint.speed;
		}
		else
		{
			m_maxSpeed = m_calcMaxSpeed;
		}
		currentWaypoint++;
		aiRespawnControllerScript.lastTimeToReachNextWP = 0f;
		if (currentWaypoint >= waypoints.Count && onLastWaypoint != null)
		{
			AIEventArgs aIEventArgs = new AIEventArgs();
			aIEventArgs.name = base.gameObject.name;
			aIEventArgs.currentWaypointIndex = currentWaypoint;
			aIEventArgs.currentWaypointName = waypoints[currentWaypoint - 1].name;
			aIEventArgs.position = base.gameObject.transform.position;
			aIEventArgs.rotation = base.gameObject.transform.rotation;
			aIEventArgs.tag = base.gameObject.tag;
			onLastWaypoint(aIEventArgs);
		}
	}

	private void FillWaypointList()
	{
		bool flag = true;
		int num = 1;
		while (flag)
		{
			string text = "/" + m_waypointFolder + "/" + m_waypointPreName + num;
			GameObject gameObject = GameObject.Find(text);
			if (gameObject != null)
			{
				waypoints.Add(gameObject.transform);
				num++;
			}
			else
			{
				flag = false;
			}
			if (num > 2 && flag)
			{
				string text2 = "/" + m_waypointFolder + "/" + m_waypointPreName + (num - 2);
				GameObject gameObject2 = GameObject.Find(text2);
				gameObject2.transform.LookAt(gameObject.transform);
			}
			if (num > 2 && !flag)
			{
				string text3 = "/" + m_waypointFolder + "/" + m_waypointPreName + (num - 1);
				GameObject gameObject3 = GameObject.Find(text3);
				string text4 = "/" + m_waypointFolder + "/" + m_waypointPreName + "1";
				GameObject gameObject4 = GameObject.Find(text4);
				gameObject3.transform.LookAt(gameObject4.transform);
			}
		}
	}

	private void GetWaypointNames()
	{
		AIWaypointEditor aIWaypointEditor = GetComponent("AIWaypointEditor") as AIWaypointEditor;
		if (aIWaypointEditor != null)
		{
			m_waypointPreName = aIWaypointEditor.preName + "_";
			m_waypointFolder = aIWaypointEditor.folderName;
		}
	}

	private float ObstacleAvoidanceSteering()
	{
		bool frontContact = false;
		float num = 0f;
		float num2 = -1f;
		float leftDistance = 0f;
		float rightDistance = 0f;
		float num3 = 0f;
		float num4 = 0f;
		RaycastHit hitInfo;
		if (Physics.Linecast(viewPoint.position, viewPointEndGO.transform.position, out hitInfo, visibleLayers))
		{
			frontContact = true;
			num = hitInfo.distance;
			num2 = hitInfo.distance;
		}
		RaycastHit hitInfo2;
		if (Physics.Linecast(viewPointLeftGO.transform.position, viewPointLeftEndGO.transform.position, out hitInfo2, visibleLayers))
		{
			frontContact = true;
			if (num == 0f || num > hitInfo2.distance)
			{
				num = hitInfo2.distance;
			}
			if (num2 != -1f && num2 < hitInfo2.distance)
			{
				num2 = hitInfo2.distance;
			}
		}
		else
		{
			num2 = -1f;
		}
		RaycastHit hitInfo3;
		if (Physics.Linecast(viewPointRightGO.transform.position, viewPointRightEndGO.transform.position, out hitInfo3, visibleLayers))
		{
			frontContact = true;
			if (num == 0f || num > hitInfo3.distance)
			{
				num = hitInfo3.distance;
			}
			if (num2 != -1f && num2 < hitInfo3.distance)
			{
				num2 = hitInfo3.distance;
			}
		}
		else
		{
			num2 = -1f;
		}
		RaycastHit hitInfo4;
		if (Physics.Linecast(viewPointLeftGO.transform.position, leftDirectionGO.transform.position, out hitInfo4, visibleLayers))
		{
			leftDistance = hitInfo4.distance;
		}
		RaycastHit hitInfo5;
		if (Physics.Linecast(viewPointRightGO.transform.position, rightDirectionGO.transform.position, out hitInfo5, visibleLayers))
		{
			rightDistance = hitInfo5.distance;
		}
		RaycastHit hitInfo6;
		if (Physics.Linecast(leftFrontSideGO.transform.position, leftFrontSideEndGO.transform.position, out hitInfo6, visibleLayers))
		{
			num3 = hitInfo6.distance;
			if (num3 == 0f)
			{
				num3 = 0.01f;
			}
		}
		if (Physics.Linecast(leftRearSideGO.transform.position, leftRearSideEndGO.transform.position, out hitInfo6, visibleLayers))
		{
			if (num3 == 0f || num3 > hitInfo6.distance)
			{
				num3 = hitInfo6.distance;
			}
			if (num3 == 0f)
			{
				num3 = 0.01f;
			}
		}
		RaycastHit hitInfo7;
		if (Physics.Linecast(rightFrontSideGO.transform.position, rightFrontSideEndGO.transform.position, out hitInfo7, visibleLayers))
		{
			num4 = hitInfo7.distance;
			if (num4 == 0f)
			{
				num4 = 0.01f;
			}
		}
		if (Physics.Linecast(rightRearSideGO.transform.position, rightRearSideEndGO.transform.position, out hitInfo7, visibleLayers))
		{
			if (num4 == 0f || num4 > hitInfo7.distance)
			{
				num4 = hitInfo7.distance;
			}
			if (num4 == 0f)
			{
				num4 = 0.01f;
			}
		}
		float num5 = SteeringDecision(num3, num4, leftDistance, rightDistance, num, frontContact, steeringMode);
		if (m_backwardDriving)
		{
			if (m_currentSpeed > 2f && !m_isBackwardDriving)
			{
				aiPreMotor.motorInput = 0f;
				aiPreMotor.brakeInput = 1f;
			}
			else
			{
				aiPreMotor.motorInput = -0.5f;
				aiPreMotor.brakeInput = 0f;
				num5 = -1f * num5;
				m_isBackwardDriving = true;
			}
			if (num > 8f || num == 0f)
			{
				m_backwardDriving = false;
			}
		}
		else
		{
			m_isBackwardDriving = false;
		}
		return num5;
	}

	private float SteeringDecision(float leftSideDistance, float rightSideDistance, float leftDistance, float rightDistance, float frontMinDistance, bool frontContact, SteeringMode style)
	{
		float currentMaxSteerAngle = m_currentMaxSteerAngle;
		float result = 0f;
		float num = 1f;
		float num2 = 1f;
		float num3 = 1f;
		float num4 = 1f;
		if (frontContact && frontMinDistance < 2f)
		{
			m_backwardDriving = true;
		}
		switch (style)
		{
		case SteeringMode.Cautious:
			if (leftSideDistance == 0f && ((leftDistance == 0f && rightDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && leftDistance > rightDistance) || (leftDistance == 0f && frontMinDistance > 0f) || (rightDistance < leftDistance && frontMinDistance > 0f && rightDistance != 0f) || (!frontContact && rightSideDistance > 0f)))
			{
				if (!steerAbsolute)
				{
					if (frontMinDistance > 0f)
					{
						result = -1f * currentMaxSteerAngle;
					}
					else
					{
						if (rightSideDistance > 0f)
						{
							num2 = rightSideDistance / m_leftRightSideDistanceLength;
						}
						if (rightDistance > 0f)
						{
							num = rightDistance / m_leftRightDistanceLength;
						}
						result = ((!(num2 < num)) ? (-1f * currentMaxSteerAngle * (1f - num)) : (-1f * currentMaxSteerAngle * (1f - num2)));
					}
				}
				else
				{
					result = -1f * currentMaxSteerAngle;
				}
			}
			if (rightSideDistance == 0f && ((rightDistance == 0f && leftDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && rightDistance > leftDistance) || (rightDistance == 0f && frontMinDistance > 0f) || (leftDistance < rightDistance && frontMinDistance > 0f && leftDistance != 0f) || (!frontContact && leftSideDistance > 0f)))
			{
				if (!steerAbsolute)
				{
					if (frontMinDistance > 0f)
					{
						result = currentMaxSteerAngle;
					}
					else
					{
						if (leftSideDistance > 0f)
						{
							num4 = leftSideDistance / m_leftRightSideDistanceLength;
						}
						if (leftDistance > 0f)
						{
							num3 = leftDistance / m_leftRightDistanceLength;
						}
						result = ((!(num4 < num3)) ? (currentMaxSteerAngle * (1f - num3)) : (currentMaxSteerAngle * (1f - num4)));
					}
				}
				else
				{
					result = currentMaxSteerAngle;
				}
			}
			if (rightSideDistance != 0f && leftSideDistance != 0f)
			{
				if (rightDistance > 0f)
				{
					num = rightDistance / m_leftRightDistanceLength;
				}
				if (leftDistance > 0f)
				{
					num3 = leftDistance / m_leftRightDistanceLength;
				}
				if (num < num3 || num3 == 0f)
				{
					result = -1f * currentMaxSteerAngle * (1f - num);
				}
				else if (num > num3 || num == 0f)
				{
					result = currentMaxSteerAngle * (1f - num3);
				}
			}
			break;
		case SteeringMode.Tough:
			if (leftSideDistance == 0f && ((leftDistance == 0f && rightDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && leftDistance > rightDistance) || (leftDistance == 0f && frontMinDistance > 0f) || (rightDistance > leftDistance && frontMinDistance > 0f)))
			{
				if (!steerAbsolute)
				{
					if (frontMinDistance > 0f)
					{
						result = -1f * currentMaxSteerAngle;
					}
					else
					{
						if (rightDistance > 0f)
						{
							num = rightDistance / m_leftRightDistanceLength;
						}
						result = -1f * currentMaxSteerAngle * (1f - num);
					}
				}
				else
				{
					result = -1f * currentMaxSteerAngle;
				}
			}
			if (rightSideDistance != 0f || ((rightDistance != 0f || !(leftDistance > 0f)) && (rightDistance == 0f || leftDistance == 0f || !(rightDistance > leftDistance)) && (rightDistance != 0f || !(frontMinDistance > 0f)) && (!(leftDistance > rightDistance) || !(frontMinDistance > 0f))))
			{
				break;
			}
			if (!steerAbsolute)
			{
				if (frontMinDistance > 0f)
				{
					result = currentMaxSteerAngle;
					break;
				}
				if (leftDistance > 0f)
				{
					num3 = leftDistance / m_leftRightDistanceLength;
				}
				result = currentMaxSteerAngle * (1f - num3);
			}
			else
			{
				result = currentMaxSteerAngle;
			}
			break;
		}
		return result;
	}

	public void SwitchOaMode(bool useOa)
	{
		useObstacleAvoidance = useOa;
		if (useOa)
		{
			m_sqrDistanceToWaypoint = m_sqrDistanceToWpOa;
		}
		else
		{
			m_sqrDistanceToWaypoint = m_sqrDistanceToWpNoOa;
		}
	}
}
