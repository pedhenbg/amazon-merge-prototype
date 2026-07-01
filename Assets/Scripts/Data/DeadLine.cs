using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeadLine : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    private HashSet<AnimalEntity> _entitiesInDanger = new HashSet<AnimalEntity>();
    private float _timer;

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Playing)
        {
            _timer = 0f;
            return;
        }

        _entitiesInDanger.RemoveWhere(e => e == null || !e.gameObject.activeInHierarchy || e.isDead || !e.isSimulated);

        if (_entitiesInDanger.Count > 0)
        {
            _timer += Time.deltaTime;

            if (_timer >= settings.deadlineToleranceTime)
            {
                GameManager.Instance.TriggerGameOver();
            }
        }
        else
        {
            _timer = 0f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.TryGetComponent(out AnimalEntity entity))
        {
            if (entity.isSimulated && !entity.isDead)
            {
                _entitiesInDanger.Add(entity);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out AnimalEntity entity))
        {
            _entitiesInDanger.Remove(entity);
        }
    }
}