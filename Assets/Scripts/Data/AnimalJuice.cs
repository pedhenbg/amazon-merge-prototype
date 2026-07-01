using UnityEngine;
using DG.Tweening;

public class AnimalJuice : MonoBehaviour
{
    [SerializeField] private float pulseScale = 0.02f;
    [SerializeField] private float pulseDuration = 1.5f;

    [SerializeField] private float mergeScaleMultiplier = 1.4f;
    [SerializeField] private float mergeDuration = 0.3f;

    private Vector3 _baseScale;
    private Sequence _idleSequence;

    public void InitIdleJuice(float visualScale)
    {
        _baseScale = Vector3.one * visualScale;

        transform.DOKill();
        transform.localScale = _baseScale;

        _idleSequence?.Kill();
        _idleSequence = DOTween.Sequence();

        _idleSequence.Join(transform.DOScaleX(_baseScale.x * (1f + pulseScale), pulseDuration).SetEase(Ease.InOutSine));
        _idleSequence.Join(transform.DOScaleY(_baseScale.y * (1f - pulseScale), pulseDuration).SetEase(Ease.InOutSine));

        _idleSequence.SetLoops(-1, LoopType.Yoyo);
        _idleSequence.SetUpdate(UpdateType.Normal, false);
    }

    public void PlayMergeImpact()
    {
        _idleSequence?.Pause();
        transform.DOKill();

        transform.DOScale(_baseScale * mergeScaleMultiplier, mergeDuration / 2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                transform.DOScale(_baseScale, mergeDuration / 2f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() => _idleSequence?.Play());
            });
    }

    private void OnDisable()
    {
        transform.DOKill();
        _idleSequence?.Kill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
        _idleSequence?.Kill();
    }
}