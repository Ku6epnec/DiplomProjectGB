using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private Button _buttonStart;

    public Button _buttonConnectRoom => _buttonStart;

    void Start()
    {
        _buttonStart.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
