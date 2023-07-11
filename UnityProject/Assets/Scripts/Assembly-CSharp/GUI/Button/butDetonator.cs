using UnityEngine;

public class butDetonator : MonoBehaviour
{
	private void OnClick()
	{
		GameController.thisScript.playerScript.DetonateGrenade();
	}
}
