using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFixedAspect : MonoBehaviour
{
    public Vector2 desiredAspectRatio = new Vector2(16, 9);

    private Camera _camera;
    private float defaultHeight;
    private float defaultAspectRatio;

    private void Awake()
    {
        _camera = GetComponent<Camera>();

        defaultAspectRatio = _camera.aspect; 
        defaultHeight = _camera.orthographicSize;

        float desiredCoeff = desiredAspectRatio.x / desiredAspectRatio.y;
        var desiredHeight = defaultHeight / defaultAspectRatio * desiredCoeff;

        if (desiredCoeff > defaultAspectRatio)
            _camera.orthographicSize = desiredHeight;
    }
}
