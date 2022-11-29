using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using UnityEngine.UI;
using System;

public class ConnectAndJoinRoom : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, ILobbyCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_Text _stateUiText;
    [SerializeField] private TMP_Text _statusRoomText;
    [SerializeField] private Button _openCloseRoomButton;
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _createRandomRoomButton;
    [SerializeField] private Button _createNewRoomButton;
    [SerializeField] private Button _connectRoomButton;
    [SerializeField] private Button _leaveRoomButton;
    [SerializeField] private Button _refreshRoomListButton;
    [SerializeField] private TMP_InputField _openCloseRoomInputField;

    [SerializeField] private GameObject _canvasControl;

    [SerializeField] private GameObject _gameManager;

    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private GameObject _roomView;
    [SerializeField] private RoomWidget _room;

    [SerializeField] private TMP_InputField _roomName;

    private string[] _friendList;

    private const string openText = "Open";
    private const string closeText = "Close";

    private const string GAME_MOD_KEY = "gm";
    private const string AI_MOD_KEY = "ai";

    private const string MONEY_PROPERTY_KEY = "C0";
    private const string MAP_PROPERTY_KEY = "C1";

    private TypedLobby _sqlLobby = new TypedLobby("sqlLobby", LobbyType.SqlLobby);
    private TypedLobby _asyncRandomLobby = new TypedLobby("asyncRandomLobby", LobbyType.AsyncRandomLobby);

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public List<RoomInfo> _roomList = new List<RoomInfo>();

    private List<RoomWidget> _roomWidgetList = new List<RoomWidget>();
    Action<List<RoomInfo>> callback = null;

    bool isConnecting;
    private LoadBalancingClient _loadBalancingClient;

    private RoomInfo _choosenRoom;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        _openCloseRoomButton.onClick.AddListener(CloseRoom);
        _startGameButton.onClick.AddListener(StartGame);
        _createNewRoomButton.onClick.AddListener(CreatedNewRoom);
        _leaveRoomButton.onClick.AddListener(LeaveRoom);
        _connectRoomButton.onClick.AddListener(OnConnectedTestRoom);
        _refreshRoomListButton.onClick.AddListener(RefreshRoomList);

        _loadBalancingClient = new LoadBalancingClient();
        _loadBalancingClient.AddCallbackTarget(this);

        if (!_loadBalancingClient.ConnectUsingSettings(_serverSettings.AppSettings))
        {
            Debug.LogError("Error connections");
        }
    }

    private void LeaveRoom()
    {
        _loadBalancingClient.OpLeaveRoom(true, true);
    }

    private void RefreshRoomList()
    {
        Debug.Log("_loadBalancingClient.InRoom: " + _loadBalancingClient.InRoom);
        Debug.Log("_loadBalancingClient.RoomsCount: " + _loadBalancingClient.RoomsCount);
        //OnRoomListUpdate(_roomList);
    }

    public void ConnectToRoomWithParams()
    {
        _loadBalancingClient.OpJoinRandomRoom();
        Debug.Log("Попытка зайти в комнату: " + PhotonNetwork.CurrentRoom.Name);
        Debug.Log(PhotonNetwork.IsConnected);
    }

    private void OnChooseRoom(int roomId)
    {
        _choosenRoom = _roomList[roomId];
    }

    private void CloseRoom()
    {
        _loadBalancingClient.CurrentRoom.IsOpen = !_loadBalancingClient.CurrentRoom.IsOpen;
        Debug.Log("Status room: " + _loadBalancingClient.CurrentRoom.IsOpen);
        CheckStatusRoom();
    }

    private void CheckStatusRoom()
    {
        if (_loadBalancingClient.CurrentRoom.IsOpen)
        {
            _openCloseRoomInputField.text = "Room " + openText;
            _openCloseRoomInputField.image.color = Color.green;
            _statusRoomText.text = closeText;
        }
        else
        {
            _openCloseRoomInputField.text = "Room " + closeText;
            _openCloseRoomInputField.image.color = Color.yellow;
            _statusRoomText.text = openText;
        }    
    }

    private void OnDestroy()
    {
        _loadBalancingClient.RemoveCallbackTarget(this);
        _openCloseRoomButton.onClick.RemoveAllListeners();
        _startGameButton.onClick.RemoveAllListeners();
        _createRandomRoomButton.onClick.RemoveAllListeners();
        _leaveRoomButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (_loadBalancingClient == null)
            return;

        _loadBalancingClient.Service();

        var state = _loadBalancingClient.State.ToString();
        _stateUiText.text = "State: " + state + ", userId" + _loadBalancingClient.UserId;
    }

    public void OnConnectedTestRoom()
    {
        isConnecting = true;
        _canvasControl.SetActive(false);

        Debug.Log("Try Connect");
        _loadBalancingClient.OpJoinRandomRoom();
    }

    public void OnConnected()
    {
        Debug.Log("OnConnected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        _loadBalancingClient.OpJoinLobby(_sqlLobby);
        OnRoomListUpdate(_roomList);
        Debug.Log("Подключились к лобби: " + _loadBalancingClient.CurrentLobby);
    }

    private void JoinTestRoom()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 12,
            CustomRoomProperties = new Hashtable { { MONEY_PROPERTY_KEY, 400 }, { MAP_PROPERTY_KEY, "Map_3" } },
            CustomRoomPropertiesForLobby = new[] { MONEY_PROPERTY_KEY, MAP_PROPERTY_KEY },
            IsVisible = true,
            PublishUserId = true
        };
        var enterRoomParams = new EnterRoomParams
        {
            RoomName = "Game Room",
            RoomOptions = roomOptions,
            Lobby = _sqlLobby,
            ExpectedUsers = new[] { "111", "222", "333", "444" }
        };
        PhotonNetwork.ConnectUsingSettings();
        _loadBalancingClient.OpCreateRoom(enterRoomParams);
    }

    public void CreatedNewRoom()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 12,
            CustomRoomProperties = new Hashtable { { MONEY_PROPERTY_KEY, 400 }, { MAP_PROPERTY_KEY, "Map_3" } },
            CustomRoomPropertiesForLobby = new[] { MONEY_PROPERTY_KEY, MAP_PROPERTY_KEY },
            IsVisible = true,
            PublishUserId = true
        };

        var enterRoomParams = new EnterRoomParams
        {
            RoomName = _roomName.text,
            RoomOptions = roomOptions,
            Lobby = _sqlLobby,
            ExpectedUsers = new[] { "111", "222", "333", "444" }
        };

        Debug.Log("Try Create new ROOM!");
        _loadBalancingClient.OpCreateRoom(enterRoomParams);

    }

    public void OnCreatedRoom()
    {
        Debug.Log("Created room");
        _roomList.Add(_loadBalancingClient.CurrentRoom);
        cachedRoomList.Add(_roomList[_roomList.Count].Name, _roomList[_roomList.Count]);
        OnRoomListUpdate(_roomList);
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create room Failed");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log("OnCustomAuthenticationFailed");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
        
    }

    public void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
        _canvasControl.SetActive(true);
    }

    public void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    {
        Debug.Log("OnFriendListUpdate");
    }

    public void OnJoinedLobby()
    {
        _loadBalancingClient.OpFindFriends(_friendList);
        cachedRoomList.Clear();
        var sqlLobbyFilter = MONEY_PROPERTY_KEY + " BETWEEN 300 AND 500 AND " + MAP_PROPERTY_KEY + "= 'Map_3'";
        var opJoinRandomRoomParams = new OpJoinRandomRoomParams
        {
            SqlLobbyFilter = sqlLobbyFilter
        };
        //_loadBalancingClient.OpJoinRandomRoom(opJoinRandomRoomParams);
        Debug.Log("Joined Lobby");
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        Debug.Log(_loadBalancingClient.CurrentRoom);
        Debug.Log("Status room: " + _loadBalancingClient.CurrentRoom);
        CheckStatusRoom();
        Debug.Log("CurrentRoom status: " + PhotonNetwork.CurrentRoom);
        Instantiate(_playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join random room Failed");
        JoinTestRoom();
        //_loadBalancingClient.OpCreateRoom(new EnterRoomParams());
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
    }

    public void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }
        
    public void OnLeftRoom()
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("OnLobbyStatisticsUpdate");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("OnRegionListReceived");
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list Update");
        foreach (RoomInfo room in roomList)
        {
            Debug.Log("RoomInfo room in roomList");
            if (room.RemovedFromList)
            {
                int index = _roomWidgetList.FindIndex(x => x._roomInfo.Name == room.Name);
                if (index != -1)
                {
                    Destroy(_roomWidgetList[index].gameObject);
                    _roomWidgetList.RemoveAt(index);
                }
            }
            else
            {
                Debug.Log("_loadBalancingClient.CurrentRoom: " + _roomName.text);
                RoomWidget NewRoom = Instantiate(_room, _roomView.transform);
                NewRoom._buttonConnectRoom.onClick.AddListener(ConnectToRoomWithParams);
                NewRoom.name = _roomName.text;
                if (NewRoom != null)
                {
                    NewRoom.AddRoomInfo(true, room);
                    _roomWidgetList.Add(NewRoom);
                }
            }
            //UpdateCachedRoomList(roomList);
        }
    }

    private void StartGame()
    {
        if (_loadBalancingClient.CurrentRoom.IsOpen)
        {
            Debug.Log("You need Close Room before start");
        }
        else
        {
            Debug.Log("Game Begin!!!");
        }
    }
}
