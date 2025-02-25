using UnityEngine;

public class TanBullet : MonoBehaviour
{
    public int damage;
    public float knockbackForce;


    // Add a public method to set bullet damage
    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    public void SetKnockback(float force)
    {
        knockbackForce = force;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyAI enemy = collision.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            enemy.ApplyKnockback(knockbackDirection * knockbackForce);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject); // Destroy Game Object when it leaves camera view
    }
}