using System;
using UnityEngine;

public class OpticalAimer : MonoBehaviour
{
	[HideInInspector]
	public bool sniperMode;

	public GameObject aimTexture;

	public GameObject centerTexture;

	public GameObject buttonAimerPrefab;

	private float normalFOV;

	private float zoomedFOV = 15f;

	private RPG_Camera cam;

	private PlayerBehavior pBehavior;

	private GameObject instantiatedAimerButton;

	private GameObject instantiatedAimerTexture;

	private GameObject instantiatedAimerCenter;

	private GameObject aimerObjWhite;

	private float initialCameraDistance;

	private void Start()
	{
		if (base.transform.root.Find("Aimer") != null)
		{
			aimerObjWhite = base.transform.root.Find("Aimer").gameObject;
			pBehavior = base.transform.root.GetComponent<PlayerBehavior>();
			cam = Camera.main.GetComponent<RPG_Camera>();
			initialCameraDistance = 5f;
			normalFOV = cam.camera.fieldOfView;
			InstantiateSniperObjects();
		}
	}

	private void SwitchRenderers(bool on)
	{
		if (pBehavior != null)
		{
			if (pBehavior.enemyCollider != null)
			{
				pBehavior.enemyCollider.SetActive(on);
			}
			pBehavior.playerMesh.renderer.enabled = on;
			base.transform.Find("SniperRifle 1/Arms_Mesh").renderer.enabled = on;
			base.transform.Find("SniperRifle 1/Arms_Mesh/Sniper_rifle_Mesh").renderer.enabled = on;
			pBehavior.playerMesh.renderer.enabled = on;
		}
	}

	private void InstantiateSniperObjects()
	{
		GameObject gameObject = GameController.thisScript.walkInterface.gameObject;
		if (!(gameObject != null))
		{
			return;
		}
		GameObject gameObject2 = gameObject.transform.Find("AnchorCenterBottom").gameObject;
		if (gameObject2 != null)
		{
			instantiatedAimerTexture = NGUITools.AddChild(gameObject2, aimTexture);
			instantiatedAimerCenter = NGUITools.AddChild(gameObject2, centerTexture);
			instantiatedAimerCenter.SetActive(false);
			instantiatedAimerTexture.SetActive(false);
		}
		GameObject gameObject3 = gameObject.transform.Find("AnchorRightBottom").gameObject;
		if (gameObject3 != null)
		{
			instantiatedAimerButton = NGUITools.AddChild(gameObject3, buttonAimerPrefab);
			instantiatedAimerButton.transform.localPosition = buttonAimerPrefab.transform.localPosition;
			UIEventListener component = instantiatedAimerButton.GetComponent<UIEventListener>();
			if (component != null)
			{
				component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(ShowSniperAimer));
			}
		}
	}

	private void Update()
	{
		if (!sniperMode)
		{
			return;
		}
		Vector3 direction = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)).direction;
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
		RaycastHit hitInfo;
		if (!Physics.Raycast(ray, out hitInfo, float.PositiveInfinity) || !((float)GameController.thisScript.playerScript.weaponManager.currentWeaponScript.maxDistance > Vector3.Distance(base.transform.position, hitInfo.point)))
		{
			return;
		}
		if (hitInfo.transform.tag.Equals("collidePoint") || hitInfo.transform.tag.Equals("Car"))
		{
			if (instantiatedAimerCenter != null)
			{
				instantiatedAimerCenter.SetActive(true);
			}
		}
		else if (instantiatedAimerCenter != null)
		{
			instantiatedAimerCenter.SetActive(false);
		}
	}

	private void ShowSniperAimer(GameObject go)
	{
		if (cam != null)
		{
			cam.mouseSpeed = 1f;
		}
		sniperMode = true;
		if (aimerObjWhite != null)
		{
			aimerObjWhite.SetActive(false);
		}
		base.transform.localPosition = base.transform.forward * -3f;
		cam.camera.fieldOfView = zoomedFOV;
		cam.desiredDistance = 0f;
		instantiatedAimerTexture.SetActive(true);
		SwitchRenderers(false);
		UIEventListener component = instantiatedAimerButton.GetComponent<UIEventListener>();
		if (component != null)
		{
			component.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(component.onClick, new UIEventListener.VoidDelegate(ShowSniperAimer));
			component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(HideSniperAimer));
		}
	}

	private void HideSniperAimer(GameObject go)
	{
		if (cam != null)
		{
			cam.mouseSpeed = 4f;
		}
		sniperMode = false;
		if (aimerObjWhite != null)
		{
			aimerObjWhite.SetActive(true);
		}
		if (instantiatedAimerCenter != null)
		{
			instantiatedAimerCenter.SetActive(false);
		}
		if (instantiatedAimerButton != null)
		{
			UIEventListener component = instantiatedAimerButton.GetComponent<UIEventListener>();
			if (component != null)
			{
				component.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(component.onClick, new UIEventListener.VoidDelegate(ShowSniperAimer));
				component.onClick = (UIEventListener.VoidDelegate)Delegate.Remove(component.onClick, new UIEventListener.VoidDelegate(HideSniperAimer));
			}
		}
		SwitchRenderers(true);
		if (cam != null)
		{
			cam.camera.fieldOfView = normalFOV;
			cam.desiredDistance = initialCameraDistance;
		}
		if (instantiatedAimerTexture != null)
		{
			instantiatedAimerTexture.SetActive(false);
		}
		base.transform.localPosition = Vector3.zero;
	}

	private void OnDisable()
	{
		HideSniperAimer(base.gameObject);
	}

	private void OnDestroy()
	{
		SwitchRenderers(true);
		if (cam != null)
		{
			cam.desiredDistance = initialCameraDistance;
			cam.camera.fieldOfView = normalFOV;
		}
		UnityEngine.Object.Destroy(instantiatedAimerButton);
		UnityEngine.Object.Destroy(instantiatedAimerTexture);
	}
}
