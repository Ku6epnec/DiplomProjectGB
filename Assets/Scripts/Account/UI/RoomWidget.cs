using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomWidget : MonoBehaviour
{
    [SerializeField] private Button _buttonConnect;
    [SerializeField] private TMP_Text _nameLabel;
    [SerializeField] private Image _backGround;

    private bool _status;

    public Button _buttonConnectRoom => _buttonConnect;

    public RoomInfo _roomInfo { get; private set; }

    public void AddRoomInfo(bool status, RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        _nameLabel.text = roomInfo.Name;
        _status = status;
        if (_status)
        {
            _backGround.color = Color.green;
        }
        else
        {
            _backGround.color = Color.red;
        }
    }
}
