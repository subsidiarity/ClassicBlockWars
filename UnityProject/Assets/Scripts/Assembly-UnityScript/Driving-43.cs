using System;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class Driving_002D43 : MonoBehaviour
{
	public WheelCollider flWheelCollider;

	public WheelCollider frWheelCollider;

	public WheelCollider rlWheelCollider;

	public WheelCollider rrWheelCollider;

	public float maxTorque;

	public float maxBrakeTorque;

	public float maxSteerAngle;

	public float maxSpeed;

	public float maxBackwardSpeed;

	public float currentSpeed;

	private bool isBraking;

	public Transform flWheel;

	public Transform frWheel;

	public Transform rlWheel;

	public Transform rrWheel;

	public int[] gearSpeed;

	private int currentGear;

	public float FullBrakeTorque;

	public AudioClip brakeSound;

	public bool groundEffectsOn;

	public Transform skidmark;

	private Vector3 lastSkidmarkPosR;

	private Vector3 lastSkidmarkPosL;

	private float oldForwardFriction;

	private float oldSidewaysFriction;

	private float brakingForwardFriction;

	private float brakingSidewaysFriction;

	private float stopForwardFriction;

	private float stopSidewaysFriction;

	private AudioSource brakeAudioSource;

	private bool isPlayingSound;

	private Transform DustL;

	private Transform DustR;

	private Transform DustFL;

	private Transform DustFR;

	public string tagNameTest;

	public float OffsetX;

	public float OffsetY;

	public Driving_002D43()
	{
		maxTorque = 150f;
		maxBrakeTorque = 500f;
		maxSteerAngle = 30f;
		maxSpeed = 200f;
		maxBackwardSpeed = 40f;
		FullBrakeTorque = 5000f;
		groundEffectsOn = true;
		brakingForwardFriction = 0.03f;
		brakingSidewaysFriction = 0.03f;
		stopForwardFriction = 1f;
		stopSidewaysFriction = 1f;
	}

	public virtual void Awake()
	{
		int num = 0;
		Vector3 centerOfMass = GetComponent<Rigidbody>().centerOfMass;
		float num2 = (centerOfMass.y = num);
		Vector3 vector2 = (GetComponent<Rigidbody>().centerOfMass = centerOfMass);
		int num3 = 0;
		Vector3 centerOfMass2 = GetComponent<Rigidbody>().centerOfMass;
		float num4 = (centerOfMass2.z = num3);
		Vector3 vector4 = (GetComponent<Rigidbody>().centerOfMass = centerOfMass2);
		oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
		oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;
		brakeAudioSource = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		brakeAudioSource.clip = brakeSound;
		brakeAudioSource.loop = true;
		brakeAudioSource.volume = 0.7f;
		brakeAudioSource.playOnAwake = false;
		brakeAudioSource.rolloffMode = AudioRolloffMode.Linear;
		DustL = transform.Find("DustL");
		DustR = transform.Find("DustR");
		DustFL = transform.Find("DustFL");
		DustFR = transform.Find("DustFR");
	}

	public virtual void FixedUpdate()
	{
		currentSpeed = (float)Math.PI * 2f * flWheelCollider.radius * flWheelCollider.rpm * 60f / 1000f;
		currentSpeed = Mathf.Round(currentSpeed);
		FullBraking();
		if ((!(currentSpeed <= 0f) && Input.GetAxis("Vertical") < 0f) || (!(currentSpeed >= 0f) && !(Input.GetAxis("Vertical") <= 0f)))
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
			if (!(currentSpeed >= maxSpeed) && !(currentSpeed <= maxBackwardSpeed * -1f))
			{
				flWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");
				frWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");
			}
			else
			{
				flWheelCollider.motorTorque = 0f;
				frWheelCollider.motorTorque = 0f;
			}
		}
		else
		{
			flWheelCollider.brakeTorque = maxBrakeTorque;
			frWheelCollider.brakeTorque = maxBrakeTorque;
			flWheelCollider.motorTorque = 0f;
			frWheelCollider.motorTorque = 0f;
		}
		flWheelCollider.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
		frWheelCollider.steerAngle = maxSteerAngle * Input.GetAxis("Horizontal");
		SetCurrentGear();
		GearSound();
	}

	public virtual void FullBraking()
	{
		if (Input.GetKey("space"))
		{
			rlWheelCollider.brakeTorque = FullBrakeTorque;
			rrWheelCollider.brakeTorque = FullBrakeTorque;
			if (Mathf.Abs(GetComponent<Rigidbody>().velocity.z) > 1f || !(Mathf.Abs(GetComponent<Rigidbody>().velocity.x) <= 1f))
			{
				SetFriction(brakingForwardFriction, brakingSidewaysFriction);
				SetBrakeEffects(true);
			}
			else
			{
				SetFriction(stopForwardFriction, stopSidewaysFriction);
				SetBrakeEffects(false);
			}
		}
		else
		{
			rlWheelCollider.brakeTorque = 0f;
			rrWheelCollider.brakeTorque = 0f;
			SetFriction(oldForwardFriction, oldSidewaysFriction);
			SetBrakeEffects(false);
		}
	}

	public virtual void SetFriction(float MyForwardFriction, float MySidewaysFriction)
	{
		WheelFrictionCurve forwardFriction = frWheelCollider.forwardFriction;
		float num2 = (forwardFriction.stiffness = MyForwardFriction);
		WheelFrictionCurve wheelFrictionCurve2 = (frWheelCollider.forwardFriction = forwardFriction);
		WheelFrictionCurve forwardFriction2 = flWheelCollider.forwardFriction;
		float num4 = (forwardFriction2.stiffness = MyForwardFriction);
		WheelFrictionCurve wheelFrictionCurve4 = (flWheelCollider.forwardFriction = forwardFriction2);
		WheelFrictionCurve forwardFriction3 = rrWheelCollider.forwardFriction;
		float num6 = (forwardFriction3.stiffness = MyForwardFriction);
		WheelFrictionCurve wheelFrictionCurve6 = (rrWheelCollider.forwardFriction = forwardFriction3);
		WheelFrictionCurve forwardFriction4 = rlWheelCollider.forwardFriction;
		float num8 = (forwardFriction4.stiffness = MyForwardFriction);
		WheelFrictionCurve wheelFrictionCurve8 = (rlWheelCollider.forwardFriction = forwardFriction4);
		WheelFrictionCurve sidewaysFriction = frWheelCollider.sidewaysFriction;
		float num10 = (sidewaysFriction.stiffness = MySidewaysFriction);
		WheelFrictionCurve wheelFrictionCurve10 = (frWheelCollider.sidewaysFriction = sidewaysFriction);
		WheelFrictionCurve sidewaysFriction2 = flWheelCollider.sidewaysFriction;
		float num12 = (sidewaysFriction2.stiffness = MySidewaysFriction);
		WheelFrictionCurve wheelFrictionCurve12 = (flWheelCollider.sidewaysFriction = sidewaysFriction2);
		WheelFrictionCurve sidewaysFriction3 = rrWheelCollider.sidewaysFriction;
		float num14 = (sidewaysFriction3.stiffness = MySidewaysFriction);
		WheelFrictionCurve wheelFrictionCurve14 = (rrWheelCollider.sidewaysFriction = sidewaysFriction3);
		WheelFrictionCurve sidewaysFriction4 = rlWheelCollider.sidewaysFriction;
		float num16 = (sidewaysFriction4.stiffness = MySidewaysFriction);
		WheelFrictionCurve wheelFrictionCurve16 = (rlWheelCollider.sidewaysFriction = sidewaysFriction4);
	}

	public virtual void SetBrakeEffects(bool PlayEffects)
	{
		bool flag = false;
		Vector3 vector = default(Vector3);
		Quaternion quaternion = default(Quaternion);
		Vector3 vector2 = default(Vector3);
		if (PlayEffects)
		{
			WheelHit hit = default(WheelHit);
			if (rlWheelCollider.GetGroundHit(out hit))
			{
				DustL.GetComponent<ParticleEmitter>().emit = true;
				flag = true;
				vector = hit.point;
				vector.y += 0.01f;
				vector.x += 0.2f;
				if (lastSkidmarkPosL != Vector3.zero)
				{
					vector2 = lastSkidmarkPosL - vector;
					quaternion = Quaternion.LookRotation(vector2);
					UnityEngine.Object.Instantiate(skidmark, vector, quaternion);
				}
				lastSkidmarkPosL = vector;
			}
			else
			{
				DustL.GetComponent<ParticleEmitter>().emit = false;
				lastSkidmarkPosL = Vector3.zero;
			}
			if (rrWheelCollider.GetGroundHit(out hit))
			{
				DustR.GetComponent<ParticleEmitter>().emit = true;
				flag = true;
				vector = hit.point;
				vector.y += 0.01f;
				vector.x -= 0.2f;
				if (lastSkidmarkPosR != Vector3.zero)
				{
					vector2 = lastSkidmarkPosR - vector;
					quaternion = Quaternion.LookRotation(vector2);
					UnityEngine.Object.Instantiate(skidmark, vector, quaternion);
				}
				lastSkidmarkPosR = vector;
			}
			else
			{
				DustR.GetComponent<ParticleEmitter>().emit = false;
				lastSkidmarkPosR = Vector3.zero;
			}
			if (!isPlayingSound && flag)
			{
				isPlayingSound = true;
				brakeAudioSource.Play();
			}
			if (!flag)
			{
				isPlayingSound = false;
				brakeAudioSource.Stop();
			}
		}
		else
		{
			isPlayingSound = false;
			brakeAudioSource.Stop();
			DustL.GetComponent<ParticleEmitter>().emit = false;
			DustR.GetComponent<ParticleEmitter>().emit = false;
			lastSkidmarkPosL = Vector3.zero;
			lastSkidmarkPosR = Vector3.zero;
		}
	}

	public virtual void Update()
	{
		RotateWheels();
		SteelWheels();
	}

	public virtual void RotateWheels()
	{
		flWheel.Rotate(flWheelCollider.rpm / 60f * 360f * Time.deltaTime, 0f, 0f);
		frWheel.Rotate(frWheelCollider.rpm / 60f * 360f * Time.deltaTime, 0f, 0f);
		rlWheel.Rotate(rlWheelCollider.rpm / 60f * 360f * Time.deltaTime, 0f, 0f);
		rrWheel.Rotate(rrWheelCollider.rpm / 60f * 360f * Time.deltaTime, 0f, 0f);
	}

	public virtual void SteelWheels()
	{
		float y = flWheelCollider.steerAngle - flWheel.localEulerAngles.z;
		Vector3 localEulerAngles = flWheel.localEulerAngles;
		float num = (localEulerAngles.y = y);
		Vector3 vector2 = (flWheel.localEulerAngles = localEulerAngles);
		float y2 = frWheelCollider.steerAngle - frWheel.localEulerAngles.z;
		Vector3 localEulerAngles2 = frWheel.localEulerAngles;
		float num2 = (localEulerAngles2.y = y2);
		Vector3 vector4 = (frWheel.localEulerAngles = localEulerAngles2);
	}

	public virtual void SetCurrentGear()
	{
		int num = default(int);
		num = gearSpeed.Length; // TMP
		// num = Extensions.get_length((System.Array)gearSpeed);
		for (int i = 0; i < num; i++)
		{
			if (!((float)gearSpeed[i] <= currentSpeed))
			{
				currentGear = i;
				break;
			}
		}
	}

	public virtual void GearSound()
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
		num3 = (Mathf.Abs(currentSpeed) - num) / (num2 - num) + 0.8f;
		GetComponent<AudioSource>().pitch = num3;
	}

	public virtual void Main()
	{
	}
}
