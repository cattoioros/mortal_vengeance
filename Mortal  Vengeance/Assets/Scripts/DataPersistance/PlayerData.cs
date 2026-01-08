using System.IO;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public class PlayerData
{
    public float health;
    public float healthRegeneration;
    public float mana;
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

    public float[] position;

    public static PlayerData CreatePlayerData(Transform player, float health, float healthRegeneration, float manaRegeneration, int strenght,
        float movementSpeed, int intelligence, float attackPower, float critChance, float critDamage, float attackSpeed, int level, float experience, float nextLevelXP)
    {
        PlayerData data = new()
        {
            level = level,
            experience = experience,
            manaRegeneration = manaRegeneration,
            nextLevelXP = nextLevelXP,
            health = health,
            healthRegeneration = healthRegeneration,
            strength = strenght,
            movementSpeed = movementSpeed,
            intelligence = intelligence,
            attackPower = attackPower,
            critChance = critChance,
            critDamage = critDamage,
            attackSpeed = attackSpeed,
            

            position = new float[3]
        };
        data.position[0] = player.position.x;
        data.position[1] = player.position.y;
        data.position[2] = player.position.z;

        return data;
    }

    
}


