using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D), typeof(SpriteRenderer))]
public class AnimalEntity : MonoBehaviour
{
    public static readonly HashSet<AnimalEntity> ActiveAnimals = new HashSet<AnimalEntity>();

    public AnimalData data;
    public bool isDead = false;
    public static event Action<AnimalEntity, AnimalEntity> OnCollisionMerge;

    [SerializeField] private float continuousVelocityThreshold = 2.0f;

    private Rigidbody2D _rb;
    private CircleCollider2D _col;
    private AnimalJuice _juiceController;

    public bool isSimulated => _rb != null && _rb.simulated;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CircleCollider2D>();
        _juiceController = GetComponent<AnimalJuice>();

        _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }

    private void OnEnable()
    {
        ActiveAnimals.Add(this);
    }

    private void OnDisable()
    {
        ActiveAnimals.Remove(this);
    }

    private void FixedUpdate()
    {
        if (!_rb.simulated || isDead) return;

        bool isMovingFast = _rb.linearVelocity.sqrMagnitude > continuousVelocityThreshold;
        CollisionDetectionMode2D desiredMode = isMovingFast ? CollisionDetectionMode2D.Continuous : CollisionDetectionMode2D.Discrete;

        if (_rb.collisionDetectionMode != desiredMode)
        {
            _rb.collisionDetectionMode = desiredMode;
        }
    }

    public void Setup(AnimalData newData, float gravityScale = 1.2f)
    {
        if (newData == null) return;

        data = newData;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = data.sprite;
        }

        _rb.mass = data.mass;
        _rb.gravityScale = gravityScale;
        isDead = false;

        if (_juiceController != null)
        {
            _juiceController.InitIdleJuice(data.visualScale);
        }
        else
        {
            transform.localScale = Vector3.one * data.visualScale;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || GameManager.Instance.CurrentState != GameState.Playing) return;

        AnimalEntity other = collision.gameObject.GetComponent<AnimalEntity>();
        if (other == null) return;
        if (other.isDead || other.data == null || this.data == null) return;

        if (this.data.level == other.data.level)
        {
            if (this.GetInstanceID() < other.GetInstanceID())
            {
                isDead = true;
                other.isDead = true;
                _rb.simulated = false;
                other.GetComponent<Rigidbody2D>().simulated = false;

                OnCollisionMerge?.Invoke(this, other);
            }
        }
    }

    public void SetSimulated(bool state)
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        _rb.simulated = state;
    }

    public void ReleaseDrop()
    {
        Vector3 offsetPosition = transform.position;
        offsetPosition.x += UnityEngine.Random.Range(-0.08f, 0.08f);
        transform.position = offsetPosition;

        SetSimulated(true);
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void ResetForPool()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _rb.simulated = false;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        isDead = false;

        if (_col == null) _col = GetComponent<CircleCollider2D>();
        _col.enabled = true;

        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }

    public void PlayMergeJiggleAnimation()
    {
        if (_juiceController != null)
        {
            _juiceController.PlayMergeImpact();
        }
    }

    public void ApplyShakeForce(Vector2 force)
    {
        if (_rb == null || !_rb.simulated) return;

        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        _rb.linearVelocity = Vector2.zero;
        _rb.AddForce(force, ForceMode2D.Impulse);
    }
}