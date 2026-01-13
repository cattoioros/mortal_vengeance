using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTeleportTrigger))]
public class GrantSkillPointsOnTeleportOnce : MonoBehaviour
{
    [Header("Grant")]
    [SerializeField] private int skillPointsToGrant = 5;

    [Tooltip("Optional unique id. If set, grant happens once per session (and optionally across restarts).")]
    [SerializeField] private string grantKey;

    [Header("Optional")]
    [Tooltip("If true, also persists the one-time grant across game restarts using PlayerPrefs.")]
    [SerializeField] private bool rememberAcrossSessions = false;

    [Tooltip("Optional message shown when points are granted.")]
    [SerializeField] private bool showMessage = true;

    private static readonly HashSet<string> GrantedThisSession = new HashSet<string>(StringComparer.Ordinal);

    private PlayerTeleportTrigger teleportTrigger;
    private bool grantedForThisInstance;

    private void Reset()
    {
#if UNITY_EDITOR
        if (string.IsNullOrWhiteSpace(grantKey))
        {
            grantKey = Guid.NewGuid().ToString("N");
        }
#endif
    }

    [ContextMenu("Generate New Grant Key")]
    private void GenerateNewGrantKey()
    {
        grantKey = Guid.NewGuid().ToString("N");
    }

    private void Awake()
    {
        teleportTrigger = GetComponent<PlayerTeleportTrigger>();

        if (teleportTrigger != null)
        {
            // Hook teleport completion so the grant happens only when the player actually arrives.
            teleportTrigger.Teleported += OnTeleported;
        }
    }

    private void OnDestroy()
    {
        if (teleportTrigger != null)
        {
            teleportTrigger.Teleported -= OnTeleported;
        }
    }

    private void OnTeleported(Transform player)
    {
        if (skillPointsToGrant <= 0) return;

        // player is provided for context/future use; the grant is global via SkillTreeManager.

        // If no key, only grant once per component instance.
        if (string.IsNullOrWhiteSpace(grantKey))
        {
            if (grantedForThisInstance) return;
            grantedForThisInstance = true;
        }
        else
        {
            // With a key, also guard against scene reloads / re-instantiation.
            if (GrantedThisSession.Contains(grantKey)) return;

            if (rememberAcrossSessions)
            {
                // PlayerPrefs makes this "one-time" even across restarts (optional).
                string prefsKey = $"SkillPointGrantOnce::{grantKey}";
                if (PlayerPrefs.GetInt(prefsKey, 0) == 1) return;
                PlayerPrefs.SetInt(prefsKey, 1);
                PlayerPrefs.Save();
            }

            GrantedThisSession.Add(grantKey);
        }

        if (SkillTreeManager.instance == null)
        {
            Debug.LogWarning("GrantSkillPointsOnTeleportOnce: SkillTreeManager.instance is missing.", this);
            return;
        }

        SkillTreeManager.instance.AddSkillPoints(skillPointsToGrant);

        if (showMessage)
        {
            TeleportStatusUI.Show($"+{skillPointsToGrant} Skill Points", 1.5f);
        }
    }
}
