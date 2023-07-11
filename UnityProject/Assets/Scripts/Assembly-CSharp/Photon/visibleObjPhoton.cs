using UnityEngine;

public class visibleObjPhoton : MonoBehaviour
{
	public lerpTransformPhoton lerpScript;

	public bool isVisible;

	private void Start()
	{
		if (settings.offlineMode)
		{
			Object.Destroy(this);
		}
		findLerpScript(base.transform);
	}

	private void findLerpScript(Transform curTransfrom)
	{
		if (curTransfrom != null)
		{
			lerpScript = curTransfrom.GetComponent<lerpTransformPhoton>();
			if (!(lerpScript != null))
			{
				findLerpScript(curTransfrom.parent);
			}
		}
	}

	private void OnBecameVisible()
	{
		if (lerpScript == null)
		{
			findLerpScript(base.transform);
		}
		if (lerpScript != null)
		{
			lerpScript.sglajEnabled = true;
			isVisible = true;
			CancelInvoke("delayInvisile");
		}
	}

	public void SetVisibleWithDelay()
	{
		Invoke("OnBecameVisible", 0.1f);
	}

	private void OnBecameInvisible()
	{
		if (lerpScript != null)
		{
			Invoke("delayInvisile", 0.3f);
		}
	}

	private void delayInvisile()
	{
		if (lerpScript == null)
		{
			findLerpScript(base.transform);
		}
		if (lerpScript != null)
		{
			lerpScript.sglajEnabled = false;
		}
		isVisible = false;
	}
}
