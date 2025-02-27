using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Azwa_PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Invincibility Frames (I-Frames)")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        // Reduce health first
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died");
            Die();
            return; // Prevent further execution
        }

        // Only apply invincibility if the player is still alive
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    void Die()
    {
        // Debug log for verification
        Debug.Log("Player is dead, changing scene...");

        // Load the death scene (Scene index 6)
        SceneManager.LoadScene(6);
    }

    private IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss")) // Check if the collided object has the "Boss" tag
        {
            TakeDamage(10f); // Adjust damage as needed
            Debug.Log("Player took damage from Boss");
        }
    }
}
