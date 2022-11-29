using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{   
    public static int _health;
    [SerializeField] private int _maxHealth = 5;
    [SerializeField] private GameObject _loseCanvas;
    [SerializeField] private GameObject _menuCanvas;

    private bool _menuState = false;

    private void Start()
    {
        _health = _maxHealth;
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
}
