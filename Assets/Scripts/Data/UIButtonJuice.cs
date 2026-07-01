using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonJuice : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Configurações do Juice")]
    [Tooltip("Para qual tamanho o botão encolhe ao ser apertado")]
    [SerializeField] private float pressScale = 0.85f;
    [Tooltip("Tempo da animação em segundos")]
    [SerializeField] private float animationDuration = 0.15f;

    private Vector3 _originalScale;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(_originalScale * pressScale, animationDuration).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(_originalScale, animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}