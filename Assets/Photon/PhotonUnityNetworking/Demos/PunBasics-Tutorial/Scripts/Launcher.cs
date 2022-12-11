using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;

namespace Photon.Pun.Demo.PunBasics
{
	#pragma warning disable 649

	public class Launcher : MonoBehaviourPunCallbacks
    {

		#region Private Serializable Fields

		[Tooltip("The Ui Panel to let the user enter name, connect and play")]
		[SerializeField]
		private GameObject controlPanel;

		[Tooltip("The Ui Text to inform the user about the connection progress")]
		[SerializeField]
		private Text feedbackText;

		[Tooltip("The maximum number of players per room")]
		[SerializeField]
		private byte maxPlayersPerRoom = 4;

		[Tooltip("The UI Loader Anime")]
		[SerializeField]
		private LoaderAnime loaderAnime;

		[SerializeField] private GameObject _startButton;
		#endregion

		[SerializeField] private AudioClip _audioButton;

		private AudioSource _audio;

		#region Private Fields
		bool isConnecting;

		string gameVersion = "1";

		#endregion

		#region MonoBehaviour CallBacks

		void Awake()
		{
			_audio = GetComponent<AudioSource>();
			if (loaderAnime==null)
			{
				Debug.LogError("<Color=Red><b>Missing</b></Color> loaderAnime Reference.",this);
			}

			//GameManager.Instance.LeaveRoom();
			//Disonnect();
			PhotonNetwork.AutomaticallySyncScene = true;

			//PhotonNetwork.JoinLobby();

			if (PhotonNetwork.IsMasterClient) Debug.Log("IsMasterClient");
			Debug.Log("Статус клиента: " + PhotonNetwork.NetworkClientState);	

			PhotonNetwork.AutomaticallySyncScene = true;

			PhotonNetwork.JoinLobby();
		}

		#endregion


		#region Public Methods

		public void Connect()
		{
			Debug.Log("Статус клиента: " + PhotonNetwork.NetworkClientState);

			feedbackText.text = "";

			isConnecting = true;

			controlPanel.SetActive(false);

			if (loaderAnime!=null)
			{
				loaderAnime.StartLoaderAnimation();
			}

			
			if (PhotonNetwork.IsConnected)
			{
				LogFeedback("Joining Room...");
				Debug.Log("Joining Room...");
				PhotonNetwork.JoinRandomRoom();
			}else{

				LogFeedback("Connecting...");
				Debug.Log("Connecting...");

				PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = this.gameVersion;
			}
		}

		void LogFeedback(string message)
		{
			// we do not assume there is a feedbackText defined.
			if (feedbackText == null) {
				return;
			}

			// add new messages as a new line and at the bottom of the log.
			feedbackText.text += System.Environment.NewLine+message;
		}

        #endregion


        #region MonoBehaviourPunCallbacks CallBacks
        public override void OnConnectedToMaster()
		{
			// we don't want to do anything if we are not attempting to join a room. 
			// this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
			// we don't want to do anything.
			if (isConnecting)
			{
				LogFeedback("OnConnectedToMaster: Next -> try to Join Random Room");
				Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room.\n Calling: PhotonNetwork.JoinRandomRoom(); Operation will fail if no room found");
		
				// #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
				PhotonNetwork.JoinRandomRoom();
			}
			else
            {
				Debug.Log("Complete OnConnectedToMaster!!!");
				PhotonNetwork.JoinLobby();
			}
		}

		public override void OnJoinRandomFailed(short returnCode, string message)
		{
			LogFeedback("<Color=Red>OnJoinRandomFailed</Color>: Next -> Create a new Room");
			Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

			PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this.maxPlayersPerRoom});
		}

		public void Disonnect()
		{
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Disconnect();
				Debug.Log("Connect to room is " + PhotonNetwork.InRoom);
				Debug.Log("Connect to lobby is " + PhotonNetwork.InLobby);
				Debug.Log("Connection has status " + PhotonNetwork.NetworkClientState);
			}
			else
			{
				Debug.Log("You are have not active connect");
			}
		}

		public override void OnDisconnected(DisconnectCause cause)
		{
			LogFeedback("<Color=Red>OnDisconnected</Color> "+cause);
			Debug.LogError("PUN Basics Tutorial/Launcher:Disconnected");

			loaderAnime.StopLoaderAnimation();

			isConnecting = false;
			controlPanel.SetActive(true);
		}

		public override void OnJoinedRoom()
		{
			LogFeedback("<Color=Green>OnJoinedRoom</Color> with "+PhotonNetwork.CurrentRoom.PlayerCount+" Player(s)");
			Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.\nFrom here on, your game would be running.");
			if (PhotonNetwork.IsMasterClient)
			{
				_startButton.SetActive(true);
			}
			if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
			{
				Debug.Log("We load the 'Room for 1' ");
			}
		}

		public void LoadGame()
        {
			PhotonNetwork.LoadLevel("Game");
			Debug.Log("We load the Game");
		}

		public void SoundButton()
		{
			_audio.PlayOneShot(_audioButton);
		}
		#endregion

	}
}