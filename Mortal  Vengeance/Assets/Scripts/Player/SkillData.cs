using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatBonus
{
    public string statName;
    public float value;
}

[System.Serializable]
public class Skill
{
    // Stable key used for unlocks/prereqs and UI mapping.
    public string skillId;
    public string skillName;
    public string description;
    public List<StatBonus> bonuses = new List<StatBonus>();
    public int skillPointCost = 1;
    // Skills that must be unlocked before this one.
    public List<string> prerequisiteSkillIds = new List<string>();
}

public class SkillData : MonoBehaviour
{
    public static SkillData instance { get; private set; }
    
    public List<Skill> allSkills = new List<Skill>();

    // The intended skill catalog for this project.
    // If the serialized/scene list differs (e.g., old int_/dex_ skills), we rebuild it on Awake.
    private static readonly HashSet<string> DefaultSkillIds = new HashSet<string>
    {
        "hlt_maxhealth_1",
        "hlt_maxhealth_2",
        "hlt_regen_1",
        "hlt_regen_2",
        "str_damage_1",
        "str_damage_2",
        "str_damage_3",
    };

    private void Awake()
    {
        // Singleton: skills are treated as a shared catalog.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;

        // Keep a default set so the UI can work in a fresh scene.
        // Also auto-upgrade any legacy/serialized catalogs so the skill tree shows only the intended 7 skills.
        if (allSkills.Count == 0 || NeedsDefaultCatalog(allSkills))
        {
            InitializeSkills();
        }
    }

    private static bool NeedsDefaultCatalog(List<Skill> skills)
    {
        if (skills == null) return true;

        // If the list differs from the intended ids, treat it as legacy/custom and rebuild.
        if (skills.Count != DefaultSkillIds.Count) return true;

        foreach (var skill in skills)
        {
            if (skill == null) return true;
            if (string.IsNullOrWhiteSpace(skill.skillId)) return true;
            if (!DefaultSkillIds.Contains(skill.skillId)) return true;
        }

        return false;
    }

    private void InitializeSkills()
    {
        // This is a simple in-code catalog; later you can replace with ScriptableObjects.
        // Clear first so re-initialization doesn't stack duplicates.
        allSkills.Clear();

        // Health (hlt_*)
        allSkills.Add(new Skill
        {
            skillId = "hlt_maxhealth_1",
            skillName = "Vitality I",
            description = "Increase max health by 20",
            bonuses = new List<StatBonus> { new StatBonus { statName = "maxHealth", value = 20 } }
        });

        allSkills.Add(new Skill
        {
            skillId = "hlt_maxhealth_2",
            skillName = "Vitality II",
            description = "Increase max health by 20",
            bonuses = new List<StatBonus> { new StatBonus { statName = "maxHealth", value = 20 } },
            prerequisiteSkillIds = new List<string> { "hlt_maxhealth_1" }
        });

        allSkills.Add(new Skill
        {
            skillId = "hlt_regen_1",
            skillName = "Regeneration I",
            description = "Increase health regeneration by 0.5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "healthRegeneration", value = 0.5f } }
        });

        allSkills.Add(new Skill
        {
            skillId = "hlt_regen_2",
            skillName = "Regeneration II",
            description = "Increase health regeneration by 0.5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "healthRegeneration", value = 0.5f } },
            prerequisiteSkillIds = new List<string> { "hlt_regen_1" }
        });

        // Strength (str_*)
        allSkills.Add(new Skill
        {
            skillId = "str_damage_1",
            skillName = "Power I",
            description = "Increase attack damage by 5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "attackPower", value = 5 } }
        });

        allSkills.Add(new Skill
        {
            skillId = "str_damage_2",
            skillName = "Power II",
            description = "Increase attack damage by 5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "attackPower", value = 5 } },
            prerequisiteSkillIds = new List<string> { "str_damage_1" }
        });

        allSkills.Add(new Skill
        {
            skillId = "str_damage_3",
            skillName = "Power III",
            description = "Increase attack damage by 5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "attackPower", value = 5 } },
            prerequisiteSkillIds = new List<string> { "str_damage_2" }
        });
    }

    public Skill GetSkill(string skillId)
    {
        // Linear lookup is fine for small lists; switch to a dictionary if this grows a lot.
        return allSkills.Find(s => s.skillId == skillId);
    }
}