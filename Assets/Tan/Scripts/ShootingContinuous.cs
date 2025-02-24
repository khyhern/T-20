using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootingContinuous : MonoBehaviour
{
    public string weaponIdentity = "SMG"; // Unique identity for weapon swapping
    public GameObject Arrow;

    protected bool isAutoShooting = false; // Toggle auto/manual shooting
    [Tooltip("Fire rate (in seconds)")]
    public float shootCooldown;
    protected float lastShootTime;

    public Text ammoText;
    public int magazineSize = 30;
    public float reloadTime = 2f;

    public int currentAmmo;
    public bool isReloading = false;

    public GameObject bulletPrefab;
    public int bulletDamage = 10;

    public Transform firePoint;
    [Tooltip("Bullet travel speed")]
    public float bulletForce = 20f;

    public float autoAimRange = 15f;

    protected virtual void Start()
    {
        currentAmmo = magazineSize;
        UpdateAmmoDisplay();
    }

    protected virtual void Update()
    {
        if (isReloading) return; // Prevent any actions during reload

        if (!isAutoShooting && Input.GetButton("Fire1") && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }

        if (!isAutoShooting && Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    public virtual void Shoot()
    {
        if (isReloading) return; // Prevent shooting while reloading

        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateAmmoDisplay();

            GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            BulletNKB bulletScript = bulletObject.GetComponent<BulletNKB>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
            }

            Rigidbody2D rb = bulletObject.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }
        else
        {
            StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading) yield break; // Prevent multiple reloads

        isReloading = true;
        ammoText.text = "Reloading...";
        yield return new WaitForSeconds(reloadTime);

        currentAmmo = magazineSize;
        isReloading = false;
        UpdateAmmoDisplay();
    }

    public void UpdateAmmoDisplay()
    {
        ammoText.text = $"{currentAmmo} / {magazineSize}";
    }

    public void SetWeaponActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}




/*{
    // Fire1 = left click
    // Fire2 = right click

    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject Arrow;

    [Tooltip("Shoot Cooldown (in seconds)")] // Tooltip will appear on hover
    public float shootCooldown;
    private float lastShootTime;

    public float bulletForce;

    private bool isAutoShooting = false; // Toggle between manual and auto-shooting

    private void Update()
    {
        // Toggle between auto and manual shooting when right-click is pressed
        if (Input.GetButtonDown("Fire2")) // Right-click
        {
            isAutoShooting = !isAutoShooting;
            Debug.Log(isAutoShooting ? "Auto Shooting Enabled" : "Manual Shooting Enabled");
        }

        if (isAutoShooting)
        {
            Arrow.SetActive(false);
            // Automatically shoot based on cooldown
            if (Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time; // Update last shot time
            }
        }
        else
        {
            Arrow.SetActive(!isAutoShooting);
            // Manual shooting (left-click)
            if (Input.GetButtonDown("Fire1") && Time.time >= lastShootTime + shootCooldown)
            {
                Shoot();
                lastShootTime = Time.time; // Update last shot time
            }
        }
    }

    void Shoot()
    {
        // Instantiate the bullet at the firePoint position and rotation
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>(); // Access the bullet's Rigidbody2D
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse); // Apply force to the bullet
    }
}*/

