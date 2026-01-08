using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Resurse Vitale")]
    public float maxHealth = 100;
    public float healthRegeneration = 1f;
    public float maxMana = 50;
    public float manaRegeneration = 0.5f;
    
    [Header("Atribute Principale")]
    public int strength = 10;
    public int intelligence = 10;
    public float movementSpeed = 5f;

    [Header("Combat")]
    public float attackPower = 15f;
    public float critChance = 5f;   // Procent 0-100
    public float critDamage = 50f;  // Procent bonus (50 = 150% dmg total)
    public float attackSpeed = 1f;

    [Header("Progresie")]
    public int level = 1;
    public float experience = 0;
    public float nextLevelXP = 100;
}