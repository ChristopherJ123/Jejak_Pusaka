using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float scrollSpeed = 10f;
    public float zoomScrollSpeed = 10f;
    public float edgeThickness = 20f; // Thickness in pixels from screen edge
    public float smoothTime = 0.2f;
    public float zoomSmoothTime = 0.2f;
    public bool isPlayerFollowing;
    public float minZoom = 3f;
    public float maxZoom = 10f;
    
    private Camera _cam;
    private Vector3 _velocity = Vector3.zero;
    private Vector3 _targetPosition = Vector3.zero;
    private float _zoomVelocity;
    private float _targetZoomSize;
    
    void Start()
    {
        _cam = Camera.main;
        if (!_cam) return;
        _targetPosition = _cam.transform.position;
        _targetZoomSize = _cam.orthographicSize;
    }

    void Update()
    {
        if (!isPlayerFollowing)
        {
            // Camera scrolling
            Vector3 pos = transform.position;
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x >= Screen.width - edgeThickness)
            {
                pos.x += scrollSpeed * Time.deltaTime;
            }
            else if (mousePos.x <= edgeThickness)
            {
                pos.x -= scrollSpeed * Time.deltaTime;
            }

            if (mousePos.y >= Screen.height - edgeThickness)
            {
                pos.y += scrollSpeed * Time.deltaTime;
            } else if (mousePos.y <= edgeThickness)
            {
                pos.y -= scrollSpeed * Time.deltaTime;
            }

            _targetPosition = Vector3.SmoothDamp(transform.position, pos, ref _velocity, smoothTime);

            transform.position = _targetPosition;
        }
        else
        {
            // Camera follow player
            transform.position = new Vector3(PlayerMovementScript.Instance.transform.position.x, PlayerMovementScript.Instance.transform.position.y, transform.position.z);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _targetZoomSize -= scroll * zoomScrollSpeed;
        _targetZoomSize = Mathf.Clamp(_targetZoomSize, minZoom, maxZoom);
        _cam.orthographicSize = Mathf.SmoothDamp(_cam.orthographicSize, _targetZoomSize, ref _zoomVelocity, zoomSmoothTime);
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            isPlayerFollowing = !isPlayerFollowing;
        }
    }
}
