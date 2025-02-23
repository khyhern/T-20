using UnityEngine;

public class XPOrb : MonoBehaviour
{
    public int xpAmount; // XP amount dynamically assigned

    private Transform player;
    public float moveSpeed = 2f; // Speed at which the orb moves toward the player
    public float pickupRadius = 1f; // Distance at which the orb will move toward the player

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= pickupRadius)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.AddXP(xpAmount);
            }
            Destroy(gameObject);
        }
    }
}
