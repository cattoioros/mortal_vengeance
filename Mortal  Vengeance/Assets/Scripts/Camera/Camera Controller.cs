using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform player;
    public Vector3 defaultOffset; // Offset-ul maxim dorit (cand nu sunt pereti)

    [Header("Rotation Settings")]
    public float rotationSpeed = 2.0f;
    public float minYAngle = -35f;
    public float maxYAngle = 60f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers; // Ce straturi sunt considerate "pereti"?
    public float cameraCollisionRadius = 0.2f; // Grosimea camerei (pentru SphereCast)
    public float cameraCollisionOffset = 0.2f; // Mic spatiu ca sa nu intre camera in perete
    public float collisionDamp = 10f; // Cat de lin revine camera la loc

    private float yaw = 0f;
    private float pitch = 0f;
    private Transform pivot;
    private float currentDistance;
    private Vector3 directionNormalized;

    void Start()
    {
        // Setam offset-ul initial daca nu e setat in inspector
        if (defaultOffset == Vector3.zero) 
            defaultOffset = transform.position - player.position;

        // Calculam directia si distanta maxima bazata pe offset
        directionNormalized = defaultOffset.normalized;
        currentDistance = defaultOffset.magnitude;

        // Setup Pivot
        pivot = new GameObject("CameraPivot").transform;
        pivot.position = player.position;
        pivot.rotation = Quaternion.identity;

        transform.SetParent(pivot);
        // Important: Resetam pozitia locala la start
        transform.localPosition = defaultOffset;
        transform.LookAt(player.position);
    }

    void LateUpdate()
    {
        HandleCamera();
        HandleWallCollision(); // Functia noua
    }

    void HandleCamera()
    {
        // Pivotul urmareste jucatorul
        pivot.position = player.position;

        // Input Mouse
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed; // scos Time.deltaTime pt mouse raw input, poti pune la loc daca vrei
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        // Rotim pivotul
        pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
        
        // Asiguram ca rotatia camerei se uita mereu spre pivot/player
        transform.LookAt(pivot);
    }

    void HandleWallCollision()
    {

        float targetDistance = defaultOffset.magnitude;
        

        Vector3 worldDirection = pivot.TransformDirection(directionNormalized);

        RaycastHit hit;


        if (Physics.SphereCast(pivot.position, cameraCollisionRadius, worldDirection, out hit, targetDistance, collisionLayers))
        {
            currentDistance = hit.distance - cameraCollisionOffset;
        }
        else
        {

            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * collisionDamp);
        }

        if (currentDistance < 0.2f) currentDistance = 0.2f;

        transform.localPosition = directionNormalized * currentDistance;
    }
}