using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using System;
using System.Collections.Generic;

public class PlayFabLogin : MonoBehaviour
{
    private const string AuthGuidKey = "auth_guid";

    [SerializeField] private TMP_InputField Result;
        
    public void TryToLogin()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = "204F2";

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());

        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = !needCreation
        };

        PlayFabClientAPI.LoginWithCustomID(request, success =>
        {
            PlayerPrefs.SetString(AuthGuidKey, id);
            OnLoginSuccess(success);
        },
            OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Result.image.color = Color.green;
        Debug.Log("Complete!");
        Result.text = "Your connection Success";

        SetUserData(result.PlayFabId);
        MakePurchase();
    }

    private void SetUserData(string playFabId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"time_recieve_daily_revard", DateTime.UtcNow.ToString() },
            }
        },
        result =>
        {
            Debug.Log("Complete update user data");
            GetUserData(playFabId, "time_recieve_daily_revard");
        }, OnLoginError);
    }

    private void GetUserData(string playFabId, string keyData)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = playFabId
        },
        result =>
        {
            Debug.Log(keyData + ": " + result.Data[keyData].Value);
        }, OnLoginError);       
    }

    private void MakePurchase()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "FirstCatalog",
            ItemId = "health_potion",
            Price = 5,
            VirtualCurrency = "SC"
        },
        result=>
        {
            Debug.Log("Complete purchase");
        }, OnLoginError);
    }

    private void OnLoginError(PlayFabError error)
    {
        Result.image.color = Color.red;
        var errorMessage = error.GenerateErrorReport();
        Debug.Log("Error: " + errorMessage);
        Result.text = "Your connection has Error: " + errorMessage;
    }

}
