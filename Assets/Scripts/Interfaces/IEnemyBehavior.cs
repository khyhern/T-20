using UnityEngine;

public interface IEnemyBehavior
{
    void InitializeBehavior(Transform player, Rigidbody2D rb, float moveSpeed); // Initialize dependencies
    void ExecuteBehavior(); // Perform behavior actions (e.g., movement)
}
