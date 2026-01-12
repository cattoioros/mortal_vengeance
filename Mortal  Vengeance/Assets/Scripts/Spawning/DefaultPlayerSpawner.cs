using UnityEngine;

public class DefaultPlayerSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [Tooltip("Where the player should appear when the scene starts. If not set, this script will try to auto-find a PlayerSpawnPoint marked as Default, or an object named 'DefaultSpawnPoint'.")]
    [SerializeField] private Transform defaultSpawnPoint;

    [Tooltip("If true, also set the player's rotation to match the spawn point.")]
    [SerializeField] private bool applyRotation = true;

    [Header("Player")]
    [Tooltip("Player tag used to find the player if GameManager is not available.")]
    [SerializeField] private string playerTag = "Player";

    [Tooltip("If true, teleports the player every time the scene loads. If false, only teleports when player is far away from the spawn.")]
    [SerializeField] private bool alwaysTeleportOnStart = true;

    [Tooltip("Only used when Always Teleport On Start is false.")]
    [SerializeField] private float teleportDistanceThreshold = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool verboseLogging = false;

    [Tooltip("How long (seconds) to wait for the Player/GameManager to be available before giving up.")]
    [SerializeField] private float findPlayerTimeoutSeconds = 2f;

    private bool didSpawn;

    private void Awake()
    {
        if (defaultSpawnPoint == null)
        {
            defaultSpawnPoint = TryAutoFindDefaultSpawnPoint();
        }

        if (defaultSpawnPoint == null)
        {
            Debug.LogWarning("DefaultPlayerSpawner: No spawn point assigned and auto-find failed.", this);
        }
    }

    private void Start()
    {
        if (didSpawn) return;
        StartCoroutine(SpawnRoutine());
    }

    private System.Collections.IEnumerator SpawnRoutine()
    {
        if (defaultSpawnPoint == null)
        {
            if (verboseLogging) Debug.LogWarning("DefaultPlayerSpawner: Spawn point is null; aborting.", this);
            yield break;
        }

        float endTime = Time.realtimeSinceStartup + Mathf.Max(0.01f, findPlayerTimeoutSeconds);
        Transform player = null;

        while (player == null && Time.realtimeSinceStartup < endTime)
        {
            player = TryFindPlayer();
            if (player == null)
                yield return null;
        }

        if (player == null)
        {
            Debug.LogError("DefaultPlayerSpawner: Player not found (check tag / GameManager / scene load order).", this);
            yield break;
        }

        if (!alwaysTeleportOnStart)
        {
            float distance = Vector3.Distance(player.position, defaultSpawnPoint.position);
            if (distance <= teleportDistanceThreshold)
            {
                if (verboseLogging) Debug.Log($"DefaultPlayerSpawner: Player already near spawn (distance {distance:0.00}), skipping.", this);
                didSpawn = true;
                yield break;
            }
        }

        if (verboseLogging)
        {
            Debug.Log($"DefaultPlayerSpawner: Teleporting '{player.name}' to '{defaultSpawnPoint.name}' at {defaultSpawnPoint.position}.", this);
        }

        TeleportPlayerTo(player, defaultSpawnPoint);
        didSpawn = true;
    }

    private Transform TryFindPlayer()
    {
        Transform player = null;

        if (GameManager.instance != null)
        {
            player = GameManager.instance.PlayerTransform;
        }

        if (player != null) return player;

        var playerObj = GameObject.FindGameObjectWithTag(playerTag);
        return playerObj != null ? playerObj.transform : null;
    }

    private Transform TryAutoFindDefaultSpawnPoint()
    {
        // Prefer explicit marker component if present.
        var spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null && spawnPoints[i].IsDefault)
                return spawnPoints[i].transform;
        }

        // Fallback: by name.
        var byName = GameObject.Find("DefaultSpawnPoint");
        return byName != null ? byName.transform : null;
    }

    public void TeleportPlayerTo(Transform player, Transform target)
    {
        if (player == null || target == null) return;

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        if (applyRotation)
        {
            player.SetPositionAndRotation(target.position, target.rotation);
        }
        else
        {
            player.position = target.position;
        }

        if (cc != null) cc.enabled = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (defaultSpawnPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(defaultSpawnPoint.position, 0.35f);
        Gizmos.DrawLine(defaultSpawnPoint.position, defaultSpawnPoint.position + defaultSpawnPoint.forward * 0.75f);
    }
#endif
}
