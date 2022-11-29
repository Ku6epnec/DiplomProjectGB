using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float _timer;
    [SerializeField] private GameObject _crabPrefab;

    [SerializeField] private float _randomSpawnRange = 15;

    private float _spawnTime = 0;
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (_spawnTime == _timer)
        {
            Vector3 pos = new Vector3(Random.Range(transform.position.x - _randomSpawnRange, transform.position.x + _randomSpawnRange),
                transform.position.y, Random.Range(transform.position.z - _randomSpawnRange, transform.position.z + _randomSpawnRange));
            Instantiate<GameObject>(_crabPrefab, pos, Quaternion.identity, transform);
            Debug.Log("Spawn Crab");
            _spawnTime = 0;
        }
        else
        {
            _spawnTime++;
        }
    }
}
