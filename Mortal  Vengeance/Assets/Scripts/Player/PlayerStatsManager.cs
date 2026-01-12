using UnityEngine;
using UnityEngine.UI;
public class PlayerStatsManager : MonoBehaviour
{
    public PlayerStats stats;


    public float currentHealth;
    public float currentMana;
    public float currentXP;

    [Header("UI")]
    public Slider playerHealthSlider;

    void Awake()
    {
        if (stats == null) stats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        // Poti decomenta linia de mai jos daca vrei sa incarci automat la start
        // LoadGame(); 
        
        if (currentHealth <= 0 && stats.maxHealth > 0)
        {
            currentHealth = stats.maxHealth;
            currentMana = stats.maxMana;
        }
    }

    void Update()
    {
        HandleRegeneration();

        // Exemplu rapid de testare (sterge in varianta finala)
        if (Input.GetKeyDown(KeyCode.F5)) SaveGame();
        if (Input.GetKeyDown(KeyCode.F9)) LoadGame();
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Folosim metodele tale de calcul procentual pentru a seta slider-ele (0 la 1)
        if (playerHealthSlider != null) playerHealthSlider.value = GetHealthPercent();
       
    }

    public void SaveGame()
{
    // Aici folosim constructorul inteligent care copiaza totul singur
    PlayerData data = new PlayerData(this);
    SaveSystem.Save(data);
    
    Debug.Log("Joc salvat complet (Stats + Pozitie).");
}

public void LoadGame()
{
    PlayerData data = SaveSystem.Load();

    if (data != null)
    {
        // 1. Incarcam valorile curente
        currentHealth = data.currentHealth;
        currentMana = data.currentMana;

        // 2. Incarcam pozitia
        if (data.position != null && data.position.Length == 3)
        {
            Vector3 newPos;
            newPos.x = data.position[0];
            newPos.y = data.position[1];
            newPos.z = data.position[2];
            
            // Trebuie sa dezactivam CharacterController temporar pentru teleport
            CharacterController cc = GetComponent<CharacterController>();
            if(cc != null) cc.enabled = false;
            
            transform.position = newPos;
            
            if(cc != null) cc.enabled = true;
        }

        // 3. Incarcam stats-urile in PlayerStats
        stats.maxHealth = data.maxHealth;
        stats.healthRegeneration = data.healthRegeneration;
        stats.maxMana = data.maxMana;
        stats.manaRegeneration = data.manaRegeneration;

        stats.strength = data.strength;
        stats.movementSpeed = data.movementSpeed;
        stats.intelligence = data.intelligence;

        stats.attackPower = data.attackPower;
        stats.critChance = data.critChance;
        stats.critDamage = data.critDamage;
        stats.attackSpeed = data.attackSpeed;

        stats.level = data.level;
        stats.experience = data.experience;
        stats.nextLevelXP = data.nextLevelXP;

        Debug.Log("Joc incarcat cu succes!");
    }
}

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log("Player lovitt" + Time.time);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, stats.maxHealth);
    }

    public bool CanAffordMana(float amount)
    {
        return currentMana >= amount;
    }

    public void UseMana(float amount)
    {
        if (CanAffordMana(amount))
        {
            currentMana -= amount;
        }
    }

    private void HandleRegeneration()
    {
        if (currentHealth < stats.maxHealth)
        {
            currentHealth += stats.healthRegeneration * Time.deltaTime;
            currentHealth = Mathf.Min(currentHealth, stats.maxHealth);
        }

        if (currentMana < stats.maxMana)
        {
            currentMana += stats.manaRegeneration * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, stats.maxMana);
        }
    }

    public float CalculateAttackDamage(out bool isCrit)
    {
        isCrit = false;
        float finalDamage = stats.attackPower;

        if (Random.Range(0, 100) <= stats.critChance)
        {
            isCrit = true;
            finalDamage *= (1 + (stats.critDamage / 100));
        }

        return finalDamage;
    }

    public void AddExperience(float amount)
    {
        currentXP += amount;
        stats.experience = currentXP;

        if (currentXP >= stats.nextLevelXP)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= stats.nextLevelXP;
        stats.level++;
        stats.nextLevelXP = Mathf.Round(stats.nextLevelXP * 1.2f);
        
        currentHealth = stats.maxHealth;
        currentMana = stats.maxMana;
    }

    private void Die()
    {
        Debug.Log("Ai murit." + Time.time);
    }

    public float GetHealthPercent()
    {
        if (stats.maxHealth <= 0) return 0;
        return currentHealth / stats.maxHealth;
    }
        public float GetManaPercent() => currentMana / stats.maxMana;
    public float GetXPPercent() => currentXP / stats.nextLevelXP;
}