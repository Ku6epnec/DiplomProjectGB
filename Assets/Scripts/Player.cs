using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Linq;

public class Player : MonoBehaviourPunCallbacks, IPunObservable
{   
    public int _health;
    public int _maxHealth = 5;
    public static bool _underAttack;
    [SerializeField] private GameObject _loseCanvas;
    [SerializeField] private GameObject _winCanvas;
    [SerializeField] private GameObject _menuCanvas;
    public string _nick;

    private Animator _boyAnimator;

    public static GameObject LocalPlayerInstance;

    //[SerializeField] private GameObject playerUiPrefab;

    private bool _menuState = false;

    private bool leavingRoom;

    [SerializeField] private float Speed = 10f;
    [SerializeField] private float JumpForce = 300f;

    [SerializeField] private float _timer;
    [SerializeField] private GameObject _crabPrefab;

    [SerializeField] private float _randomSpawnRange = 15;

    private Transform _spawnPoint;

    private float _spawnTime = 0;

    private bool _isGrounded;
    private Rigidbody _rb;

    private ItemInstance _jumpersInstance;

    private float _jumpersCooldown = 200.0f;
    private float _jumpersTime = 200.0f;

    private void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => OnGetInventorySuccess(result.Inventory), OnError);
    }

    private void OnGetInventorySuccess(List<ItemInstance> item)
    {
        _jumpersInstance = item.First();
    }

    public void GruntVirtualCurrency()
    {
        var request = new AddUserVirtualCurrencyRequest
        {
            VirtualCurrency = "GC",
            Amount = 30
        };
        PlayFabClientAPI.AddUserVirtualCurrency(request, OnGrantVirtualCurrencySuccess, OnError);
    }

    void UseJumpers()
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = _jumpersInstance.ItemInstanceId
        }, OnUseJumpersSuccess, OnUseJumpersError);
    }

    private void OnUseJumpersSuccess(ConsumeItemResult obj)
    {
        Debug.Log("Use 1 Jumper!");
        _rb.AddForce(Vector3.up * JumpForce);
    }

    private void OnUseJumpersError(PlayFabError obj)
    {
        Debug.Log("Error use Jumpers: " + obj);      
    }
    private void OnError(PlayFabError obj)
    {
        Debug.Log("Error Grunt Currency!");
    }

    private void OnGrantVirtualCurrencySuccess(ModifyUserVirtualCurrencyResult obj)
    {
        Debug.Log("Currency Grunted!");
    }

    public void Awake()
    {
        if (photonView.IsMine)
        {
            GetInventory();
            LocalPlayerInstance = gameObject;
        }
        //_spawnPoint = FindObjectOfType<SpawnPoint>().transform;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            name = PlayFabAccountManager._characterName;
            _nick = name;
        }
        Debug.Log("Player name = " + name);
        _rb = GetComponent<Rigidbody>();
        _health = _maxHealth;

        _boyAnimator = GetComponent<Animator>();
        _boyAnimator.SetTrigger("Rest1");

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

    public void LoseHealth()
    {
        _health--;
    }

    private void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        //GameObject _uiGo = Instantiate(this.playerUiPrefab);
        //_uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public override void OnLeftRoom()
    {
        leavingRoom = false;
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            if (_jumpersTime < _jumpersCooldown) _jumpersTime++;
            //SpawnerEnemy();
            MovementLogic();
            if (_jumpersTime >= _jumpersCooldown)
            JumpLogic();
            if (!_isGrounded)
            {
                _boyAnimator.SetTrigger("Jump");
            }
            if (this._maxHealth <= 0f && !this.leavingRoom)
            {
                this.leavingRoom = GameManager.Instance.LeaveRoom();
            }
        }
    }


    private void SpawnerEnemy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_spawnTime == _timer)
            {
                Vector3 pos = new Vector3(Random.Range(-_randomSpawnRange, _randomSpawnRange),
                    1, Random.Range(-_randomSpawnRange, _randomSpawnRange));
                Instantiate<GameObject>(_crabPrefab, pos, Quaternion.identity, _spawnPoint);
                Debug.Log("Spawn Crab");
                _spawnTime = 0;
            }
            else
            {
                _spawnTime++;
            }
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            name = _nick;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuState = !_menuState;
            _menuCanvas.SetActive(_menuState);
        }
        if (_health == 0)
        {
            Debug.Log("PlayerDeath!!! nick: " + _nick);

            Time.timeScale = 0;

            _health--;

            if (photonView.IsMine)
            {
                _loseCanvas.SetActive(true);
            }
            else
            {
                GruntVirtualCurrency();
                _winCanvas.SetActive(true);
            }

        }
    }

    private void MovementLogic()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");

        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        if (movement != null && _isGrounded) _boyAnimator.SetTrigger("Run");

        _rb.AddForce(movement * Speed);
    }

    private void JumpLogic()
    {   
        if (Input.GetAxis("Jump") > 0)
        {
            if (_isGrounded)
            {
                _jumpersTime = 0;
                UseJumpers();
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

        public void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Enemy") && _underAttack)
            {
                this._health--;
                _underAttack = false;
                Debug.Log("Player is underAttack OnTriggerStay");
                Debug.Log("Player with ID = " + photonView.InstantiationId + " has Health = " + _health);
        }
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
            stream.SendNext(_health);
            stream.SendNext(_nick);
        }
        else
        {
            this._maxHealth = (int)stream.ReceiveNext();
            _health = (int)stream.ReceiveNext();
            _nick = (string)stream.ReceiveNext();
        }
    }
}
