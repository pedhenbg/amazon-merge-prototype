using UnityEngine;
using UnityEngine.UI;

public class UIShakeController : MonoBehaviour
{
    [SerializeField] private GameObject shakeVisualWrapper;
    [SerializeField] private Image barFill;
    [SerializeField] private Button shakeButton;
    [SerializeField] private ShakeSystem shakeSystem;
    [SerializeField] private GameObject shakeMessagePanel;

    [SerializeField] private float uiShakeSpeed = 20f;
    [SerializeField] private float uiShakeAmount = 5f;
    [SerializeField] private float messageWobbleSpeed = 15f;
    [SerializeField] private float messageWobbleAmount = 10f;

    private RectTransform _buttonRect;
    private RectTransform _messageRect;
    private Quaternion _originalButtonRotation;
    private Quaternion _originalMessageRotation;
    private bool _isButtonShaking;

    private void Awake()
    {
        if (shakeButton != null)
        {
            _buttonRect = shakeButton.GetComponent<RectTransform>();
            _originalButtonRotation = _buttonRect.localRotation;
        }

        if (shakeMessagePanel != null)
        {
            _messageRect = shakeMessagePanel.GetComponent<RectTransform>();
            _originalMessageRotation = _messageRect.localRotation;
        }
    }

    private void OnEnable()
    {
        ShakeSystem.OnEnergyUpdated += UpdateEnergyBar;
        ShakeSystem.OnShakeAvailable += EnableShakeButton;
        ShakeSystem.OnShakeWindowEnded += DisableShakeState;
        GameManager.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        ShakeSystem.OnEnergyUpdated -= UpdateEnergyBar;
        ShakeSystem.OnShakeAvailable -= EnableShakeButton;
        ShakeSystem.OnShakeWindowEnded -= DisableShakeState;
        GameManager.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        if (shakeButton != null)
        {
            shakeButton.interactable = false;
            shakeButton.onClick.AddListener(TriggerShakeWindow);
        }

        if (barFill != null)
        {
            barFill.fillAmount = 0f;
        }

        if (shakeMessagePanel != null)
        {
            shakeMessagePanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isButtonShaking && _buttonRect != null)
        {
            float zRotation = Mathf.Sin(Time.time * uiShakeSpeed) * uiShakeAmount;
            _buttonRect.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        }

        if (shakeMessagePanel != null && shakeMessagePanel.activeSelf && _messageRect != null)
        {
            float zMessageRotation = Mathf.Sin(Time.time * messageWobbleSpeed) * messageWobbleAmount;
            _messageRect.localRotation = Quaternion.Euler(0f, 0f, zMessageRotation);
        }
    }

    private void HandleStateChanged(GameState state)
    {
        if (shakeVisualWrapper != null)
        {
            shakeVisualWrapper.SetActive(state == GameState.Playing);
        }

        if (state != GameState.Playing)
        {
            DisableShakeState();
        }
    }

    private void UpdateEnergyBar(float normalizedEnergy)
    {
        if (barFill != null)
        {
            barFill.fillAmount = normalizedEnergy;
        }
    }

    private void EnableShakeButton()
    {
        if (shakeButton != null)
        {
            shakeButton.interactable = true;
            _isButtonShaking = true;
        }
    }

    private void DisableShakeState()
    {
        if (shakeButton != null)
        {
            shakeButton.interactable = false;
        }

        if (shakeMessagePanel != null)
        {
            shakeMessagePanel.SetActive(false);
        }

        StopAnimations();
    }

    private void TriggerShakeWindow()
    {
        if (shakeSystem != null)
        {
            shakeSystem.ActivateShakeWindow();
        }

        if (shakeButton != null)
        {
            shakeButton.interactable = false;
        }

        if (shakeMessagePanel != null)
        {
            shakeMessagePanel.SetActive(true);
        }

        StopAnimations();
    }

    private void StopAnimations()
    {
        _isButtonShaking = false;

        if (_buttonRect != null)
        {
            _buttonRect.localRotation = _originalButtonRotation;
        }

        if (_messageRect != null)
        {
            _messageRect.localRotation = _originalMessageRotation;
        }
    }
}