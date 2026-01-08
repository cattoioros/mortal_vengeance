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
    public string skillId; // unique id
    public string skillName;
    public string description;
    public List<StatBonus> bonuses = new List<StatBonus>();
    public int skillPointCost = 1;
    public List<string> prerequisiteSkillIds = new List<string>(); // skills that must be unlocked first
}

public class SkillData : MonoBehaviour
{
    public static SkillData instance { get; private set; }
    
    public List<Skill> allSkills = new List<Skill>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        if (allSkills.Count == 0)
        {
            InitializeSkills();
        }
    }

    private void InitializeSkills()
    {
        // strength 
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

        // intelligence 
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

        // dexterity
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
        return allSkills.Find(s => s.skillId == skillId);
    }
}