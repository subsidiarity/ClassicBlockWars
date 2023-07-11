using UnityEngine;

public class CoinCollecter : MonoBehaviour
{
	private void OnTriggerEnter(Collider col)
	{
		if (col.tag.Equals("Pickable_Coin"))
		{
			int dataFromCurrentMission = MissionManager.Instance.GetDataFromCurrentMission<int>("Coins");
			Debug.Log("Before: " + dataFromCurrentMission);
			Debug.Log("After: " + MissionManager.Instance.SetDataForCurrentMission("Coins", --dataFromCurrentMission));
			Object.Destroy(col.gameObject);
		}
	}
}
