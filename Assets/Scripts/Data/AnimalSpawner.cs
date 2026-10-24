using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class AnimalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject animalPrefabBase;
    [SerializeField] private AnimalData[] startingPool;
    [SerializeField] private GameSettings settings;

    [SerializeField] private float spawnY = 4.5f;
    [SerializeField] private float dropDelay = 0.5f;
    [SerializeField] private Transform wallLeft;
    [SerializeField] private Transform wallRight;
    [SerializeField] private float sideMargin = 0.2f;

    [SerializeField] private LineRenderer aimLine;
    [SerializeField] private float floorYPosition = -4.5f;

    public static event Action<AnimalData> OnNextAnimalUpdated;

    private float _minX, _maxX;
    private AnimalEntity _currentAnimal;
    private AnimalData _nextData;
    private Camera _mainCam;
    private bool _isReady = true;
    private bool _canPlay = false;

    private void Awake()
    {
        _mainCam = Camera.main;
        if (aimLine != null) aimLine.enabled = false;
    }

    private void Start()
    {
        if (wallLeft != null && wallRight != null)
        {
            _minX = wallLeft.position.x + sideMargin;
            _maxX = wallRight.position.x - sideMargin;
        }
    }

    private void OnEnable() => GameManager.OnStateChanged += HandleStateChanged;
    private void OnDisable() => GameManager.OnStateChanged -= HandleStateChanged;

    private void HandleStateChanged(GameState state)
    {
        _canPlay = (state == GameState.Playing);

        if (state == GameState.Countdown)
        {
            if (_currentAnimal != null) PoolManager.Instance.ReturnToPool(_currentAnimal.gameObject);
            _currentAnimal = null;

            if (startingPool != null && startingPool.Length > 0)
            {
                _nextData = startingPool[UnityEngine.Random.Range(0, startingPool.Length)];
                SpawnAnimal(true);
            }
        }
        else if (state == GameState.Menu || state == GameState.GameOver)
        {
            if (_currentAnimal != null) PoolManager.Instance.ReturnToPool(_currentAnimal.gameObject);
            _currentAnimal = null;
            _canPlay = false;
            if (aimLine != null) aimLine.enabled = false;
        }
    }

    private void Update()
    {
        if (!_canPlay || _currentAnimal == null || !_isReady)
        {
            if (aimLine != null) aimLine.enabled = false;
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = _mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _mainCam.nearClipPlane));
        float clampedX = Mathf.Clamp(worldPos.x, _minX, _maxX);

        Vector3 animalPos = new Vector3(clampedX, spawnY, 0);
        _currentAnimal.transform.position = animalPos;

        if (aimLine != null)
        {
            aimLine.enabled = true;
            aimLine.SetPosition(0, animalPos);
            aimLine.SetPosition(1, new Vector3(clampedX, floorYPosition, 0));
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isReady = false;
            if (aimLine != null) aimLine.enabled = false;

            _currentAnimal.ReleaseDrop();
            _currentAnimal = null;
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(dropDelay);
        SpawnAnimal(false);
    }

    private void SpawnAnimal(bool ignorePlayState)
    {
        if (_currentAnimal != null) return;
        if (!ignorePlayState && !_canPlay) return;
        if (startingPool == null || startingPool.Length == 0) return;

        AnimalData dataToSpawn = _nextData;
        _nextData = startingPool[UnityEngine.Random.Range(0, startingPool.Length)];
        OnNextAnimalUpdated?.Invoke(_nextData);

        GameObject go = PoolManager.Instance.GetFromPool(animalPrefabBase, new Vector3(0, spawnY, 0), Quaternion.identity);
        if (go.TryGetComponent(out AnimalEntity entity))
        {
            _currentAnimal = entity;
            float gravity = settings != null ? settings.globalGravityScale : 1.2f;
            _currentAnimal.Setup(dataToSpawn, gravity);
            _currentAnimal.SetSimulated(false);
            go.tag = "Animal";
        }
        _isReady = true;
    }
}