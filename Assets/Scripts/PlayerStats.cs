using UnityEngine;
using UnityEngine.UI; // For UI components

public class PlayerStats : MonoBehaviour
{
    [Header("Experience Points")]
    public int currentXP = 0; // Player's current experience points
    public int level = 1; // Player's current level
    public Slider xpBar; // Reference to the XP bar slider
    private LevelUIController levelUIController;
    private GunSystem gunSystem; // Reference to GunSystem
    private PlayerMovement playerMovement; // Reference to PlayerMovement
    private Bullet bullet; // Reference to Bullet (for damage upgrades)

    [Header("Health")]
    public int maxHP = 5; // Maximum health points
    public int currentHP; // Current health points

    [Header("Invincibility")]
    public float invincibilityDuration = 1f; // Duration of invincibility after taking damage
    private bool isInvincible = false; // Tracks if the player is invincible
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer for visual feedback
    private Color defaultColor; // Default color of the sprite

    private int xpToNextLevel; // XP required to level up


    private void Start()
    {
        currentHP = maxHP; // Initialize health to maximum
        xpToNextLevel = CalculateXPToNextLevel(level); // Calculate XP needed for the first level-up

        levelUIController = FindObjectOfType<LevelUIController>(); // Find the UI controller
        gunSystem = FindObjectOfType<GunSystem>(); // Find the GunSystem
        playerMovement = FindObjectOfType<PlayerMovement>(); // Find the PlayerMovement script

        // Initialize the XP bar
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel; // Set the slider's max value
            xpBar.value = currentXP; // Set the current value
        }

        // Cache the SpriteRenderer and its default color
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// Adds experience points to the player.
    /// </summary>
    /// <param name="amount">The amount of XP to add.</param>
    public void AddXP(int amount)
    {
        currentXP += amount;
        Debug.Log($"Gained {amount} XP. Total XP: {currentXP}/{xpToNextLevel}");

        // Update the XP bar
        if (xpBar != null)
        {
            xpBar.value = currentXP; // Update the slider's value
        }

        // Check if the player has enough XP to level up
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    /// <summary>
    /// Handles leveling up the player.
    /// </summary>
    private void LevelUp()
    {
        currentXP -= xpToNextLevel; // Deduct XP for the current level
        level++; // Increase the player's level
        xpToNextLevel = CalculateXPToNextLevel(level); // Calculate XP for the next level

        Debug.Log($"Level Up! New Level: {level}. XP needed for next level: {xpToNextLevel}");

        // Update the XP bar
        if (xpBar != null)
        {
            xpBar.maxValue = xpToNextLevel; // Update the slider's max value
            xpBar.value = currentXP; // Update the current value
        }

        // FULL HEAL on level up
        currentHP = maxHP;
        Debug.Log($"Level Up Healing: HP fully restored! Current HP: {currentHP}/{maxHP}");


        // Randomly increase a gun stat, movement speed, or bullet damage
        if (gunSystem != null || playerMovement != null || bullet != null)
        {
            UpgradeRandomStat();
        }


        // Ensure UI updates when leveling up
        if (levelUIController != null)
        {
            levelUIController.UpdateLevelUI();
        }
    }

    /// <summary>
    /// Calculates the XP required to reach the next level.
    /// </summary>
    /// <param name="level">The current level.</param>
    /// <returns>The XP required to level up.</returns>
    private int CalculateXPToNextLevel(int level)
    {
        if (level >= 1 && level <= 19)
        {
            return 10 * level - 5;
        }
        else if (level == 20)
        {
            return 16 * level - 8;
        }
        else if (level >= 21 && level <= 39)
        {
            return 13 * level - 6;
        }
        else if (level >= 40 && level <= 59)
        {
            return 16 * level - 8;
        }
        else if (level >= 60)
        {
            return level * level;
        }

        return 0; // Fallback for invalid levels
    }

    /// <summary>
    /// Reduces player health when damage is taken.
    /// </summary>
    /// <param name="damage">The amount of damage to deal to the player.</param>
    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // Prevent damage if invincible

        currentHP -= damage;
        Debug.Log($"Player took {damage} damage. Current HP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(ActivateInvincibility()); // Start invincibility coroutine
        }
    }

    /// <summary>
    /// Handles player death.
    /// </summary>
    private void Die()
    {
        Debug.Log("Player has died!");
        // Implement death logic here (e.g., game over screen, respawn)
    }

    /// <summary>
    /// Activates invincibility for a short duration.
    /// </summary>
    private System.Collections.IEnumerator ActivateInvincibility()
    {
        isInvincible = true;

        // Optional: Flash the sprite to indicate invincibility
        if (spriteRenderer != null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < invincibilityDuration)
            {
                spriteRenderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.5f); // Half transparency
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = defaultColor; // Restore color
                yield return new WaitForSeconds(0.1f);
                elapsedTime += 0.2f;
            }
        }

        isInvincible = false;
    }

    /// <summary>
    /// Upgrades a random stat in GunSystem, PlayerMovement, or Bullet when the player levels up.
    /// </summary>
    /// <summary>
    /// Upgrades multiple random stats in GunSystem, PlayerMovement, or Bullet when the player levels up.
    /// </summary>
    private void UpgradeRandomStat()
    {
        int numUpgrades = Random.Range(3, 6); // Upgrade 3 to 5 stats at once

        for (int i = 0; i < numUpgrades; i++)
        {
            int randomStat = Random.Range(0, 6); // Select a random stat (0 to 5)

            switch (randomStat)
            {
                case 0: // Max Ammo Boost
                    int maxAmmoIncrease = Random.value < 0.5f ? 20 : 15; // 50% chance for +20, otherwise +15
                    gunSystem.maxAmmo += maxAmmoIncrease;
                    Debug.Log($"Level Up Reward: Max Ammo increased to {gunSystem.maxAmmo}!");
                    break;

                case 1: // Fire Rate Improvement
                    float fireRateMultiplier = Random.value < 0.6f ? 0.6f : 0.75f; // 60% chance for a major boost
                    gunSystem.fireRate *= fireRateMultiplier;
                    Debug.Log($"Level Up Reward: Fire Rate improved to {gunSystem.fireRate:F2} seconds per shot!");
                    break;

                case 2: // Reload Speed Boost
                    float reloadReduction = Random.value < 0.6f ? 0.7f : 0.8f; // 60% chance for stronger reload speed boost
                    gunSystem.reloadTime *= reloadReduction;
                    Debug.Log($"Level Up Reward: Reload Time reduced to {gunSystem.reloadTime:F2} seconds!");
                    break;

                case 3: // Bullet Speed Increase
                    float bulletSpeedIncrease = Random.value < 0.6f ? 12f : 8f; // 60% chance for a bigger boost
                    gunSystem.bulletSpeed += bulletSpeedIncrease;
                    Debug.Log($"Level Up Reward: Bullet Speed increased to {gunSystem.bulletSpeed}!");
                    break;

                case 4: // Movement Speed Boost
                    float movementBoost = Random.value < 0.5f ? 2.0f : 1.5f; // 50% chance for a bigger movement boost
                    if (playerMovement != null)
                    {
                        playerMovement.moveSpeed += movementBoost;
                        Debug.Log($"Level Up Reward: Movement Speed increased to {playerMovement.moveSpeed}!");
                    }
                    break;

                case 5: // Bullet Damage Boost
                    if (bullet != null)
                    {
                        if (Random.value < 0.5f) // 50% chance to **double** bullet damage
                        {
                            bullet.damage *= 2;
                            Debug.Log($"?? CRITICAL Level Up Reward: Bullet Damage DOUBLED to {bullet.damage}!");
                        }
                        else
                        {
                            bullet.damage += 10; // Otherwise, +10 base damage
                            Debug.Log($"Level Up Reward: Bullet Damage increased to {bullet.damage}!");
                        }
                    }
                    break;
            }
        }
    }



}
