using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{   
    public static int _health;
    public int _maxHealth = 5;
    [SerializeField] private GameObject _loseCanvas;
    [SerializeField] private GameObject _menuCanvas;

    public static GameObject LocalPlayerInstance;

    [SerializeField] private GameObject playerUiPrefab;

    private bool _menuState = false;

    private bool leavingRoom;

    [SerializeField] private float Speed = 10f;
    [SerializeField] private float JumpForce = 300f;

    private bool _isGrounded;
    private Rigidbody _rb;

    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

        _rb = GetComponent<Rigidbody>();
        _health = _maxHealth;

        /*if (this.playerUiPrefab != null)
        {
            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
        }*/

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

    }


    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }

    private void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject _uiGo = Instantiate(this.playerUiPrefab);
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnLeftRoom()
    {
        this.leavingRoom = false;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            MovementLogic();
            JumpLogic();
            if (this._maxHealth <= 0f && !this.leavingRoom)
            {
                this.leavingRoom = GameManager.Instance.LeaveRoom();
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuState = !_menuState;
            _menuCanvas.SetActive(_menuState);
        }
        if (_health < 1)
        {
            Debug.Log("PlayerDeath!!!");
            _loseCanvas.SetActive(true);
            Time.timeScale = 0;
        }
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        _rb.AddForce(movement * Speed);
    }

    private void JumpLogic()
    {
        if (Input.GetAxis("Jump") > 0)
        {
            if (_isGrounded)
            {
                _rb.AddForce(Vector3.up * JumpForce);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        IsGroundedUpate(collision, true);
    }

    void OnCollisionExit(Collision collision)
    {
        IsGroundedUpate(collision, false);
    }

    private void IsGroundedUpate(Collision collision, bool value)
    {
        if (collision.gameObject.tag == ("Ground"))
        {
            _isGrounded = value;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(this._maxHealth);
        }
        else
        {
            this._maxHealth = (int)stream.ReceiveNext();
        }
    }
}
