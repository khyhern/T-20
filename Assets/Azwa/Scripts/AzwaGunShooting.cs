using UnityEngine;

public class GunShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Bullet prefab to shoot
    public Transform firePoint; // Where the bullet spawns
    public float bulletSpeed = 10f; // Speed of the bullet
    public float fireRate = 0.2f; // Time between shots
    private float nextFireTime = 0f;

    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime) // Left mouse click
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Create the bullet at the firePoint position and rotation
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // Add velocity to the bullet in the firePoint's forward direction
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.right * bulletSpeed; // Moves the bullet forward
            }
        }
    }
}
