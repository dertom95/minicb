using UnityEngine;

/// <summary>
/// CameraController
/// AI generated. 
/// </summary>
public class CameraController : MonoBehaviour {
    [Header("Panning")]
    public float panSpeed = 30f;
    public float panSmoothTime = 0.1f;
    public Vector2 panLimitX = new Vector2(-100, 100);
    public Vector2 panLimitZ = new Vector2(-100, 100);

    [Header("Zoom")]
    public float zoomSpeed = 200f;
    public float zoomSmoothTime = 0.1f;
    public float minZoom = 20f;
    public float maxZoom = 80f;

    [Header("Rotation")]
    public float rotationSpeed = 100f;
    public float rotationSmoothTime = 0.1f;
    public float minRotation = 0f;
    public float maxRotation = 360f;

    [Header("Edge Scrolling")]
    public bool enableEdgeScroll = true;
    public float edgeScrollThreshold = 10f;

    private Vector3 targetPosition;
    private float targetZoom;
    private float targetRotationY;

    private Vector3 panVelocity;
    private float zoomVelocity;
    private float rotationVelocity;

    private Camera cam;

    void Start() {
        cam = Camera.main;
        targetPosition = transform.position;
        targetZoom = cam.transform.position.y;
        targetRotationY = transform.eulerAngles.y;
    }

    void Update() {
        HandlePan();
        HandleZoom();
        HandleRotation();

        // Smoothly move camera
        Vector3 desiredPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, panLimitX.x, panLimitX.y),
            Mathf.Clamp(targetZoom, minZoom, maxZoom),
            Mathf.Clamp(targetPosition.z, panLimitZ.x, panLimitZ.y)
        );
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref panVelocity, panSmoothTime);

        // Smoothly rotate camera
        float currentY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotationY, ref rotationVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(45f, currentY, 0f); // 45° isometric angle
    }

    void HandlePan() {
        Vector3 move = Vector3.zero;

        // Keyboard pan
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.back;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.right;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.left;
        }

        // Mouse drag pan (right mouse button)
        if (Input.GetMouseButton(1)) {
            float h = -Input.GetAxis("Mouse X");
            float v = -Input.GetAxis("Mouse Y");
            move += Quaternion.Euler(0, targetRotationY, 0) * new Vector3(h, 0, v) * 10f;
        }

        // Edge scrolling
        if (enableEdgeScroll) {
            Vector3 mouse = Input.mousePosition;
            if (mouse.x >= 0 && mouse.x < edgeScrollThreshold) {
                move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.left;
            }
            if (mouse.x <= Screen.width && mouse.x > Screen.width - edgeScrollThreshold) {
                move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.right;
            }
            if (mouse.y >= 0 && mouse.y < edgeScrollThreshold) {
                move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.back;
            }
            if (mouse.y <= Screen.height && mouse.y > Screen.height - edgeScrollThreshold) {
                move += Quaternion.Euler(0, targetRotationY, 0) * Vector3.forward;
            }
        }

        targetPosition += move.normalized * panSpeed * Time.deltaTime;
    }

    void HandleZoom() {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f) {
            targetZoom -= scroll * zoomSpeed * Time.deltaTime;
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
        }
    }

    void HandleRotation() {
        // Q/E or Middle Mouse Button + Drag
        if (Input.GetKey(KeyCode.Q)) {
            targetRotationY -= rotationSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E)) {
            targetRotationY += rotationSpeed * Time.deltaTime;
        }

        if (Input.GetMouseButton(2)) {
            float mouseX = Input.GetAxis("Mouse X");
            targetRotationY += mouseX * rotationSpeed * 0.02f;
        }

        targetRotationY = Mathf.Repeat(targetRotationY, 360f);
    }
}
