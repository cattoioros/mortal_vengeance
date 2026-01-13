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

    private void Awake()
    {
        // Singleton: skills are treated as a shared catalog.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        if (allSkills.Count == 0)
        {
            // Keep a default set so the UI can work in a fresh scene.
            InitializeSkills();
        }
    }

    private void InitializeSkills()
    {
        // This is a simple in-code catalog; later you can replace with ScriptableObjects.
        // Strength
        allSkills.Add(new Skill
        {
            skillId = "str_health_1",
            skillName = "Healthy",
            description = "Increase max health by 20",
            bonuses = new List<StatBonus> { new StatBonus { statName = "maxHealth", value = 20 } }
        });

        allSkills.Add(new Skill
        {
            skillId = "str_health_2",
            skillName = "Toughness",
            description = "Increase max health by 40",
            bonuses = new List<StatBonus> { new StatBonus { statName = "maxHealth", value = 40 } },
            prerequisiteSkillIds = new List<string> { "str_health_1" }
        });

        allSkills.Add(new Skill
        {
            skillId = "str_damage_1",
            skillName = "Strong",
            description = "Increase damage by 5",
            bonuses = new List<StatBonus> { new StatBonus { statName = "attackPower", value = 5 } }
        });

        // Intelligence
        allSkills.Add(new Skill
        {
            skillId = "int_mana_1",
            skillName = "Meditate",
            description = "Increase max mana by 20",
            bonuses = new List<StatBonus> { new StatBonus { statName = "maxMana", value = 20 } }
        });

        allSkills.Add(new Skill
        {
            skillId = "int_mana_regen_1",
            skillName = "Mana Flow",
            description = "Increase mana regen by 1",
            bonuses = new List<StatBonus> { new StatBonus { statName = "manaRegeneration", value = 1 } }
        });

        // Dexterity
        allSkills.Add(new Skill
        {
            skillId = "dex_crit_1",
            skillName = "Precision",
            description = "Increase crit chance by 5%",
            bonuses = new List<StatBonus> { new StatBonus { statName = "critChance", value = 5 } }
        });

        allSkills.Add(new Skill
        {
            skillId = "dex_crit_2",
            skillName = "Deadly",
            description = "Increase crit damage by 20%",
            bonuses = new List<StatBonus> { new StatBonus { statName = "critDamage", value = 20 } },
            prerequisiteSkillIds = new List<string> { "dex_crit_1" }
        });
    }

    public Skill GetSkill(string skillId)
    {
        // Linear lookup is fine for small lists; switch to a dictionary if this grows a lot.
        return allSkills.Find(s => s.skillId == skillId);
    }
}