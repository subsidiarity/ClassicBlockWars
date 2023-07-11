using Holoville.HOTween;
using Holoville.HOTween.Plugins;
using Photon;
using UnityEngine;

public class lerpTransformPhoton : Photon.MonoBehaviour
{
	private Vector3 correctPlayerPos = Vector3.zero;

	private Quaternion correctPlayerRot = Quaternion.identity;

	public bool objVistavlen;

	public bool sglajEnabled;

	public bool isLocalChange;

	private float izmenUglaOsi;

	private float maxTimeLerp = 0.3f;

	private float curTimeLerp;

	private float curTimeMove;

	private float lastTime;

	private CarBehavior car;

	private PlayerBehavior player;

	private void Start()
	{
		if (settings.offlineMode)
		{
			Object.Destroy(this);
		}
		else if (!base.photonView.isMine)
		{
			HOTween.Init();
			sglajEnabled = false;
			lastTime = Time.realtimeSinceStartup;
			car = GetComponent<CarBehavior>();
			player = GetComponent<PlayerBehavior>();
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (isLocalChange)
			{
				stream.SendNext(base.transform.localPosition);
				stream.SendNext(base.transform.localRotation);
			}
			else
			{
				stream.SendNext(base.transform.position);
				stream.SendNext(base.transform.rotation);
			}
			return;
		}
		correctPlayerPos = (Vector3)stream.ReceiveNext();
		correctPlayerRot = (Quaternion)stream.ReceiveNext();
		if (objVistavlen && sglajEnabled)
		{
			if (isLocalChange)
			{
				HOTween.To(base.gameObject.transform, maxTimeLerp, new TweenParms().Prop("localPosition", correctPlayerPos).Prop("localRotation", new PlugQuaternion(correctPlayerRot)));
			}
			else
			{
				HOTween.To(base.gameObject.transform, maxTimeLerp, new TweenParms().Prop("position", correctPlayerPos).Prop("rotation", new PlugQuaternion(correctPlayerRot)));
			}
			return;
		}
		objVistavlen = true;
		if (isLocalChange)
		{
			base.transform.localPosition = correctPlayerPos;
			base.transform.localRotation = correctPlayerRot;
		}
		else
		{
			base.transform.position = correctPlayerPos;
			base.transform.rotation = correctPlayerRot;
		}
	}
}
