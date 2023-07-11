using System.Collections.Generic;
using UnityEngine;

public class miniMap : MonoBehaviour
{
	public static miniMap thisScript;

	public Vector2 sizeLevelXZ;

	public Vector2 sizeMiniMapXY;

	public Vector2 offsetMiniMapXY;

	public bool resizeOnlyStart = true;

	public GameObject pointForRotation;

	public UITexture textMiniMap;

	public Object exampleSprObj;

	private List<objMiniMap> listObjMiniMap = new List<objMiniMap>();

	private float koefMapX;

	private float koefMapY;

	private float offsetX;

	private float offsetY;

	private float predUgol;

	private float ugolPlayer;

	private void Awake()
	{
		thisScript = this;
	}

	private void Start()
	{
		koefMapX = sizeMiniMapXY.x / sizeLevelXZ.x;
		koefMapY = sizeMiniMapXY.y / sizeLevelXZ.y;
	}

	private void Update()
	{
		if (GameController.thisScript.myPlayer == null)
		{
			return;
		}
		if (!resizeOnlyStart)
		{
			koefMapX = sizeMiniMapXY.x / sizeLevelXZ.x;
			koefMapY = sizeMiniMapXY.y / sizeLevelXZ.y;
		}
		offsetX = GameController.thisScript.myPlayer.transform.position.x * koefMapX + offsetMiniMapXY.x;
		offsetY = GameController.thisScript.myPlayer.transform.position.z * koefMapY + offsetMiniMapXY.y;
		textMiniMap.transform.localPosition = new Vector2(offsetX, offsetY);
		if (GameController.thisScript.playerScript.inCar)
		{
			ugolPlayer = GameController.thisScript.myCar.transform.eulerAngles.y;
		}
		else
		{
			ugolPlayer = GameController.thisScript.myPlayer.transform.eulerAngles.y;
		}
		pointForRotation.transform.localEulerAngles = new Vector3(0f, 0f, ugolPlayer + 180f);
		foreach (objMiniMap item in listObjMiniMap)
		{
			item.sprMiniMap.transform.localPosition = new Vector2(0f - (item.objToWorld.transform.position.x * koefMapX + offsetMiniMapXY.x), 0f - (item.objToWorld.transform.position.z * koefMapY + offsetMiniMapXY.y));
		}
	}

	public void addObjMiniMap(GameObject gameObj)
	{
		foreach (objMiniMap item2 in listObjMiniMap)
		{
			if (item2.objToWorld == gameObj)
			{
				Debug.Log(string.Concat("объект ", gameObj, " уже добавлен на миникарту"));
				return;
			}
		}
		GameObject gameObject = (GameObject)Object.Instantiate(exampleSprObj);
		gameObject.transform.parent = textMiniMap.transform;
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
		gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		gameObject.SetActive(true);
		objMiniMap item = new objMiniMap(gameObj, gameObject);
		listObjMiniMap.Add(item);
	}

	public void removeObjMiniMap(GameObject gameObj)
	{
		foreach (objMiniMap item in listObjMiniMap)
		{
			if (item.objToWorld == gameObj)
			{
				Object.Destroy(item.sprMiniMap);
				listObjMiniMap.Remove(item);
				break;
			}
		}
	}
}
