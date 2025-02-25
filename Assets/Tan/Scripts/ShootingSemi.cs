using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShootingSemi : MonoBehaviour
{
    public string weaponIdentity = "Shotgun"; // Unique identity for weapon swapping
    public GameObject Arrow; // Pointer for aiming
    public Transform firePoint; // Actual bullet spawn location

    [Tooltip("Time between shots (in seconds)")]
    public float shootCooldown = 1f; // Delay between shots
    private float lastShootTime;

    public Text ammoText;
    public int magazineSize = 8; // Shotgun has fewer rounds
    public float reloadTime = 2.5f;

    public int currentAmmo;
    public bool isReloading = false;

    public GameObject bulletPrefab; // Shotgun bullet prefab
    public int bulletDamage = 25;
    public float knockbackForce = 10f; // New: Knockback strength for the enemy

    public AudioSource gunAudioSource;
    public AudioClip shootingSound;


    protected virtual void Start()
    {
        currentAmmo = magazineSize;
        UpdateAmmoDisplay();
    }

    protected virtual void Update()
    {
        if (isReloading) return; // Prevent actions during reload

        // Single-click fire
        if (Input.GetButtonDown("Fire1") && Time.time >= lastShootTime + shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    public virtual void Shoot()
    {
        if (isReloading) return; // Prevent shooting while reloading
        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        currentAmmo--;
        UpdateAmmoDisplay();

        // Spawn bullet at firePoint (stays for 0.1 seconds)
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        TanBullet bulletScript = bulletObject.GetComponent<TanBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDamage(bulletDamage);
            bulletScript.SetKnockback(knockbackForce); // Pass knockback force
        }


        Destroy(bulletObject, 0.1f); // Auto-destroy after 0.2 seconds

        // Play gun sound with overlap
        if (gunAudioSource != null && shootingSound != null)
        {
            gunAudioSource.PlayOneShot(shootingSound);
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
}
