using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerTeleportTrigger))]
public class GrantSkillPointsOnTeleportOnce : MonoBehaviour
{
    [Header("Grant")]
    [SerializeField] private int skillPointsToGrant = 5;

    [Tooltip("Unique id used to ensure this grant happens only once per play session (even if the object is recreated). Leave empty to use instance-only tracking.")]
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

        // Instance-only safety (covers cases where grantKey is empty).
        if (string.IsNullOrWhiteSpace(grantKey))
        {
            if (grantedForThisInstance) return;
            grantedForThisInstance = true;
        }
        else
        {
            // Session-wide safety (covers scene reloads / re-instantiation).
            if (GrantedThisSession.Contains(grantKey)) return;

            if (rememberAcrossSessions)
            {
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
