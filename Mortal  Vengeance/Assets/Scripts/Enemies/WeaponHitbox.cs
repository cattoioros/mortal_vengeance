using System.Collections.Generic;
using UnityEngine;

public class WeaponHitbox : MonoBehaviour
{
    public float damage = 10f;
    public float hitCooldown = 0.5f; 
    private bool active = false;

    private Dictionary<GameObject, float> lastHitTime = new Dictionary<GameObject, float>();

    private void OnTriggerStay(Collider other)
    {
        if (!active) return;
        if (!other.CompareTag("Player")) return;

        GameObject go = other.gameObject;
        float now = Time.time;
        if (lastHitTime.TryGetValue(go, out float last) && now - last < hitCooldown)
            return;

        if (other.TryGetComponent<PlayerStatsManager>(out var ph))
        {
            
            ph.TakeDamage((int)damage);
            lastHitTime[go] = now;
        }
    }

    public void ActivateHitbox()
    {
        active = true;
        GetComponent<Collider>().enabled = true;
    }

    public void DeactivateHitbox()
    {
        active = false;
        GetComponent<Collider>().enabled = false;
        lastHitTime.Clear();
    }
}
