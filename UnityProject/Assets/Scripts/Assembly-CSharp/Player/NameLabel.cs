using System;
using UnityEngine;

[RequireComponent(typeof(GUIText))]
public class NameLabel : MonoBehaviour
{
	public static Camera currentCamera;

	public bool isVisible;

	public Transform target;

	public Vector3 offset = Vector3.up;

	public bool clampToScreen;

	public float clampBorderSize = 0.05f;

	public bool useMainCamera = true;

	public Camera cameraToUse;

	public Camera cam;

	public Vector3 posLabel;

	private Transform thisTransform;

	private Transform camTransform;

	public GUIText nameText;

	private void Start()
	{
		offset = new Vector3(0f, 2.4f, 0f);
		thisTransform = base.transform;
		cam = Camera.main;
		camTransform = cam.transform;
		if (nameText == null)
		{
			nameText = GetComponent<GUIText>();
		}
	}

	private void Update()
	{
		if (target == null || cam == null)
		{
			Debug.Log("target=null");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		try
		{
			if (nameText == null)
			{
				nameText = GetComponent<GUIText>();
			}
			nameText.text = target.GetComponent<PlayerBehavior>().nick;
			if (clampToScreen)
			{
				Vector3 vector = camTransform.InverseTransformPoint(target.position);
				vector.z = Mathf.Max(vector.z, 1f);
				thisTransform.position = cam.WorldToViewportPoint(camTransform.TransformPoint(vector + offset));
				thisTransform.position = new Vector3(Mathf.Clamp(thisTransform.position.x, clampBorderSize, 1f - clampBorderSize), Mathf.Clamp(thisTransform.position.y, clampBorderSize, 1f - clampBorderSize), thisTransform.position.z);
			}
			else if (isVisible)
			{
				posLabel = cam.WorldToViewportPoint(target.position + offset);
				thisTransform.position = posLabel;
			}
			else
			{
				thisTransform.position = new Vector3(-1000f, -1000f, -1000f);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception in ObjectLabel: " + ex);
		}
	}
}
