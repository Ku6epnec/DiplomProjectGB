using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonLauncer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField Result;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Result.image.color = Color.yellow;
        Result.text = "Connection has status " + PhotonNetwork.NetworkClientState;
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
            Debug.Log("Connection has status " + PhotonNetwork.NetworkClientState);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = Application.version;
            Debug.Log("Connection has status " + PhotonNetwork.NetworkClientState);
        }        
    }

    public void Disonnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Connect to room is " + PhotonNetwork.InRoom);
            Debug.Log("Connect to lobby is " + PhotonNetwork.InLobby);
            Debug.Log("Connection has status " + PhotonNetwork.NetworkClientState);

            Result.image.color = Color.red;
            Result.text = "Connection has status " + PhotonNetwork.NetworkClientState;
        }
        else
        {
            Debug.Log("You are have not active connect");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Complete OnConnectedToMaster!!!");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Connect to lobby is " + PhotonNetwork.InLobby);
        PhotonNetwork.JoinOrCreateRoom("RoomName", new RoomOptions { MaxPlayers = 5, IsVisible = true }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Connect to room is " + PhotonNetwork.InRoom);
        Result.image.color = Color.green;
        Result.text = "Connection has status " + PhotonNetwork.NetworkClientState;
    }
}
