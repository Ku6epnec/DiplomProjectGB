using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crab : MonoBehaviour
{
    [SerializeField] private AudioSource _crabAudioSource;
    [SerializeField] private AudioClip _crabAttackAudio;
    [SerializeField] private AudioClip _crabWalkAudio;

    [SerializeField] private float _speed = 1;
    [SerializeField] private int _timerAttack;

    private Transform _target;
    private Animator _crabAnimator;

    private int _timer;
    private bool _inRadiusAttack = false;

    private void Start()
    {
        _target = FindObjectOfType<PlayerInput>().transform;
        _crabAnimator = GetComponent<Animator>();
        _crabAnimator.SetTrigger("Walk_Cycle_1");
    }

    private void FixedUpdate()
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

    private void Attack()
    {
        if (_timer == _timerAttack)
        {
            Debug.Log("Crabs attacks");
            _crabAudioSource.PlayOneShot(_crabAttackAudio);
            _crabAnimator.SetTrigger("Attack_3");
            _timer = 0;
            Player._health--;
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
}
