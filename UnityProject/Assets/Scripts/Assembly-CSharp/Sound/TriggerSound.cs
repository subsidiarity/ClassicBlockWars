using UnityEngine;

public class TriggerSound : MonoBehaviour
{
	public string tagName1 = string.Empty;

	public string tagName2 = "bot";

	public AudioClip triggerSound;

	public float soundVolume = 1f;

	private AudioSource triggerAudioSource;

	private NewDriving newDrivingScript;

	private void Awake()
	{
	}

	private void InitSound(out AudioSource myAudioSource, AudioClip myClip, float myVolume, bool looping)
	{
		myAudioSource = base.gameObject.AddComponent("AudioSource") as AudioSource;
		myAudioSource.playOnAwake = false;
		myAudioSource.clip = myClip;
		myAudioSource.loop = looping;
		myAudioSource.volume = myVolume;
	}
}
