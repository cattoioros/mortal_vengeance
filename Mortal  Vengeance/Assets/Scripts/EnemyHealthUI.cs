using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}