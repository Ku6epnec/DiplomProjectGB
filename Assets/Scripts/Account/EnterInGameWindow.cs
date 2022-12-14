using UnityEngine;
using UnityEngine.UI;

public class EnterInGameWindow : MonoBehaviour
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _createAccountButton;

    [SerializeField] private Canvas _enterInGameCanvas;
    [SerializeField] private Canvas _createAccountCanvas;
    [SerializeField] private Canvas _signInCanvas;

    [SerializeField] private AudioClip _audioButton;

    private AudioSource _audio;


    private void Start()
    {
        _audio = GetComponent<AudioSource>();
        _signInButton.onClick.AddListener(OpenSignInWindow);
        _createAccountButton.onClick.AddListener(OpenCreateAccountWindow);
    }

    private void OnDestroy()
    {
        _signInButton.onClick.RemoveAllListeners();
        _createAccountButton.onClick.RemoveAllListeners();
    }

    private void OpenSignInWindow()
    {
        _signInCanvas.enabled = true;
        _enterInGameCanvas.enabled = false;
    }

    private void OpenCreateAccountWindow()
    {
        _createAccountCanvas.enabled = true;
        _enterInGameCanvas.enabled = false;
    }

    public void SoundButton()
    {
        _audio.PlayOneShot(_audioButton);
    }
}
