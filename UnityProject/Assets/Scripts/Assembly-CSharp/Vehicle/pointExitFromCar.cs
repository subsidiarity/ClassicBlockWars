using System.Collections.Generic;
using UnityEngine;

public class pointExitFromCar : MonoBehaviour
{
	public bool exitVozmojen;

	public bool colliderWithGround;

	private List<GameObject> listKolPrepiadstvii = new List<GameObject>();

	private void Update()
	{
		proverkaNaVozmojExit();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Untagged" || other.tag == "ground")
		{
			if (other.tag == "ground")
			{
				colliderWithGround = true;
			}
			listKolPrepiadstvii.Add(other.gameObject);
			proverkaNaVozmojExit();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Untagged" || other.tag == "ground")
		{
			if (other.tag == "ground")
			{
				colliderWithGround = false;
			}
			listKolPrepiadstvii.Remove(other.gameObject);
			proverkaNaVozmojExit();
		}
	}

	private void proverkaNaVozmojExit()
	{
		if (listKolPrepiadstvii.Count == 0)
		{
			exitVozmojen = true;
		}
		else
		{
			exitVozmojen = false;
		}
	}

	public void clearListPrepiadstvii()
	{
		listKolPrepiadstvii.Clear();
	}
}
