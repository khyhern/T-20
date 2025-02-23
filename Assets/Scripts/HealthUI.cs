using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public PlayerStats playerStats; // Reference to PlayerStats
    public TextMeshProUGUI healthText; // UI Text for displaying HP values

    private void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>(); // Auto-find PlayerStats if not assigned
        }

        UpdateHealthUI();
    }

    private void Update()
    {
        if (playerStats != null)
        {
            UpdateHealthUI();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"{playerStats.currentHP} / {playerStats.maxHP}"; // Format: "5 / 5"
        }
    }
}
