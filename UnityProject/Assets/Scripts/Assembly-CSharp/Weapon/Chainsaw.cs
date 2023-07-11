using UnityEngine;

public class Chainsaw : MonoBehaviour
{
	public AudioClip chainsawOn;

	private Weapon chWeapon;

	private AudioSource aSource;

	private void Start()
	{
		chWeapon = GetComponent<Weapon>();
		aSource = GetComponent<AudioSource>();
		aSource.clip = chainsawOn;
		aSource.loop = true;
		aSource.Play();
	}

	private void Update()
	{
	}
}
