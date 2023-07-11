using Holoville.HOTween;
using Photon;
using UnityEngine;

public class lerpTransformCarPhoton : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;

	private Vector3 correctPlayerRot = Vector3.zero;

	public bool objVistavlen;

	public bool sglajEnabled;

	private void Start()
	{
		if (!base.photonView.isMine)
		{
			HOTween.Init(false, false, false);
			sglajEnabled = false;
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		correctPlayerPos = (Vector3)stream.ReceiveNext();
		correctPlayerRot = (Vector3)stream.ReceiveNext();
		if (objVistavlen && sglajEnabled)
		{
			HOTween.Kill(base.gameObject.transform);
			float num = correctPlayerRot.x - base.transform.position.x;
			if (num > 100f)
			{
				correctPlayerRot.x = (int)correctPlayerRot.x - 360;
			}
			else if (num < -100f)
			{
				correctPlayerRot.x = (int)correctPlayerRot.x + 360;
			}
			num = correctPlayerRot.z - base.transform.position.z;
			if (num > 100f)
			{
				correctPlayerRot.z = (int)correctPlayerRot.z - 360;
			}
			else if (num < -100f)
			{
				correctPlayerRot.z = (int)correctPlayerRot.z + 360;
			}
			Debug.Log(string.Concat("from ", base.transform.eulerAngles, " to ", correctPlayerRot));
			HOTween.To(base.gameObject.transform, 0.2f, new TweenParms().Prop("position", correctPlayerPos).Prop("eulerAngles", correctPlayerRot).Ease(EaseType.Linear));
		}
		else
		{
			LeanTween.cancel(base.gameObject);
			objVistavlen = true;
			base.transform.position = correctPlayerPos;
			base.transform.eulerAngles = correctPlayerRot;
		}
	}
}
