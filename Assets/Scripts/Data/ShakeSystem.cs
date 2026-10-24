using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShakeSystem : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private float shakeWindowDuration = 5f;
    [SerializeField] private float internalCooldown = 0.5f;
    [SerializeField] private Collider2D shakeRoofCollider;
    [SerializeField] private float roofPersistDuration = 1.5f;

    public static event Action<float> OnEnergyUpdated;
    public static event Action OnShakeAvailable;
    public static event Action OnShakeWindowEnded;

    private float _currentEnergy;
    private bool _isShakeReady;
    private bool _isShakeWindowOpen;
    private bool _isRoofPersisting;

    private float _windowTimer;
    private float _cooldownTimer;
    private float _roofTimer;

    private void OnEnable()
    {
        MergeManager.OnMergeProcessed += HandleMerge;
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        MergeManager.OnMergeProcessed -= HandleMerge;
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        if (shakeRoofCollider != null)
        {
            shakeRoofCollider.enabled = false;
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (state == GameState.Menu || state == GameState.Countdown)
        {
            ResetEnergy();
        }
    }

    private void HandleMerge(AnimalData data, Vector3 position)
    {
        if (_isShakeReady || _isShakeWindowOpen || GameManager.Instance.CurrentState != GameState.Playing) return;

        _currentEnergy += data.shakeEnergyReward;
        OnEnergyUpdated?.Invoke(_currentEnergy / settings.maxShakeEnergy);

        if (_currentEnergy >= settings.maxShakeEnergy)
        {
            _isShakeReady = true;
            OnShakeAvailable?.Invoke();
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        if (_isRoofPersisting)
        {
            _roofTimer -= Time.deltaTime;
            if (_roofTimer <= 0f)
            {
                _isRoofPersisting = false;
                if (shakeRoofCollider != null)
                {
                    shakeRoofCollider.enabled = false;
                }
            }
        }

        if (!_isShakeWindowOpen) return;

        _windowTimer -= Time.deltaTime;
        _cooldownTimer -= Time.deltaTime;

        if (_windowTimer <= 0f)
        {
            CloseShakeWindow();
            return;
        }

        if (_cooldownTimer <= 0f && CheckInputTriggers())
        {
            ApplyOptimizedForce();
            _cooldownTimer = internalCooldown;
        }
    }

    public void ActivateShakeWindow()
    {
        if (!_isShakeReady || _isShakeWindowOpen || GameManager.Instance.CurrentState != GameState.Playing) return;

        _isShakeReady = false;
        _isShakeWindowOpen = true;
        _isRoofPersisting = false;

        _windowTimer = shakeWindowDuration;
        _cooldownTimer = 0f;

        if (shakeRoofCollider != null)
        {
            shakeRoofCollider.enabled = true;
        }
    }

    private bool CheckInputTriggers()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    }

    private void ApplyOptimizedForce()
    {
        foreach (var animal in AnimalEntity.ActiveAnimals)
        {
            if (animal.isSimulated && !animal.isDead && animal.data != null)
            {
                float randomX = UnityEngine.Random.Range(-0.5f, 0.5f);
                float mass = animal.data.mass;

                float neutralizedForce = settings.shakeForce * mass;

                if (mass < 1f)
                {
                    neutralizedForce *= 0.85f;
                }

                Vector2 finalForce = new Vector2(randomX, 1f).normalized * neutralizedForce;

                animal.ApplyShakeForce(finalForce);
            }
        }
    }

    private void CloseShakeWindow()
    {
        _isShakeWindowOpen = false;
        _currentEnergy = 0f;
        _isShakeReady = false;

        OnEnergyUpdated?.Invoke(0f);
        OnShakeWindowEnded?.Invoke();

        if (shakeRoofCollider != null && shakeRoofCollider.gameObject.activeInHierarchy)
        {
            _isRoofPersisting = true;
            _roofTimer = roofPersistDuration;
        }
    }

    private void ResetEnergy()
    {
        _currentEnergy = 0f;
        _isShakeReady = false;
        _isShakeWindowOpen = false;
        _isRoofPersisting = false;

        if (shakeRoofCollider != null)
        {
            shakeRoofCollider.enabled = false;
        }

        OnEnergyUpdated?.Invoke(0f);
    }
}