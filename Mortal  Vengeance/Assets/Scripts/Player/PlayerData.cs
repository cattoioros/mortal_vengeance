using System;
using UnityEngine; // Avem nevoie de asta pentru Vector3 sau acces la transform

[Serializable]
public class PlayerData
{
    // --- STATUSURI CURENTE ---
    public float currentHealth;
    public float currentMana;
    public float[] position; // x, y, z

    // --- STATUSURI DIN PLAYERSTATS ---
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

    // Constructor gol (necesar pentru citirea din JSON)
    public PlayerData() {}

    // Constructorul "Inteligent" care preia datele din Manager
    public PlayerData(PlayerStatsManager manager)
    {
        // 1. Luam datele dinamice din Manager
        currentHealth = manager.currentHealth;
        currentMana = manager.currentMana;

        // 2. Luam pozitia jucatorului
        position = new float[3];
        position[0] = manager.transform.position.x;
        position[1] = manager.transform.position.y;
        position[2] = manager.transform.position.z;

        // 3. Luam datele statice din PlayerStats (Scriptul atasat)
        PlayerStats s = manager.stats;

        maxHealth = s.maxHealth;
        healthRegeneration = s.healthRegeneration;
        maxMana = s.maxMana;
        manaRegeneration = s.manaRegeneration;

        strength = s.strength;
        movementSpeed = s.movementSpeed;
        intelligence = s.intelligence;

        attackPower = s.attackPower;
        critChance = s.critChance;
        critDamage = s.critDamage;
        attackSpeed = s.attackSpeed;

        level = s.level;
        experience = s.experience;
        nextLevelXP = s.nextLevelXP;
    }
}