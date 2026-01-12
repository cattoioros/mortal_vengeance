using System.Collections.Generic;
using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager instance { get; private set; }

    public int availableSkillPoints = 0; // to be modified(base value for testing)
    private HashSet<string> unlockedSkills = new HashSet<string>();

    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private PlayerStatsManager playerStatsManager;

    private PlayerStatsSnapshot baseStats;
    private bool hasBaseStats;

    private struct PlayerStatsSnapshot
    {
        public float maxHealth;
        public float healthRegeneration;
        public float maxMana;
        public float manaRegeneration;

        public int strength;
        public float movementSpeed;
        public int intelligence;

        public float attackPower;
        public float critChance;
        public float critDamage;
        public float attackSpeed;

        public int level;
        public float experience;
        public float nextLevelXP;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        // Respect the Inspector assignment; only auto-find if missing.
        if (playerStats == null)
        {
            playerStats = GetComponent<PlayerStats>();
            if (playerStats == null)
            {
                playerStats = FindAnyObjectByType<PlayerStats>();
            }
        }

        if (playerStatsManager == null)
        {
            playerStatsManager = GetComponent<PlayerStatsManager>();
            if (playerStatsManager == null)
            {
                playerStatsManager = FindAnyObjectByType<PlayerStatsManager>();
            }
        }

        if (playerStats != null)
        {
            CacheBaseStats();
        }
    }

    private void CacheBaseStats()
    {
        baseStats = new PlayerStatsSnapshot
        {
            maxHealth = playerStats.maxHealth,
            healthRegeneration = playerStats.healthRegeneration,
            maxMana = playerStats.maxMana,
            manaRegeneration = playerStats.manaRegeneration,
            strength = playerStats.strength,
            movementSpeed = playerStats.movementSpeed,
            intelligence = playerStats.intelligence,
            attackPower = playerStats.attackPower,
            critChance = playerStats.critChance,
            critDamage = playerStats.critDamage,
            attackSpeed = playerStats.attackSpeed,
            level = playerStats.level,
            experience = playerStats.experience,
            nextLevelXP = playerStats.nextLevelXP
        };
        hasBaseStats = true;
    }

    public bool TryUnlockSkill(string skillId)
    {
        Skill skill = SkillData.instance.GetSkill(skillId);
        
        if (skill == null)
        {
            Debug.LogError("Skill not found: " + skillId);
            return false;
        }

        // already unlocked check
        if (unlockedSkills.Contains(skillId))
        {
            Debug.Log("Skill already unlocked: " + skillId);
            return false;
        }

        // skill points check
        if (availableSkillPoints < skill.skillPointCost)
        {
            Debug.Log("Not enough skill points");
            return false;
        }

        // prerequisites check
        foreach (string prereqId in skill.prerequisiteSkillIds)
        {
            if (!unlockedSkills.Contains(prereqId))
            {
                Debug.Log("Prerequisite not met: " + prereqId);
                return false;
            }
        }

        // skill unlock
        unlockedSkills.Add(skillId);
        availableSkillPoints -= skill.skillPointCost;
        ApplySkillBonuses();

        Debug.Log("Skill unlocked: " + skill.skillName);
        return true;
    }

    private void ApplySkillBonuses()
    {
        if (playerStats == null) return;

        if (!hasBaseStats)
        {
            CacheBaseStats();
        }

        float oldMaxHealth = playerStats.maxHealth;
        float oldMaxMana = playerStats.maxMana;

        // Reset to base stats then re-apply all unlocked bonuses.
        playerStats.maxHealth = baseStats.maxHealth;
        playerStats.healthRegeneration = baseStats.healthRegeneration;
        playerStats.maxMana = baseStats.maxMana;
        playerStats.manaRegeneration = baseStats.manaRegeneration;
        playerStats.strength = baseStats.strength;
        playerStats.movementSpeed = baseStats.movementSpeed;
        playerStats.intelligence = baseStats.intelligence;
        playerStats.attackPower = baseStats.attackPower;
        playerStats.critChance = baseStats.critChance;
        playerStats.critDamage = baseStats.critDamage;
        playerStats.attackSpeed = baseStats.attackSpeed;
        
        foreach (string skillId in unlockedSkills)
        {
            Skill skill = SkillData.instance.GetSkill(skillId);
            
            foreach (StatBonus bonus in skill.bonuses)
            {
                ApplyBonus(bonus.statName, bonus.value);
            }
        }

        // Keep current values consistent with new max values.
        if (playerStatsManager != null)
        {
            // Preserve percent filled when max changes.
            if (oldMaxHealth > 0f)
            {
                float hpPercent = Mathf.Clamp01(playerStatsManager.currentHealth / oldMaxHealth);
                playerStatsManager.currentHealth = hpPercent * playerStats.maxHealth;
            }
            playerStatsManager.currentHealth = Mathf.Clamp(playerStatsManager.currentHealth, 0f, playerStats.maxHealth);

            if (oldMaxMana > 0f)
            {
                float manaPercent = Mathf.Clamp01(playerStatsManager.currentMana / oldMaxMana);
                playerStatsManager.currentMana = manaPercent * playerStats.maxMana;
            }
            playerStatsManager.currentMana = Mathf.Clamp(playerStatsManager.currentMana, 0f, playerStats.maxMana);
        }
    }

    private void ApplyBonus(string statName, float value)
    {
        switch (statName) // some of the skills need some further implementation(ex: mana, crit)
        {
            case "maxHealth":
                playerStats.maxHealth += value;
                break;
            case "maxMana":
                playerStats.maxMana += value;
                break;
            case "attackPower":
                playerStats.attackPower += value;
                break;
            case "critChance":
                playerStats.critChance += value;
                break;
            case "critDamage":
                playerStats.critDamage += value;
                break;
            case "manaRegeneration":
                playerStats.manaRegeneration += value;
                break;
            case "healthRegeneration":
                playerStats.healthRegeneration += value;
                break;
            case "attackSpeed":
                playerStats.attackSpeed += value;
                break;
        }
    }

    public bool IsSkillUnlocked(string skillId) => unlockedSkills.Contains(skillId);
    public int GetAvailablePoints() => availableSkillPoints;
    public HashSet<string> GetUnlockedSkills() => unlockedSkills;
}