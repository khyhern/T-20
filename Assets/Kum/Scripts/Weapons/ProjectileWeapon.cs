using UnityEngine;

public class ProjectileWeapon : Weapon
{
    protected float currentAttackInterval;
    protected int currentAttackCount; // Number of times this attack will happen.

    protected override void Update()
    {
        base.Update();

        // Decrease attack interval over time and trigger attack when needed
        if (currentAttackInterval > 0)
        {
            currentAttackInterval -= Time.deltaTime;
            if (currentAttackInterval <= 0) Attack(currentAttackCount);
        }
    }

    public override bool CanAttack()
    {
        if (currentAttackCount > 0) return true;
        return base.CanAttack();
    }

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, log a warning
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning($"Projectile prefab has not been set for {name}");
            ActivateCooldown(true);
            return false;
        }

        // Can we attack?
        if (!CanAttack()) return false;

        // Calculate the spawn angle
        float spawnAngle = GetSpawnAngle();

        // Spawn a copy of the projectile
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)GetSpawnOffset(spawnAngle),
            Quaternion.Euler(0, 0, spawnAngle)
        );

        prefab.weapon = this;
        prefab.owner = owner;

        ActivateCooldown(true);

        attackCount--;

        // If there are additional attacks, set up the next attack interval
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }

    // ✅ FIXED: Corrected GetSpawnAngle() to properly reference PlayerMovement
    protected virtual float GetSpawnAngle()
    {
        PLAYERMovement playerMovement = FindObjectOfType<PLAYERMovement>(); // ✅ Ensure correct case

        if (playerMovement != null)
        {
            return Mathf.Atan2(playerMovement.lastMovedVector.y, playerMovement.lastMovedVector.x) * Mathf.Rad2Deg;
        }
        else
        {
            Debug.LogError("PLAYERMovement script not found in scene!");
            return 0f; // Return default angle
        }
    }

    // Generates a random point to spawn the projectile and rotates it by spawnAngle.
    protected virtual Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return Quaternion.Euler(0, 0, spawnAngle) * new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }
}
