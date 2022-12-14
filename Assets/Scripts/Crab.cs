using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Crab : MonoBehaviourPunCallbacks, IPunObservable
{
    //public static GameObject _target;
    [SerializeField] private AudioSource _crabAudioSource;
    [SerializeField] private AudioClip _crabAttackAudio;
    [SerializeField] private AudioClip _crabWalkAudio;

    [SerializeField] private float _speed = 1;
    [SerializeField] private int _timerAttack;

    private PhotonView _view;
    private Transform _target;
    private List<Player> Targets = new List<Player>();
    private Animator _crabAnimator;

    private int _timer;
    private bool _inRadiusAttack = false;

    private int _playerCounter;

    private void Start()
    {
        _view = GetComponent<PhotonView>();
        //_target = FindObjectOfType<Player>().transform;
        foreach (Player target in Resources.FindObjectsOfTypeAll(typeof(Player)) as Player[])
        {
            Targets.Add(target);
            if (target.name == "character")
            {
                Targets.Remove(target);
            }

            Debug.Log("????? ??????: " + target.name);
        }
        _playerCounter = UnityEngine.Random.Range(0, Targets.Count);
        _target = Targets[_playerCounter].transform;
        _crabAnimator = GetComponent<Animator>();
        _crabAnimator.SetTrigger("Walk_Cycle_1");
    }

    private void FixedUpdate()
    {
        //if (_view.IsMine)
        {
            if (!_inRadiusAttack)
            {
                //_crabAnimator.SetTrigger("Walk_Cycle_1");
                transform.LookAt(_target);
                transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
                //_crabAudioSource.PlayOneShot(_crabWalkAudio);
                if (_timer < _timerAttack) _timer++;

            }
            else
            {
                Attack();
            }
        }
    }

   
    private void Attack()
    {
        if (_timer == _timerAttack)
        {
            Debug.Log("Crabs attacks");
            _crabAudioSource.PlayOneShot(_crabAttackAudio);
            _crabAnimator.SetTrigger("Attack_3");
            _timer = 0;
            Player._underAttack = true;
            //Player._health--;
        }
        else
        {
            _timer++;
        } 
            
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _inRadiusAttack = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _inRadiusAttack = false;
            _crabAnimator.SetTrigger("Walk_Cycle_1");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
