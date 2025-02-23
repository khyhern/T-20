using UnityEngine;

public class XPChest : MonoBehaviour
{
    public int xpAmount; // XP amount awarded by this chest

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddXP(xpAmount);
            }
            Destroy(gameObject); // Destroy the chest after awarding XP
        }
    }
}
