using UnityEngine;

public class AzwaBullet : MonoBehaviour
{
    public float lifetime = 2f;
    public int damage = 10;

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy bullet after some time
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            // Call enemy's damage function (assuming they have one)
            BossHealth enemy = collision.GetComponent<BossHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destroy bullet on impact
            Destroy(gameObject);
        }
    }
}
