using System;
using UnityEngine;

[Serializable]
public class MotorIdle : MonoBehaviour
{
	public virtual void FixedUpdate()
	{
		IdleSound();
	}

	public virtual void IdleSound()
	{
		float num = 0f;
		num = Input.GetAxis("Vertical") + 0.8f;
		audio.pitch = num;
	}

	public virtual void Main()
	{
	}
}
