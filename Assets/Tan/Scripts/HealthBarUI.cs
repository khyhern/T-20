using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Image mask; // The radial fill mask
    private PlayerScript player;

    void Start()
    {
        player = Object.FindAnyObjectByType<PlayerScript>();

        if (player == null)
        {
            Debug.LogError("PlayerScript not found in the scene!");
            return;
        }

        UpdateHealthBar();
    }

    void Update()
    {
        if (player != null)
        {
            UpdateHealthBar();
        }
    }

    void UpdateHealthBar()
    {
        if (mask != null && player.maxHealth > 0)
        {
            mask.fillAmount = (float)player.playerHealthPoints / player.maxHealth;
        }
    }
}
