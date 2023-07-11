using UnityEngine;

public class AIMotorMapping : MonoBehaviour
{
	[HideInInspector]
	public float steerInput;

	[HideInInspector]
	public float motorInput;

	[HideInInspector]
	public float brakeInput;

	[HideInInspector]
	public float handbrakeInput;

	[HideInInspector]
	public float steerMax;

	[HideInInspector]
	public float speedMax;

	public Transform flWheelMesh;

	public Transform frWheelMesh;

	public bool usingAIDriverMotor = true;

	private AIDriverMotor aIDriverMotor;

	private void Awake()
	{
		if (usingAIDriverMotor)
		{
			aIDriverMotor = GetComponent<AIDriverMotor>();
			steerMax = aIDriverMotor.maxSteerAngle;
			speedMax = aIDriverMotor.maxSpeed;
		}
	}

	private void Update()
	{
		if (usingAIDriverMotor)
		{
			aIDriverMotor.aiSteerAngle = steerInput;
			aIDriverMotor.aiSpeedPedal = motorInput;
			aIDriverMotor.aiBrakePedal = brakeInput;
		}
	}
}
