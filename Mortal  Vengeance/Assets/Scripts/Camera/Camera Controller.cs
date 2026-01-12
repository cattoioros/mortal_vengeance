using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform player;
    public Vector3 defaultOffset; // Offset-ul maxim dorit (cand nu sunt pereti)

    [Tooltip("Tag used to auto-find the player if the reference is missing/destroyed.")]
    public string playerTag = "Player";

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

    private bool initialized;

    void Start()
    {
        EnsurePlayer();
        if (player != null)
        {
            InitializeIfNeeded();
        }
    }

    void LateUpdate()
    {
        EnsurePlayer();
        if (player == null) return;
        InitializeIfNeeded();

        HandleCamera();
        HandleWallCollision(); 
    }

    private void EnsurePlayer()
    {
        if (player != null) return;

        if (GameManager.instance != null && GameManager.instance.PlayerTransform != null)
        {
            player = GameManager.instance.PlayerTransform;
            return;
        }

        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null) player = playerObj.transform;
    }

    private void InitializeIfNeeded()
    {
        if (initialized) return;

        if (pivot == null)
        {
            pivot = new GameObject("CameraPivot").transform;
            pivot.rotation = Quaternion.identity;
        }

        // Setam offset-ul initial daca nu e setat in inspector
        if (defaultOffset == Vector3.zero)
            defaultOffset = transform.position - player.position;

        // Calculam directia si distanta maxima bazata pe offset
        directionNormalized = defaultOffset.normalized;
        currentDistance = defaultOffset.magnitude;

        pivot.position = player.position;

        transform.SetParent(pivot);
        // Important: Resetam pozitia locala la start
        transform.localPosition = defaultOffset;
        transform.LookAt(player.position);

        initialized = true;
    }

    void HandleCamera()
    {
        // Pivotul urmareste jucatorul
        pivot.position = player.position;

        // Input Mouse
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
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