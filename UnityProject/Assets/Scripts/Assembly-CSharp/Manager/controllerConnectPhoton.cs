using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;

public class controllerConnectPhoton : MonoBehaviour
{
	public enum connectTo
	{
		none = 0,
		quickGame = 1,
		customGame = 2
	}

	public static controllerConnectPhoton thisScript;

	private List<RoomInfo> filteredRoomList = new List<RoomInfo>();

	private List<butScrollRoom> butRoomList = new List<butScrollRoom>();

	public connectTo tekConnect;

	public string filterForSearchRoom = string.Empty;

	private void Awake()
	{
		thisScript = this;
	}

	public void _initializeWorldwide()
	{
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings(settings.verConnectPhoton);
			butRoomList.Clear();
			removeAllButBezRoom();
		}
	}

	private void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters);
		controllerMenu.thisScript.hideIndicator();
		controllerMenu.thisScript.showMessage("Ops, something happened. Check the internet connection.");
	}

	private void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon.");
		controllerMenu.thisScript.hideIndicator();
	}

	public void OnConnectedToPhoton()
	{
		Debug.Log("OnConnectedToPhotoninit");
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		Debug.Log("OnPhotonInstantiate init" + info.sender);
	}

	public void OnJoinedLobby()
	{
		Debug.Log("OnJoinedLobby");
		controllerMenu.thisScript.hideIndicator();
		updateFilteredRoomList(filterForSearchRoom);
		if (!controllerMenu.thisScript.ClickOnButton)
		{
			tekConnect = connectTo.none;
		}
		if (tekConnect == connectTo.quickGame)
		{
			ConnectCreate_RandomRoom();
		}
		if (tekConnect == connectTo.customGame)
		{
			controllerMenu.thisScript.connectCreate_showCustomGame();
		}
	}

	private void OnConnectionFail(DisconnectCause cause)
	{
		controllerMenu.thisScript.hideIndicator();
		controllerMenu.thisScript.showMainMenu();
		controllerMenu.thisScript.showMessage("Ops, something happened. Check the internet connection.");
	}

	private void OnPhotonJoinRoomFailed()
	{
		Debug.Log("OnPhotonJoinRoomFailed");
		controllerMenu.thisScript.hideIndicator();
		controllerMenu.thisScript.showMessage("Connection failed. Try again please.");
	}

	public void connectRandomRoom()
	{
		if (!PhotonNetwork.connected)
		{
			tekConnect = connectTo.quickGame;
			_initializeWorldwide();
		}
		else
		{
			ConnectCreate_RandomRoom();
		}
	}

	private void ConnectCreate_RandomRoom()
	{
		controllerMenu.thisScript.showIndicator();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable["pass"] = string.Empty;
		PhotonNetwork.JoinRandomRoom(hashtable, 2);
	}

	private void OnPhotonRandomJoinFailed()
	{
		string[] propsToListInLobby = new string[1] { "pass" };
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("pass", string.Empty);
		ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
		PhotonNetwork.CreateRoom(null, true, true, 10, customRoomProperties, propsToListInLobby);
	}

	private void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom" + PhotonNetwork.room);
		StartCoroutine(MoveToGameScene());
	}

	private void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		StartCoroutine(MoveToGameScene());
	}

	public void createNewRoom(string NameRoom, int MaxKolPlayers, string Password)
	{
		if (!roomWithNameCreate(NameRoom))
		{
			controllerMenu.thisScript.showIndicator();
			string[] propsToListInLobby = new string[1] { "pass" };
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("pass", Password);
			ExitGames.Client.Photon.Hashtable customRoomProperties = hashtable;
			PhotonNetwork.CreateRoom(NameRoom, true, true, MaxKolPlayers, customRoomProperties, propsToListInLobby);
		}
		else
		{
			controllerMenu.thisScript.showMessage("This name is already exist.");
		}
	}

	private bool roomWithNameCreate(string nameRoom)
	{
		foreach (RoomInfo filteredRoom in filteredRoomList)
		{
			if (filteredRoom.name == nameRoom)
			{
				return true;
			}
		}
		return false;
	}

	public void joinRoomName(string name)
	{
		controllerMenu.thisScript.showIndicator();
		settings.offlineMode = false;
		PhotonNetwork.JoinRoom(name);
	}

	public void OnReceivedRoomListUpdate()
	{
		updateFilteredRoomList(filterForSearchRoom);
	}

	public void updateFilteredRoomList(string gFilter)
	{
		filteredRoomList.Clear();
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].name.StartsWith(gFilter, true, null))
			{
				filteredRoomList.Add(roomList[i]);
			}
		}
		obnovitSpisokButForRoom();
	}

	public void obnovitSpisokButForRoom()
	{
		GameObject objGrid = controllerMenu.thisScript.objGrid;
		Object exampleButRoom = controllerMenu.thisScript.exampleButRoom;
		int num = 0;
		removeAllButBezRoom();
		foreach (RoomInfo filteredRoom in filteredRoomList)
		{
			if (sozdanaLiButRoom(filteredRoom.name))
			{
				butScrollRoom butScrollRoom2 = butRoomList[num];
				butScrollRoom2.gameObject.name = string.Empty + num;
				butScrollRoom2.labelKolPlayers.text = string.Empty + filteredRoom.playerCount + "/" + filteredRoom.maxPlayers;
			}
			else
			{
				GameObject gameObject = (GameObject)Object.Instantiate(exampleButRoom);
				gameObject.name = string.Empty + num;
				butScrollRoom component = gameObject.GetComponent<butScrollRoom>();
				component.nameRoom = filteredRoom.name;
				component.labelKolPlayers.text = string.Empty + filteredRoom.playerCount + "/" + filteredRoom.maxPlayers;
				if (filteredRoom.name.Length == 36 && filteredRoom.name.IndexOf("-") == 8 && filteredRoom.name.LastIndexOf("-") == 23)
				{
					component.labelNameRoom.text = "Random Server";
				}
				else if (!CompilationSettings.NoBadWordFilter)
				{
					component.labelNameRoom.text = FilterBadWorld.FilterString(filteredRoom.name);
				}
				if (filteredRoom.customProperties["pass"].Equals(string.Empty))
				{
					component.sprLock.SetActive(false);
				}
				else
				{
					component.sprUnlock.SetActive(false);
					component.password = filteredRoom.customProperties["pass"].ToString();
				}
				gameObject.transform.parent = objGrid.transform;
				gameObject.SetActive(true);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				butRoomList.Add(component);
			}
			num++;
		}
		controllerMenu.thisScript.gridScript.repositionNow = true;
	}

	private void removeAllButBezRoom()
	{
		bool flag = false;
		List<butScrollRoom> list = new List<butScrollRoom>();
		foreach (butScrollRoom butRoom in butRoomList)
		{
			flag = false;
			foreach (RoomInfo filteredRoom in filteredRoomList)
			{
				if (butRoom.nameRoom == filteredRoom.name)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				list.Add(butRoom);
			}
		}
		foreach (butScrollRoom item in list)
		{
			Object.Destroy(item.gameObject);
			butRoomList.Remove(item);
		}
		list.Clear();
	}

	private bool sozdanaLiButRoom(string nameRoomForProv)
	{
		foreach (butScrollRoom butRoom in butRoomList)
		{
			if (butRoom.nameRoom == nameRoomForProv)
			{
				return true;
			}
		}
		return false;
	}

	public void OnReceivedRoomList()
	{
	}

	private IEnumerator MoveToGameScene()
	{
		while (PhotonNetwork.room == null)
		{
			yield return 0;
		}
		if (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers)
		{
			controllerMenu.thisScript.showMessage("This room is full. Please, try another room!");
			yield break;
		}
		Save.SaveString(settings.keyRoomName, PhotonNetwork.room.name);
		Save.SaveString(settings.keyRoomPass, PhotonNetwork.room.customProperties["pass"].ToString());
		PhotonNetwork.isMessageQueueRunning = false;
		yield return Application.LoadLevelAsync("LoadingGame");
	}

	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
	}

	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		controllerMenu.thisScript.hideIndicator();
	}
}
