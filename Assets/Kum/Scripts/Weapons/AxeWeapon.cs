using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terresquall;

public class AxeWeapon : ProjectileWeapon
{

    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        PLAYERMovement PLAYERMovement = FindObjectOfType<PLAYERMovement>(); // ✅ Get PLAYERMovement script

if (PLAYERMovement != null) 
{
    return 90f - Mathf.Sign(PLAYERMovement.lastMovedVector.x) * (5 * offset);
}
else 
{
    Debug.LogError("PLAYERMovement script not found in scene!");
    return 90f; // Default value
}
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
        );
    }

}
