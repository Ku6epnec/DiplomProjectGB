using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using Photon.Pun;


#pragma warning disable 649

public class NetworkManager : MonoBehaviourPunCallbacks
	{

		#region Public Fields

		static public NetworkManager Instance;

		#endregion

		#region Private Fields

		private GameObject instance;

		[Tooltip("The prefab to use for representing the player")]
		[SerializeField]
		private GameObject playerPrefab;

		#endregion

		#region MonoBehaviour CallBacks

		void Start()
		{
			Instance = this;

			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("Hub");

				return;
			}

			
			Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
			//PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
			Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);

			/*if (playerPrefab == null)
			{ 
				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			}
			else
			{
			Debug.Log("Local player instance: " + Player.LocalPlayerInstance);
				if (Player.LocalPlayerInstance == null)
				{
					Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
					Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity);
					//PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity);
					PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
				}
				else
				{
					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}
			}*/

		}

	void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}

		#endregion

		#region Photon Callbacks

		public override void OnLeftRoom()
		{
			SceneManager.LoadScene("PunBasics-Launcher");
		}

		#endregion

		#region Public Methods

		public bool LeaveRoom()
		{
			return PhotonNetwork.LeaveRoom();
		}

		public void QuitApplication()
		{
			Application.Quit();
		}

		#endregion

		#region Private Methods

		void LoadArena()
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
			}

			Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

			PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
		}

		#endregion

	}
