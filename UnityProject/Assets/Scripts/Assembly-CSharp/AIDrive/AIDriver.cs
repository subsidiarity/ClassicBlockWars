using System;
using System.Collections.Generic;
using UnityEngine;

public class AIDriver : MonoBehaviour
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

	public float calcMaxSpeed = 200f;

	public float torque = 150f;

	public float brakeTorque = 500f;

	public float steerAngle = 20f;

	public float hsSteerAngle = 5f;

	public float steeringSpeed = 100f;

	private float maxSpeed;

	private float currentSpeed;

	private bool isBraking;

	private bool inverseWheelTurning;

	private int wheelTurningParameter = 1;

	public int gears = 5;

	private List<int> gearSpeed = new List<int>();

	private int currentGear;

	public bool playSound = true;

	public AudioClip motorSound;

	public float soundVolume = 1f;

	private AudioSource motorAudioSource;

	public DriveMode driveMode;

	private string waypointPreName = "MyWaypoint";

	private string waypointFolder = "MyWaypoints";

	public List<Transform> waypoints = new List<Transform>();

	public float currentAngle;

	private float targetAngle;

	private float wheelRadius;

	private AIRespawn aiRespawnScript;

	public int currentWaypoint;

	public float aiSteerAngle;

	private float aiSpeedPedal = 1f;

	public float centerOfMassY;

	public Transform flWheel;

	public Transform frWheel;

	public Transform rlWheel;

	public Transform rrWheel;

	public WheelCollider flWheelCollider;

	public WheelCollider frWheelCollider;

	public WheelCollider rlWheelCollider;

	public WheelCollider rrWheelCollider;

	public Transform viewPoint;

	public bool useObstacleAvoidance = true;

	public float oADistance = 10f;

	public float oAWidth = 2f;

	public float oASideDistance = 1f;

	public float oASideOffset = 1.5f;

	public SteeringMode steeringMode;

	public float roadMaxWidth = 20f;

	public LayerMask visibleLayers = -1;

	public GameObject viewPointLeftGO;

	public GameObject viewPointRightGO;

	public GameObject viewPointEndGO;

	public GameObject viewPointLeftEndGO;

	public GameObject viewPointRightEndGO;

	private float sqrDistanceToWaypoint = 4f;

	private Vector3 leftDirection;

	private Vector3 rightDirection;

	private Vector3 centerPointL;

	private Vector3 centerPointR;

	private float obstacleAvoidanceWidth;

	private bool backwardDriving;

	public GameObject leftDirectionGO;

	public GameObject rightDirectionGO;

	public GameObject leftSideGO;

	public GameObject rightSideGO;

	public GameObject centerPointLGO;

	public GameObject centerPointRGO;

	public GameObject centerPointEndLGO;

	public GameObject centerPointEndRGO;

	public Transform frontCollider;

	public static LastWaypointHandler onLastWaypoint;

	private void Awake()
	{
		maxSpeed = calcMaxSpeed;
		wheelRadius = flWheelCollider.radius;
		GetWaypointNames();
		FillWaypointList();
		InitGearSpeeds();
		if (inverseWheelTurning)
		{
			wheelTurningParameter = -1;
		}
		else
		{
			wheelTurningParameter = 1;
		}
		base.GetComponent<Rigidbody>().centerOfMass = new Vector3(0f, centerOfMassY, 0f);
		if (playSound && motorSound != null)
		{
			InitSound();
		}
		aiRespawnScript = base.gameObject.GetComponent("AIRespawn") as AIRespawn;
	}

	private void Start()
	{
		if (useObstacleAvoidance)
		{
			sqrDistanceToWaypoint += roadMaxWidth * roadMaxWidth;
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
			obstacleAvoidanceWidth = viewPointRightGO.transform.localPosition.x + oAWidth;
			leftDirection = viewPoint.position + viewPoint.TransformDirection(Vector3.left * obstacleAvoidanceWidth + Vector3.forward * oADistance);
			rightDirection = viewPoint.position + viewPoint.TransformDirection(Vector3.right * obstacleAvoidanceWidth + Vector3.forward * oADistance);
			centerPointL = base.transform.position + base.transform.TransformDirection(Vector3.left * oASideOffset);
			centerPointL.y = viewPoint.position.y;
			centerPointR = base.transform.position + base.transform.TransformDirection(Vector3.right * oASideOffset);
			centerPointR.y = viewPoint.position.y;
			leftDirectionGO = new GameObject("leftDirectionGO");
			leftDirectionGO.transform.parent = base.transform;
			leftDirectionGO.transform.position = leftDirection;
			leftDirectionGO.transform.rotation = base.transform.rotation;
			rightDirectionGO = new GameObject("rightDirectionGO");
			rightDirectionGO.transform.parent = base.transform;
			rightDirectionGO.transform.position = rightDirection;
			rightDirectionGO.transform.rotation = base.transform.rotation;
			centerPointLGO = new GameObject("centerPointLGO");
			centerPointLGO.transform.parent = base.transform;
			centerPointLGO.transform.position = centerPointL;
			centerPointLGO.transform.rotation = base.transform.rotation;
			centerPointRGO = new GameObject("centerPointRGO");
			centerPointRGO.transform.parent = base.transform;
			centerPointRGO.transform.position = centerPointR;
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
			centerPointEndLGO = new GameObject("centerPointEndLGO");
			centerPointEndLGO.transform.parent = base.transform;
			centerPointEndLGO.transform.position = centerPointL + base.transform.TransformDirection(Vector3.left * oASideDistance);
			centerPointEndLGO.transform.rotation = base.transform.rotation;
			centerPointEndRGO = new GameObject("centerPointEndRGO");
			centerPointEndRGO.transform.parent = base.transform;
			centerPointEndRGO.transform.position = centerPointR + base.transform.TransformDirection(Vector3.right * oASideDistance);
			centerPointEndRGO.transform.rotation = base.transform.rotation;
			frontCollider = base.transform.Find("ViewPointCollider");
			Vector3 localPosition = viewPoint.transform.localPosition;
			localPosition.y += 0.1f;
			frontCollider.transform.localPosition = localPosition;
			frontCollider.transform.rotation = base.transform.rotation;
			frontCollider.transform.localScale = new Vector3(frWheel.localPosition.x * 2f + 0.1f, 0.05f, 0.05f);
		}
	}

	private void InitSound()
	{
		motorAudioSource = base.gameObject.AddComponent<AudioSource>() as AudioSource;
		motorAudioSource.clip = motorSound;
		motorAudioSource.loop = true;
		motorAudioSource.volume = soundVolume;
		motorAudioSource.playOnAwake = false;
		motorAudioSource.pitch = 0.1f;
		motorAudioSource.Play();
	}

	private void FixedUpdate()
	{
		currentSpeed = (float)Math.PI * 2f * wheelRadius * flWheelCollider.rpm * 60f / 1000f;
		currentSpeed = Mathf.Round(currentSpeed);
		if (currentSpeed > maxSpeed + 10f)
		{
			isBraking = true;
		}
		else
		{
			isBraking = false;
			flWheelCollider.brakeTorque = 0f;
			frWheelCollider.brakeTorque = 0f;
		}
		if (!isBraking)
		{
			if (currentSpeed < maxSpeed)
			{
				flWheelCollider.motorTorque = torque * aiSpeedPedal;
				frWheelCollider.motorTorque = torque * aiSpeedPedal;
			}
			else
			{
				flWheelCollider.motorTorque = 0f;
				frWheelCollider.motorTorque = 0f;
			}
		}
		else
		{
			flWheelCollider.brakeTorque = brakeTorque;
			frWheelCollider.brakeTorque = brakeTorque;
			flWheelCollider.motorTorque = 0f;
			frWheelCollider.motorTorque = 0f;
		}
		AiSteering();
		flWheelCollider.steerAngle = aiSteerAngle;
		frWheelCollider.steerAngle = aiSteerAngle;
		if (playSound && motorSound != null)
		{
			SetCurrentGear();
			GearSound();
		}
	}

	private void Update()
	{
		RotateWheels();
		SteelWheels();
	}

	private void RotateWheels()
	{
		flWheel.Rotate(flWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		frWheel.Rotate(frWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		rlWheel.Rotate(rlWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		rrWheel.Rotate(rrWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
	}

	private void SteelWheels()
	{
		flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
		frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
	}

	private void SetCurrentGear()
	{
		int count = gearSpeed.Count;
		currentGear = count - 1;
		for (int i = 0; i < count; i++)
		{
			if ((float)gearSpeed[i] >= currentSpeed)
			{
				currentGear = i;
				break;
			}
		}
	}

	private void GearSound()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (currentGear == 0)
		{
			num = 0f;
			num2 = gearSpeed[currentGear];
		}
		else
		{
			num = gearSpeed[currentGear - 1];
			num2 = gearSpeed[currentGear];
		}
		float num4 = num2 - num;
		num3 = (float)((double)((currentSpeed - num) / num4 / 2f) + 0.8);
		if (num3 > 2f)
		{
			num3 = 2f;
		}
		motorAudioSource.pitch = num3;
	}

	private void AiSteering()
	{
		if (currentWaypoint < waypoints.Count)
		{
			Vector3 position = waypoints[currentWaypoint].position;
			Vector3 vector = position - base.transform.position;
			Vector3 vector2 = base.transform.InverseTransformPoint(waypoints[currentWaypoint].position);
			if (!useObstacleAvoidance)
			{
				targetAngle = Mathf.Atan2(vector2.x, vector2.z) * 57.29578f;
			}
			else
			{
				currentAngle = ObstacleAvoidanceSteering();
			}
			if (currentAngle < targetAngle)
			{
				currentAngle += Time.deltaTime * steeringSpeed;
				if (currentAngle > targetAngle)
				{
					currentAngle = targetAngle;
				}
			}
			else if (currentAngle > targetAngle)
			{
				currentAngle -= Time.deltaTime * steeringSpeed;
				if (currentAngle < targetAngle)
				{
					currentAngle = targetAngle;
				}
			}
			float num = calcMaxSpeed;
			float value = currentSpeed / num;
			value = Mathf.Clamp(value, 0f, 1f);
			float num2 = steerAngle - (steerAngle - hsSteerAngle) * value;
			aiSteerAngle = Mathf.Clamp(currentAngle, -1f * num2, num2);
			if (vector.sqrMagnitude < sqrDistanceToWaypoint)
			{
				AIWaypoint aIWaypoint = waypoints[currentWaypoint].GetComponent("AIWaypoint") as AIWaypoint;
				if (aIWaypoint != null)
				{
					maxSpeed = aIWaypoint.speed;
				}
				else
				{
					maxSpeed = calcMaxSpeed;
				}
				currentWaypoint++;
				aiRespawnScript.lastTimeToReachNextWP = 0f;
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
		}
		else if (driveMode == DriveMode.Laps)
		{
			currentWaypoint = 0;
		}
		else
		{
			aiSpeedPedal = 0f;
			aiRespawnScript.enabled = false;
		}
	}

	private void FillWaypointList()
	{
		bool flag = true;
		int num = 1;
		while (flag)
		{
			string text = "/" + waypointFolder + "/" + waypointPreName + num;
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
				string text2 = "/" + waypointFolder + "/" + waypointPreName + (num - 2);
				GameObject gameObject2 = GameObject.Find(text2);
				gameObject2.transform.LookAt(gameObject.transform);
			}
			if (num > 2 && !flag)
			{
				string text3 = "/" + waypointFolder + "/" + waypointPreName + (num - 1);
				GameObject gameObject3 = GameObject.Find(text3);
				string text4 = "/" + waypointFolder + "/" + waypointPreName + "1";
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
			waypointPreName = aIWaypointEditor.preName + "_";
			waypointFolder = aIWaypointEditor.folderName;
		}
	}

	private void InitGearSpeeds()
	{
		if (gears < 1)
		{
			gears = 1;
		}
		int num = (int)Mathf.Round(calcMaxSpeed / (float)gears);
		gearSpeed.Clear();
		for (int i = 0; i < gears; i++)
		{
			gearSpeed.Add(num * (i + 1));
		}
	}

	private float ObstacleAvoidanceSteering()
	{
		bool frontContact = false;
		float num = 0f;
		float num2 = -1f;
		float leftDistance = 0f;
		float rightDistance = 0f;
		float leftSideDistance = 0f;
		float rightSideDistance = 0f;
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
		if (Physics.Linecast(centerPointLGO.transform.position, centerPointEndLGO.transform.position, out hitInfo6, visibleLayers))
		{
			leftSideDistance = hitInfo6.distance;
		}
		RaycastHit hitInfo7;
		if (Physics.Linecast(centerPointRGO.transform.position, centerPointEndRGO.transform.position, out hitInfo7, visibleLayers))
		{
			rightSideDistance = hitInfo7.distance;
		}
		currentAngle = SteeringDecision(leftSideDistance, rightSideDistance, leftDistance, rightDistance, num, frontContact, steeringMode);
		if (backwardDriving)
		{
			if (currentSpeed > 2f)
			{
				flWheelCollider.motorTorque = 0f;
				frWheelCollider.motorTorque = 0f;
				flWheelCollider.brakeTorque = brakeTorque;
				frWheelCollider.brakeTorque = brakeTorque;
			}
			else
			{
				flWheelCollider.motorTorque = -100f;
				frWheelCollider.motorTorque = -100f;
				flWheelCollider.brakeTorque = 0f;
				frWheelCollider.brakeTorque = 0f;
				currentAngle = -1f * currentAngle;
			}
			if (num > 8f || num == 0f)
			{
				backwardDriving = false;
			}
		}
		return currentAngle;
	}

	private float SteeringDecision(float leftSideDistance, float rightSideDistance, float leftDistance, float rightDistance, float frontMinDistance, bool frontContact, SteeringMode style)
	{
		float num = steerAngle;
		float result = 0f;
		switch (style)
		{
		case SteeringMode.Cautious:
			if (leftSideDistance == 0f && ((leftDistance == 0f && rightDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && leftDistance > rightDistance) || (leftDistance == 0f && frontMinDistance > 0f) || (rightDistance > leftDistance && frontMinDistance > 0f) || (!frontContact && rightSideDistance > 0f)))
			{
				result = -1f * num;
			}
			if (rightSideDistance == 0f && ((rightDistance == 0f && leftDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && rightDistance > leftDistance) || (rightDistance == 0f && frontMinDistance > 0f) || (leftDistance > rightDistance && frontMinDistance > 0f) || (!frontContact && leftSideDistance > 0f)))
			{
				result = num;
			}
			break;
		case SteeringMode.Tough:
			if (leftSideDistance == 0f && ((leftDistance == 0f && rightDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && leftDistance > rightDistance) || (leftDistance == 0f && frontMinDistance > 0f) || (rightDistance > leftDistance && frontMinDistance > 0f)))
			{
				result = -1f * num;
			}
			if (rightSideDistance == 0f && ((rightDistance == 0f && leftDistance > 0f) || (rightDistance != 0f && leftDistance != 0f && rightDistance > leftDistance) || (rightDistance == 0f && frontMinDistance > 0f) || (leftDistance > rightDistance && frontMinDistance > 0f)))
			{
				result = num;
			}
			break;
		}
		return result;
	}
}
