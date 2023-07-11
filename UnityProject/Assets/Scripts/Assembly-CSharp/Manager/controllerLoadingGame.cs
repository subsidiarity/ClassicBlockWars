using UnityEngine;

public class controllerLoadingGame : MonoBehaviour
{
	private void Start()
	{
		ActivityIndicator.activEnabled = false;
		Application.LoadLevel("Level1");
	}
}
