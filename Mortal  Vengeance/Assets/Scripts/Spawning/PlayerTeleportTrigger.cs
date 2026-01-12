using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class PlayerTeleportTrigger : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private Transform destination;
    [SerializeField] private bool applyRotation = true;

    [Header("Activation")]
    [Tooltip("If true, teleports immediately when the player enters the trigger.")]
    [SerializeField] private bool teleportOnEnter = true;

    [Tooltip("If Teleport On Enter is false, the player must press this key while inside the trigger.")]
    [SerializeField] private Key interactKey = Key.E;

    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    private Transform currentPlayer;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        currentPlayer = other.transform;

        // Show message on enter (persistent until exit).
        TeleportStatusUI.ShowPersistent($"{gameObject.name} \n Press '{interactKey}' to Teleport", 0f);

        if (teleportOnEnter)
        {
            Teleport(currentPlayer);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentPlayer != null && other.transform == currentPlayer)
        {
            currentPlayer = null;
            TeleportStatusUI.Hide();
        }
    }

    private void Update()
    {
        if (teleportOnEnter) return;
        if (currentPlayer == null) return;
        if (destination == null) return;

        if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            Teleport(currentPlayer);
        }
    }

    public void Teleport(Transform player)
    {
        if (player == null)
        {
            Debug.LogWarning("PlayerTeleportTrigger: Player is null.", this);
            return;
        }

        if (destination == null)
        {
            Debug.LogWarning("PlayerTeleportTrigger: Destination not set.", this);
            return;
        }

        TeleportStatusUI.Hide();

        var cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        if (applyRotation)
        {
            player.SetPositionAndRotation(destination.position, destination.rotation);
        }
        else
        {
            player.position = destination.position;
        }

        if (cc != null) cc.enabled = true;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (destination == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, destination.position);
        Gizmos.DrawWireSphere(destination.position, 0.35f);
        Gizmos.DrawLine(destination.position, destination.position + destination.forward * 0.75f);
    }
#endif
}
