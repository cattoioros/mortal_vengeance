using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform player;
    public Vector3 offset;  // poziție relativă față de pivot

    [Header("Rotation Settings")]
    public float rotationSpeed;
    public float minYAngle;
    public float maxYAngle;

    private float yaw = 0f;
    private float pitch = 0f; 

    private Transform pivot;

    void Start()
    {
        
        pivot = new GameObject("CameraPivot").transform;
        pivot.position = player.position;
        pivot.rotation = Quaternion.identity;

        
        transform.SetParent(pivot);
        transform.localPosition = offset;
        transform.LookAt(player.position);
    }

    void LateUpdate()
    {
        HandleCamera();
    }

    void HandleCamera()
    {
        pivot.position = player.position;

        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
