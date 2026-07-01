using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Countdown,
    Playing,
    Paused,
    GameOver,
    PowerUpSelection
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        ChangeState(GameState.Menu);
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeKey();
        }
    }

    private void HandleEscapeKey()
    {
        switch (CurrentState)
        {
            case GameState.Menu:
                QuitGame();
                break;

            case GameState.Playing:
                ChangeState(GameState.Paused);
                break;

            case GameState.Paused:
                ChangeState(GameState.Playing);
                break;

            case GameState.Countdown:
            case GameState.GameOver:
            case GameState.PowerUpSelection:
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

        if (newState == GameState.Paused || newState == GameState.GameOver)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void StartCountdown() => ChangeState(GameState.Countdown);

    public void StartPlaying() => ChangeState(GameState.Playing);

    public void TriggerGameOver()
    {
        if (CurrentState == GameState.GameOver) return;

        int currentScore = ScoreManager.Instance.GetCurrentScore();
        SaveData data = SaveSystem.Load();

        if (currentScore > data.highScore)
        {
            data.highScore = currentScore;
            SaveSystem.Save(data);
        }

        ChangeState(GameState.GameOver);
    }

    public void UpdateGameState(GameState newState)
    {
        CurrentState = newState;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame() => Application.Quit();
}