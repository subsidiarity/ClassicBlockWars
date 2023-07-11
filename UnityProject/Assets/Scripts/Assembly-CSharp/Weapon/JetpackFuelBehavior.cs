using UnityEngine;

public class JetpackFuelBehavior : MonoBehaviour
{
	[HideInInspector]
	public bool attached;

	private void Start()
	{
	}

	private void Update()
	{
		if (attached)
		{
			base.gameObject.SetActive(true);
		}
		else
		{
			if (GameController.thisScript.myPlayer == null)
			{
				return;
			}
			if (settings.offlineMode)
			{
				ThirdPersonController component = GameController.thisScript.myPlayer.GetComponent<ThirdPersonController>();
				if (component != null && component.jetpack != null)
				{
					component.jetpack.fuelBar = GetComponent<UISlider>();
					base.gameObject.SetActive(false);
					attached = true;
				}
			}
			else if (GameController.thisScript.playerScript.photonView.isMine)
			{
				ThirdPersonController component2 = GameController.thisScript.myPlayer.GetComponent<ThirdPersonController>();
				if (component2 != null && component2.jetpack != null)
				{
					component2.jetpack.fuelBar = GetComponent<UISlider>();
					base.gameObject.SetActive(false);
					attached = true;
				}
			}
		}
	}

	private void OnDestroy()
	{
		attached = false;
	}
}
