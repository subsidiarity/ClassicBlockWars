using UnityEngine;

public class showResaultStars : MonoBehaviour
{
	public int showKolStars;

	public GameObject[] arrStars = new GameObject[3];

	public void updateShowKolStars(int kol)
	{
		showKolStars = kol;
		for (int i = 0; i < 3; i++)
		{
			if (i < showKolStars)
			{
				arrStars[i].SetActive(true);
			}
			else
			{
				arrStars[i].SetActive(false);
			}
		}
	}
}
