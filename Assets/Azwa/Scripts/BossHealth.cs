using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Health Settings")]
    public float maxHealth = 500f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;

    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {

        // Disable boss components (e.g., AI, Collider)
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        // Destroy the boss after animation plays
        Destroy(gameObject, 2f);
    }
}
