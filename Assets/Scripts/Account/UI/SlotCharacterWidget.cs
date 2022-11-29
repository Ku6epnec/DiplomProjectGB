using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotCharacterWidget : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _emptySlot;
    [SerializeField] private GameObject _infoCharacterSlot;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private TMP_Text _levelLabel;
    [SerializeField] private TMP_Text _xpLabel;
    [SerializeField] private TMP_Text _goldLabel;

    public Button SlotButton => _button;
    
    public void ShowInfoCharacterSlot(string name, string level, string xp, string gold)
    {
        _infoCharacterSlot.SetActive(true);
        _emptySlot.SetActive(false);

        _nameLabel.text = name;
        _levelLabel.text = level;
        _xpLabel.text = xp;
        _goldLabel.text = gold;

    }

    public void ShowEmptySlot()
    {
        _infoCharacterSlot.SetActive(false);
        _emptySlot.SetActive(true);
    }
}
