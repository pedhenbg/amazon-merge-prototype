using UnityEngine;
using System.Collections;

public class VFXController : MonoBehaviour
{
    private void OnEnable()
    {
        MergeManager.OnMergeProcessed += HandleMergeProcessed;
    }

    private void OnDisable()
    {
        MergeManager.OnMergeProcessed -= HandleMergeProcessed;
    }

    private void HandleMergeProcessed(AnimalData data, Vector3 position)
    {
        if (data.mergeVFXPrefab != null)
        {
            GameObject vfx = PoolManager.Instance.GetFromPool(data.mergeVFXPrefab, position, Quaternion.identity);
            StartCoroutine(ReturnVFXToPool(vfx));
        }
    }

    private IEnumerator ReturnVFXToPool(GameObject vfx)
    {
        ParticleSystem ps = vfx.GetComponent<ParticleSystem>();

        if (ps != null)
        {
            yield return new WaitForSeconds(ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        PoolManager.Instance.ReturnToPool(vfx);
    }
}