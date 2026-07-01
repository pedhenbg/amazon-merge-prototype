using UnityEngine;
using System;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI[] highScoreTexts;

    public event Action<int> OnScoreUpdated;
    public static event Action<int> OnScoreUpdatedStatic;

    private int _currentScore;
    private int _highScore;
    private Vector3 _originalScale;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (scoreText != null)
        {
            _originalScale = scoreText.transform.localScale;
        }

        LoadHighScore();
    }

    private void OnEnable() => GameManager.OnStateChanged += HandleStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= HandleStateChanged;

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Menu || state == GameState.Countdown)
        {
            ResetScore();
        }
        else if (state == GameState.GameOver)
        {
            SaveCurrentHighScore();
        }
    }

    public void AddScore(int amount)
    {
        _currentScore += amount;

        if (scoreText != null)
        {
            scoreText.text = _currentScore.ToString();

            scoreText.transform.DOKill();
            scoreText.transform.localScale = _originalScale * 1.3f;
            scoreText.transform.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad);
        }

        if (_currentScore > _highScore)
        {
            _highScore = _currentScore;
            UpdateHighScoreUI();
        }

        OnScoreUpdated?.Invoke(_currentScore);
        OnScoreUpdatedStatic?.Invoke(_currentScore);
    }

    public int GetCurrentScore() => _currentScore;

    public void ResetScore()
    {
        _currentScore = 0;

        if (scoreText != null)
        {
            scoreText.text = "0";
        }

        OnScoreUpdated?.Invoke(_currentScore);
        OnScoreUpdatedStatic?.Invoke(_currentScore);
    }

    private void LoadHighScore()
    {
        SaveData data = SaveSystem.Load();
        _highScore = data.highScore;
        UpdateHighScoreUI();
    }

    private void SaveCurrentHighScore()
    {
        SaveData data = SaveSystem.Load();

        if (_currentScore > data.highScore)
        {
            data.highScore = _currentScore;
            SaveSystem.Save(data);
        }
    }

    private void UpdateHighScoreUI()
    {
        foreach (var txt in highScoreTexts)
        {
            if (txt != null) txt.text = _highScore.ToString();
        }
    }
}