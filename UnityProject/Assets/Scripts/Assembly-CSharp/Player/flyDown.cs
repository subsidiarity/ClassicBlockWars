using UnityEngine;

// TODO: I don't think this is needed.
public class flyDown : MonoBehaviour
{
	private GameObject player;

	private void Start()
	{
		player = GameController.thisScript.myPlayer;
	}

	private void FixedUpdate()
	{
		if (player != null && player.transform.position.y < -1f)
		{
			player.transform.position = new Vector3(player.transform.position.x, 0.2f, player.transform.position.z);
		}
	}
}
