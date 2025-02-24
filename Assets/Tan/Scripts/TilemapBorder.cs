using UnityEngine;

public class TilemapBorder : MonoBehaviour
{
    private Collider2D myCollider;
    private GameObject player;
    private float lastX; // Store last frame's X position

    void Start()
    {
        myCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            lastX = player.transform.position.x;
        }
    }

    void Update()
    {
        if (player == null) return;

        float currentX = player.transform.position.x;

        if (currentX > lastX)
        {
            // Player moving RIGHT → Keep trigger ON
            myCollider.isTrigger = true;
        }
        else if (currentX < lastX)
        {
            // Player moving LEFT → Turn OFF trigger
            myCollider.isTrigger = false;
        }

        lastX = currentX; // Update lastX for next frame
    }
}
