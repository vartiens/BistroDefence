using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Custom
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class DefaultRoom
{
    public string roomName;
    public int roomIndex;
    public int maxPlayer;
}

public class NetworkManager : MonoBehaviourPunCallbacks
{
    /***** No Use
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }
    *****/

    public List<DefaultRoom> defaultRooms;
    public GameObject roomUI;

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Try Connect To Server...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Server.");
        base.OnConnectedToMaster();
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined the Lobby.");
        roomUI.SetActive(true); 
    }

    public void InitializeRoom(int defaultRoomIndex)
    {
        DefaultRoom roomSetting = defaultRooms[defaultRoomIndex];

        // Load the scene
        PhotonNetwork.LoadLevel(roomSetting.roomIndex);

        // Create the room
        RoomOptions roomOptions = new()
        {
            MaxPlayers = (byte)roomSetting.maxPlayer, // For test (Default = Max: 10)
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.JoinOrCreateRoom(roomSetting.roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined the Room.");
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new Player joined the Room.");
        base.OnPlayerEnteredRoom(newPlayer);
    }

    /***** No Use
    // Update is called once per frame
    void Update()
    {
        
    }
    *****/
}
