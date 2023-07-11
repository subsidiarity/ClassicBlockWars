using UnityEngine;

public class showHideNamePlayer : MonoBehaviour
{
	public PlayerBehavior playerScript;

	private void Start()
	{
		if (playerScript == null)
		{
			findPlayerScript(base.transform);
		}
	}

	private void findPlayerScript(Transform curTransfrom)
	{
		if (curTransfrom != null)
		{
			playerScript = curTransfrom.GetComponent<PlayerBehavior>();
			if (!(playerScript != null))
			{
				findPlayerScript(curTransfrom.parent);
			}
		}
	}

	private void OnBecameVisible()
	{
		if (playerScript != null && playerScript.scriptPlayerName != null)
		{
			playerScript.scriptPlayerName.isVisible = true;
		}
	}

	private void OnBecameInvisible()
	{
		if (playerScript != null && playerScript.scriptPlayerName != null)
		{
			playerScript.scriptPlayerName.isVisible = false;
		}
	}
}
