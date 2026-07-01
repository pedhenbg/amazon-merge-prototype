using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void HandleStateChanged(GameState state)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(state == GameState.Menu);

        if (gameOverPanel != null) gameOverPanel.SetActive(state == GameState.GameOver);

        if (hudPanel != null)
        {
            hudPanel.SetActive(state == GameState.Playing || state == GameState.Countdown || state == GameState.Paused);
        }

        if (state == GameState.GameOver)
        {
            UpdateGameOverUI();
        }
    }

    private void UpdateGameOverUI()
    {
        SaveData data = SaveSystem.Load();
        if (highScoreText != null) highScoreText.text = $"{data.highScore}";
    }

    public void OnStartButtonClicked()
    {
        GameManager.Instance.StartCountdown();
    }

    public void OnRestartButtonClicked()
    {
        GameManager.Instance.RestartGame();
    }

    public void OnQuitButtonClicked()
    {
        GameManager.Instance.QuitGame();
    }
}