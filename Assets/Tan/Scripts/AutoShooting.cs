using UnityEngine;

public class AutoShooting : MonoBehaviour
{
    private ShootingContinuous activeWeapon;
    private GameObject nearestEnemy;

    private bool isAutoShooting = false;
    private float lastShootTime;

    public AudioSource gunAudioSource;
    public AudioClip shootingSound;

    private void Start()
    {
        FindActiveWeapon();
    }

    private void Update()
    {
        if (activeWeapon == null)
        {
            FindActiveWeapon();
            return;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            isAutoShooting = !isAutoShooting;
            if (!isAutoShooting)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        if (isAutoShooting)
        {
            activeWeapon.Arrow.SetActive(false);
            nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                Vector3 direction = (nearestEnemy.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                transform.rotation = Quaternion.Euler(0, 0, angle);

                // Only shoot if not reloading
                if (!activeWeapon.isReloading && Time.time >= lastShootTime + activeWeapon.shootCooldown)
                {
                    ShootFromObject(); // Shoot from this object instead of the firePoint

                    // Play gun sound
                    if (gunAudioSource != null && shootingSound != null)
                    {
                        gunAudioSource.PlayOneShot(shootingSound);
                    }

                    lastShootTime = Time.time;
                }
            }
        }
        else
        {
            activeWeapon.Arrow.SetActive(!isAutoShooting);
        }
    }

    private void FindActiveWeapon()
    {
        ShootingContinuous[] weapons = FindObjectsByType<ShootingContinuous>(FindObjectsSortMode.None);

        foreach (ShootingContinuous weapon in weapons)
        {
            if (weapon.gameObject.activeSelf)
            {
                activeWeapon = weapon;
                break;
            }
        }
    }

    private GameObject FindNearestEnemy()
    {
        if (activeWeapon == null) return null;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, activeWeapon.autoAimRange);
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector2.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.gameObject;
                }
            }
        }
        return nearestEnemy;
    }

    private void ShootFromObject()
    {
        // If reloading, do nothing (prevent shooting instead of stopping sound suddenly)
        if (activeWeapon.isReloading) return;

        if (activeWeapon.currentAmmo > 0)
        {
            activeWeapon.currentAmmo--;
            activeWeapon.UpdateAmmoDisplay();

            GameObject bulletObject = Instantiate(activeWeapon.bulletPrefab, transform.position, transform.rotation);

            TanBullet bulletScript = bulletObject.GetComponent<TanBullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(activeWeapon.bulletDamage);
            }

            Rigidbody2D rb = bulletObject.GetComponent<Rigidbody2D>();
            rb.AddForce(transform.up * activeWeapon.bulletForce, ForceMode2D.Impulse);
        }
        else
        {
            // Start reloading
            StartCoroutine(activeWeapon.ReloadCoroutine());
        }
    }
}
