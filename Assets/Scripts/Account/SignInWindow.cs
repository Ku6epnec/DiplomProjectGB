using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class SignInWindow : AccountDataWindowBase
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _returnButton;

    [SerializeField] private Canvas _enterInGameCanvas;
    [SerializeField] private Canvas _signInCanvas;

    protected override void SubscriptionsElementsUi()
    {
        base.SubscriptionsElementsUi();

        _signInButton.onClick.AddListener(SignIn);
        _returnButton.onClick.AddListener(Return);
    }

    private void OnDestroy()
    {
        _signInButton.onClick.RemoveAllListeners();
        _returnButton.onClick.RemoveAllListeners();
    }

    private void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _password
        }, result =>
        {
            Debug.Log("Success: " + _username);
            EnterInGameScene();
        }, error =>
        {
            Debug.Log("Fail: " + error.ErrorMessage);
        });
    }

    private void Return()
    {
        _enterInGameCanvas.enabled = true;
        _signInCanvas.enabled = false;
    }
}
