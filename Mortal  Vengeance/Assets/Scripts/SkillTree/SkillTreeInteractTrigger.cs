using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class SkillTreeInteractTrigger : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Key interactKey = Key.E;
    [SerializeField] private string playerTag = "Player";

    [Tooltip("Optional: if set, uses this controller; otherwise uses SkillTreeUIController.Instance.")]
    [SerializeField] private SkillTreeUIController controller;

    private Transform currentPlayer;

    private void Reset()
    {
        var c = GetComponent<Collider>();
        if (c != null) c.isTrigger = true;
    }

    private void Awake()
    {
        if (controller == null) controller = SkillTreeUIController.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        currentPlayer = other.transform;

        TeleportStatusUI.ShowPersistent($"Press '{interactKey}' to open Skill Tree", 0f);
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
        if (currentPlayer == null) return;
        if (SystemMenuController.IsUIBlockingInput) return;

        if (Keyboard.current != null && Keyboard.current[interactKey].wasPressedThisFrame)
        {
            if (controller == null) controller = SkillTreeUIController.Instance;
            if (controller == null)
            {
                Debug.LogWarning("SkillTreeInteractTrigger: No SkillTreeUIController found in scene.", this);
                return;
            }

            TeleportStatusUI.Hide();
            currentPlayer = null; // prevent re-trigger if teleport/disable skips OnTriggerExit
            controller.Open();
        }
    }
}
