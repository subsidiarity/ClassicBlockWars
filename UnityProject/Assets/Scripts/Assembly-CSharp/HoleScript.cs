using UnityEngine;

// TODO: Is this used?
public class HoleScript : MonoBehaviour
{
	public float liveTime = 5f;

	private void Start()
	{
	}

	private void Update()
	{
		liveTime -= Time.deltaTime;
		if (liveTime < 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
