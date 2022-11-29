using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using System;

public class PlayFabAccountData : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerManager _player;
    private ItemInstance health_potion;
    private float _maxHealthPoints = 1f;

    public static GameObject LocalPlayerInstance;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }

        DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInventorySuccess, OnError);
        //PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnGetUserDataSuccess, OnError);
        _player = GetComponent<PlayerManager>();
    }

    /*private void OnGetUserDataSuccess(GetUserDataResult result)
    {
        foreach (var data in result.Data)
        {
            Debug.Log("data.Key " + data.Key);
            Debug.Log("data.Value " + data.Value);
            if (data.Key == "HealthPoints")
            {
                Debug.Log("_maxHealthPoints: " +  _maxHealthPoints);
                //_maxHealthPoints = data.Value;
                Debug.Log("_maxHealthPoints: " + _maxHealthPoints);
            }
        }
    }*/

    private void OnGetInventorySuccess(GetUserInventoryResult result)
    {
        foreach (var item in result.Inventory)
        {
            Debug.Log("ItemID: " + item.ItemId);
            if (item.ItemId == "health_potion")
            {
                health_potion = item;
            }
        }
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        ShowCatalog(result.Catalog);
        //Debug.Log("Complete load catalog!");
    }

    private void ShowCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            if (item.Bundle == null)
            {
                //Debug.Log("item_id: " + item.ItemId);
            }
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                MakePurchase();
            };
            if (Input.GetKeyDown(KeyCode.E))
            {
                MakeConsume();
            }
        }
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
        result =>
        {
            Debug.Log("Complete purchase");
        }, OnLoginError);
    }

    private void MakeConsume()
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = health_potion.ItemInstanceId,
        },
        result =>
        {

            _player.Health = _maxHealthPoints;
            Debug.Log("Complete consume health_potion");
        }, OnLoginError);
    }

    private void OnLoginError(PlayFabError error)
    {
        Debug.Log("Error: " + error);
    }


    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        //_catalogLabel.text = "Catalog.data: " + File.ReadAllText("Assets/title-1B50D-FirstCatalog.json");
    }


    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_player.Health);
        }
        else
        {
            _player.Health = (float)stream.ReceiveNext();
        }
    }
}
