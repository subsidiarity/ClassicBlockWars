using UnityEngine;

public class JetpackBehavior : MonoBehaviour
{
	public bool modeOfADonkey;

	public bool activated;

	public bool isFlying;

	public UISlider fuelBar;

	public float fuel = 100f;

	public GameObject reactiveFire;

	private ThirdPersonController cacheController;

	private AudioSource cacheAudioSource;

	private bool CanBeActivated()
	{
		if (modeOfADonkey)
		{
			return true;
		}
		if (Load.LoadBool(settings.keyJetpackBought))
		{
			fuel = Load.LoadFloat("fuel");
			if (fuel <= 0f)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public static void ActivateFromShop()
	{
		settings.jetpackFuel = 100f;
		Save.SaveBool(settings.keyJetpackBought, true);
		Save.SaveFloat("fuel", settings.jetpackFuel);
	}

	public void Activate()
	{
		if (modeOfADonkey)
		{
			activated = true;
		}
		else if (CanBeActivated() && fuel > 0f)
		{
			activated = true;
			if (fuelBar != null)
			{
				fuelBar.gameObject.SetActive(true);
				fuelBar.value = fuel / 100f;
			}
			base.gameObject.SetActive(true);
		}
	}

	public void BroadcastParticle()
	{
		if (!modeOfADonkey)
		{
			if (isFlying)
			{
				reactiveFire.SetActive(true);
			}
			else
			{
				reactiveFire.SetActive(false);
			}
		}
	}

	public void DeactivateFromDisconnect()
	{
		if (modeOfADonkey)
		{
			activated = false;
			CheckCache();
			return;
		}
		activated = false;
		if (fuelBar != null)
		{
			fuelBar.GetComponent<JetpackFuelBehavior>().attached = false;
		}
		base.gameObject.SetActive(false);
		CheckCache();
		cacheController.pBehavior.StopJetpackSound();
	}

	public void Deactivate()
	{
		if (modeOfADonkey)
		{
			activated = false;
			CheckCache();
			return;
		}
		activated = false;
		if (fuelBar != null)
		{
			fuelBar.gameObject.SetActive(false);
		}
		base.gameObject.SetActive(false);
		settings.jetpackFuel = 0f;
		CheckCache();
		cacheController.pBehavior.StopJetpackSound();
		Save.SaveFloat("fuel", settings.jetpackFuel);
		Save.SaveBool(settings.keyJetpackBought, false);
	}

	private bool OnLadder()
	{
		return GameController.thisScript.curScriptLadder != null;
	}

	private void DisableLadder()
	{
		if (GameController.thisScript.curScriptLadder != null)
		{
			GameController.thisScript.curScriptLadder.dontUselLestnicu();
		}
	}

	private void CheckCache()
	{
		if (cacheController == null)
		{
			cacheController = GameController.thisScript.myPlayer.GetComponent<ThirdPersonController>();
		}
		if (cacheAudioSource == null)
		{
			cacheAudioSource = GameController.thisScript.myPlayer.GetComponent<AudioSource>();
		}
	}

	private void Update()
	{
		if (!activated)
		{
			return;
		}
		CheckCache();
		if (isFlying)
		{
			if (modeOfADonkey)
			{
				if (OnLadder())
				{
					DisableLadder();
				}
			}
			else if (fuel > 0f)
			{
				if (OnLadder())
				{
					DisableLadder();
				}
				cacheAudioSource.enabled = true;
				if (!settings.offlineMode && !reactiveFire.activeSelf)
				{
					cacheController.pBehavior.photonView.RPC("PlayJetpackSound", PhotonTargets.All);
				}
				fuel -= Time.deltaTime * 3f;
				Save.SaveFloat("fuel", fuel);
				if (fuelBar != null)
				{
					fuelBar.value = fuel / 100f;
				}
				if (!reactiveFire.activeSelf)
				{
					reactiveFire.SetActive(true);
				}
			}
			else
			{
				Deactivate();
			}
		}
		else if (!modeOfADonkey)
		{
			cacheAudioSource.enabled = false;
			if (!settings.offlineMode && reactiveFire.activeSelf)
			{
				cacheController.pBehavior.photonView.RPC("StopJetpackSound", PhotonTargets.All);
			}
			if (reactiveFire.activeSelf)
			{
				reactiveFire.SetActive(false);
			}
		}
	}
}
