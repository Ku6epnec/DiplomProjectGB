using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    private Transform _target;
    [SerializeField] private float _speed = 1;
    [SerializeField] private int _timerAttack;
    [SerializeField] private AudioSource _crabAudioSource;
    [SerializeField] private AudioClip _crabAttackAudio;

    //[SerializeField] private Animator _crabAnimator;

    private int _timer;
    private bool _inRadiusAttack = false;
    private void Start()
    {
        _target = FindObjectOfType<PlayerInput>().transform;
    }
    private void FixedUpdate()
    {
        if (!_inRadiusAttack)
        {
            transform.LookAt(_target);
            transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);
            if (_timer < _timerAttack) _timer++;
        }
        else
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (_timer == _timerAttack)
        {
            Debug.Log("Crabs attacks");
            _crabAudioSource.PlayOneShot(_crabAttackAudio);
            _timer = 0;
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
        }
    }
}
