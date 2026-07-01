using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BucketLid : MonoBehaviour
{
    private BoxCollider2D _col;
    private Camera _cam;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();
        _cam = Camera.main;
    }

    private void Start()
    {
        PositionLid();
    }

    private void PositionLid()
    {
        if (_cam == null) return;

        Vector2 topRightCorner = _cam.ViewportToWorldPoint(new Vector3(1, 1, _cam.nearClipPlane));
        Vector2 topLeftCorner = _cam.ViewportToWorldPoint(new Vector3(0, 1, _cam.nearClipPlane));

        float screenWidth = Vector2.Distance(topLeftCorner, topRightCorner);

        transform.position = new Vector3(_cam.transform.position.x, topRightCorner.y + (_col.size.y / 2f), 0f);

        _col.size = new Vector2(screenWidth * 1.5f, _col.size.y);
    }
}