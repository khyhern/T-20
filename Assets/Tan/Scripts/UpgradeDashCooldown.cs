using UnityEngine;

public class UpgradeDashCooldown : MonoBehaviour
{
    public float decreaseCooldown = 2f;
    private PlayerScript player;
    public GameObject upgradeMenu;


    void Start()
    {
        player = FindAnyObjectByType<PlayerScript>(); // Find the PlayerScript in the scene
    }

    public void ApplyUpgrade()
    {
        if (player != null)
        {
            player.dashCooldown -= decreaseCooldown; // Reduce dash cooldown by 2 seconds

            if (player.dashCooldown < 0.5f) // Set a minimum cooldown limit
            {
                player.dashCooldown = 0.5f;
            }

            CloseUpgradeMenu();

            Debug.Log("Dash cooldown upgraded! New cooldown: " + player.dashCooldown);
        }
        else
        {
            Debug.LogError("PlayerScript not found!");
        }
    }

    private void CloseUpgradeMenu()
    {
        if (upgradeMenu != null)
        {
            upgradeMenu.SetActive(false);
        }
        Time.timeScale = 1f; // Resume game
    }
}
