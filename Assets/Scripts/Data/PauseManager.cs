using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button musicButton;
    [SerializeField] private TextMeshProUGUI musicButtonText;
    [SerializeField] private Button sfxButton;
    [SerializeField] private TextMeshProUGUI sfxButtonText;
    [SerializeField] private float toggleCooldown = 0.25f;

    private float _lastToggleTime;
    private bool _isMusicOn;
    private bool _isSfxOn;

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        LoadAudioSettings();
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (musicButton != null)
        {
            musicButton.onClick.AddListener(ToggleMusic);
        }

        if (sfxButton != null)
        {
            sfxButton.onClick.AddListener(ToggleSFX);
        }
    }

    private void LoadAudioSettings()
    {
        _isMusicOn = PlayerPrefs.GetInt("GameMusic", 1) == 1;
        _isSfxOn = PlayerPrefs.GetInt("GameSFX", 1) == 1;

        ApplyMusicSettings();
        ApplySFXSettings();
    }

    public void TogglePause()
    {
        if (Time.unscaledTime - _lastToggleTime < toggleCooldown) return;

        _lastToggleTime = Time.unscaledTime;

        if (pausePanel != null && pausePanel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Paused);
        }

        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Playing);
        }

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    private void ToggleMusic()
    {
        _isMusicOn = !_isMusicOn;
        ApplyMusicSettings();
    }

    private void ToggleSFX()
    {
        _isSfxOn = !_isSfxOn;
        ApplySFXSettings();
    }

    private void ApplyMusicSettings()
    {
        PlayerPrefs.SetInt("GameMusic", _isMusicOn ? 1 : 0);
        PlayerPrefs.Save();

        if (musicButtonText != null)
        {
            musicButtonText.text = _isMusicOn ? "Music ON" : "Music OFF";
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicMute(!_isMusicOn);
        }
    }

    private void ApplySFXSettings()
    {
        PlayerPrefs.SetInt("GameSFX", _isSfxOn ? 1 : 0);
        PlayerPrefs.Save();

        if (sfxButtonText != null)
        {
            sfxButtonText.text = _isSfxOn ? "FX ON" : "FX OFF";
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXMute(!_isSfxOn);
        }
    }
}