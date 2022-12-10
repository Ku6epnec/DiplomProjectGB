using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class Spawner : MonoBehaviourPunCallbacks//, IPunObservable
{
    [SerializeField] private float _timer;
    [SerializeField] private float _timerSmall;
    [SerializeField] private GameObject _crabPrefab;
    [SerializeField] private GameObject _crabPrefabSmall;

    [SerializeField] private float _randomSpawnRange = 15;

    private PhotonView _view;

    private float _spawnTime = 0;

    void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        //if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        //if (_view.IsMine)
        
            //SpawnEnemy();
        
    }

    private void SpawnEnemy()
    {
        if (_spawnTime == _timer)
        {
            Vector3 pos = new Vector3(Random.Range(-_randomSpawnRange, _randomSpawnRange),
                1, Random.Range(-_randomSpawnRange, _randomSpawnRange));
            Instantiate<GameObject>(_crabPrefab, pos, Quaternion.identity, transform);
            Debug.Log("Spawn Crab");
            _spawnTime = 0;
        }
        else if (_spawnTime == _timerSmall)
        {
            Vector3 pos = new Vector3(Random.Range(-_randomSpawnRange, _randomSpawnRange),
                1, Random.Range(-_randomSpawnRange, _randomSpawnRange));
            Instantiate<GameObject>(_crabPrefabSmall, pos, Quaternion.identity, transform);
            Debug.Log("Spawn small Crab");
        }
        if (_spawnTime < _timer)
        {
            _spawnTime++;
        }
    }

    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_crabPrefab);
        }
        else
        {
            _crabPrefab = (GameObject)stream.ReceiveNext();
        }
    }*/
}
