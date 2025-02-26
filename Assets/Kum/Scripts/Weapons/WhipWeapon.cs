using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordWeapon : ProjectileWeapon
{
    int currentSpawnCount; // How many times the whip has been attacking in this iteration.
    float currentSpawnYOffset; // If there are more than 2 whips, we will start offsetting it upwards.

    protected override bool Attack(int attackCount = 1)
    {
        // If no projectile prefab is assigned, leave a warning message.
        if (!currentStats.projectilePrefab)
        {
            Debug.LogWarning($"Projectile prefab has not been set for {name}");
            ActivateCooldown(true);
            return false;
        }

        // If there is no projectile assigned, set the weapon on cooldown.
        if (!CanAttack()) return false;

        // If this is the first time the attack has been fired,
        // we reset the currentSpawnCount.
        if (currentCooldown <= 0)
        {
            currentSpawnCount = 0;
            currentSpawnYOffset = 0f;
        }

        // ✅ FIX: Get PLAYERMovement reference
        PLAYERMovement playerMovement = FindObjectOfType<PLAYERMovement>();

        if (playerMovement == null)
        {
            Debug.LogError("PLAYERMovement script not found in scene!");
            return false; // Prevent error if player is missing
        }

        // ✅ FIX: Use playerMovement.lastMovedVector instead of movement.lastMovedVector
        float spawnDir = Mathf.Sign(playerMovement.lastMovedVector.x) * (currentSpawnCount % 2 != 0 ? -1 : 1);
        Vector2 spawnOffset = new Vector2(
            spawnDir * Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            currentSpawnYOffset
        );

        // And spawn a copy of the projectile.
        Projectile prefab = Instantiate(
            currentStats.projectilePrefab,
            owner.transform.position + (Vector3)spawnOffset,
            Quaternion.identity
        );
        prefab.owner = owner; // Set ourselves to be the owner.

        // Flip the projectile's sprite.
        if (spawnDir < 0)
        {
            prefab.transform.localScale = new Vector3(
                -Mathf.Abs(prefab.transform.localScale.x),
                prefab.transform.localScale.y,
                prefab.transform.localScale.z
            );
            Debug.Log(spawnDir + " | " + prefab.transform.localScale);
        }

        // Assign the stats.
        prefab.weapon = this;
        ActivateCooldown(true);
        attackCount--;

        // Determine where the next projectile should spawn.
        currentSpawnCount++;
        if (currentSpawnCount > 1 && currentSpawnCount % 2 == 0)
            currentSpawnYOffset += 1;

        // Do we perform another attack?
        if (attackCount > 0)
        {
            currentAttackCount = attackCount;
            currentAttackInterval = data.baseStats.projectileInterval;
        }

        return true;
    }
}
