using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class CreateAccountWindow : AccountDataWindowBase
{
    [SerializeField] private InputField _emailField;
    [SerializeField] private Button _createAccountButton;

    [SerializeField] private Button _returnButton;

    [SerializeField] private Canvas _enterInGameCanvas;
    [SerializeField] private Canvas _createAccountCanvas;

    protected string _email;

    protected override void SubscriptionsElementsUi()
    {
        base.SubscriptionsElementsUi();

        _emailField.onValueChanged.AddListener(UpdateEmail);
        _createAccountButton.onClick.AddListener(CreateAccount);
        _returnButton.onClick.AddListener(Return);
    }

    private void OnDestroy()
    {
        _emailField.onValueChanged.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();
        _returnButton.onClick.RemoveAllListeners();
    }

    private void UpdateEmail(string email)
    {
        _email = email;
    }

    private void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _email,
            Password = _password
        }, result =>
        {
        Debug.Log("Success: " + _username);
        }, error =>
        {
        Debug.Log("Fail: " + error.ErrorMessage);
        });
    }

    private void Return()
    {
        _enterInGameCanvas.enabled = true;
        _createAccountCanvas.enabled = false;
    }
}
