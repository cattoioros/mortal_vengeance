using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Tooltip("Used by DefaultPlayerSpawner/PlayerRecall when no explicit spawn is assigned.")]
    [SerializeField] private bool isDefault = true;

    // Multiple spawn points are allowed; the first IsDefault found will be used.

    public bool IsDefault => isDefault;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Visualize spawn direction in the scene view.
        Gizmos.color = isDefault ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.75f);
    }
#endif
}
