using System.Collections;
using UnityEngine;

public class controllerConnectGame : MonoBehaviour
{
	public static controllerConnectGame thisScript;

	public static bool reconnectRoom;

	private PhotonView photonView;

	private GameObject myPlayer;

	private PlayerBehavior playerScript;

	private int countConnectToRoom;

	private bool ReconnectCanceled;

	private int reconPoint;

	private int reconHealth;

	private Vector3 reconPosition;

	private Quaternion reconRotation;

	private float offsetPositionPlayerY = 0.2f;

	public Transform nearPoint;

	public Vector3 lastPointForRespawn = Vector3.zero;

	private void Awake()
	{
		thisScript = this;
	}

	private void Start()
	{
		if (settings.offlineMode)
		{
			base.enabled = false;
		}
		else
		{
			PhotonNetwork.isMessageQueueRunning = true;
			photonView = PhotonView.Get(this);
			instantiateNetworkObjects();
		}
		if (settings.testBuilding && !settings.offlineMode)
		{
			createTestPlayers();
		}
	}

	public void createTestPlayers()
	{
		StartCoroutine(addTestPlayers());
	}

	private void OnDestroy()
	{
		reconnectRoom = false;
		thisScript = null;
	}

	private IEnumerator addTestPlayers()
	{
		for (int j = 0; j < 10; j++)
		{
			GameObject testP = PhotonNetwork.Instantiate("PlayerTest", Vector3.zero, Quaternion.identity, 0);
			testP.GetComponent<hoMove>().SetPath(GameController.thisScript.arrRoads[0]);
			testP.GetComponent<hoMove>().enabled = true;
			yield return new WaitForSeconds(2f);
		}
		for (int i = 0; i < 10; i++)
		{
			GameObject testP2 = PhotonNetwork.Instantiate("PlayerTest", Vector3.zero, Quaternion.identity, 0);
			testP2.GetComponent<hoMove>().SetPath(GameController.thisScript.arrRoads[1]);
			testP2.GetComponent<hoMove>().enabled = true;
			yield return new WaitForSeconds(2f);
		}
	}

	public void instantiateNetworkObjects()
	{
		if (!(myPlayer != null))
		{
			if (reconnectRoom)
			{
				myPlayer = PhotonNetwork.Instantiate("Player", reconPosition, reconRotation, 0);
				GameController.thisScript.showWindowGame();
			}
			else
			{
				myPlayer = PhotonNetwork.Instantiate("Player", getRandomPointNearPlayer().position, Quaternion.identity, 0);
			}
			playerScript = myPlayer.GetComponent<PlayerBehavior>();
			GameController.thisScript.myPlayer = myPlayer;
			GameController.thisScript.playerScript = playerScript;
			GameController.thisScript.weaponManagerScripts = myPlayer.GetComponent<WeaponManager>();
			if (reconnectRoom)
			{
				playerScript.Health = reconHealth;
				playerScript.Points = reconPoint;
				playerScript.photonView.RPC("updatePoints", PhotonTargets.OthersBuffered, playerScript.Points);
				playerScript.tController.gravity = 20f;
			}
			reconnectRoom = false;
		}
	}

	public Transform getRandomPointNearPlayer()
	{
		if (GameController.thisScript.myPlayer == null)
		{
			Transform randomPointAllMap = getRandomPointAllMap();
			randomPointAllMap.position = new Vector3(randomPointAllMap.position.x, randomPointAllMap.position.y + offsetPositionPlayerY, randomPointAllMap.position.z);
			lastPointForRespawn = randomPointAllMap.position;
			return randomPointAllMap;
		}
		if (GameController.thisScript.allPlayerRespawnPoints != null)
		{
			Transform[] componentsInChildren = GameController.thisScript.allPlayerRespawnPoints.GetComponentsInChildren<Transform>();
			nearPoint = null;
			Transform[] array = componentsInChildren;
			foreach (Transform transform in array)
			{
				if (!lastPointForRespawn.Equals(transform.position) && Vector3.Distance(GameController.thisScript.myPlayer.transform.position, transform.position) > 25f)
				{
					if (nearPoint == null)
					{
						nearPoint = transform;
					}
					else if (Vector3.Distance(nearPoint.position, GameController.thisScript.myPlayer.transform.position) > Vector3.Distance(transform.position, GameController.thisScript.myPlayer.transform.position))
					{
						nearPoint = transform;
					}
				}
			}
			if (nearPoint != null)
			{
				nearPoint.position = new Vector3(nearPoint.position.x, nearPoint.position.y + offsetPositionPlayerY, nearPoint.position.z);
				lastPointForRespawn = nearPoint.position;
			}
			return nearPoint;
		}
		return null;
	}

	public Transform getRandomPointAllMap()
	{
		if (GameController.thisScript.allPlayerRespawnPoints != null)
		{
			Transform[] componentsInChildren = GameController.thisScript.allPlayerRespawnPoints.GetComponentsInChildren<Transform>();
			int num = Random.Range(0, componentsInChildren.Length);
			return componentsInChildren[num];
		}
		return null;
	}

	public void OnLeftRoom()
	{
		if (!reconnectRoom)
		{
			GameController.thisScript.exitAfterDisconect();
		}
	}

	public void saveSettingsPlayer()
	{
		reconPoint = playerScript.Points;
		reconHealth = playerScript.Health;
		reconPosition = myPlayer.transform.position;
		reconRotation = myPlayer.transform.rotation;
	}

	public void stopRecconect()
	{
		reconnectRoom = false;
		ReconnectCanceled = true;
		ActivityIndicator.activEnabled = false;
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		if (settings.offlineMode)
		{
			return;
		}
		Debug.Log("reconnect");
		countConnectToRoom = 0;
		reconnectRoom = true;
		Invoke("ConnectToPhoton", 3f);
		if (GameController.thisScript.playerScript != null)
		{
			saveSettingsPlayer();
			if (GameController.thisScript.playerScript.inCar)
			{
				CarBehavior currentCar = GameController.thisScript.playerScript.currentCar;
				GameController.thisScript.playerScript.GetOutOfCar();
				currentCar.resetCarOnline();
			}
			if (GameController.thisScript.playerScript.inHelic)
			{
				HelicopterBehavior currentHelic = GameController.thisScript.playerScript.currentHelic;
				GameController.thisScript.playerScript.GetOutOfHelic();
				currentHelic.resetHelicOnline();
			}
			GameController.thisScript.playerScript.tController.jetpack.DeactivateFromDisconnect();
		}
		GameController.thisScript.playerScript.RemoveAllGrenade();
		GameController.thisScript.listPlayers.Clear();
		GameController.thisScript.removeAllButBezPlayers();
		GameController.thisScript.showPanelReconnect();
	}

	private void ConnectToPhoton()
	{
		if (!ReconnectCanceled)
		{
			Debug.Log("ConnectToPhoton");
			PhotonNetwork.ConnectUsingSettings(settings.verConnectPhoton);
		}
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
		if (!ReconnectCanceled)
		{
			Invoke("ConnectToPhoton", 3f);
		}
	}

	public void OnJoinedLobby()
	{
		ConnectToRoom();
	}

	private void ConnectToRoom()
	{
		if (!settings.offlineMode)
		{
			Debug.Log("OnJoinedLobby " + Load.LoadString(settings.keyRoomName));
			if (!ReconnectCanceled)
			{
				PhotonNetwork.JoinRoom(Load.LoadString(settings.keyRoomName));
			}
		}
	}

	private void OnPhotonJoinRoomFailed()
	{
		countConnectToRoom++;
		Debug.Log("OnPhotonJoinRoomFailed - init");
		if (countConnectToRoom < 6)
		{
			Invoke("ConnectToRoom", 3f);
			return;
		}
		Debug.Log("reconnect failed");
		GameController.thisScript.exitAfterDisconect();
	}

	private void OnJoinedRoom()
	{
		if (reconnectRoom)
		{
			GameController.thisScript.hidePanelReconnect();
		}
		Debug.Log("OnJoinedRoom - init");
		instantiateNetworkObjects();
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnectedinit: " + player);
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconnecedinit: " + player.ID);
		GameController.thisScript.removePlayerWithID(player.ID);
	}

	public void OnReceivedRoomList()
	{
		Debug.Log("OnReceivedRoomListinit");
	}

	public void OnReceivedRoomListUpdate()
	{
		Debug.Log("OnReceivedRoomListUpdateinit");
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhotoninit");
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate init" + info.sender);
	}
}
