using UnityEngine;

public class removeAfterDelay : MonoBehaviour
{
	public float timer;

	private void Start()
	{
		Invoke("remove", timer);
	}

	private void remove()
	{
		Object.Destroy(base.gameObject);
	}
}
