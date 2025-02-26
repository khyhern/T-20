using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An aura is a damage-over-time effect that applies to a specific area in timed intervals.
/// It is used to give the functionality of Garlic, and it can also be used to spawn holy
/// water effects as well.
/// </summary>
public class Aura : WeaponEffect
{

    Dictionary<Enemy_Stats, float> affectedTargets = new Dictionary<Enemy_Stats, float>();
    List<Enemy_Stats> targetsToUnaffect = new List<Enemy_Stats>();

    // Update is called once per frame
    void Update()
    {
        Dictionary<Enemy_Stats, float> affectedTargsCopy = new Dictionary<Enemy_Stats, float>(affectedTargets);

        // Loop through every target affected by the aura, and reduce the cooldown
        // of the aura for it. If the cooldown reaches 0, deal damage to it.
        foreach (KeyValuePair<Enemy_Stats, float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    // If the target is marked for removal, remove it.
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    // Reset the cooldown and deal damage.
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown * Owner.Stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy_Stats es))
        {
            // If the target is not yet affected by this aura, add it
            // to our list of affected targets.
            if (!affectedTargets.ContainsKey(es))
            {
                // Always starts with an interval of 0, so that it will get
                // damaged in the next Update() tick.
                affectedTargets.Add(es, 0);
            }
            else
            {
                if (targetsToUnaffect.Contains(es))
                {
                    targetsToUnaffect.Remove(es);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy_Stats es))
        {
            // Do not directly remove the target upon leaving,
            // because we still have to track their cooldowns.
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}