using UnityEngine;
using System;

public class MergeManager : MonoBehaviour
{
    [SerializeField] private GameObject animalPrefabBase;
    [SerializeField] private GameSettings settings;

    public static event Action<AnimalData, Vector3> OnMergeProcessed;

    private void OnEnable() => AnimalEntity.OnCollisionMerge += HandleMerge;
    private void OnDisable() => AnimalEntity.OnCollisionMerge -= HandleMerge;

    private void HandleMerge(AnimalEntity a, AnimalEntity b)
    {
        if (a == null || b == null || a.data == null) return;

        AnimalData currentData = a.data;
        Vector3 spawnPos = (a.transform.position + b.transform.position) / 2f;

        PoolManager.Instance.ReturnToPool(a.gameObject);
        PoolManager.Instance.ReturnToPool(b.gameObject);

        if (currentData.mergeSound != null)
        {
            AudioManager.Instance.PlaySFX(currentData.mergeSound);
        }

        if (currentData.nextEvolution != null)
        {
            GameObject newObj = PoolManager.Instance.GetFromPool(animalPrefabBase, spawnPos, Quaternion.identity);
            if (newObj.TryGetComponent(out AnimalEntity entity))
            {
                float gravity = settings != null ? settings.globalGravityScale : 1.2f;
                entity.Setup(currentData.nextEvolution, gravity);
                entity.SetSimulated(true);
                entity.PlayMergeJiggleAnimation();
                newObj.tag = "Animal";
            }
        }

        OnMergeProcessed?.Invoke(currentData, spawnPos);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(currentData.scoreValue);
        }
    }
}