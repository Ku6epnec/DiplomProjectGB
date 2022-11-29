using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AccountDataWindowBase : MonoBehaviour
{
    [SerializeField] private InputField _usernameField;
    [SerializeField] private InputField _passwordField;

    protected string _username;
    protected string _password;

    private void Start()
    {
        SubscriptionsElementsUi();
    }

    private void Update()
    {
        
    }

    protected virtual void SubscriptionsElementsUi()
    {
        _usernameField.onValueChanged.AddListener(UpdateUsername);
        _passwordField.onValueChanged.AddListener(UpdatePassword);
    }

    private void UpdateUsername(string username)
    {
        _username = username;
    }

    private void UpdatePassword(string password)
    {
        _password = password;
    }

    protected void EnterInGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
