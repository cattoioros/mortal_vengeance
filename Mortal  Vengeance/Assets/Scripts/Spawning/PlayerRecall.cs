using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerRecall : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private Key recallKey = Key.B;

    [Header("Recall")]
    [Tooltip("If true, also set the player's rotation to match the spawn point.")]
    [SerializeField] private bool applyRotation = true;

    [Tooltip("Optional: if true, shows a short UI message when recalling.")]
    [SerializeField] private bool showMessage = true;

    [Tooltip("Only used when Show Message is true.")]
    [SerializeField] private float messageSeconds = 1.25f;

    private CharacterController characterController;
    private Transform cachedDefaultSpawn;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        cachedDefaultSpawn = FindDefaultSpawnPoint();
    }

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current[recallKey].wasPressedThisFrame)
        {
            Recall();
        }
    }

    public void Recall()
    {
        if (cachedDefaultSpawn == null)
        {
            // Spawn point can appear later (scene load) so we re-try if missing.
            cachedDefaultSpawn = FindDefaultSpawnPoint();
        }

        if (cachedDefaultSpawn == null)
        {
            Debug.LogWarning("PlayerRecall: Default spawn point not found (add a PlayerSpawnPoint with Is Default = true, or name an object 'DefaultSpawnPoint').", this);
            return;
        }

        // Clear any lingering prompt UI.
        TeleportStatusUI.Hide();

        if (showMessage)
        {
            TeleportStatusUI.Show($"Recalling to: {cachedDefaultSpawn.name}", messageSeconds);
        }

        // Disable CharacterController to avoid collision issues during teleport.
        if (characterController != null) characterController.enabled = false;

        if (applyRotation)
        {
            transform.SetPositionAndRotation(cachedDefaultSpawn.position, cachedDefaultSpawn.rotation);
        }
        else
        {
            transform.position = cachedDefaultSpawn.position;
        }

        if (characterController != null) characterController.enabled = true;
    }

    private static Transform FindDefaultSpawnPoint()
    {
        // Prefer explicit PlayerSpawnPoint markers; fallback to a known name.
        var spawnPoints = Object.FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i] != null && spawnPoints[i].IsDefault)
                return spawnPoints[i].transform;
        }

        var byName = GameObject.Find("DefaultSpawnPoint");
        return byName != null ? byName.transform : null;
    }
}
