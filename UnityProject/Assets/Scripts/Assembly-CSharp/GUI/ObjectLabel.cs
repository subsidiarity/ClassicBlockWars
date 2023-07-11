using System;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class ObjectLabel : MonoBehaviour
{
	public Transform target;

	public Vector3 offset = Vector3.down;

	public bool clampToScreen;

	public float clampBorderSize = 0.05f;

	public bool useMainCamera = true;

	public Camera cameraToUse;

	public Camera cam;

	public Vector3 posLabel;

	public GUIText nameLabel;

	private Transform thisTransform;

	private Transform camTransform;

	private PhotonView photonView;

	private GUITexture aimTexture;

	private float aimWidth;

	private void Start()
	{
		thisTransform = base.transform;
		cam = Camera.main;
		camTransform = cam.transform;
		if (!settings.offlineMode)
		{
			photonView = base.transform.root.gameObject.GetComponent<PhotonView>();
		}
		base.transform.localScale = Vector3.zero;
		nameLabel = GetComponent<GUIText>();
		aimTexture = base.gameObject.GetComponent<GUITexture>();
		float num = (float)Screen.height / 768f;
		aimWidth = (float)aimTexture.texture.width * num;
		int fontSize = (int)(20f * num);
		nameLabel.fontSize = fontSize;
		aimTexture.pixelInset = new Rect((0f - aimWidth) / 2f, (0f - aimWidth) / 2f, aimWidth, aimWidth);
	}

	private void Update()
	{
		if ((!settings.offlineMode && !photonView.isMine) || !(target != null))
		{
			return;
		}
		try
		{
			if (!(target.collider != null))
			{
				return;
			}
			offset = new Vector3(0f, target.collider.bounds.center.y - target.position.y, 0f);
			if (target.tag.Equals("Player"))
			{
				offset = new Vector3(0f, 1f, 0f);
			}
			if (clampToScreen)
			{
				Vector3 position = camTransform.InverseTransformPoint(target.position);
				position.z = Mathf.Max(position.z, 1f);
				thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(position));
				thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1f - clampBorderSize), Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1f - clampBorderSize), thisTransform.position.z);
			}
			else
			{
				posLabel = cam.WorldToViewportPoint(target.position + offset);
				if (posLabel.z >= 0f)
				{
					thisTransform.position = posLabel;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception in ObjectLabel: " + ex);
		}
	}
}
