using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraAdjuster : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playAreaBounds;
    [SerializeField] private float horizontalPadding = 0.5f;

    private Camera _cam;

    private void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    private void Start()
    {
        AdjustCameraToFixedWidth();
    }

    private void AdjustCameraToFixedWidth()
    {
        if (playAreaBounds == null) return;

        float targetWidth = playAreaBounds.bounds.size.x + horizontalPadding;
        float screenRatio = (float)Screen.width / Screen.height;

        _cam.orthographicSize = (targetWidth / 2f) / screenRatio;
    }
}