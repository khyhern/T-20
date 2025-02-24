using UnityEngine;
using UnityEngine.UI;

public class DashCooldownUI : MonoBehaviour
{
    private PlayerScript player;
    public Image cooldownMask; // The radial UI Image

    void Start()
    {
        // Automatically finds PlayerScript
        player = FindAnyObjectByType<PlayerScript>();

        if (player == null)
        {
            Debug.LogError("PlayerScript not found! Make sure it's in the scene.");
        }
    }

    void Update()
    {
        if (player != null)
        {
            // Get cooldown progress (0 = ready, 1 = fully cooling down)
            float cooldownRatio = player.DashCooldownRemaining / player.dashCooldown;
            cooldownMask.fillAmount = 1 - cooldownRatio; // UI should gradually fill
        }
    }
}
