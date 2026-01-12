using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0); 
    private Transform cam;
    private Transform enemy;

    void Start()
    {
        cam = Camera.main.transform;

        enemy = transform.parent;

        if (healthSlider == null)
        {
            healthSlider = GetComponentInChildren<Slider>();
        }
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        if (healthSlider == null) return;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        gameObject.SetActive(currentHealth < maxHealth);
    }

    void LateUpdate()
    {
        if (cam == null) return;

        if (enemy != null)
        {
            transform.position = enemy.position + offset;
        }

        transform.LookAt(transform.position + cam.forward);
    }
}