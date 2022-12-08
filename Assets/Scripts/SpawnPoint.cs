using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class SpawnPoint : MonoBehaviourPunCallbacks
{
    private PhotonView _view;

    void Start()
    {
        _view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
