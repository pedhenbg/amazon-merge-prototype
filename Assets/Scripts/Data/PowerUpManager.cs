using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance { get; private set; }

    [SerializeField] private GameObject selectionOverlayPanel;
    [SerializeField] private float minDeflateForce = 15f;
    [SerializeField] private float maxDeflateForce = 25f;
    [SerializeField] private float torqueForce = 100f;
    [SerializeField] private float destroyDelay = 2f;

    private Camera _mainCam;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _mainCam = Camera.main;
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
        if (selectionOverlayPanel != null)
        {
            selectionOverlayPanel.SetActive(state == GameState.PowerUpSelection);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.PowerUpSelection) return;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            ProcessSelection();
        }
    }

    private void ProcessSelection()
    {
        Vector2 screenPos = Pointer.current.position.ReadValue();
        Vector3 worldPos = _mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, _mainCam.nearClipPlane));

        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null && hit.collider.TryGetComponent(out AnimalEntity entity))
        {
            if (!entity.isDead && entity.isSimulated)
            {
                StartCoroutine(DeflateRoutine(entity));
            }
        }
    }

    private IEnumerator DeflateRoutine(AnimalEntity target)
    {
        GameManager.Instance.ChangeState(GameState.Playing);

        target.isDead = true;

        Collider2D col = target.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;

            float randomX = UnityEngine.Random.Range(-1f, 1f);
            Vector2 forceDir = new Vector2(randomX, 1f).normalized;
            float force = UnityEngine.Random.Range(minDeflateForce, maxDeflateForce);

            rb.AddForce(forceDir * force, ForceMode2D.Impulse);
            rb.AddTorque(UnityEngine.Random.Range(-torqueForce, torqueForce));
        }

        yield return new WaitForSeconds(destroyDelay);

        if (target != null)
        {
            target.ResetForPool();
            PoolManager.Instance.ReturnToPool(target.gameObject);
        }
    }
}