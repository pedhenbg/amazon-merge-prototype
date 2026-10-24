using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Image nextAnimalImage;
    [SerializeField] private Slider shakeSlider;
    [SerializeField] private Button shakeButton;

    private void OnEnable()
    {
        ScoreManager.OnScoreUpdatedStatic += UpdateScore;
        AnimalSpawner.OnNextAnimalUpdated += UpdateNextAnimal;
        ShakeSystem.OnEnergyUpdated += UpdateShakeBar;
        ShakeSystem.OnShakeAvailable += EnableShakeButton;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreUpdatedStatic -= UpdateScore;
        AnimalSpawner.OnNextAnimalUpdated -= UpdateNextAnimal;
        ShakeSystem.OnEnergyUpdated -= UpdateShakeBar;
        ShakeSystem.OnShakeAvailable -= EnableShakeButton;
    }

    private void Start()
    {
        UpdateScore(0);
        if (shakeButton != null) shakeButton.interactable = false;
    }

    private void UpdateScore(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.transform.parent.gameObject.SetActive(true);
            scoreText.enabled = true;
            scoreText.text = newScore.ToString();
        }
    }

    private void UpdateNextAnimal(AnimalData nextData)
    {
        if (nextAnimalImage != null && nextData != null)
        {
            nextAnimalImage.gameObject.SetActive(true);
            nextAnimalImage.enabled = true;
            nextAnimalImage.sprite = nextData.sprite;
            nextAnimalImage.preserveAspect = true;
        }
    }

    private void UpdateShakeBar(float fillPercentage)
    {
        if (shakeSlider != null) shakeSlider.value = fillPercentage;
    }

    private void EnableShakeButton()
    {
        if (shakeButton != null) shakeButton.interactable = true;
    }
}