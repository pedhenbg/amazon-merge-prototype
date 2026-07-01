using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AimLineAnimator : MonoBehaviour
{
    [Tooltip("Velocidade da animação. Valores negativos fazem descer, positivos fazem subir.")]
    [SerializeField] private float scrollSpeed = -2f;

    private LineRenderer _lineRenderer;
    private Material _materialInstance;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();

        _materialInstance = _lineRenderer.material;
    }

    private void Update()
    {
        if (!_lineRenderer.enabled) return;

        float offset = Time.time * scrollSpeed;

        _materialInstance.mainTextureOffset = new Vector2(offset, 0f);
    }

    private void OnDestroy()
    {
        if (_materialInstance != null)
        {
            Destroy(_materialInstance);
        }
    }
}