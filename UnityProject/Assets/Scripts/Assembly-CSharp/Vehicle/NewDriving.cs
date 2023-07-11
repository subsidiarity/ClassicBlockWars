using System;
using System.Collections.Generic;
using UnityEngine;

public class NewDriving : MonoBehaviour
{
	public bool isGo;

	public bool isBack;

	public AudioClip collisionSound;

	public WheelCollider flWheelCollider;

	public WheelCollider frWheelCollider;

	public WheelCollider rlWheelCollider;

	public WheelCollider rrWheelCollider;

	public float maxTorque = 150f;

	public float maxBrakeTorque = 500f;

	public float maxSteerAngle = 30f;

	public float maxSpeedSteerAngle = 10f;

	public float maxSpeed;

	public float maxBackwardSpeed = 40f;

	public float currentSpeed;

	public int currentSpeedReal;

	private Vector3 predPosCar = Vector3.zero;

	private bool isBraking;

	public Transform flWheel;

	public Transform frWheel;

	public Transform rlWheel;

	public Transform rrWheel;

	public Transform[] rightAnyWheel;

	public Transform[] leftAnyWheel;

	public int turboForce = 500000;

	public int explosionForce = 500000;

	public bool inverseWheelTurning;

	private int wheelTurningParameter = 1;

	public List<int> gearSpeed;

	private int currentGear;

	public float FullBrakeTorque = 5000f;

	public AudioClip brakeSound;

	public bool groundEffectsOn = true;

	public AudioClip fonMenuClip;

	public AudioSource fonMenuAudioSource;

	public AudioClip backgroundClip;

	public AudioSource backgroundAudioSource;

	public AudioClip motorSound;

	public AudioSource motorAudioSource;

	public AudioClip motorSoundLow;

	public AudioSource motorAudioSourceLow;

	public AudioClip motorSoundMid;

	public AudioSource motorAudioSourceMid;

	public AudioClip turboSound;

	public AudioSource turboAudioSource;

	private WheelFrictionCurve frForwardFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve flForwardFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve rrForwardFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve rlForwardFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve frSidewaysFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve flSidewaysFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve rrSidewaysFriction = default(WheelFrictionCurve);

	private WheelFrictionCurve rlSidewaysFriction = default(WheelFrictionCurve);

	private float oldForwardFriction;

	private float oldSidewaysFriction;

	private float brakingForwardFriction = 0.03f;

	private float brakingSidewaysFriction = 0.03f;

	private float brakingSidewaysFrictionBackward = 0.01f;

	private float stopForwardFriction = 1f;

	private float stopSidewaysFriction = 1f;

	public AudioSource brakeAudioSource;

	private bool isPlayingSound;

	public Transform DustL;

	public Transform DustR;

	public float centerOfMassY;

	public Transform skidmark;

	private Vector3 lastSkidmarkPosR;

	private Vector3 lastSkidmarkPosL;

	public bool isFullStop;

	private CarBehavior carScript;

	private void Awake()
	{
		carScript = GetComponent<CarBehavior>();
		if (!settings.offlineMode && !carScript.photonView.isMine)
		{
			base.enabled = false;
		}
		if (inverseWheelTurning)
		{
			wheelTurningParameter = -1;
		}
		else
		{
			wheelTurningParameter = 1;
		}
		base.rigidbody.centerOfMass = new Vector3(0f, centerOfMassY, 0f);
		oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
		oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;
	}

	private void Start()
	{
		frForwardFriction = frWheelCollider.forwardFriction;
		flForwardFriction = flWheelCollider.forwardFriction;
		rrForwardFriction = rrWheelCollider.forwardFriction;
		rlForwardFriction = rlWheelCollider.forwardFriction;
		frSidewaysFriction = frWheelCollider.sidewaysFriction;
		flSidewaysFriction = flWheelCollider.sidewaysFriction;
		rrSidewaysFriction = rrWheelCollider.sidewaysFriction;
		rlSidewaysFriction = rlWheelCollider.sidewaysFriction;
		predPosCar = base.gameObject.transform.position;
		InitSound();
	}

	private void InitSound()
	{
		if (fonMenuClip != null)
		{
			fonMenuAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
			fonMenuAudioSource.clip = fonMenuClip;
			fonMenuAudioSource.loop = true;
			fonMenuAudioSource.volume = 1f;
			fonMenuAudioSource.playOnAwake = false;
			fonMenuAudioSource.pitch = 1f;
		}
		if (backgroundClip != null)
		{
			backgroundAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
			backgroundAudioSource.clip = backgroundClip;
			backgroundAudioSource.loop = true;
			backgroundAudioSource.volume = 0.5f;
			backgroundAudioSource.playOnAwake = false;
			backgroundAudioSource.pitch = 1f;
			if (settings.soundEnabled)
			{
				backgroundAudioSource.Play();
			}
		}
		if (brakeSound != null)
		{
			brakeAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
			brakeAudioSource.clip = brakeSound;
			brakeAudioSource.loop = true;
			brakeAudioSource.volume = 0.175f;
			brakeAudioSource.playOnAwake = false;
		}
		if (turboSound != null)
		{
			turboAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
			turboAudioSource.clip = turboSound;
			turboAudioSource.loop = false;
			turboAudioSource.volume = 0.25f;
			turboAudioSource.playOnAwake = false;
		}
		if (motorSound != null)
		{
			motorAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
			motorAudioSource.clip = motorSound;
			motorAudioSource.loop = true;
			motorAudioSource.volume = 0.25f;
			motorAudioSource.playOnAwake = false;
			motorAudioSource.pitch = 0.1f;
		}
		if (motorSoundLow != null)
		{
			motorAudioSourceLow = base.gameObject.AddComponent("AudioSource") as AudioSource;
			motorAudioSourceLow.clip = motorSoundLow;
			motorAudioSourceLow.loop = true;
			motorAudioSourceLow.volume = 0.1f;
			motorAudioSourceLow.playOnAwake = false;
			motorAudioSourceLow.pitch = 0.1f;
		}
		if (motorSoundMid != null)
		{
			motorAudioSourceMid = base.gameObject.AddComponent("AudioSource") as AudioSource;
			motorAudioSourceMid.clip = motorSoundMid;
			motorAudioSourceMid.loop = true;
			motorAudioSourceMid.volume = 0.15f;
			motorAudioSourceMid.playOnAwake = false;
			motorAudioSourceMid.pitch = 0.1f;
		}
		if (settings.soundEnabled)
		{
			if (motorAudioSource != null)
			{
				motorAudioSource.Play();
			}
			if (motorAudioSourceLow != null)
			{
				motorAudioSourceLow.Play();
			}
			if (motorAudioSourceMid != null)
			{
				motorAudioSourceMid.Play();
			}
		}
		else
		{
			if (motorAudioSource != null)
			{
				motorAudioSource.Stop();
			}
			if (motorAudioSourceLow != null)
			{
				motorAudioSourceLow.Stop();
			}
			if (motorAudioSourceMid != null)
			{
				motorAudioSourceMid.Stop();
			}
		}
	}

	private void FixedUpdate()
	{
		currentSpeed = (float)Math.PI * 2f * flWheelCollider.radius * flWheelCollider.rpm * 60f / 1000f;
		currentSpeed = Mathf.Round(currentSpeed);
		float num = Vector3.Distance(predPosCar, base.gameObject.transform.position);
		currentSpeedReal = (int)(CompilationSettings.CarTurningSensitivity * num / Time.fixedDeltaTime);
		predPosCar = base.gameObject.transform.position;
		Vector2 input = KeyboardManager.GetMovmentVector();
		if ((currentSpeed > 0f && input.y < 0f) || (currentSpeed < 0f && input.y > 0f))
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
			if (currentSpeed < maxSpeed && currentSpeed > maxBackwardSpeed * -1f)
			{
				float num2 = 0f;

				// /* Checking for keyboard input. */
				// if (CompilationSettings.CarUseKeyboard)
				// {
				// 	if (Input.GetKey(KeyCode.S))
				// 	{
				// 		num2 = -1.0f;
				// 	}
				// 	else if (Input.GetKey(KeyCode.W))
				// 	{
				// 		num2 = 1.0f;
				// 	}
				// }

				num2 = input.y;

				if (isBack)
				{
					num2 = -1f;
				}
				else if (isGo)
				{
					num2 = 1f;
				}

				flWheelCollider.motorTorque = maxTorque * num2;
				frWheelCollider.motorTorque = maxTorque * num2;
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
		float value = currentSpeed / maxSpeed * CompilationSettings.CarTurningSpeedMultiplier;
		value = Mathf.Clamp(value, 0f, CompilationSettings.CarTurningMaximumSpeed);
		float num3 = maxSteerAngle - (maxSteerAngle - maxSpeedSteerAngle) * value;
		if ((settings.offlineMode && carScript.playerInCar) || (!settings.offlineMode && carScript.idPlayerInCar == GameController.thisScript.playerScript.photonView.viewID))
		{
			// TODO: Change this name.
			if (CompilationSettings.CarUseKeyboard)
			{
				flWheelCollider.steerAngle = num3 * input.x;
				frWheelCollider.steerAngle = num3 * input.x;
			}
			else
			{
				flWheelCollider.steerAngle = num3 * Input.acceleration.x;
				frWheelCollider.steerAngle = num3 * Input.acceleration.x;
			}
		}

		FullBraking();
		SetCurrentGear();
		GearSound();

		CursorManager.UpdateMouseLock();

		if (!CompilationSettings.CarUseMouse)
		{
			return;
		}

		if (Input.GetMouseButton(0))
		{
			if (!carScript.isShooting)
			{
				carScript.startShoot();
			}
		}
		else if (carScript.isShooting)
		{
			carScript.cancelShoot();
		}
	}

	public void FullStopCar()
	{
		InvokeRepeating("StopCar", 0.02f, 0.02f);
		Invoke("CancelStopCar", 1f);
	}

	private void StopCar()
	{
		flWheelCollider.brakeTorque = maxBrakeTorque;
		frWheelCollider.brakeTorque = maxBrakeTorque;
		flWheelCollider.motorTorque = 0f;
		frWheelCollider.motorTorque = 0f;
	}

	private void CancelStopCar()
	{
		CancelInvoke("StopCar");
		CancelInvoke("CancelStopCar");
	}

	private void FullBraking()
	{
		// TODO: This has to be a common button.
		if (Input.GetKey("space"))
		{
			rlWheelCollider.brakeTorque = FullBrakeTorque;
			rrWheelCollider.brakeTorque = FullBrakeTorque;
			if (Mathf.Abs(base.rigidbody.velocity.z) > 1f || Mathf.Abs(base.rigidbody.velocity.x) > 1f)
			{
				SetFriction(brakingForwardFriction, brakingSidewaysFriction, brakingSidewaysFrictionBackward);
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

	private void SetFriction(float MyForwardFriction, float MySidewaysFriction)
	{
		frForwardFriction.stiffness = MyForwardFriction;
		frWheelCollider.forwardFriction = frForwardFriction;
		flForwardFriction.stiffness = MyForwardFriction;
		flWheelCollider.forwardFriction = flForwardFriction;
		rrForwardFriction.stiffness = MyForwardFriction;
		rrWheelCollider.forwardFriction = rrForwardFriction;
		rlForwardFriction.stiffness = MyForwardFriction;
		rlWheelCollider.forwardFriction = rlForwardFriction;
		frSidewaysFriction.stiffness = MySidewaysFriction;
		frWheelCollider.sidewaysFriction = frSidewaysFriction;
		flSidewaysFriction.stiffness = MySidewaysFriction;
		flWheelCollider.sidewaysFriction = flSidewaysFriction;
		rrSidewaysFriction.stiffness = MySidewaysFriction;
		rrWheelCollider.sidewaysFriction = rrSidewaysFriction;
		rlSidewaysFriction.stiffness = MySidewaysFriction;
		rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
	}

	private void SetFriction(float MyForwardFriction, float MySidewaysFriction, float MySidewaysFrictionBackward)
	{
		frForwardFriction.stiffness = MyForwardFriction;
		frWheelCollider.forwardFriction = frForwardFriction;
		flForwardFriction.stiffness = MyForwardFriction;
		flWheelCollider.forwardFriction = flForwardFriction;
		rrForwardFriction.stiffness = MyForwardFriction;
		rrWheelCollider.forwardFriction = rrForwardFriction;
		rlForwardFriction.stiffness = MyForwardFriction;
		rlWheelCollider.forwardFriction = rlForwardFriction;
		frSidewaysFriction.stiffness = MySidewaysFriction;
		frWheelCollider.sidewaysFriction = frSidewaysFriction;
		flSidewaysFriction.stiffness = MySidewaysFriction;
		flWheelCollider.sidewaysFriction = flSidewaysFriction;
		rrSidewaysFriction.stiffness = MySidewaysFrictionBackward;
		rrWheelCollider.sidewaysFriction = rrSidewaysFriction;
		rlSidewaysFriction.stiffness = MySidewaysFrictionBackward;
		rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
	}

	private void SetBrakeEffects(bool PlayEffects)
	{
		bool flag = false;
		if (PlayEffects)
		{
			WheelHit hit;
			if (rlWheelCollider.GetGroundHit(out hit))
			{
				DustL.particleEmitter.emit = true;
				flag = true;
				Vector3 point = hit.point;
				point.y += 0.1f;
				if (lastSkidmarkPosL != Vector3.zero)
				{
					Vector3 forward = lastSkidmarkPosL - point;
					Quaternion rotation = Quaternion.LookRotation(forward);
					UnityEngine.Object.Instantiate(skidmark, point, rotation);
				}
				lastSkidmarkPosL = point;
			}
			else
			{
				DustL.particleEmitter.emit = false;
				lastSkidmarkPosL = Vector3.zero;
			}
			if (rrWheelCollider.GetGroundHit(out hit))
			{
				DustR.particleEmitter.emit = true;
				flag = true;
				Vector3 point = hit.point;
				point.y += 0.1f;
				if (lastSkidmarkPosR != Vector3.zero)
				{
					Vector3 forward = lastSkidmarkPosR - point;
					Quaternion rotation = Quaternion.LookRotation(forward);
					UnityEngine.Object.Instantiate(skidmark, point, rotation);
				}
				lastSkidmarkPosR = point;
			}
			else
			{
				DustR.particleEmitter.emit = false;
				lastSkidmarkPosR = Vector3.zero;
			}
			if (!isPlayingSound && flag)
			{
				isPlayingSound = true;
				if (brakeAudioSource != null && settings.soundEnabled && Time.timeScale > 0f)
				{
					brakeAudioSource.Play();
				}
			}
			if (!flag)
			{
				isPlayingSound = false;
				if (brakeAudioSource != null && brakeAudioSource.isPlaying)
				{
					brakeAudioSource.Stop();
				}
			}
		}
		else
		{
			isPlayingSound = false;
			if (brakeAudioSource != null && brakeAudioSource.isPlaying)
			{
				brakeAudioSource.Stop();
			}
			DustL.particleEmitter.emit = false;
			DustR.particleEmitter.emit = false;
			lastSkidmarkPosL = Vector3.zero;
			lastSkidmarkPosR = Vector3.zero;
		}
	}

	private void vklLevCompl()
	{
		Time.timeScale = 0f;
		if (settings.soundEnabled)
		{
			if (motorAudioSource != null)
			{
				motorAudioSource.Stop();
			}
			if (motorAudioSourceLow != null)
			{
				motorAudioSourceLow.Stop();
			}
			if (motorAudioSourceMid != null)
			{
				motorAudioSourceMid.Stop();
			}
			if (brakeAudioSource != null)
			{
				brakeAudioSource.Stop();
			}
		}
	}

	private void Update()
	{
		RotateWheels();
		SteelWheels();

		/*
		 * This must be in the "Update" function and not the "FixedUpdate" function because entering
		 * and exiting vehicles uses "GetKeyDown" and that would be missed on a fixed update.
		 */
		if (CompilationSettings.CarUseKeyboard)
		{
			KeyboardManager.Update(GameController.thisScript.playerScript);
		}
	}

	private void RotateWheels()
	{
		if (flWheel != null)
		{
			flWheel.Rotate(flWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
		if (frWheel != null)
		{
			frWheel.Rotate(frWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
		if (rlWheel != null)
		{
			rlWheel.Rotate(rlWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
		if (rrWheel != null)
		{
			rrWheel.Rotate(rrWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
		Transform[] array = rightAnyWheel;
		foreach (Transform transform in array)
		{
			transform.Rotate(rrWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
		Transform[] array2 = leftAnyWheel;
		foreach (Transform transform2 in array2)
		{
			transform2.Rotate(rlWheelCollider.rpm / 60f * 360f * Time.deltaTime * (float)wheelTurningParameter, 0f, 0f);
		}
	}

	private void SteelWheels()
	{
		if (flWheel != null)
		{
			flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
		}
		if (frWheel != null)
		{
			frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
		}
	}

	private void SetCurrentGear()
	{
		int count = gearSpeed.Count;
		for (int i = 0; i < count; i++)
		{
			if ((float)gearSpeed[i] > currentSpeed)
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
		num3 = (float)((double)((Mathf.Abs(currentSpeed) - num) / (num2 - num)) + 0.8);
		if (num3 > 2f)
		{
			num3 = 2f;
		}
		if (motorAudioSource != null)
		{
			motorAudioSource.pitch = num3;
		}
		if (motorAudioSourceLow != null)
		{
			motorAudioSourceLow.pitch = num3;
		}
		if (motorAudioSourceMid != null)
		{
			motorAudioSourceMid.pitch = num3;
		}
	}
}
