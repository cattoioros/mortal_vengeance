using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Tooltip("If true, DefaultPlayerSpawner will use this spawn point when no explicit spawn is assigned.")]
    [SerializeField] private bool isDefault = true;

    public bool IsDefault => isDefault;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = isDefault ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.35f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.75f);
    }
#endif
}
