using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _catalogLabel = null;
    [SerializeField] private Slider _sliderLoadingProcess;
    [SerializeField] private TMP_Text _loadingValue;
    [SerializeField] private Image _imageEndLoading;

    [SerializeField] private Button _fightCharacterButton;
    //[SerializeField] TMP_InputField _inputField;
    [SerializeField] private GameObject _conteinerLayout;
    [SerializeField] private TMP_Text _nickName;

    public static string _characterName;

    private int _endTimer = 100;
    private bool _timerStatus = true;

    private int _minXP = 5;
    private int _maxXP = 200;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        //LoadingTimer();
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
;
        _fightCharacterButton.onClick.AddListener(StartFightCharacter);
        //_inputField.onValueChanged.AddListener(OnNameChange);
         
    }

    private void StartFightCharacter()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
           result =>
           {
               Debug.Log("Characters count: " + result.Characters.Count);
               for (int characterNumber = 0; characterNumber < result.Characters.Count; characterNumber++)
               {
                   Debug.Log("CharacterNumber: " + characterNumber);
                   Debug.Log("characters[characterNumber].CharacterId: " + result.Characters[characterNumber].CharacterId);
               }
           }, OnError);
    }

    private void UpdateCharacterXP(CharacterResult character, Dictionary<string, int> characterStatistics)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = character.CharacterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", characterStatistics["Level"]},
                {"XP", characterStatistics["XP"] + Random.Range(_minXP, _maxXP)},
                {"Gold", characterStatistics["Gold"]}
            }
        }, result =>
        {
            foreach (var characterStatistic in characterStatistics)
            {
                if (characterStatistic.Key == "XP")
                {
                    Debug.Log("У персонажа: " + character + " итоговый опыт: " + characterStatistic);
                }
            }

        }, OnError);
    }

    private void OnNameChange(string characterName)
    {
        _characterName = characterName;
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        ShowCatalog(result.Catalog);
        Debug.Log("Complete load catalog!");
    }

    private void ShowCatalog(List<CatalogItem> catalog)
    {
        foreach (var item in catalog)
        {
            if (item.Bundle == null)
            {
                Debug.Log("item_id: " + item.ItemId);
                _catalogLabel.text += "item_id: " + item.ItemId + "\n";
                _catalogLabel.text += "item_id: " + item.IsStackable + "\n";
            }
        }
    }

    private void Update()
    {
        //LoadingTimer();
        //string aba = PlayFabClientAPI.UpdateUserTitleDisplayName();
        //string name = result.InfoResultPayload.PlayerProfile.DisplayName;
    }

    private void ChangeAccount()
    {
        SceneManager.LoadScene(0);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _titleLabel.text = "Welcome " + accountInfo.Username + "\n" + accountInfo.PlayFabId;
        _nickName.text = accountInfo.Username;
        _characterName = accountInfo.Username;
        _sliderLoadingProcess.value = _endTimer; 
        _timerStatus = false;
        _imageEndLoading.color = Color.magenta;
        //_catalogLabel.text = "Catalog.data: " + File.ReadAllText("Assets/title-1B50D-FirstCatalog.json");
    }


    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        //Debug.LogError(errorMessage);
        _sliderLoadingProcess.value = _endTimer;
        _timerStatus = false;
        _imageEndLoading.color = Color.red;
    }

    private void LoadingTimer()
    {
        if (_timerStatus)
        {
            _sliderLoadingProcess.value += 0.3f;
            _loadingValue.text = "Loading " + Mathf.Round(_sliderLoadingProcess.value) + "%";
        }
        else
        {
            _loadingValue.text = "Loading " + Mathf.Round(_sliderLoadingProcess.value) + "%";
        }
    }
}
