using UnityEngine;

public class carMovePath : MonoBehaviour
{
	public string namePath = string.Empty;

	public float procentRoad;

	public float speedCar_MS = 10f;

	public bool pereklNaDrugoiPut;

	public float pereklPriDostigProcenta = 100f;

	public carMovePath nextPath;

	private Vector3[] path;

	private float predProcentRoad;

	private float nextProcentRoad = 1f;

	private float procentPeredvij;

	private float lengthPath;

	private PhotonView photonView;

	private void Start()
	{
		photonView = PhotonView.Get(this);
		if (photonView.isMine)
		{
			path = iTweenPath.GetPath(namePath);
			lengthPath = iTween.PathLength(path);
		}
		else
		{
			base.enabled = false;
		}
	}

	private void FixedUpdate()
	{
		procentPeredvij = Time.deltaTime * 100f / (lengthPath / speedCar_MS);
		procentRoad += procentPeredvij;
		startZoom(procentRoad);
	}

	private void startZoom(float procenMove)
	{
		if (!object.Equals(procenMove, predProcentRoad))
		{
			predProcentRoad = procenMove;
			if (pereklNaDrugoiPut && procentRoad > pereklPriDostigProcenta)
			{
				base.enabled = false;
				nextPath.enabled = true;
			}
			if (procentRoad < 0f)
			{
				procentRoad = 100f;
			}
			if (procentRoad > 100f)
			{
				procentRoad = 0f;
			}
			nextProcentRoad = procentRoad + 1f;
			iTween.PutOnPath(base.gameObject, path, procentRoad / 100f);
			iTween.LookUpdate(base.gameObject, iTween.PointOnPath(path, nextProcentRoad / 100f), 1f);
		}
	}
}
