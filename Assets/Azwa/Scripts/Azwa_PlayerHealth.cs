using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class Azwa_PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Invincibility Frames (I-Frames)")]
    public float invincibilityDuration = 1f;
    private bool isInvincible = false;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText; // Assign this in Inspector

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // Initialize UI
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        UpdateHealthUI(); // Update the UI

        if (currentHealth <= 0)
        {
            Debug.Log("Player has died");
            Die();
            return;
        }

        if (!isInvincible)
        {
            StartCoroutine(InvincibilityFrames());
        }
    }

    void UpdateHealthUI()
    {
        healthText.text = "Health: " + currentHealth.ToString("0"); // Display as whole number
    }

    void Die()
    {
        Debug.Log("Player is dead, changing scene...");
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
