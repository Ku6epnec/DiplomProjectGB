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
    [SerializeField] private Button _changeAccountButton;
    [SerializeField] private Slider _sliderLoadingProcess;
    [SerializeField] private TMP_Text _loadingValue;
    [SerializeField] private Image _imageEndLoading;

    [SerializeField] private GameObject _newCharacterCreatePanel;
    [SerializeField] private Button _createCharacterButton;
    [SerializeField] private Button _openCreateCharacterPanelButton;
    [SerializeField] private Button _fightCharacterButton;
    [SerializeField] TMP_InputField _inputField;
    [SerializeField] private SlotCharacterWidget _characterSlot;
    [SerializeField] private GameObject _conteinerLayout;
    private List<SlotCharacterWidget> _slots = new List<SlotCharacterWidget>();

    private string _characterName;

    private int _endTimer = 100;
    private bool _timerStatus = true;

    private int _minXP = 5;
    private int _maxXP = 200;

    private string _level;
    private string _xp;
    private string _gold;

    private void Start()
    {
        LoadingTimer();
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnError);
        _changeAccountButton.onClick.AddListener(ChangeAccount);
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);

        _createCharacterButton.onClick.AddListener(CreateCharacterWithItem);
        _openCreateCharacterPanelButton.onClick.AddListener(OpenPanelCreateCharacter);
        _fightCharacterButton.onClick.AddListener(StartFightCharacter);
        _inputField.onValueChanged.AddListener(OnNameChange);

        GenerateCharacterList();             
    }

    private void GenerateCharacterList()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
               result =>
               {
                   Debug.Log("Characters count: " + result.Characters.Count);
                   WithdrawCharacters(result.Characters);
               }, OnError);
    }

    private void WithdrawCharacters(List<CharacterResult> characters)
    {
        Debug.Log("Всего персонажей! " + characters.Count);
        Debug.Log("Всего слотов было! " + _slots.Count);
        CreateSlotsList(characters);
        for (int characterNumber = 0; characterNumber < characters.Count; characterNumber++)
        {
            Debug.Log("CharacterNumber: " + characterNumber);
            Debug.Log("characters[characterNumber].CharacterId: " + characters[characterNumber].CharacterId);
            DrawCharacter(characters, characterNumber);
        }
        Debug.Log("Всего слотов стало! " + _slots.Count);
    }

    private void CreateSlotsList(List<CharacterResult> characters)
    {
        for (int characterNumber = 0; characterNumber < characters.Count + 1; characterNumber++)
        {
            _slots.Add(_characterSlot);
        }
    }

    private void DrawCharacter(List<CharacterResult> characters, int characterNumber)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = characters[characterNumber].CharacterId
        },
                            result =>
                            {
                                _level = result.CharacterStatistics["Level"].ToString();
                                _xp = result.CharacterStatistics["XP"].ToString();
                                _gold = result.CharacterStatistics["Gold"].ToString();
                                Debug.Log("Персонаж! " + characters[characterNumber].CharacterName);
                                Debug.Log("Уровень: " + _level + ", Опыт: " + _xp + ", Золото: " + _gold);
                                Debug.Log("_slots.Count: " + _slots.Count);
                                Debug.Log("_slots[characterNumber] было: " + _slots[characterNumber]);
                                _slots[characterNumber] = Instantiate(_characterSlot, _conteinerLayout.transform);
                                Debug.Log("_slots[characterNumber] стало: " + _slots[characterNumber]);
                                _slots[characterNumber].ShowInfoCharacterSlot(characters[characterNumber].CharacterName, _level, _xp, _gold);
                            }, OnError);
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
                   FindCharacter(result.Characters, characterNumber);
               }
           }, OnError);
    }

    private void FindCharacter(List<CharacterResult> characters, int characterNumber)
    {
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
            {
                CharacterId = characters[characterNumber].CharacterId
            },
            result =>
            {
                var level = result.CharacterStatistics["Level"].ToString();
                var xp = result.CharacterStatistics["XP"].ToString();
                var gold = result.CharacterStatistics["Gold"].ToString();

                Debug.Log("Персонаж! " + characters[characterNumber].CharacterName);
                Debug.Log("Уровень: " + level + ", Опыт: " + xp + ", Золото: " + gold);

                if (characters[characterNumber].CharacterName == "qwerty")
                {
                    Debug.Log("Кверти, подтверждаю! " + characters[characterNumber].CharacterName);
                    UpdateCharacterXP(characters[characterNumber], result.CharacterStatistics);
                }
                else
                {
                    Debug.Log("Не Кверти! подтверждаю, не получает опыт! " + characters[characterNumber].CharacterName);
                }

                _slots[characterNumber].ShowInfoCharacterSlot(characters[characterNumber].CharacterName, level, xp, gold);
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

    private void CreateCharacterWithItem()
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = "character_item"
        },
        result =>
        {
            _slots.Add(Instantiate(_characterSlot, _conteinerLayout.transform));
            UpdateCharacterStatistics(result.CharacterId);
        }, OnError);
    }

    private void UpdateCharacterStatistics(string characterId)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterId,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", 1},
                {"XP", 0},
                {"Gold", 0}
            }
        }, result =>
        {
            Debug.Log("Complete character!");
            ClosePanelCreateCharacter();
            GetCharacter(); 
        }, OnError);
    }

    private void OpenPanelCreateCharacter()
    {
        _newCharacterCreatePanel.SetActive(true);
    }

    private void ClosePanelCreateCharacter()
    {
        _newCharacterCreatePanel.SetActive(false);
    }

    private void GetCharacter()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            result =>
            {
                Debug.Log("Character count: " + result.Characters.Count);
                for (int characterNumber = 0; characterNumber < result.Characters.Count; characterNumber++)
                {
                    Debug.Log("CharacterNumber: " + characterNumber);
                    Debug.Log("characters[characterNumber].CharacterId: " + result.Characters[characterNumber].CharacterId);
                    ShowInfoCharacter(result.Characters, characterNumber);
                }         
            }, OnError);
    }

    private void ShowInfoCharacter(List<CharacterResult> characters, int characterNumber)
    {
        if (characters.Count == 0)
        {
            foreach(var slot in _slots)
            {
                slot.ShowEmptySlot();
            }
        }
        else if(characters.Count > 0 && characters.Count <= _slots.Count)
        {
            PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
            {
                CharacterId = characters[characterNumber].CharacterId
            },
            result =>
            {
                var level = result.CharacterStatistics["Level"].ToString();
                var xp = result.CharacterStatistics["XP"].ToString();
                var gold = result.CharacterStatistics["Gold"].ToString();

                Debug.Log("Уровень, опыт, золото: " + level + xp + gold);
                _slots[characterNumber].ShowInfoCharacterSlot(characters[characterNumber].CharacterName, level, xp, gold);
            }, OnError);
        }
        else
        {
            Debug.Log("Add a slots for characters");
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
                _catalogLabel.text += "item_id: " + item.ItemId + "\n";
            }
        }
    }

    private void Update()
    {
        LoadingTimer();
    }

    private void ChangeAccount()
    {
        SceneManager.LoadScene(0);
    }

    private void OnGetAccountSuccess(GetAccountInfoResult result)
    {
        var accountInfo = result.AccountInfo;
        _titleLabel.text = "Welcome " + accountInfo.Username + "\n" + accountInfo.PlayFabId;
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
