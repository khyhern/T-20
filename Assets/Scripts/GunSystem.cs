using System.Collections;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bulletPrefab; // Bullet prefab
    public Transform firePoint; // Where bullets are spawned
    public int maxAmmo = 30; // Max ammo
    public float fireRate = 0.2f; // Time between shots
    public float reloadTime = 1.5f; // Time to reload
    public float bulletSpeed = 10f; // Speed of the bullet
    public float shootingSlowFactor = 0.5f; // Factor to slow player movement while shooting

    [Header("Audio Settings")]
    public AudioSource audioSource; // Reference to the existing AudioSource
    public AudioClip shootSound;    // Gunfire sound
    public AudioClip reloadSound;   // Reloading sound


    [Header("Cursor Aiming")]
    public Transform gunPivot; // The pivot point for gun rotation
    public SpriteRenderer gunSpriteRenderer; // The gun sprite's SpriteRenderer for flipping

    [Header("Pooling")]
    private ObjectPoolManager poolManager; // Object pooling for bullets

    public int currentAmmo { get; private set; } // Current ammo
    private float nextFireTime = 0f; // Tracks the next time the gun can fire
    private bool isReloading = false; // Is the gun currently reloading?

    private PlayerMovement playerMovement; // Reference to the player's movement script

    void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>(); // Get the player's movement script
        currentAmmo = maxAmmo; // Initialize ammo

        // Get ObjectPoolManager from the scene
        poolManager = FindObjectOfType<ObjectPoolManager>();

        // Initialize bullet pool if pooling is available
        if (poolManager != null && bulletPrefab != null)
        {
            poolManager.InitializePool(bulletPrefab, 50); // Ensure the pool size matches your requirements
            Debug.Log("Bullet pool initialized.");
        }
        else
        {
            Debug.LogError("ObjectPoolManager or BulletPrefab is missing!");
        }

        // Get the existing AudioSource component from the Gun GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            Debug.LogError("Missing AudioSource! Add one to the Gun object.");
        }
    }

    void Update()
    {
        AimAtCursor();

        if (isReloading) return;

        // Shoot when LMB is pressed and gun is ready
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
        }

        // Manually reload when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }

        // Auto-reload when out of ammo
        if (currentAmmo <= 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    void AimAtCursor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDirection = (mousePosition - gunPivot.position).normalized;

        playerMovement.SetShootingDirection(aimDirection); // Update shooting direction in player script

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        gunPivot.rotation = Quaternion.Euler(0, 0, angle);

        // Flip the gun sprite based on the aiming direction
        gunSpriteRenderer.flipY = angle > 90 || angle < -90;
    }

    void Shoot()
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of Ammo!");
            return;
        }

        // Reduce player speed while shooting
        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(shootingSlowFactor);
        }

        currentAmmo--;
        nextFireTime = Time.time + fireRate;

        // Play gunfire sound
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        // Spawn bullet using pooling if available
        GameObject bullet = poolManager != null
            ? poolManager.GetObject(bulletPrefab, firePoint.position)
            : Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Assign the prefab reference for pooling
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetPrefabReference(bulletPrefab); // Set the prefab reference
            bulletScript.SetVelocity(firePoint.right * bulletSpeed); // Apply velocity
        }

        // Restore player speed after the next shot delay
        Invoke(nameof(ResetPlayerSpeed), fireRate);
    }

    void ResetPlayerSpeed()
    {
        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(1f);
            playerMovement.StopShooting(); // Stop shooting and reset shooting state
        }
    }

    IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        isReloading = true;

        // Play reload sound
        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        Debug.Log("Reload Complete!");
    }
}
