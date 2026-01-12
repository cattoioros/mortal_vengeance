using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Offset")]
    public Transform player;
    public Vector3 defaultOffset;

    [Tooltip("Tag used to auto-find the player if the reference is missing/destroyed.")]
    public string playerTag = "Player";

    [Header("Rotation Settings")]
    public float rotationSpeed = 2.0f;
    public float minYAngle = -35f;
    public float maxYAngle = 60f;

    [Header("Collision Settings")]
    public LayerMask collisionLayers; 
    public float cameraCollisionRadius = 0.2f;
    public float collisionDamp = 10f; 
    public float cameraCollisionOffset = 0.2f;

    // Variabilă publică în caz că vrei să blochezi camera manual din alt script
    [Header("State")]
    public bool lockCameraRotation = false; 

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

        directionNormalized = defaultOffset.normalized;
        currentDistance = defaultOffset.magnitude;

        pivot = new GameObject("CameraPivot").transform;
        pivot.position = player.position;

        transform.SetParent(pivot);
        transform.localPosition = defaultOffset;
        transform.LookAt(player.position);
        
        // Ascundem cursorul la start (opțional, dacă vrei să înceapă ascuns)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        HandleCamera();
        HandleWallCollision();
    }

    void HandleCamera()
    {
        // 1. Pivotul urmărește mereu jucătorul (ca să nu rămână camera în urmă)
        pivot.position = player.position;

        // 2. VERIFICARE CRITICĂ: 
        // Dacă cursorul e vizibil (ești în inventar) SAU camera e blocată manual -> NU rotim
        if (Cursor.visible || lockCameraRotation) return;

        // --- Doar dacă trecem de verificare, calculăm rotația ---
        
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);

        pivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
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