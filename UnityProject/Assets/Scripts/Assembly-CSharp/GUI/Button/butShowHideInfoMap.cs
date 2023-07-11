using UnityEngine;

public class butShowHideInfoMap : MonoBehaviour
{
	public GameObject sprInfo;

	private void Start()
	{
		sprInfo.SetActive(false);
	}

	private void OnClick()
	{
		if (sprInfo.activeSelf)
		{
			sprInfo.SetActive(false);
		}
		else
		{
			sprInfo.SetActive(true);
		}
	}
}
