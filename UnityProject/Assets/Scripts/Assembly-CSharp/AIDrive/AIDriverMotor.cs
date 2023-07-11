using System;
using System.Collections.Generic;
using UnityEngine;

public class AIDriverMotor : MonoBehaviour
{
	public delegate void LastWaypointHandler(AIEventArgs e);

	public float maxSpeed = 200f;

	public float torque = 150f;

	public float brakeTorque = 500f;

	public float maxSteerAngle = 20f;

	private float m_currentMaxSpeed;

	private float m_currentSpeed;

	private bool m_isBraking;

	private bool m_inverseWheelTurning;

	private int m_wheelTurningParameter = 1;

	public int gears = 5;

	private List<int> m_gearSpeed = new List<int>();

	private int m_currentGear;

	public bool playSound = true;

	public AudioClip motorSound;

	public float soundVolume = 1f;

	private AudioSource m_motorAudioSource;

	private float m_targetAngle;

	private float m_wheelRadius;

	[HideInInspector]
	public int currentWaypoint;

	[HideInInspector]
	public float aiSteerAngle;

	[HideInInspector]
	public float aiSpeedPedal = 1f;

	[HideInInspector]
	public float aiBrakePedal;

	public Transform centerOfMass;

	public Transform flWheel;

	public Transform frWheel;

	public Transform rlWheel;

	public Transform rrWheel;

	public WheelCollider flWheelCollider;

	public WheelCollider frWheelCollider;

	public WheelCollider rlWheelCollider;

	public WheelCollider rrWheelCollider;

	public static LastWaypointHandler onLastWaypoint;

	private void Awake()
	{
		m_currentMaxSpeed = maxSpeed;
		m_wheelRadius = flWheelCollider.radius;
		InitGearSpeeds();
		if (m_inverseWheelTurning)
		{
			m_wheelTurningParameter = -1;
		}
		else
		{
			m_wheelTurningParameter = 1;
		}
		if (playSound && motorSound != null)
		{
			InitSound();
		}
		if (centerOfMass != null)
		{
			base.rigidbody.centerOfMass = centerOfMass.localPosition;
		}
	}

	private void Start()
	{
	}

	private void InitSound()
	{
		m_motorAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
		m_motorAudioSource.clip = motorSound;
		m_motorAudioSource.loop = true;
		m_motorAudioSource.volume = soundVolume;
		m_motorAudioSource.playOnAwake = false;
		m_motorAudioSource.pitch = 0.1f;
		m_motorAudioSource.Play();
	}

	private void FixedUpdate()
	{
		m_currentSpeed = (float)Math.PI * 2f * m_wheelRadius * flWheelCollider.rpm * 60f / 1000f;
		m_currentSpeed = Mathf.Round(m_currentSpeed);
		flWheelCollider.motorTorque = torque * aiSpeedPedal;
		frWheelCollider.motorTorque = torque * aiSpeedPedal;
		flWheelCollider.brakeTorque = brakeTorque * aiBrakePedal;
		frWheelCollider.brakeTorque = brakeTorque * aiBrakePedal;
		flWheelCollider.steerAngle = maxSteerAngle * aiSteerAngle;
		frWheelCollider.steerAngle = maxSteerAngle * aiSteerAngle;
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
		flWheel.Rotate(flWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)m_wheelTurningParameter, 0f, 0f);
		frWheel.Rotate(frWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)m_wheelTurningParameter, 0f, 0f);
		rlWheel.Rotate(rlWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)m_wheelTurningParameter, 0f, 0f);
		rrWheel.Rotate(rrWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)m_wheelTurningParameter, 0f, 0f);
	}

	private void SteelWheels()
	{
		flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
		frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
	}

	private void SetCurrentGear()
	{
		int count = m_gearSpeed.Count;
		m_currentGear = count - 1;
		for (int i = 0; i < count; i++)
		{
			if ((float)m_gearSpeed[i] >= m_currentSpeed)
			{
				m_currentGear = i;
				break;
			}
		}
	}

	private void GearSound()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		if (m_currentGear == 0)
		{
			num = 0f;
			num2 = m_gearSpeed[m_currentGear];
		}
		else
		{
			num = m_gearSpeed[m_currentGear - 1];
			num2 = m_gearSpeed[m_currentGear];
		}
		float num4 = num2 - num;
		num3 = (float)((double)((m_currentSpeed - num) / num4 / 2f) + 0.8);
		if (num3 > 2f)
		{
			num3 = 2f;
		}
		m_motorAudioSource.pitch = num3;
	}

	private void InitGearSpeeds()
	{
		if (gears < 1)
		{
			gears = 1;
		}
		int num = (int)Mathf.Round(maxSpeed / (float)gears);
		m_gearSpeed.Clear();
		for (int i = 0; i < gears; i++)
		{
			m_gearSpeed.Add(num * (i + 1));
		}
	}
}
