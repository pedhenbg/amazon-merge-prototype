using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening; // Importação obrigatória para o Juice

public class CountdownController : MonoBehaviour
{
    [SerializeField] private GameObject countdownPanel;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameSettings settings;

    private Vector3 _originalScale;

    private void Awake()
    {
        if (countdownText != null)
        {
            _originalScale = countdownText.transform.localScale;
        }
    }

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
        if (state == GameState.Countdown)
        {
            countdownPanel.SetActive(true);
            StartCoroutine(CountdownRoutine());
        }
        else
        {
            countdownPanel.SetActive(false);
            StopAllCoroutines();
        }
    }

    private IEnumerator CountdownRoutine()
    {
        float timeRemaining = settings.startCountdownTime;

        while (timeRemaining > 0)
        {
            countdownText.text = Mathf.CeilToInt(timeRemaining).ToString();

            countdownText.transform.DOKill(); 
            countdownText.transform.localScale = _originalScale * 0.2f; 

            countdownText.transform.DOScale(_originalScale, 0.5f).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        countdownText.text = "GO!";

        countdownText.transform.DOKill();
        countdownText.transform.localScale = _originalScale * 0.2f;

        countdownText.transform.DOScale(_originalScale * 1.5f, 0.4f).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(0.5f);

        countdownPanel.SetActive(false);
        GameManager.Instance.StartPlaying();
    }

    private void OnDestroy()
    {
        if (countdownText != null)
        {
            countdownText.transform.DOKill();
        }
    }
}