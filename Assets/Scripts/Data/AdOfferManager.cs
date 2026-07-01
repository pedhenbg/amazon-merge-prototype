using UnityEngine;
using System.Collections;
using System.Linq;

public class AdOfferManager : MonoBehaviour
{
    [SerializeField] private GameObject caramelDogUIOffer;
    [SerializeField] private Transform deadLineTransform;
    [SerializeField] private float criticalHeightOffset = 2f;

    [SerializeField] private int maxOffersPerMatch = 2;
    [SerializeField] private float cooldownBetweenOffers = 90f;
    [SerializeField] private float checkInterval = 0.5f;

    private int _offersUsedThisMatch = 0;
    private float _nextAllowedOfferTime = 0f;
    private bool _isOfferActive = false;

    private float CriticalY => deadLineTransform.position.y - criticalHeightOffset;

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
        if (state == GameState.Menu || state == GameState.Countdown)
        {
            ResetManager();
        }
        else if (state == GameState.Playing)
        {
            StartCoroutine(HeightCheckRoutine());
        }
        else
        {
            StopAllCoroutines();
            HideOffer();
        }
    }

    private IEnumerator HeightCheckRoutine()
    {
        while (GameManager.Instance.CurrentState == GameState.Playing)
        {
            yield return new WaitForSeconds(checkInterval);

            if (CanOfferAd() && HasReachedCriticalHeight())
            {
                ShowOffer();
            }
            else if (_isOfferActive && !HasReachedCriticalHeight())
            {
                HideOffer();
            }
        }
    }

    private bool HasReachedCriticalHeight()
    {
        if (AnimalEntity.ActiveAnimals.Count == 0) return false;

        float highestY = AnimalEntity.ActiveAnimals
            .Where(a => a.isSimulated && !a.isDead)
            .Max(a => a.transform.position.y);

        return highestY >= CriticalY;
    }

    private bool CanOfferAd()
    {
        return !_isOfferActive &&
               _offersUsedThisMatch < maxOffersPerMatch &&
               Time.time >= _nextAllowedOfferTime;
    }

    private void ShowOffer()
    {
        _isOfferActive = true;

        if (caramelDogUIOffer != null)
        {
            caramelDogUIOffer.SetActive(true);
        }
    }

    private void HideOffer()
    {
        _isOfferActive = false;

        if (caramelDogUIOffer != null)
        {
            caramelDogUIOffer.SetActive(false);
        }
    }

    public void OnPlayerClickedAdButton()
    {
        HideOffer();
        _offersUsedThisMatch++;
        _nextAllowedOfferTime = Time.time + cooldownBetweenOffers;

        GameManager.Instance.ChangeState(GameState.PowerUpSelection);

        Debug.Log("Iniciando SDK de anuncio aqui no futuro...");
    }

    public void OnPlayerIgnoredAd()
    {
        HideOffer();
        _nextAllowedOfferTime = Time.time + 30f;
    }

    private void ResetManager()
    {
        StopAllCoroutines();
        _offersUsedThisMatch = 0;
        _nextAllowedOfferTime = 0f;
        HideOffer();
    }
}